using System;
using System.Collections.Generic;
using DanfeSharp.Blocos;
using DanfeSharp.Modelo;

namespace DanfeSharp
{
    public class Danfe : DocumentoAuxiliar
    {
        public DanfeViewModel ViewModel { get; private set; }

        internal BlocoCanhoto Canhoto { get; private set; }
        internal BlocoIdentificacaoEmitente IdentificacaoEmitente { get; private set; }

        internal List<BlocoBase> _Blocos;
        internal Estilo EstiloPadrao { get; private set; }

        internal List<DanfePagina> Paginas { get; private set; }

        private Boolean _FoiGerado;

        public Danfe(DanfeViewModel viewModel)
        {
            ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));

            _Blocos = new List<BlocoBase>();
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

            var estiloTotais = ViewModel.Orientacao == Orientacao.Paisagem ? EstiloPadrao : CriarEstilo(4.75F);

            if (ViewModel.UsarLeiauteReformaTributaria)
            {
                // NT 2026.010: o "Cálculo do Imposto" é dividido nos quadros de totais da Reforma Tributária.
                AdicionarBloco<BlocoTotalProdutosNota>(estiloTotais);
                AdicionarBloco<BlocoTotalIcmsIpi>(estiloTotais);
                AdicionarBloco<BlocoTotalIbsCbsIs>(estiloTotais);
            }
            else
            {
                AdicionarBloco<BlocoCalculoImposto>(estiloTotais);
            }

            AdicionarBloco<BlocoTransportador>();
            AdicionarBloco<BlocoDadosAdicionais>(CriarEstilo(tFonteCampoConteudo: 8));

            if(ViewModel.CalculoIssqn.Mostrar)
                AdicionarBloco<BlocoCalculoIssqn>();

            DefinirMetadados("DANFE", "DANFE (Documento auxiliar da NFe)", ViewModel.ChaveAcesso);

            _FoiGerado = false;
        }

        public override void Gerar()
        {
            if (_FoiGerado) throw new InvalidOperationException("O Danfe já foi gerado.");

            IdentificacaoEmitente.Logo = Logo;
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
    }
}
