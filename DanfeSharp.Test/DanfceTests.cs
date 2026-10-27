using System;
using System.IO;
using System.Linq;
using DanfeSharp.Modelo;
using DanfeSharp.NFCe;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DanfeSharp.Test
{
    [TestClass]
    public class DanfceTests
    {
        public readonly string OutputDirectory = Path.Combine("Output", "NFCe");
        public readonly string InputDirectory = Path.Combine("Xml", "NFCe");

        public DanfceTests()
        {
            if (!Directory.Exists(OutputDirectory))
                Directory.CreateDirectory(OutputDirectory);
        }

        private DanfceViewModel Ler(string arquivo) => DanfceViewModelCreator.CriarDeArquivoXml(Path.Combine(InputDirectory, arquivo));

        private void Gerar(DanfceViewModel model, string nomePdf, bool comLogo = false)
        {
            using (var danfce = new Danfce(model))
            {
                if (comLogo) danfce.AdicionarLogoImagem(Path.Combine("Logos", "JPG", "Horizontal.jpg"));
                danfce.Gerar();
                danfce.Salvar(Path.Combine(OutputDirectory, nomePdf));
            }
        }

        [TestMethod]
        public void Autorizada_80mm() => Gerar(Ler("nfce_autorizada.xml"), "nfce_autorizada_80mm.pdf");

        [TestMethod]
        public void Autorizada_58mm()
        {
            var model = Ler("nfce_autorizada.xml");
            model.LarguraBobina = LarguraBobina.Mm58;
            Gerar(model, "nfce_autorizada_58mm.pdf");
        }

        [TestMethod]
        public void Autorizada_ComLogo() => Gerar(Ler("nfce_autorizada.xml"), "nfce_autorizada_logo.pdf", true);

        [TestMethod]
        public void ContingenciaOffline_80mm() => Gerar(Ler("nfce_contingencia_offline.xml"), "nfce_contingencia_80mm.pdf");

        [TestMethod]
        public void Autorizada_Modelo()
        {
            var m = Ler("nfce_autorizada.xml");

            Assert.AreEqual(321, m.NfNumero);
            Assert.AreEqual("41261012345678000195650010000003211000003217", m.ChaveAcesso);
            Assert.AreEqual(4, m.Produtos.Count);
            Assert.AreEqual(125.11, m.CalculoImposto.ValorTotalProdutos.Value, 0.001);
            Assert.AreEqual(2.00, m.CalculoImposto.Desconto, 0.001);
            Assert.AreEqual(123.11, m.CalculoImposto.ValorTotalNota, 0.001);
            Assert.AreEqual("12345678909", m.Destinatario.CnpjCpf);
            Assert.AreEqual("141260000054321", m.ProtocoloAutorizacao);
            StringAssert.StartsWith(m.QrCode, "https://www.fazenda.pr.gov.br/nfce/qrcode?p=4126");
            Assert.AreEqual("http://www.fazenda.pr.gov.br/nfce/consulta", m.UrlConsulta);

            CollectionAssert.AreEqual(new[] { "Cartão de Crédito", "Dinheiro" }, m.Pagamentos.Select(p => p.Descricao).ToArray());
            Assert.AreEqual(20.00, m.Troco);
            Assert.IsTrue(m.CalculoImposto.PossuiIbsCbsIs);
            Assert.IsTrue(m.IsHomologacao);
            Assert.IsFalse(m.IsContingencia);
        }

        [TestMethod]
        public void ContingenciaOffline_Modelo()
        {
            var m = Ler("nfce_contingencia_offline.xml");

            Assert.IsTrue(m.IsContingencia);
            Assert.IsNull(m.ProtocoloAutorizacao);
            Assert.IsNull(m.Destinatario);
            Assert.AreEqual("Falha de comunicacao com a SEFAZ autorizadora", m.ContingenciaJustificativa);
            Assert.AreEqual("PIX Dinâmico", m.Pagamentos.Single().Descricao);
            Assert.IsFalse(m.CalculoImposto.PossuiIbsCbsIs);
        }

        [TestMethod]
        public void NFe_Modelo55_NaoEhAceita()
        {
            Assert.ThrowsException<NotSupportedException>(() => DanfceViewModelCreator.CriarDeArquivoXml(Path.Combine("Xml", "NFe", "v4.00", "v4_ReformaTributaria.xml")));
        }

        [TestMethod]
        public void NFCe_NaoEhAceitaPeloDanfe()
        {
            Assert.ThrowsException<NotSupportedException>(() => DanfeViewModelCreator.CriarDeArquivoXml(Path.Combine(InputDirectory, "nfce_autorizada.xml")));
        }
    }
}
