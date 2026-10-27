using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using DanfeSharp.Esquemas;

namespace DanfeSharp.CTe
{
    // Subconjunto do leiaute do CT-e 4.00 (modelo 57) usado no DACTE.

    /// <summary>
    /// CT-e processado
    /// </summary>
    [XmlType(Namespace = Namespaces.CTe)]
    [XmlRoot("cteProc", Namespace = Namespaces.CTe, IsNullable = false)]
    public class CteProc
    {
        public Cte CTe { get; set; }
        public ProtCte protCTe { get; set; }

        [XmlAttribute]
        public string versao { get; set; }
    }

    [Serializable]
    [XmlType(Namespace = Namespaces.CTe)]
    public class Cte
    {
        public InfCte infCte { get; set; }
        public InfCteSupl infCTeSupl { get; set; }
    }

    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Namespaces.CTe)]
    public class InfCteSupl
    {
        /// <summary>
        /// Texto do QR Code impresso no DACTE
        /// </summary>
        public string qrCodCTe { get; set; }
    }

    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Namespaces.CTe)]
    public class ProtCte
    {
        public InfProtCte infProt { get; set; }
    }

    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Namespaces.CTe)]
    public class InfProtCte
    {
        public int tpAmb { get; set; }
        public string chCTe { get; set; }
        public DateTimeOffsetIso8601? dhRecbto { get; set; }
        public string nProt { get; set; }
        public int cStat { get; set; }
        public string xMotivo { get; set; }
    }

    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Namespaces.CTe)]
    public class InfCte
    {
        public IdeCte ide { get; set; }
        public ComplCte compl { get; set; }
        public ParticipanteCte emit { get; set; }
        public ParticipanteCte rem { get; set; }
        public ParticipanteCte exped { get; set; }
        public ParticipanteCte receb { get; set; }
        public ParticipanteCte dest { get; set; }
        public VPrest vPrest { get; set; }
        public ImpCte imp { get; set; }
        public InfCteNorm infCTeNorm { get; set; }
        public InfCteComp infCteComp { get; set; }

        [XmlAttribute(DataType = "ID")]
        public string Id { get; set; }

        [XmlAttribute]
        public string versao { get; set; }
    }

    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Namespaces.CTe)]
    public class IdeCte
    {
        public string CFOP { get; set; }
        public string natOp { get; set; }
        public int mod { get; set; }
        public int serie { get; set; }
        public int nCT { get; set; }
        public DateTimeOffsetIso8601? dhEmi { get; set; }
        public int tpImp { get; set; }
        public int tpEmis { get; set; }
        public int tpAmb { get; set; }
        public int tpCTe { get; set; }
        public string modal { get; set; }
        public int tpServ { get; set; }
        public string xMunIni { get; set; }
        public string UFIni { get; set; }
        public string xMunFim { get; set; }
        public string UFFim { get; set; }
        public Toma3 toma3 { get; set; }
        public Toma4 toma4 { get; set; }
        public DateTimeOffsetIso8601? dhCont { get; set; }
        public string xJust { get; set; }
    }

    /// <summary>
    /// Tomador do serviço que é um dos participantes: 0-Remetente; 1-Expedidor; 2-Recebedor; 3-Destinatário
    /// </summary>
    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Namespaces.CTe)]
    public class Toma3
    {
        public int toma { get; set; }
    }

    /// <summary>
    /// Tomador do serviço que não é um dos participantes (toma = 4).
    /// </summary>
    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Namespaces.CTe)]
    public class Toma4 : ParticipanteCte
    {
        public int toma { get; set; }

        [XmlElement("enderToma")]
        public EnderecoCte enderToma { get => Endereco; set => Endereco = value; }
    }

    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Namespaces.CTe)]
    public class ComplCte
    {
        public string xCaracAd { get; set; }
        public string xCaracSer { get; set; }
        public string xEmi { get; set; }
        public string xObs { get; set; }

        [XmlElement("ObsCont")]
        public List<ObsCte> ObsCont { get; set; }

        [XmlElement("ObsFisco")]
        public List<ObsCte> ObsFisco { get; set; }

        public ComplCte()
        {
            ObsCont = new List<ObsCte>();
            ObsFisco = new List<ObsCte>();
        }
    }

    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Namespaces.CTe)]
    public class ObsCte
    {
        public string xTexto { get; set; }

        [XmlAttribute]
        public string xCampo { get; set; }
    }

    /// <summary>
    /// Emitente, remetente, expedidor, recebedor e destinatário. Cada um tem o endereço numa tag com nome próprio.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Namespaces.CTe)]
    public class ParticipanteCte
    {
        public string CNPJ { get; set; }
        public string CPF { get; set; }
        public string IE { get; set; }
        public string xNome { get; set; }
        public string xFant { get; set; }
        public string fone { get; set; }
        public string email { get; set; }

        [XmlElement("enderEmit")]
        public EnderecoCte enderEmit { get => Endereco; set => Endereco = value; }

        [XmlElement("enderReme")]
        public EnderecoCte enderReme { get => Endereco; set => Endereco = value; }

        [XmlElement("enderExped")]
        public EnderecoCte enderExped { get => Endereco; set => Endereco = value; }

        [XmlElement("enderReceb")]
        public EnderecoCte enderReceb { get => Endereco; set => Endereco = value; }

        [XmlElement("enderDest")]
        public EnderecoCte enderDest { get => Endereco; set => Endereco = value; }

        [XmlIgnore]
        public EnderecoCte Endereco { get; set; }

        // Evita que o XmlSerializer escreva o mesmo endereço em todas as tags.
        public bool ShouldSerializeenderEmit() => false;
        public bool ShouldSerializeenderReme() => false;
        public bool ShouldSerializeenderExped() => false;
        public bool ShouldSerializeenderReceb() => false;
        public bool ShouldSerializeenderDest() => false;
    }

    [Serializable]
    [XmlType(Namespace = Namespaces.CTe)]
    public class EnderecoCte
    {
        public string xLgr { get; set; }
        public string nro { get; set; }
        public string xCpl { get; set; }
        public string xBairro { get; set; }
        public string xMun { get; set; }
        public string CEP { get; set; }
        public string UF { get; set; }
        public string xPais { get; set; }
    }

    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Namespaces.CTe)]
    public class VPrest
    {
        public double vTPrest { get; set; }
        public double vRec { get; set; }

        [XmlElement("Comp")]
        public List<ComponenteCte> Comp { get; set; }

        public VPrest()
        {
            Comp = new List<ComponenteCte>();
        }
    }

    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Namespaces.CTe)]
    public class ComponenteCte
    {
        public string xNome { get; set; }
        public double vComp { get; set; }
    }

    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Namespaces.CTe)]
    public class ImpCte
    {
        public IcmsCteGrupo ICMS { get; set; }
        public double? vTotTrib { get; set; }
        public string infAdFisco { get; set; }

        /// <summary>
        /// Grupo do IBS e da CBS (Reforma Tributária)
        /// </summary>
        public IbsCbsCte IBSCBS { get; set; }
    }

    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Namespaces.CTe)]
    public class IcmsCteGrupo
    {
        [XmlElement("ICMS00", typeof(IcmsCte00))]
        [XmlElement("ICMS20", typeof(IcmsCte20))]
        [XmlElement("ICMS45", typeof(IcmsCte45))]
        [XmlElement("ICMS60", typeof(IcmsCte60))]
        [XmlElement("ICMS90", typeof(IcmsCte90))]
        [XmlElement("ICMSOutraUF", typeof(IcmsCteOutraUF))]
        [XmlElement("ICMSSN", typeof(IcmsCteSN))]
        public IcmsCte ICMS { get; set; }
    }

    /// <summary>
    /// Campos de todas as variantes do ICMS do CT-e.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Namespaces.CTe)]
    public class IcmsCte
    {
        public string CST { get; set; }
        public double? pRedBC { get; set; }
        public double? vBC { get; set; }
        public double? pICMS { get; set; }
        public double? vICMS { get; set; }
        public double? vBCSTRet { get; set; }
        public double? vICMSSTRet { get; set; }
        public double? pICMSSTRet { get; set; }
        public double? vCred { get; set; }
        public double? pRedBCOutraUF { get; set; }
        public double? vBCOutraUF { get; set; }
        public double? pICMSOutraUF { get; set; }
        public double? vICMSOutraUF { get; set; }
        public int? indSN { get; set; }
    }

    public class IcmsCte00 : IcmsCte { }
    public class IcmsCte20 : IcmsCte { }
    public class IcmsCte45 : IcmsCte { }
    public class IcmsCte60 : IcmsCte { }
    public class IcmsCte90 : IcmsCte { }
    public class IcmsCteOutraUF : IcmsCte { }
    public class IcmsCteSN : IcmsCte { }

    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Namespaces.CTe)]
    public class IbsCbsCte
    {
        public string CST { get; set; }
        public string cClassTrib { get; set; }
        public GrupoIbsCbsCte gIBSCBS { get; set; }
    }

    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Namespaces.CTe)]
    public class GrupoIbsCbsCte
    {
        public double? vBC { get; set; }
        public TributoIbsCbsCte gIBSUF { get; set; }
        public TributoIbsCbsCte gIBSMun { get; set; }
        public double? vIBS { get; set; }
        public TributoIbsCbsCte gCBS { get; set; }
    }

    /// <summary>
    /// Campos dos grupos gIBSUF, gIBSMun e gCBS do CT-e.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Namespaces.CTe)]
    public class TributoIbsCbsCte
    {
        public double? pIBSUF { get; set; }
        public double? vIBSUF { get; set; }
        public double? pIBSMun { get; set; }
        public double? vIBSMun { get; set; }
        public double? pCBS { get; set; }
        public double? vCBS { get; set; }
        public ReducaoCte gRed { get; set; }

        [XmlIgnore]
        public double? Aliquota => gRed?.pAliqEfet ?? pIBSUF ?? pIBSMun ?? pCBS;

        [XmlIgnore]
        public double? Valor => vIBSUF ?? vIBSMun ?? vCBS;
    }

    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Namespaces.CTe)]
    public class ReducaoCte
    {
        public double? pRedAliq { get; set; }
        public double? pAliqEfet { get; set; }
    }

    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Namespaces.CTe)]
    public class InfCteNorm
    {
        public InfCarga infCarga { get; set; }
        public InfDocCte infDoc { get; set; }
        public InfModal infModal { get; set; }
    }

    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Namespaces.CTe)]
    public class InfCteComp
    {
        public string chCTe { get; set; }
    }

    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Namespaces.CTe)]
    public class InfCarga
    {
        public double? vCarga { get; set; }
        public string proPred { get; set; }
        public string xOutCat { get; set; }

        [XmlElement("infQ")]
        public List<InfQ> infQ { get; set; }

        public InfCarga()
        {
            infQ = new List<InfQ>();
        }
    }

    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Namespaces.CTe)]
    public class InfQ
    {
        /// <summary>
        /// Unidade de medida: 00-M3; 01-KG; 02-TON; 03-UNIDADE; 04-LITROS; 05-MMBTU
        /// </summary>
        public string cUnid { get; set; }
        public string tpMed { get; set; }
        public double qCarga { get; set; }
    }

    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Namespaces.CTe)]
    public class InfDocCte
    {
        [XmlElement("infNF")]
        public List<InfNFCte> infNF { get; set; }

        [XmlElement("infNFe")]
        public List<InfNFeCte> infNFe { get; set; }

        [XmlElement("infOutros")]
        public List<InfOutrosCte> infOutros { get; set; }

        public InfDocCte()
        {
            infNF = new List<InfNFCte>();
            infNFe = new List<InfNFeCte>();
            infOutros = new List<InfOutrosCte>();
        }
    }

    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Namespaces.CTe)]
    public class InfNFCte
    {
        public string mod { get; set; }
        public string serie { get; set; }
        public string nDoc { get; set; }
        public double? vNF { get; set; }
    }

    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Namespaces.CTe)]
    public class InfNFeCte
    {
        public string chave { get; set; }
    }

    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Namespaces.CTe)]
    public class InfOutrosCte
    {
        public string tpDoc { get; set; }
        public string descOutros { get; set; }
        public string nDoc { get; set; }
        public double? vDocFisc { get; set; }
    }

    /// <summary>
    /// Informações do modal. O conteúdo depende do modal e é lido de forma genérica.
    /// </summary>
    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Namespaces.CTe)]
    public class InfModal
    {
        [XmlAttribute]
        public string versaoModal { get; set; }

        [XmlAnyElement]
        public XmlElement[] Conteudo { get; set; }
    }
}
