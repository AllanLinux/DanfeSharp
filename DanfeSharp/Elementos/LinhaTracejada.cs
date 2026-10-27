using System;
using System.Drawing;
using DanfeSharp.Graphics;

namespace DanfeSharp
{
    internal class LinhaTracejada : DrawableBase
    {
        public float Margin { get; set; }
        public double[] DashPattern { get; set; }

        public LinhaTracejada(float margin)
        {
            Margin = margin;
        }

        public override void Draw(Gfx gfx)
        {
            base.Draw(gfx);

            gfx.BeginLocalState();
            gfx.SetLineDash(3, 2);
            gfx.DrawLine(new PointF(BoundingBox.Left, Y + Margin), new PointF(BoundingBox.Right, Y + Margin));
            gfx.Stroke();
            gfx.EndLocalState();

        }

        public override float Height { get => 2 * Margin; set => throw new NotSupportedException(); }
    }
}
