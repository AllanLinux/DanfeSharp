using System;
using System.Collections.Generic;
using DanfeSharp.Modelo;

namespace DanfeSharp.Eventos
{
    /// <summary>
    /// Modelo de dados do documento auxiliar de um evento da NF-e/NFC-e (carta de correção, cancelamento e outros).
    /// </summary>
    public class EventoViewModel
    {
        /// <summary>
        /// <para>Código do tipo de evento, ex.: 110110 (CC-e) e 110111 (cancelamento)</para>
        /// <para>Tag tpEvento</para>
        /// </summary>
        public String TipoEvento { get; set; }

        /// <summary>
        /// <para>Descrição do evento</para>
        /// <para>Tag descEvento</para>
        /// </summary>
        public String DescricaoEvento { get; set; }

        public int Sequencia { get; set; }
        public String ChaveAcesso { get; set; }
        public String CnpjCpfAutor { get; set; }
        public DateTime? DataHoraEvento { get; set; }
        public int Orgao { get; set; }
        public int TipoAmbiente { get; set; }

        /// <summary>
        /// Texto da correção da CC-e.
        /// </summary>
        public String Correcao { get; set; }

        /// <summary>
        /// Condições de uso da CC-e.
        /// </summary>
        public String CondicaoUso { get; set; }

        /// <summary>
        /// Justificativa do cancelamento.
        /// </summary>
        public String Justificativa { get; set; }

        /// <summary>
        /// Protocolo de autorização da NF-e cancelada.
        /// </summary>
        public String ProtocoloDocumento { get; set; }

        /// <summary>
        /// Chave da NF-e substituta, no cancelamento por substituição.
        /// </summary>
        public String ChaveSubstituta { get; set; }

        /// <summary>
        /// Demais campos do detalhe do evento, como nome e valor.
        /// </summary>
        public List<KeyValuePair<String, String>> OutrosCampos { get; set; }

        #region Registro do evento pela SEFAZ

        public int? CodigoStatus { get; set; }
        public String Motivo { get; set; }
        public String ProtocoloEvento { get; set; }
        public DateTime? DataHoraRegistro { get; set; }

        #endregion

        /// <summary>
        /// Dados da NF-e/NFC-e do evento, quando informada, para exibir emitente e destinatário.
        /// </summary>
        public DanfeViewModel Documento { get; set; }

        public bool IsHomologacao => TipoAmbiente == 2;
        public bool IsCartaCorrecao => TipoEvento == "110110";

        /// <summary>
        /// Título do documento auxiliar.
        /// </summary>
        public String Titulo
        {
            get
            {
                switch (TipoEvento)
                {
                    case "110110": return "CARTA DE CORREÇÃO ELETRÔNICA";
                    case "110111": return "CANCELAMENTO";
                    case "110112": return "CANCELAMENTO POR SUBSTITUIÇÃO";
                    default: return String.IsNullOrWhiteSpace(DescricaoEvento) ? $"EVENTO {TipoEvento}" : DescricaoEvento.ToUpperInvariant();
                }
            }
        }

        /// <summary>
        /// Modelo do documento fiscal, extraído da chave de acesso (55 = NF-e, 65 = NFC-e).
        /// </summary>
        public String Modelo => ParteChave(20, 2);
        public String Serie => ParteChave(22, 3);
        public String Numero => ParteChave(25, 9);

        private String ParteChave(int inicio, int tamanho) => ChaveAcesso != null && ChaveAcesso.Length == 44 ? ChaveAcesso.Substring(inicio, tamanho) : null;

        public EventoViewModel()
        {
            OutrosCampos = new List<KeyValuePair<String, String>>();
        }
    }
}
