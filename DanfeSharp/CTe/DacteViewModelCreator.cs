using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using DanfeSharp.Modelo;

namespace DanfeSharp.CTe
{
    public static class DacteViewModelCreator
    {
        public static DacteViewModel CriarDeArquivoXml(String caminho)
        {
            using (var sr = new StreamReader(caminho, true))
            {
                return CriarDeStringXml(sr.ReadToEnd());
            }
        }

        public static DacteViewModel CriarDeArquivoXml(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            using (var sr = new StreamReader(stream, true))
            {
                return CriarDeStringXml(sr.ReadToEnd());
            }
        }

        public static DacteViewModel CriarDeStringXml(String xml)
        {
            if (xml == null) throw new ArgumentNullException(nameof(xml));

            try
            {
                using (var reader = new StringReader(xml))
                {
                    return CreateFromXml((CteProc)new XmlSerializer(typeof(CteProc)).Deserialize(reader));
                }
            }
            catch (InvalidOperationException e)
            {
                if (e.InnerException is XmlException ex)
                    throw new Exception(String.Format("Não foi possível interpretar o Xml. Linha {0} Posição {1}.", ex.LineNumber, ex.LinePosition), e);

                throw new XmlException("O Xml não parece ser um CT-e processado (cteProc).", e);
            }
        }

        public static DacteViewModel CreateFromXml(CteProc proc)
        {
            var inf = proc?.CTe?.infCte ?? throw new ArgumentException("O CT-e não possui o grupo infCte.", nameof(proc));
            var ide = inf.ide;

            if (ide.mod != 57)
                throw new NotSupportedException("O DACTE é gerado apenas para o CT-e (mod 57).");

            var m = new DacteViewModel
            {
                ChaveAcesso = inf.Id.Substring(3),
                Numero = ide.nCT,
                Serie = ide.serie,
                Modelo = ide.mod,
                DataHoraEmissao = ide.dhEmi?.DateTimeOffsetValue.DateTime,
                Cfop = ide.CFOP,
                NaturezaOperacao = ide.natOp,
                Modal = ide.modal,
                TipoCte = ide.tpCTe,
                TipoServico = ide.tpServ,
                InicioPrestacao = $"{ide.xMunIni} - {ide.UFIni}",
                TerminoPrestacao = $"{ide.xMunFim} - {ide.UFFim}",
                TipoAmbiente = ide.tpAmb,
                TipoEmissao = ide.tpEmis,
                QrCode = proc.CTe.infCTeSupl?.qrCodCTe?.Trim(),
                Emitente = Participante(inf.emit),
                Remetente = Participante(inf.rem),
                Expedidor = Participante(inf.exped),
                Recebedor = Participante(inf.receb),
                Destinatario = Participante(inf.dest),
                ChaveCteComplementado = inf.infCteComp?.chCTe
            };

            ExtrairTomador(m, ide);

            var prot = proc.protCTe?.infProt;
            if (prot != null && !String.IsNullOrWhiteSpace(prot.nProt))
                m.ProtocoloAutorizacao = $"{prot.nProt} - {prot.dhRecbto?.DateTimeOffsetValue.DateTime.ToString("dd/MM/yyyy HH:mm:ss")}";

            var norm = inf.infCTeNorm;
            var carga = norm?.infCarga;

            if (carga != null)
            {
                m.ProdutoPredominante = carga.proPred;
                m.OutrasCaracteristicas = carga.xOutCat;
                m.ValorCarga = carga.vCarga;
                m.Quantidades.AddRange(carga.infQ.Select(q => new QuantidadeCargaViewModel { TipoMedida = q.tpMed, Unidade = Unidade(q.cUnid), Quantidade = q.qCarga }));
            }

            if (inf.vPrest != null)
            {
                m.ValorTotalServico = inf.vPrest.vTPrest;
                m.ValorReceber = inf.vPrest.vRec;
                m.Componentes.AddRange(inf.vPrest.Comp.Select(c => new ComponenteViewModel { Nome = c.xNome, Valor = c.vComp }));
            }

            ExtrairImpostos(m, inf.imp);
            ExtrairDocumentos(m, norm?.infDoc);
            ExtrairModal(m, norm?.infModal);
            m.Observacoes = Observacoes(inf.compl);
            m.InformacoesFisco = String.Join(" ", new[] { inf.imp?.infAdFisco }.Concat(inf.compl?.ObsFisco.Select(o => $"{o.xCampo}: {o.xTexto}") ?? Enumerable.Empty<String>()).Where(s => !String.IsNullOrWhiteSpace(s)));

            return m;
        }

