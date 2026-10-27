using DanfeSharp.Modelo;

namespace DanfeSharp.Blocos
{
    /// <summary>
    /// Quadro "Total do IBS/CBS/IS", NT 2026.010 item 4.1.
    /// </summary>
    class BlocoTotalIbsCbsIs : BlocoBase
    {
        public BlocoTotalIbsCbsIs(DanfeViewModel viewModel, Estilo estilo) : base(viewModel, estilo)
        {
            var m = ViewModel.CalculoImposto;

            AdicionarLinhaCampos()
            .ComCampoNumerico("Valor da CBS", m.ValorCbs)
            .ComCampoNumerico("Valor do IBS UF", m.ValorIbsUf)
            .ComCampoNumerico("Valor do IBS Município", m.ValorIbsMunicipio)
            .ComCampoNumerico("Valor do Imposto Seletivo", m.ValorImpostoSeletivo)
            .ComLargurasIguais();

            // Campos facultativos, exibidos somente quando informado o grupo de monofasia (gMono - W57).
            if (m.PossuiMonofasiaIbsCbs)
            {
                AdicionarLinhaCampos()
                .ComCampoNumerico("Valor do IBS Monofásico", m.ValorIbsMonofasico)
                .ComCampoNumerico("Valor da CBS Monofásica", m.ValorCbsMonofasica)
                .ComCampoNumerico("Valor do IBS Monofásico por Retenção", m.ValorIbsMonofasicoRetencao)
                .ComCampoNumerico("Valor da CBS Monofásica por Retenção", m.ValorCbsMonofasicaRetencao)
                .ComLargurasIguais();
            }
        }

        public override PosicaoBloco Posicao => PosicaoBloco.Topo;
        public override string Cabecalho => "Total do IBS / CBS / IS";
    }
}
