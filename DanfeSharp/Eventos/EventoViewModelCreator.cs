using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using DanfeSharp.Modelo;

namespace DanfeSharp.Eventos
{
    public static class EventoViewModelCreator
    {
        /// <summary>
        /// Cria o modelo a partir do XML do evento processado (procEventoNFe).
        /// </summary>
        /// <param name="caminhoEvento">XML do evento.</param>
        /// <param name="caminhoNFe">XML da NF-e do evento (opcional), para exibir emitente e destinatário.</param>
        public static EventoViewModel CriarDeArquivoXml(String caminhoEvento, String caminhoNFe = null)
        {
            EventoViewModel model;

            using (var sr = new StreamReader(caminhoEvento, true))
            {
                model = CriarDeStringXml(sr.ReadToEnd());
            }

            if (!String.IsNullOrWhiteSpace(caminhoNFe))
            {
                model.Documento = DanfeViewModelCreator.CriarDeArquivoXml(caminhoNFe);
            }

            return model;
        }

        public static EventoViewModel CriarDeStringXml(String xml)
        {
            if (xml == null) throw new ArgumentNullException(nameof(xml));

            try
            {
                using (var reader = new StringReader(xml))
                {
                    var proc = (ProcEventoNFe)new XmlSerializer(typeof(ProcEventoNFe)).Deserialize(reader);
                    return CreateFromXml(proc);
                }
            }
            catch (InvalidOperationException e)
            {
                if (e.InnerException is XmlException ex)
                    throw new Exception(String.Format("Não foi possível interpretar o Xml. Linha {0} Posição {1}.", ex.LineNumber, ex.LinePosition), e);

                throw new XmlException("O Xml não parece ser um evento processado da NF-e (procEventoNFe).", e);
            }
        }

        public static EventoViewModel CreateFromXml(ProcEventoNFe proc)
        {
            if (proc?.evento?.infEvento == null) throw new ArgumentException("O evento não possui o grupo infEvento.", nameof(proc));

            var inf = proc.evento.infEvento;
            var det = inf.detEvento ?? new DetEvento();

            var model = new EventoViewModel
            {
                TipoEvento = inf.tpEvento,
                DescricaoEvento = det.descEvento,
                Sequencia = inf.nSeqEvento,
                ChaveAcesso = inf.chNFe,
                CnpjCpfAutor = !String.IsNullOrWhiteSpace(inf.CNPJ) ? inf.CNPJ : inf.CPF,
                DataHoraEvento = inf.dhEvento?.DateTimeOffsetValue.DateTime,
                Orgao = inf.cOrgao,
                TipoAmbiente = inf.tpAmb,
                Correcao = det.xCorrecao,
                CondicaoUso = det.xCondUso,
                Justificativa = det.xJust,
                ProtocoloDocumento = det.nProt,
                ChaveSubstituta = det.chNFeRef
            };

            if (det.Outros != null)
            {
                foreach (var elemento in det.Outros)
                    AdicionarCampos(model.OutrosCampos, elemento, null);
            }

            var ret = proc.retEvento?.infEvento;

            if (ret != null)
            {
                model.CodigoStatus = ret.cStat;
                model.Motivo = ret.xMotivo;
                model.ProtocoloEvento = ret.nProt;
                model.DataHoraRegistro = ret.dhRegEvento?.DateTimeOffsetValue.DateTime;
            }

            return model;
        }

        /// <summary>
        /// Achata um grupo XML em pares "caminho: valor", um para cada elemento com texto.
        /// </summary>
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
    }
}
