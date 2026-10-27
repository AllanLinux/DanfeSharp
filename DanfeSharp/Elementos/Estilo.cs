using DanfeSharp.Graphics;
using System;

namespace DanfeSharp
{
    /// <summary>
    /// Coleção de fontes e medidas a serem compartilhadas entre os elementos básicos.
    /// </summary>
    internal class Estilo 
    {
        public float PaddingSuperior { get; set; }
        public float PaddingInferior { get; set; }
        public float PaddingHorizontal { get; set; }
        public float FonteTamanhoMinimo { get; set; }

        public FamiliaFonte FonteInternaRegular { get; set; }
        public FamiliaFonte FonteInternaNegrito { get; set; }
        public FamiliaFonte FonteInternaItalico { get; set; }

        public Fonte FonteCampoCabecalho { get; private set; }
        public Fonte FonteCampoConteudo { get; private set; }
        public Fonte FonteCampoConteudoNegrito { get; private set; }
        public Fonte FonteBlocoCabecalho { get; private set; }
        public Fonte FonteNumeroFolhas { get; private set; }

        public Estilo(FamiliaFonte fontRegular, FamiliaFonte fontBold, FamiliaFonte fontItalic, float tamanhoFonteCampoCabecalho = 6, float tamanhoFonteConteudo = 10)
        {
            PaddingHorizontal = 0.75F;
            PaddingSuperior = 0.65F;
            PaddingInferior = 0.3F;

            FonteInternaRegular = fontRegular;
            FonteInternaNegrito = fontBold;
            FonteInternaItalico = fontItalic;

            FonteCampoCabecalho = CriarFonteRegular(tamanhoFonteCampoCabecalho);
            FonteCampoConteudo = CriarFonteRegular(tamanhoFonteConteudo);
            FonteCampoConteudoNegrito = CriarFonteNegrito(tamanhoFonteConteudo);
            FonteBlocoCabecalho = CriarFonteRegular(7);
            FonteNumeroFolhas = CriarFonteNegrito(10F);
            FonteTamanhoMinimo = 5.75F;
        }

        public Fonte CriarFonteRegular(float emSize) => new Fonte(FonteInternaRegular, emSize);
        public Fonte CriarFonteNegrito(float emSize) => new Fonte(FonteInternaNegrito, emSize);
        public Fonte CriarFonteItalico(float emSize) => new Fonte(FonteInternaItalico, emSize);

    }
}