        private static EmpresaViewModel Participante(ParticipanteCte p)
        {
            if (p == null) return null;

            var e = new EmpresaViewModel
            {
                RazaoSocial = p.xNome,
                NomeFantasia = p.xFant,
                CnpjCpf = !String.IsNullOrWhiteSpace(p.CNPJ) ? p.CNPJ : p.CPF,
                Ie = p.IE,
                Telefone = p.fone,
                Email = p.email
            };

            var end = p.Endereco;
            if (end != null)
            {
                e.EnderecoLogadrouro = end.xLgr;
                e.EnderecoNumero = end.nro;
                e.EnderecoComplemento = end.xCpl;
                e.EnderecoBairro = end.xBairro;
                e.Municipio = end.xMun;
                e.EnderecoCep = end.CEP;
                e.EnderecoUf = end.UF;
            }

            return e;
        }

        private static void ExtrairTomador(DacteViewModel m, IdeCte ide)
        {
            if (ide.toma4 != null)
            {
                m.Tomador = Participante(ide.toma4);
                m.TomadorPapel = "OUTROS";
                return;
            }

            switch (ide.toma3?.toma)
            {
                case 0: m.Tomador = m.Remetente; m.TomadorPapel = "REMETENTE"; break;
                case 1: m.Tomador = m.Expedidor; m.TomadorPapel = "EXPEDIDOR"; break;
                case 2: m.Tomador = m.Recebedor; m.TomadorPapel = "RECEBEDOR"; break;
                case 3: m.Tomador = m.Destinatario; m.TomadorPapel = "DESTINATÁRIO"; break;
            }
        }

        private static void ExtrairImpostos(DacteViewModel m, ImpCte imp)
        {
            if (imp == null) return;

            m.ValorAproximadoTributos = imp.vTotTrib;
            var icms = imp.ICMS?.ICMS;

            if (icms != null)
            {
                m.IcmsSituacaoTributaria = SituacaoTributaria(icms);
                m.IcmsBaseCalculo = icms.vBC ?? icms.vBCOutraUF ?? icms.vBCSTRet;
                m.IcmsAliquota = icms.pICMS ?? icms.pICMSOutraUF ?? icms.pICMSSTRet;
                m.IcmsValor = icms.vICMS ?? icms.vICMSOutraUF;
                m.IcmsReducaoBaseCalculo = icms.pRedBC ?? icms.pRedBCOutraUF;
                m.IcmsSt = icms.vICMSSTRet;
            }

            var rt = imp.IBSCBS;
            if (rt != null)
            {
                m.IbsCbsCst = rt.CST;
                m.IbsCbsClassificacaoTributaria = rt.cClassTrib;
                m.IbsCbsBaseCalculo = rt.gIBSCBS?.vBC;
                m.IbsUfAliquota = rt.gIBSCBS?.gIBSUF?.Aliquota;
                m.IbsUfValor = rt.gIBSCBS?.gIBSUF?.Valor;
                m.IbsMunicipioAliquota = rt.gIBSCBS?.gIBSMun?.Aliquota;
                m.IbsMunicipioValor = rt.gIBSCBS?.gIBSMun?.Valor;
                m.CbsAliquota = rt.gIBSCBS?.gCBS?.Aliquota;
                m.CbsValor = rt.gIBSCBS?.gCBS?.Valor;
            }
        }

