using System;
using System.IO;
using DanfeSharp.Graphics;

namespace DanfeSharp
{
    /// <summary>
    /// Configuração das fontes usadas no DANFE.
    /// </summary>
    public static class Fontes
    {
        /// <summary>
        /// Informa os arquivos TrueType a serem usados no lugar da Times New Roman no .NET 8+, por exemplo em containers Linux
        /// sem a Times New Roman ou a Liberation Serif instaladas. Deve ser chamado antes de gerar o primeiro DANFE.
        /// No .NET Framework são usadas as fontes instaladas no Windows e esta configuração é ignorada.
        /// </summary>
        public static void Registrar(String arquivoRegular, String arquivoNegrito, String arquivoItalico)
        {
            ResolvedorFontes.Registrar(File.ReadAllBytes(arquivoRegular), File.ReadAllBytes(arquivoNegrito), File.ReadAllBytes(arquivoItalico));
        }

        /// <summary>
        /// Informa o conteúdo dos arquivos TrueType a serem usados no lugar da Times New Roman no .NET 8+.
        /// </summary>
        public static void Registrar(byte[] regular, byte[] negrito, byte[] italico)
        {
            ResolvedorFontes.Registrar(regular, negrito, italico);
        }
    }
}
