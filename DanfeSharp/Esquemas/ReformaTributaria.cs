using System;
using System.Xml.Serialization;

namespace DanfeSharp.Esquemas.NFe
{
    // Grupos da Reforma Tributária do Consumo (IBS, CBS e IS), conforme a NT 2025.002-RTC.

    #region Item (Grupo UB)

    /// <summary>
    /// Grupo de informações do Imposto Seletivo (IS) do item - UB01
    /// </summary>
    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Namespaces.NFe)]
    public partial class ImpostoSeletivo
    {
        /// <summary>
        /// Código de Situação Tributária do IS - UB02
        /// </summary>
        public string CSTIS { get; set; }

        /// <summary>
        /// Código de Classificação Tributária do IS - UB03
        /// </summary>
        public string cClassTribIS { get; set; }

        /// <summary>
        /// Valor da Base de Cálculo do IS - UB05
        /// </summary>
        public double? vBCIS { get; set; }

        /// <summary>
        /// Alíquota do IS - UB06
        /// </summary>
        public double? pIS { get; set; }

        /// <summary>
        /// Alíquota específica por unidade de medida apropriada
        /// </summary>
        public double? pISEspec { get; set; }

        public string uTrib { get; set; }
        public double? qTrib { get; set; }

        /// <summary>
        /// Valor do IS - UB11
        /// </summary>
        public double? vIS { get; set; }
    }

    /// <summary>
    /// Grupo de informações do IBS e da CBS do item - UB12
    /// </summary>
    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Namespaces.NFe)]
    public partial class ImpostoIBSCBS
    {
        /// <summary>
        /// Código de Situação Tributária do IBS e da CBS - UB13
        /// </summary>
        public string CST { get; set; }

        /// <summary>
        /// Código de Classificação Tributária do IBS e da CBS - UB14
        /// </summary>
        public string cClassTrib { get; set; }

        /// <summary>
        /// Grupo de informações do IBS e da CBS - UB15
        /// </summary>
        public GrupoIBSCBS gIBSCBS { get; set; }
    }

    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Namespaces.NFe)]
    public partial class GrupoIBSCBS
    {
        /// <summary>
        /// Base de cálculo do IBS e da CBS - UB16
        /// </summary>
        public double? vBC { get; set; }

        public GrupoIBSUF gIBSUF { get; set; }
        public GrupoIBSMun gIBSMun { get; set; }

        /// <summary>
        /// Valor do IBS (UF + Município)
        /// </summary>
        public double? vIBS { get; set; }

        public GrupoCBS gCBS { get; set; }
    }

    /// <summary>
    /// Campos comuns aos grupos gIBSUF, gIBSMun e gCBS.
    /// </summary>
    public abstract class GrupoTributoIBSCBS
    {
        public GrupoDiferimento gDif { get; set; }
        public GrupoDevolucaoTributo gDevTrib { get; set; }

        /// <summary>
        /// Grupo de redução de alíquota, informado na redução de alíquota ou compra governamental.
        /// </summary>
        public GrupoReducaoAliquota gRed { get; set; }

        /// <summary>
        /// Alíquota vigente do tributo.
        /// </summary>
        [XmlIgnore]
        public abstract double? AliquotaVigente { get; }

        /// <summary>
        /// Alíquota que deve ser impressa no DANFE (NT 2026.010 item 4.3): pAliqEfet quando gRed for informado, senão a alíquota vigente.
        /// </summary>
        [XmlIgnore]
        public double? AliquotaDanfe => gRed != null && gRed.pAliqEfet.HasValue ? gRed.pAliqEfet : AliquotaVigente;
    }

    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Namespaces.NFe)]
    public partial class GrupoIBSUF : GrupoTributoIBSCBS
    {
        /// <summary>
        /// Alíquota do IBS de competência da UF - UB18
        /// </summary>
        public double? pIBSUF { get; set; }

        /// <summary>
        /// Valor do IBS de competência da UF - UB35
        /// </summary>
        public double? vIBSUF { get; set; }

        public override double? AliquotaVigente => pIBSUF;
    }

    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Namespaces.NFe)]
    public partial class GrupoIBSMun : GrupoTributoIBSCBS
    {
        /// <summary>
        /// Alíquota do IBS de competência do Município - UB37
        /// </summary>
        public double? pIBSMun { get; set; }

        /// <summary>
        /// Valor do IBS de competência do Município - UB54
        /// </summary>
        public double? vIBSMun { get; set; }

        public override double? AliquotaVigente => pIBSMun;
    }

    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Namespaces.NFe)]
    public partial class GrupoCBS : GrupoTributoIBSCBS
    {
        /// <summary>
        /// Alíquota da CBS - UB56
        /// </summary>
        public double? pCBS { get; set; }

        /// <summary>
        /// Valor da CBS - UB67
        /// </summary>
        public double? vCBS { get; set; }

        public override double? AliquotaVigente => pCBS;
    }

    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Namespaces.NFe)]
    public partial class GrupoDiferimento
    {
        public double? pDif { get; set; }
        public double? vDif { get; set; }
    }

    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Namespaces.NFe)]
    public partial class GrupoDevolucaoTributo
    {
        public double? vDevTrib { get; set; }
    }

    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Namespaces.NFe)]
    public partial class GrupoReducaoAliquota
    {
        /// <summary>
        /// Percentual da redução de alíquota
        /// </summary>
        public double? pRedAliq { get; set; }

        /// <summary>
        /// Alíquota efetiva - UB28 (IBS UF), UB47 (IBS Município), UB66 (CBS)
        /// </summary>
        public double? pAliqEfet { get; set; }
    }

    #endregion

    #region Totais (Grupo W)

    /// <summary>
    /// Grupo total do Imposto Seletivo - W31
    /// </summary>
    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Namespaces.NFe)]
    public partial class ISTotal
    {
        /// <summary>
        /// Valor total do IS - W33
        /// </summary>
        public double? vIS { get; set; }
    }

    /// <summary>
    /// Grupo total do IBS e da CBS - W34
    /// </summary>
    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Namespaces.NFe)]
    public partial class IBSCBSTotal
    {
        /// <summary>
        /// Valor total da BC do IBS e da CBS
        /// </summary>
        public double? vBCIBSCBS { get; set; }

        public IBSTotal gIBS { get; set; }
        public CBSTotal gCBS { get; set; }

        /// <summary>
        /// Grupo total da Monofasia - W57
        /// </summary>
        public MonofasiaTotal gMono { get; set; }
    }

    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Namespaces.NFe)]
    public partial class IBSTotal
    {
        public IBSUFTotal gIBSUF { get; set; }
        public IBSMunTotal gIBSMun { get; set; }

        /// <summary>
        /// Valor total do IBS
        /// </summary>
        public double? vIBS { get; set; }

        public double? vCredPres { get; set; }
        public double? vCredPresCondSus { get; set; }
    }

    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Namespaces.NFe)]
    public partial class IBSUFTotal
    {
        public double? vDif { get; set; }
        public double? vDevTrib { get; set; }

        /// <summary>
        /// Valor total do IBS da UF - W41
        /// </summary>
        public double? vIBSUF { get; set; }
    }

    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Namespaces.NFe)]
    public partial class IBSMunTotal
    {
        public double? vDif { get; set; }
        public double? vDevTrib { get; set; }

        /// <summary>
        /// Valor total do IBS do Município - W46
        /// </summary>
        public double? vIBSMun { get; set; }
    }

    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Namespaces.NFe)]
    public partial class CBSTotal
    {
        public double? vDif { get; set; }
        public double? vDevTrib { get; set; }

        /// <summary>
        /// Valor total da CBS - W56
        /// </summary>
        public double? vCBS { get; set; }

        public double? vCredPres { get; set; }
        public double? vCredPresCondSus { get; set; }
    }

    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Namespaces.NFe)]
    public partial class MonofasiaTotal
    {
        /// <summary>
        /// Valor total do IBS monofásico - W58
        /// </summary>
        public double? vIBSMono { get; set; }

        /// <summary>
        /// Valor total da CBS monofásica - W59
        /// </summary>
        public double? vCBSMono { get; set; }

        /// <summary>
        /// Valor total do IBS monofásico sujeito a retenção - W59a
        /// </summary>
        public double? vIBSMonoReten { get; set; }

        /// <summary>
        /// Valor total da CBS monofásica sujeita a retenção - W59b
        /// </summary>
        public double? vCBSMonoReten { get; set; }

        /// <summary>
        /// Valor do IBS monofásico retido anteriormente
        /// </summary>
        public double? vIBSMonoRet { get; set; }

        /// <summary>
        /// Valor da CBS monofásica retida anteriormente
        /// </summary>
        public double? vCBSMonoRet { get; set; }
    }

    #endregion
}
