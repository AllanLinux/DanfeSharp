using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace DanfeSharp.NFSe
{
    /// <summary>
    /// Nomes dos municípios pelo código do IBGE, pois a NFS-e informa apenas o código no endereço dos participantes.
    /// A tabela é a lista de municípios do IBGE, embutida na biblioteca.
    /// </summary>
    internal static class Municipios
    {
        private const String Recurso = "DanfeSharp.NFSe.municipios.txt.gz";

        private static readonly Dictionary<String, String> _Ufs = new Dictionary<String, String>
        {
            { "11", "RO" }, { "12", "AC" }, { "13", "AM" }, { "14", "RR" }, { "15", "PA" }, { "16", "AP" }, { "17", "TO" },
            { "21", "MA" }, { "22", "PI" }, { "23", "CE" }, { "24", "RN" }, { "25", "PB" }, { "26", "PE" }, { "27", "AL" }, { "28", "SE" }, { "29", "BA" },
            { "31", "MG" }, { "32", "ES" }, { "33", "RJ" }, { "35", "SP" },
            { "41", "PR" }, { "42", "SC" }, { "43", "RS" },
            { "50", "MS" }, { "51", "MT" }, { "52", "GO" }, { "53", "DF" },
        };

        private static readonly Lazy<Dictionary<String, String>> _Nomes = new Lazy<Dictionary<String, String>>(Carregar);

        /// <summary>
        /// Nome do município, ou null quando o código não existe na tabela.
        /// </summary>
        public static String Nome(String codigoIbge)
        {
            if (String.IsNullOrWhiteSpace(codigoIbge)) return null;
            return _Nomes.Value.TryGetValue(codigoIbge.Trim(), out String nome) ? nome : null;
        }

        /// <summary>
        /// Sigla da UF, obtida dos dois primeiros dígitos do código do município.
        /// </summary>
        public static String Uf(String codigoIbge)
        {
            if (String.IsNullOrWhiteSpace(codigoIbge) || codigoIbge.Trim().Length < 2) return null;
            return _Ufs.TryGetValue(codigoIbge.Trim().Substring(0, 2), out String uf) ? uf : null;
        }

        private static Dictionary<String, String> Carregar()
        {
            var nomes = new Dictionary<String, String>();

            using (var recurso = typeof(Municipios).Assembly.GetManifestResourceStream(Recurso))
            {
                if (recurso == null) return nomes;

                using (var gzip = new GZipStream(recurso, CompressionMode.Decompress))
                using (var leitor = new StreamReader(gzip, Encoding.UTF8))
                {
                    String linha;
                    while ((linha = leitor.ReadLine()) != null)
                    {
                        int i = linha.IndexOf(';');
                        if (i > 0) nomes[linha.Substring(0, i)] = linha.Substring(i + 1);
                    }
                }
            }

            return nomes;
        }
    }
}
