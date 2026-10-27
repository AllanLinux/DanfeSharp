using System;
using System.Collections.Generic;
using System.IO;
using DanfeSharp.Blocos;
using DanfeSharp.Graphics;
using DanfeSharp.Modelo;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace DanfeSharp
{
    public class Danfe : IDisposable
    {
        public DanfeViewModel ViewModel { get; private set; }
        internal PdfDocument PdfDocument { get; private set; }

        internal BlocoCanhoto Canhoto { get; private set; }
        internal BlocoIdentificacaoEmitente IdentificacaoEmitente { get; private set; }

        internal List<BlocoBase> _Blocos;
        internal Estilo EstiloPadrao { get; private set; }

        internal List<DanfePagina> Paginas { get; private set; }

        private FamiliaFonte _FonteRegular;
        private FamiliaFonte _FonteNegrito;
        private FamiliaFonte _FonteItalico;

        private Boolean _FoiGerado;

        private XImage _LogoObject = null;
        private MemoryStream _LogoStream = null;

        public Danfe(DanfeViewModel viewModel)
        {
            ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));

            _Blocos = new List<BlocoBase>();
            PdfDocument = new PdfDocument();
            PdfDocument.Options.CompressContentStreams = true;

            // De acordo com o item 7.7, a fonte deve ser Times New Roman ou Courier New.
            _FonteRegular = FamiliaFonte.TimesRegular();
            _FonteNegrito = FamiliaFonte.TimesNegrito();
            _FonteItalico = FamiliaFonte.TimesItalico();

            EstiloPadrao = CriarEstilo();

            Paginas = new List<DanfePagina>();
            Canhoto = CriarBloco<BlocoCanhoto>();
            IdentificacaoEmitente = AdicionarBloco<BlocoIdentificacaoEmitente>();  
            AdicionarBloco<BlocoDestinatarioRemetente>();

            if (ViewModel.LocalRetirada != null && ViewModel.ExibirBlocoLocalRetirada)
                AdicionarBloco<BlocoLocalRetirada>();

            if (ViewModel.LocalEntrega != null && ViewModel.ExibirBlocoLocalEntrega)
                AdicionarBloco<BlocoLocalEntrega>();

            if (ViewModel.Duplicatas.Count > 0)
                AdicionarBloco<BlocoDuplicataFatura>();

            AdicionarBloco<BlocoCalculoImposto>(ViewModel.Orientacao == Orientacao.Paisagem ? EstiloPadrao : CriarEstilo(4.75F));

            if (ViewModel.UsarLeiauteReformaTributaria)
                AdicionarBloco<BlocoTotalIbsCbsIs>(ViewModel.Orientacao == Orientacao.Paisagem ? EstiloPadrao : CriarEstilo(4.75F));

            AdicionarBloco<BlocoTransportador>();
            AdicionarBloco<BlocoDadosAdicionais>(CriarEstilo(tFonteCampoConteudo: 8));

            if(ViewModel.CalculoIssqn.Mostrar)
                AdicionarBloco<BlocoCalculoIssqn>();

            AdicionarMetadata();

            _FoiGerado = false;
        }
        
        public void AdicionarLogoImagem(System.IO.Stream stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            try
            {
                DefinirLogo(stream, s => XImage.FromStream(s));
            }
            catch (Exception e) when (!(e is ArgumentException))
            {
                throw new InvalidOperationException("O logotipo não pode ser carregado, certifique-se que a imagem esteja no formato JPEG, PNG, GIF, BMP ou TIFF.", e);
            }
        }

        public void AdicionarLogoPdf(System.IO.Stream stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            DefinirLogo(stream, s => XPdfForm.FromStream(s));
        }

        /// <summary>
        /// Copia o logotipo para a memória, pois o PDFsharp lê o conteúdo novamente ao salvar o DANFE.
        /// </summary>
        private void DefinirLogo(System.IO.Stream stream, Func<System.IO.Stream, XImage> carregar)
        {
            var ms = new MemoryStream();
            stream.CopyTo(ms);
            ms.Position = 0;

            var logo = carregar(ms);

            _LogoObject?.Dispose();
            _LogoStream?.Dispose();
            _LogoObject = logo;
            _LogoStream = ms;
        }

        public void AdicionarLogoImagem(String path)
        {
            if (String.IsNullOrWhiteSpace(path)) throw new ArgumentException(nameof(path));

            using(var fs = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                AdicionarLogoImagem(fs);
            }
        }

        public void AdicionarLogoPdf(String path)
        {
            if (String.IsNullOrWhiteSpace(path)) throw new ArgumentException(nameof(path));

            using (var fs = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                AdicionarLogoPdf(fs);
            }
        }

        private void AdicionarMetadata()
        {
            var info = PdfDocument.Info;
            info.Elements.SetString("/ChaveAcesso", ViewModel.ChaveAcesso);
            info.Elements.SetString("/TipoDocumento", "DANFE");
            info.CreationDate = DateTime.Now;
            info.Creator = String.Format("{0} {1} - {2}", "DanfeSharp", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version, "https://github.com/SilverCard/DanfeSharp");
            info.Title = "DANFE (Documento auxiliar da NFe)";
        }

        private Estilo CriarEstilo(float tFonteCampoCabecalho = 6, float tFonteCampoConteudo = 10)
        {
            return new Estilo(_FonteRegular, _FonteNegrito, _FonteItalico, tFonteCampoCabecalho, tFonteCampoConteudo);
        }

        public void Gerar()
        {
            if (_FoiGerado) throw new InvalidOperationException("O Danfe já foi gerado.");

            IdentificacaoEmitente.Logo = _LogoObject;
            var tabela = new TabelaProdutosServicos(ViewModel, EstiloPadrao);      
                    
            while (true)
            {
                DanfePagina p = CriarPagina();                   
               
                tabela.SetPosition(p.RetanguloCorpo.Location);
                tabela.SetSize(p.RetanguloCorpo.Size);
                tabela.Draw(p.Gfx);

                p.Gfx.Stroke();
                p.Gfx.Flush();

                if (tabela.CompletamenteDesenhada) break;            

            }

            PreencherNumeroFolhas();

            foreach (var pagina in Paginas)
                pagina.Gfx.Dispose();

            _FoiGerado = true;

        }

        private DanfePagina CriarPagina()
        {
            DanfePagina p = new DanfePagina(this);
            Paginas.Add(p);
            p.DesenharBlocos(Paginas.Count == 1);
            p.DesenharCreditos();

            // Ambiente de homologação
            // 7. O DANFE emitido para representar NF-e cujo uso foi autorizado em ambiente de
            // homologação sempre deverá conter a frase “SEM VALOR FISCAL” no quadro “Informações
            // Complementares” ou em marca d’água destacada.
            if (ViewModel.TipoAmbiente == 2)
                p.DesenharAvisoHomologacao();

            return p;
        }

        internal T CriarBloco<T>() where T : BlocoBase
        {
            return (T)Activator.CreateInstance(typeof(T), ViewModel, EstiloPadrao);
        }

        internal T CriarBloco<T>(Estilo estilo) where T : BlocoBase
        {
            return (T)Activator.CreateInstance(typeof(T), ViewModel, estilo);
        }

        internal T AdicionarBloco<T>() where T: BlocoBase
        {
            var bloco = CriarBloco<T>();
            _Blocos.Add(bloco);
            return bloco;
        }

        internal T AdicionarBloco<T>(Estilo estilo) where T : BlocoBase
        {
            var bloco = CriarBloco<T>(estilo);
            _Blocos.Add(bloco);
            return bloco;
        }

        internal void AdicionarBloco(BlocoBase bloco)
        {
            _Blocos.Add(bloco);
        }

        internal void PreencherNumeroFolhas()
        {
            int nFolhas = Paginas.Count;
            for (int i = 0; i < Paginas.Count; i++)
            {
                Paginas[i].DesenhaNumeroPaginas(i + 1, nFolhas);               
            }
        }

        public void Salvar(String path)
        {
            if (String.IsNullOrWhiteSpace(path)) throw new ArgumentException(nameof(path));

            PdfDocument.Save(path);
        }

        public void Salvar(System.IO.Stream stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            PdfDocument.Save(stream, false);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    PdfDocument.Dispose();
                    _LogoObject?.Dispose();
                    _LogoStream?.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Danfe() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
