using System;
using System.Collections;
using DanfeSharp.Graphics;
using QRCoder;

namespace DanfeSharp
{
    /// <summary>
    /// Desenha um QR Code em vetor, com retângulos, a partir da matriz de módulos gerada pelo QRCoder.
    /// </summary>
    internal class QrCode : DrawableBase
    {
        /// <summary>
        /// Texto codificado no QR Code.
        /// </summary>
        public String Texto { get; private set; }

        /// <summary>
        /// Lado do QR Code, em mm, incluindo a zona de silêncio de 4 módulos.
        /// </summary>
        public float Lado { get; private set; }

        public QrCode(String texto, float lado)
        {
            if (String.IsNullOrWhiteSpace(texto)) throw new ArgumentException("O texto do QR Code não pode ser vazio.", nameof(texto));
            if (lado <= 0) throw new ArgumentOutOfRangeException(nameof(lado));

            Texto = texto;
            Lado = lado;
            Width = lado;
            Height = lado;
        }

        public override void Draw(Gfx gfx)
        {
            base.Draw(gfx);

            // Os manuais do DANFE NFC-e e do DACTE pedem o nível de correção de erros M.
            using (var gerador = new QRCodeGenerator())
            using (var dados = gerador.CreateQrCode(Texto, QRCodeGenerator.ECCLevel.M))
            {
                var matriz = dados.ModuleMatrix;
                float modulo = Lado / matriz.Count;

                for (int linha = 0; linha < matriz.Count; linha++)
                {
                    BitArray bits = matriz[linha];
                    int inicio = -1;

                    // Une os módulos escuros consecutivos da linha em um único retângulo.
                    for (int coluna = 0; coluna <= bits.Length; coluna++)
                    {
                        bool escuro = coluna < bits.Length && bits[coluna];

                        if (escuro && inicio < 0) inicio = coluna;

                        if (!escuro && inicio >= 0)
                        {
                            gfx.DrawRectangle(X + inicio * modulo, Y + linha * modulo, (coluna - inicio) * modulo, modulo);
                            inicio = -1;
                        }
                    }
                }

                gfx.Fill();
            }
        }
    }
}
