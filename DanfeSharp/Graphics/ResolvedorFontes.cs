using System;
using System.Collections.Generic;
using System.IO;
#if !NET452
using PdfSharp.Fonts;
#endif

namespace DanfeSharp.Graphics
{
    /// <summary>
    /// Localiza os arquivos da fonte Times New Roman usados no DANFE.
    /// No .NET Framework o PDFsharp usa as fontes instaladas no Windows e este resolvedor não é necessário.
    /// No .NET 8+ ele procura a Times New Roman (Windows e macOS) ou a Liberation Serif, que tem as mesmas métricas (Linux).
    /// Outros arquivos podem ser informados com <see cref="DanfeSharp.Fontes.Registrar(string, string, string)"/>.
    /// </summary>
    internal static class ResolvedorFontes
    {
        internal const String FaceRegular = "DanfeSharp#Regular";
        internal const String FaceNegrito = "DanfeSharp#Negrito";
        internal const String FaceItalico = "DanfeSharp#Italico";

        private static readonly object _Lock = new object();
        private static readonly Dictionary<String, byte[]> _Faces = new Dictionary<String, byte[]>();
#if !NET452
        private static bool _Configurado;
#endif

        private static readonly String[][] _Candidatos =
        {
            // Windows
            new[] { @"%WINDIR%\Fonts\times.ttf", @"%WINDIR%\Fonts\timesbd.ttf", @"%WINDIR%\Fonts\timesi.ttf" },
            // macOS
            new[] { "/System/Library/Fonts/Supplemental/Times New Roman.ttf", "/System/Library/Fonts/Supplemental/Times New Roman Bold.ttf", "/System/Library/Fonts/Supplemental/Times New Roman Italic.ttf" },
            new[] { "/Library/Fonts/Times New Roman.ttf", "/Library/Fonts/Times New Roman Bold.ttf", "/Library/Fonts/Times New Roman Italic.ttf" },
            // Linux, pacote ttf-mscorefonts-installer
            new[] { "/usr/share/fonts/truetype/msttcorefonts/Times_New_Roman.ttf", "/usr/share/fonts/truetype/msttcorefonts/Times_New_Roman_Bold.ttf", "/usr/share/fonts/truetype/msttcorefonts/Times_New_Roman_Italic.ttf" },
            // Linux, Liberation Serif (Debian/Ubuntu, Fedora/RHEL e Alpine)
            new[] { "/usr/share/fonts/truetype/liberation/LiberationSerif-Regular.ttf", "/usr/share/fonts/truetype/liberation/LiberationSerif-Bold.ttf", "/usr/share/fonts/truetype/liberation/LiberationSerif-Italic.ttf" },
            new[] { "/usr/share/fonts/liberation-serif/LiberationSerif-Regular.ttf", "/usr/share/fonts/liberation-serif/LiberationSerif-Bold.ttf", "/usr/share/fonts/liberation-serif/LiberationSerif-Italic.ttf" },
            new[] { "/usr/share/fonts/liberation/LiberationSerif-Regular.ttf", "/usr/share/fonts/liberation/LiberationSerif-Bold.ttf", "/usr/share/fonts/liberation/LiberationSerif-Italic.ttf" },
        };

        /// <summary>
        /// Registra os arquivos de fonte a serem usados no lugar da Times New Roman.
        /// </summary>
        internal static void Registrar(byte[] regular, byte[] negrito, byte[] italico)
        {
            lock (_Lock)
            {
                _Faces[FaceRegular] = regular ?? throw new ArgumentNullException(nameof(regular));
                _Faces[FaceNegrito] = negrito ?? throw new ArgumentNullException(nameof(negrito));
                _Faces[FaceItalico] = italico ?? throw new ArgumentNullException(nameof(italico));
            }
        }

        /// <summary>
        /// Instala o resolvedor no PDFsharp, caso a aplicação não tenha definido um próprio.
        /// </summary>
        internal static void Configurar()
        {
#if !NET452
            lock (_Lock)
            {
                if (_Configurado) return;
                _Configurado = true;

                if (GlobalFontSettings.FontResolver == null)
                {
                    GlobalFontSettings.FontResolver = new Resolvedor();
                }
            }
#endif
        }

        private static void CarregarFacesPadrao()
        {
            if (_Faces.Count > 0) return;

            foreach (var candidato in _Candidatos)
            {
                var arquivos = Array.ConvertAll(candidato, Environment.ExpandEnvironmentVariables);

                if (Array.TrueForAll(arquivos, File.Exists))
                {
                    Registrar(File.ReadAllBytes(arquivos[0]), File.ReadAllBytes(arquivos[1]), File.ReadAllBytes(arquivos[2]));
                    return;
                }
            }

            throw new InvalidOperationException("A fonte Times New Roman não foi encontrada. Instale a Times New Roman ou a Liberation Serif (pacote fonts-liberation), " +
                "ou informe os arquivos da fonte com DanfeSharp.Fontes.Registrar.");
        }

        private static byte[] ObterFace(String face)
        {
            lock (_Lock)
            {
                CarregarFacesPadrao();
                return _Faces.TryGetValue(face, out byte[] dados) ? dados : null;
            }
        }

#if !NET452
        private class Resolvedor : IFontResolver
        {
            public FontResolverInfo ResolveTypeface(string familyName, bool bold, bool italic)
            {
                if (!String.Equals(familyName, FamiliaFonte.NomeTimesNewRoman, StringComparison.OrdinalIgnoreCase)) return null;

                if (bold) return new FontResolverInfo(FaceNegrito);
                if (italic) return new FontResolverInfo(FaceItalico);
                return new FontResolverInfo(FaceRegular);
            }

            public byte[] GetFont(string faceName) => ObterFace(faceName);
        }
#endif
    }
}
