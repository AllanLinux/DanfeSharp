using System;
using System.Linq;
using DanfeSharp.Graphics;

namespace DanfeSharp.CTe
{
    /// <summary>
    /// Caixa com linhas de texto centralizadas.
    /// </summary>
    internal class CaixaTexto : ElementoBase
    {
        private readonly Tuple<String, Fonte>[] _Linhas;

        public CaixaTexto(Estilo estilo, float altura, params Tuple<String, Fonte>[] linhas) : base(estilo)
        {
            _Linhas = linhas;
            Height = altura;
        }

        public override void Draw(Gfx gfx)
        {
            base.Draw(gfx);

            var ts = new TextStack(BoundingBox.InflatedRetangle(1F)) { AlinhamentoHorizontal = AlinhamentoHorizontal.Centro, AlinhamentoVertical = AlinhamentoVertical.Centro };
            foreach (var l in _Linhas.Where(l => !String.IsNullOrWhiteSpace(l.Item1)))
                ts.AddLine(l.Item1, l.Item2);

            ts.Draw(gfx);
        }
    }

    /// <summary>
    /// Caixa com o QR Code centralizado.
    /// </summary>
    internal class CaixaQrCode : ElementoBase
    {
        private readonly String _Texto;

        public CaixaQrCode(Estilo estilo, String texto, float altura) : base(estilo)
        {
            _Texto = texto;
            Height = altura;
        }

        public override void Draw(Gfx gfx)
        {
            base.Draw(gfx);

            float lado = Math.Min(Width, Height) - 2F;
            var qr = new QrCode(_Texto, lado);
            qr.SetPosition(X + (Width - lado) / 2F, Y + (Height - lado) / 2F);
            qr.Draw(gfx);
        }
    }
}
