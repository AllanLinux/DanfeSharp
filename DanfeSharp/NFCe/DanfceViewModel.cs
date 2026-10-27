using System;
using System.Collections.Generic;
using DanfeSharp.Esquemas.NFe;
using DanfeSharp.Modelo;

namespace DanfeSharp.NFCe
{
    /// <summary>
    /// Largura da bobina da impressora térmica.
    /// </summary>
    public enum LarguraBobina
    {
        /// <summary>
        /// Bobina de 80 mm, com área de impressão de 72 mm.
        /// </summary>
        Mm80,

        /// <summary>
        /// Bobina de 58 mm, com área de impressão de 48 mm.
        /// </summary>
        Mm58
    }

    /// <summary>
    /// Forma de pagamento da NFC-e.
    /// </summary>
    public class PagamentoViewModel
    {
        /// <summary>
        /// <para>Meio de pagamento</para>
        /// <para>Tag tPag</para>
        /// </summary>
        public String Codigo { get; set; }

        /// <summary>
        /// Descrição do meio de pagamento.
        /// </summary>
        public String Descricao { get; set; }

        /// <summary>
        /// <para>Valor do pagamento</para>
        /// <para>Tag vPag</para>
        /// </summary>
        public double Valor { get; set; }
    }

    /// <summary>
    /// Modelo de dados utilizado para o DANFE NFC-e.
    /// </summary>
    public class DanfceViewModel
    {
        public LarguraBobina LarguraBobina { get; set; } = LarguraBobina.Mm80;

        public int NfNumero { get; set; }
        public int NfSerie { get; set; }
        public String ChaveAcesso { get; set; }
        public DateTime? DataHoraEmissao { get; set; }

        public EmpresaViewModel Emitente { get; set; }

        /// <summary>
        /// Consumidor. Nulo quando não identificado.
        /// </summary>
        public EmpresaViewModel Destinatario { get; set; }

        public List<ProdutoViewModel> Produtos { get; set; }
        public CalculoImpostoViewModel CalculoImposto { get; set; }
        public List<PagamentoViewModel> Pagamentos { get; set; }

        /// <summary>
        /// <para>Valor do troco</para>
        /// <para>Tag vTroco</para>
        /// </summary>
        public double? Troco { get; set; }

        /// <summary>
        /// <para>Texto do QR Code</para>
        /// <para>Tag qrCode</para>
        /// </summary>
        public String QrCode { get; set; }

        /// <summary>
        /// <para>URL de consulta pela chave de acesso</para>
        /// <para>Tag urlChave</para>
        /// </summary>
        public String UrlConsulta { get; set; }

        /// <summary>
        /// Número do protocolo de autorização. Nulo quando emitida em contingência e ainda não autorizada.
        /// </summary>
        public String ProtocoloAutorizacao { get; set; }

        public DateTime? DataHoraAutorizacao { get; set; }

        public int TipoAmbiente { get; set; }
        public FormaEmissao TipoEmissao { get; set; }

        public DateTime? ContingenciaDataHora { get; set; }
        public String ContingenciaJustificativa { get; set; }

        public String InformacoesComplementares { get; set; }
        public String InformacoesAdicionaisFisco { get; set; }

        /// <summary>
        /// Valor dos acréscimos: frete, seguro e outras despesas.
        /// </summary>
        public double Acrescimos => CalculoImposto.ValorFrete + CalculoImposto.ValorSeguro + CalculoImposto.OutrasDespesas;

        public bool IsHomologacao => TipoAmbiente == 2;
        public bool IsContingencia => TipoEmissao == FormaEmissao.ContingenciaOffLineNFCe;

        public DanfceViewModel()
        {
            Emitente = new EmpresaViewModel();
            Produtos = new List<ProdutoViewModel>();
            CalculoImposto = new CalculoImpostoViewModel();
            Pagamentos = new List<PagamentoViewModel>();
        }

        /// <summary>
        /// Descrição do meio de pagamento (tPag) conforme o MOC.
        /// </summary>
        public static String DescricaoFormaPagamento(String tPag, String xPag = null)
        {
            switch (tPag)
            {
                case "01": return "Dinheiro";
                case "02": return "Cheque";
                case "03": return "Cartão de Crédito";
                case "04": return "Cartão de Débito";
                case "05": return "Crédito Loja";
                case "10": return "Vale Alimentação";
                case "11": return "Vale Refeição";
                case "12": return "Vale Presente";
                case "13": return "Vale Combustível";
                case "15": return "Boleto Bancário";
                case "16": return "Depósito Bancário";
                case "17": return "PIX Dinâmico";
                case "18": return "Transferência Bancária";
                case "19": return "Programa de Fidelidade";
                case "20": return "PIX Estático";
                case "21": return "Crédito em Loja";
                case "22": return "Pagamento Eletrônico";
                case "90": return "Sem Pagamento";
                case "99": return String.IsNullOrWhiteSpace(xPag) ? "Outros" : xPag;
                default: return String.IsNullOrWhiteSpace(xPag) ? tPag : xPag;
            }
        }
    }
}
