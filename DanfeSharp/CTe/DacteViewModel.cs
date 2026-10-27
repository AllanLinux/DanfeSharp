using System;
using System.Collections.Generic;
using DanfeSharp.Modelo;

namespace DanfeSharp.CTe
{
    /// <summary>
    /// Quantidade da carga (infQ).
    /// </summary>
    public class QuantidadeCargaViewModel
    {
        public String TipoMedida { get; set; }
        public String Unidade { get; set; }
        public double Quantidade { get; set; }
    }

    /// <summary>
    /// Componente do valor da prestação (Comp).
    /// </summary>
    public class ComponenteViewModel
    {
        public String Nome { get; set; }
        public double Valor { get; set; }
    }

    /// <summary>
    /// Documento originário transportado: NF-e, nota fiscal em papel ou outros documentos.
    /// </summary>
    public class DocumentoOriginarioViewModel
    {
        public String Tipo { get; set; }

        /// <summary>
        /// Chave de acesso da NF-e ou identificação do documento.
        /// </summary>
        public String Identificacao { get; set; }

        public String SerieNumero { get; set; }
    }

    /// <summary>
    /// Modelo de dados utilizado para o DACTE (CT-e 4.00, modelo 57).
    /// </summary>
    public class DacteViewModel
    {
        public String ChaveAcesso { get; set; }
        public int Numero { get; set; }
        public int Serie { get; set; }
        public int Modelo { get; set; }
        public DateTime? DataHoraEmissao { get; set; }
        public String Cfop { get; set; }
        public String NaturezaOperacao { get; set; }

        /// <summary>
        /// <para>Modal: 01-Rodoviário; 02-Aéreo; 03-Aquaviário; 04-Ferroviário; 05-Dutoviário; 06-Multimodal</para>
        /// <para>Tag modal</para>
        /// </summary>
        public String Modal { get; set; }

        /// <summary>
        /// <para>Tipo do CT-e: 0-Normal; 1-Complementar; 3-Substituto</para>
        /// <para>Tag tpCTe</para>
        /// </summary>
        public int TipoCte { get; set; }

        /// <summary>
        /// <para>Tipo do serviço: 0-Normal; 1-Subcontratação; 2-Redespacho; 3-Redespacho intermediário; 4-Serviço vinculado a multimodal</para>
        /// <para>Tag tpServ</para>
        /// </summary>
        public int TipoServico { get; set; }

        public String InicioPrestacao { get; set; }
        public String TerminoPrestacao { get; set; }

        public int TipoAmbiente { get; set; }
        public int TipoEmissao { get; set; }
        public String ProtocoloAutorizacao { get; set; }
        public String QrCode { get; set; }

        public EmpresaViewModel Emitente { get; set; }
        public EmpresaViewModel Remetente { get; set; }
        public EmpresaViewModel Expedidor { get; set; }
        public EmpresaViewModel Recebedor { get; set; }
        public EmpresaViewModel Destinatario { get; set; }
        public EmpresaViewModel Tomador { get; set; }

        /// <summary>
        /// Papel do tomador: Remetente, Expedidor, Recebedor, Destinatário ou Outros.
        /// </summary>
        public String TomadorPapel { get; set; }

        #region Carga

        public String ProdutoPredominante { get; set; }
        public String OutrasCaracteristicas { get; set; }
        public double? ValorCarga { get; set; }
        public List<QuantidadeCargaViewModel> Quantidades { get; set; }

        #endregion

        #region Valores e impostos

        public List<ComponenteViewModel> Componentes { get; set; }
        public double ValorTotalServico { get; set; }
        public double ValorReceber { get; set; }

        public String IcmsSituacaoTributaria { get; set; }
        public double? IcmsBaseCalculo { get; set; }
        public double? IcmsAliquota { get; set; }
        public double? IcmsValor { get; set; }
        public double? IcmsReducaoBaseCalculo { get; set; }
        public double? IcmsSt { get; set; }
        public double? ValorAproximadoTributos { get; set; }

        public String IbsCbsCst { get; set; }
        public String IbsCbsClassificacaoTributaria { get; set; }
        public double? IbsCbsBaseCalculo { get; set; }
        public double? IbsUfAliquota { get; set; }
        public double? IbsUfValor { get; set; }
        public double? IbsMunicipioAliquota { get; set; }
        public double? IbsMunicipioValor { get; set; }
        public double? CbsAliquota { get; set; }
        public double? CbsValor { get; set; }

        public bool PossuiIbsCbs => !String.IsNullOrWhiteSpace(IbsCbsClassificacaoTributaria) || IbsCbsBaseCalculo.HasValue;

        #endregion

        public List<DocumentoOriginarioViewModel> Documentos { get; set; }

        public String Observacoes { get; set; }
        public String InformacoesFisco { get; set; }

        /// <summary>
        /// RNTRC da empresa, no modal rodoviário.
        /// </summary>
        public String Rntrc { get; set; }

        /// <summary>
        /// Campos do grupo do modal quando não é rodoviário, como nome e valor.
        /// </summary>
        public List<KeyValuePair<String, String>> InformacoesModal { get; set; }

        /// <summary>
        /// Chave do CT-e complementado, no CT-e complementar.
        /// </summary>
        public String ChaveCteComplementado { get; set; }

        public bool IsHomologacao => TipoAmbiente == 2;

        public String DescricaoModal
        {
            get
            {
                switch (Modal)
                {
                    case "01": return "RODOVIÁRIO";
                    case "02": return "AÉREO";
                    case "03": return "AQUAVIÁRIO";
                    case "04": return "FERROVIÁRIO";
                    case "05": return "DUTOVIÁRIO";
                    case "06": return "MULTIMODAL";
                    default: return Modal;
                }
            }
        }

        public String DescricaoTipoCte
        {
            get
            {
                switch (TipoCte)
                {
                    case 0: return "NORMAL";
                    case 1: return "COMPLEMENTAR";
                    case 2: return "ANULAÇÃO";
                    case 3: return "SUBSTITUTO";
                    default: return TipoCte.ToString();
                }
            }
        }

        public String DescricaoTipoServico
        {
            get
            {
                switch (TipoServico)
                {
                    case 0: return "NORMAL";
                    case 1: return "SUBCONTRATAÇÃO";
                    case 2: return "REDESPACHO";
                    case 3: return "REDESPACHO INTERMEDIÁRIO";
                    case 4: return "SERVIÇO VINCULADO A MULTIMODAL";
                    default: return TipoServico.ToString();
                }
            }
        }

        public DacteViewModel()
        {
            Emitente = new EmpresaViewModel();
            Quantidades = new List<QuantidadeCargaViewModel>();
            Componentes = new List<ComponenteViewModel>();
            Documentos = new List<DocumentoOriginarioViewModel>();
            InformacoesModal = new List<KeyValuePair<String, String>>();
        }
    }
}
