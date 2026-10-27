using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using DanfeSharp.Esquemas;
using DanfeSharp.Esquemas.NFe;
using DanfeSharp.Modelo;

namespace DanfeSharp.NFCe
{
    public static class DanfceViewModelCreator
    {
        /// <summary>
        /// Cria o modelo a partir do XML da NFC-e, autorizada (nfeProc) ou emitida em contingência e ainda sem protocolo (NFe).
        /// </summary>
        public static DanfceViewModel CriarDeArquivoXml(String caminho)
        {
            using (var sr = new StreamReader(caminho, true))
            {
                return CriarDeStringXml(sr.ReadToEnd());
            }
        }

        public static DanfceViewModel CriarDeArquivoXml(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            using (var sr = new StreamReader(stream, true))
            {
                return CriarDeStringXml(sr.ReadToEnd());
            }
        }

        public static DanfceViewModel CriarDeStringXml(String xml)
        {
            if (xml == null) throw new ArgumentNullException(nameof(xml));

            try
            {
                var raiz = XDocument.Parse(xml).Root;

                if (raiz.Name.LocalName == "NFe")
                {
                    var serializer = new XmlSerializer(typeof(NFe), new XmlRootAttribute("NFe") { Namespace = Namespaces.NFe });
                    using (var reader = raiz.CreateReader())
                    {
                        return CreateFromXml((NFe)serializer.Deserialize(reader), null);
                    }
                }

                using (var reader = raiz.CreateReader())
                {
                    var proc = (ProcNFe)new XmlSerializer(typeof(ProcNFe)).Deserialize(reader);
                    return CreateFromXml(proc.NFe, proc.protNFe);
                }
            }
            catch (XmlException e)
            {
                throw new Exception(String.Format("Não foi possível interpretar o Xml. Linha {0} Posição {1}.", e.LineNumber, e.LinePosition), e);
            }
            catch (InvalidOperationException e)
            {
                throw new XmlException("O Xml não parece ser uma NFC-e.", e);
            }
        }

        public static DanfceViewModel CreateFromXml(NFe nfe, ProtNFe protNfe)
        {
            if (nfe == null) throw new ArgumentNullException(nameof(nfe));

            var infNfe = nfe.infNFe;
            var ide = infNfe.ide;

            if (ide.mod != 65)
            {
                throw new NotSupportedException("O DANFE NFC-e é gerado apenas para a NFC-e (mod 65). Para a NF-e (mod 55), use DanfeSharp.Danfe.");
            }

            var model = new DanfceViewModel
            {
                NfNumero = ide.nNF,
                NfSerie = ide.serie,
                ChaveAcesso = infNfe.Id.Substring(3),
                TipoAmbiente = (int)ide.tpAmb,
                TipoEmissao = ide.tpEmis,
                Emitente = DanfeViewModelCreator.CreateEmpresaFrom(infNfe.emit),
                QrCode = nfe.infNFeSupl?.qrCode?.Trim(),
                UrlConsulta = nfe.infNFeSupl?.urlChave?.Trim(),
                InformacoesComplementares = infNfe.infAdic?.infCpl,
                InformacoesAdicionaisFisco = infNfe.infAdic?.infAdFisco
            };

            model.DataHoraEmissao = ide.dhEmi?.DateTimeOffsetValue.DateTime ?? ide.dEmi;

            if (infNfe.dest != null && (!String.IsNullOrWhiteSpace(infNfe.dest.CNPJ) || !String.IsNullOrWhiteSpace(infNfe.dest.CPF) || !String.IsNullOrWhiteSpace(infNfe.dest.xNome)))
            {
                model.Destinatario = DanfeViewModelCreator.CreateEmpresaFrom(infNfe.dest);
            }

            foreach (var det in infNfe.det)
            {
                model.Produtos.Add(DanfeViewModelCreator.CriarProduto(det));
            }

            model.CalculoImposto = DanfeViewModelCreator.CriarCalculoImpostoViewModel(infNfe.total.ICMSTot);
            DanfeViewModelCreator.ExtrairTotaisReformaTributaria(model.CalculoImposto, infNfe.total);

            if (infNfe.pag != null)
            {
                model.Pagamentos.AddRange(infNfe.pag.detPag.Select(p => new PagamentoViewModel
                {
                    Codigo = p.tPag,
                    Descricao = DanfceViewModel.DescricaoFormaPagamento(p.tPag, p.xPag),
                    Valor = p.vPag
                }));

                model.Troco = infNfe.pag.vTroco;
            }

            if (model.IsContingencia)
            {
                model.ContingenciaDataHora = ide.dhCont?.DateTimeOffsetValue.DateTime;
                model.ContingenciaJustificativa = ide.xJust;
            }

            var infProt = protNfe?.infProt;

            if (infProt != null && !String.IsNullOrWhiteSpace(infProt.nProt))
            {
                model.ProtocoloAutorizacao = infProt.nProt;
                model.DataHoraAutorizacao = infProt.dhRecbto.DateTimeOffsetValue.DateTime;
            }

            return model;
        }
    }
}
