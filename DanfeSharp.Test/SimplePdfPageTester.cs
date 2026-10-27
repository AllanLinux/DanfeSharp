using DanfeSharp.Graphics;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Diagnostics;

namespace DanfeSharp
{
    internal class SimplePdfPageTester : IDisposable
    {
        public PdfDocument Document { get; set; }
        public Gfx Gfx { get; set; }
        public PdfPage Page { get; set; }

        public SimplePdfPageTester()
        {
            Document = new PdfDocument();
            Page = Document.AddPage();
            Gfx = new Gfx(XGraphics.FromPdfPage(Page));
        }

        public void Save(String path)
        {
            Gfx.Dispose();
            Document.Save(path);
        }

        public Estilo CriarEstilo()
        {
            return new Estilo(FamiliaFonte.TimesRegular(), FamiliaFonte.TimesNegrito(), FamiliaFonte.TimesItalico());
        }

        public void Save()
        {
            Save(new StackTrace().GetFrame(1).GetMethod().Name + ".pdf");
        }

        public void Dispose()
        {
            Document?.Dispose();
        }
    }
}