        private static String SituacaoTributaria(IcmsCte icms)
        {
            if (icms is IcmsCteSN) return "SIMPLES NACIONAL";
            if (icms is IcmsCteOutraUF) return "90 - ICMS DEVIDO À UF DE ORIGEM";

            switch (icms.CST)
            {
                case "00": return "00 - TRIBUTAÇÃO NORMAL";
                case "20": return "20 - COM REDUÇÃO DE BC";
                case "40": return "40 - ISENTA";
                case "41": return "41 - NÃO TRIBUTADA";
                case "51": return "51 - DIFERIDA";
                case "60": return "60 - COBRADO ANTERIORMENTE POR ST";
                case "90": return "90 - OUTROS";
                default: return icms.CST;
            }
        }

        private static void ExtrairDocumentos(DacteViewModel m, InfDocCte doc)
        {
            if (doc == null) return;

            foreach (var nfe in doc.infNFe)
            {
                var chave = nfe.chave ?? String.Empty;
                m.Documentos.Add(new DocumentoOriginarioViewModel
                {
                    Tipo = "NF-e",
                    Identificacao = Formatador.FormatarChaveAcesso(chave),
                    SerieNumero = chave.Length == 44 ? $"{chave.Substring(22, 3)}/{chave.Substring(25, 9)}" : null
                });
            }

            foreach (var nf in doc.infNF)
                m.Documentos.Add(new DocumentoOriginarioViewModel { Tipo = $"NF mod. {nf.mod}", Identificacao = m.Remetente?.CnpjCpf != null ? Formatador.FormatarCpfCnpj(m.Remetente.CnpjCpf) : null, SerieNumero = $"{nf.serie}/{nf.nDoc}" });

            foreach (var o in doc.infOutros)
                m.Documentos.Add(new DocumentoOriginarioViewModel { Tipo = o.tpDoc == "99" ? (o.descOutros ?? "Outros") : $"Outros ({o.tpDoc})", Identificacao = o.descOutros, SerieNumero = o.nDoc });
        }

        private static void ExtrairModal(DacteViewModel m, InfModal modal)
        {
            if (modal?.Conteudo == null) return;

            foreach (var grupo in modal.Conteudo)
            {
                if (grupo.LocalName == "rodo")
                {
                    m.Rntrc = grupo.ChildNodes.OfType<XmlElement>().FirstOrDefault(e => e.LocalName == "RNTRC")?.InnerText.Trim();
                }
                else
                {
                    AdicionarCampos(m.InformacoesModal, grupo, null);
                }
            }
        }

        private static void AdicionarCampos(List<KeyValuePair<String, String>> campos, XmlElement elemento, String prefixo)
        {
            var nome = prefixo == null ? elemento.LocalName : prefixo + "/" + elemento.LocalName;
            var filhos = elemento.ChildNodes.OfType<XmlElement>().ToList();

            if (filhos.Count == 0)
            {
                campos.Add(new KeyValuePair<String, String>(nome, elemento.InnerText.Trim()));
                return;
            }

            foreach (var filho in filhos)
                AdicionarCampos(campos, filho, nome);
        }

        private static String Observacoes(ComplCte compl)
        {
            if (compl == null) return null;

            var sb = new StringBuilder();
            if (!String.IsNullOrWhiteSpace(compl.xObs)) sb.Append(compl.xObs.Trim());

            foreach (var o in compl.ObsCont)
                sb.AppendChaveValor(o.xCampo, o.xTexto);

            if (!String.IsNullOrWhiteSpace(compl.xCaracAd)) sb.AppendChaveValor("Característica adicional do transporte", compl.xCaracAd);
            if (!String.IsNullOrWhiteSpace(compl.xCaracSer)) sb.AppendChaveValor("Característica adicional do serviço", compl.xCaracSer);

            return sb.ToString();
        }

        private static String Unidade(String cUnid)
        {
            switch (cUnid)
            {
                case "00": return "M3";
                case "01": return "KG";
                case "02": return "TON";
                case "03": return "UNIDADE";
                case "04": return "LITROS";
                case "05": return "MMBTU";
                default: return cUnid;
            }
        }
    }
}
