using System;
using System.Xml;
using System.Xml.Serialization;
using DanfeSharp.Esquemas;

namespace DanfeSharp.Eventos
{
    /// <summary>
    /// Evento da NF-e/NFC-e processado: evento assinado e retorno da SEFAZ.
    /// </summary>
    [XmlType(Namespace = Namespaces.NFe)]
    [XmlRoot("procEventoNFe", Namespace = Namespaces.NFe, IsNullable = false)]
    public class ProcEventoNFe
    {
        public Evento evento { get; set; }
        public RetEvento retEvento { get; set; }

        [XmlAttribute]
        public string versao { get; set; }
    }

    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Namespaces.NFe)]
    public class Evento
    {
        public InfEvento infEvento { get; set; }
    }

    /// <summary>
    /// Informações do evento - HP06
    /// </summary>
    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Namespaces.NFe)]
    public class InfEvento
    {
        public int cOrgao { get; set; }
        public int tpAmb { get; set; }
        public string CNPJ { get; set; }
        public string CPF { get; set; }
        public string chNFe { get; set; }
        public DateTimeOffsetIso8601? dhEvento { get; set; }
        public string tpEvento { get; set; }
        public int nSeqEvento { get; set; }
        public DetEvento detEvento { get; set; }
    }

    /// <summary>
    /// Detalhe do evento. Os campos conhecidos são mapeados e os demais ficam em <see cref="Outros"/>.
    /// </summary>
    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Namespaces.NFe)]
    public class DetEvento
    {
        public string descEvento { get; set; }

        /// <summary>
        /// Texto da correção da CC-e
        /// </summary>
        public string xCorrecao { get; set; }

        /// <summary>
        /// Condições de uso da CC-e
        /// </summary>
        public string xCondUso { get; set; }

        /// <summary>
        /// Protocolo de autorização da NF-e, no cancelamento
        /// </summary>
        public string nProt { get; set; }

        /// <summary>
        /// Justificativa do cancelamento
        /// </summary>
        public string xJust { get; set; }

        /// <summary>
        /// Chave da NF-e substituta, no cancelamento por substituição
        /// </summary>
        public string chNFeRef { get; set; }

        [XmlAnyElement]
        public XmlElement[] Outros { get; set; }
    }

    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Namespaces.NFe)]
    public class RetEvento
    {
        public InfEventoRetorno infEvento { get; set; }
    }

    /// <summary>
    /// Retorno do registro do evento pela SEFAZ.
    /// </summary>
    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Namespaces.NFe)]
    public class InfEventoRetorno
    {
        public int cStat { get; set; }
        public string xMotivo { get; set; }
        public string xEvento { get; set; }
        public string CNPJDest { get; set; }
        public string CPFDest { get; set; }
        public DateTimeOffsetIso8601? dhRegEvento { get; set; }
        public string nProt { get; set; }
    }
}
