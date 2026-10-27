using DanfeSharp.Modelo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanfeSharp.Test
{
    [TestClass]
    public class DanfeXmlTests
    {
        public readonly string OutputDirectory = Path.Combine("Output", "DeXml");
        public readonly string InputXmlDirectoryPrefix = Path.Combine("Xml", "NFe");

        public DanfeXmlTests()
        {
            if (!Directory.Exists(OutputDirectory))
                Directory.CreateDirectory(OutputDirectory);
        }

        public void TestXml(String xmlPath)
        {
            var outPdfFilePath = Path.Combine(OutputDirectory, Path.GetFileNameWithoutExtension(xmlPath) + ".pdf");
            var model = DanfeViewModelCreator.CriarDeArquivoXml(Path.Combine(InputXmlDirectoryPrefix, xmlPath));
            using (Danfe danfe = new Danfe(model))
            {
                danfe.Gerar();
                danfe.Salvar(outPdfFilePath);
            }
        }

        [TestMethod]
        public void v1() => TestXml("v1.00/v1.xml");

        [TestMethod]
        public void v2_Retrato() => TestXml("v2.00/v2_Retrato.xml");

        [TestMethod]
        public void v3_10_Retrato() => TestXml("v3.10/v3.10_Retrato.xml");

        [TestMethod]
        public void v4_ComLocalEntrega() => TestXml("v4.00/v4_ComLocalEntrega.xml");

        [TestMethod]
        public void v4_ComLocalRetirada() => TestXml("v4.00/v4_ComLocalRetirada.xml");

        [TestMethod]
        public void v4_ReformaTributaria() => TestXml("v4.00/v4_ReformaTributaria.xml");

        [TestMethod]
        public void v4_ReformaTributaria_Paisagem()
        {
            var model = DanfeViewModelCreator.CriarDeArquivoXml(Path.Combine(InputXmlDirectoryPrefix, "v4.00/v4_ReformaTributaria.xml"));
            model.Orientacao = Orientacao.Paisagem;

            using (Danfe danfe = new Danfe(model))
            {
                danfe.Gerar();
                danfe.Salvar(Path.Combine(OutputDirectory, "v4_ReformaTributaria_Paisagem.pdf"));
            }
        }

        [TestMethod]
        public void v4_ReformaTributaria_Totais()
        {
            var model = DanfeViewModelCreator.CriarDeArquivoXml(Path.Combine(InputXmlDirectoryPrefix, "v4.00/v4_ReformaTributaria.xml"));
            var m = model.CalculoImposto;

            Assert.IsTrue(model.UsarLeiauteReformaTributaria);
            Assert.AreEqual(1700.00, m.BaseCalculoIbsCbs);
            Assert.AreEqual(12.60, m.ValorCbs);
            Assert.AreEqual(1.40, m.ValorIbsUf);
            Assert.AreEqual(0.00, m.ValorIbsMunicipio);
            Assert.AreEqual(1.40, m.ValorIbs);
            Assert.AreEqual(20.00, m.ValorImpostoSeletivo);
            Assert.AreEqual(1754.00, m.ValorTotalNotaComIbsCbsIs);
            Assert.IsFalse(m.PossuiMonofasiaIbsCbs);
        }

        [TestMethod]
        public void v4_ReformaTributaria_Itens()
        {
            var model = DanfeViewModelCreator.CriarDeArquivoXml(Path.Combine(InputXmlDirectoryPrefix, "v4.00/v4_ReformaTributaria.xml"));

            var p1 = model.Produtos[0];
            Assert.AreEqual("000", p1.CstIbsCbs);
            Assert.AreEqual("000001", p1.ClassificacaoTributaria);
            Assert.AreEqual(1000.00, p1.BaseIbsCbs);
            Assert.AreEqual(0.10, p1.AliquotaIbsUf);
            Assert.AreEqual(1.00, p1.ValorIbsUf);
            Assert.AreEqual(0.90, p1.AliquotaCbs);
            Assert.AreEqual(9.00, p1.ValorCbs);
            Assert.IsNull(p1.ValorIs);

            // Com gRed informado, a alíquota impressa é a efetiva (NT 2026.010 item 4.3).
            var p2 = model.Produtos[1];
            Assert.AreEqual(0.04, p2.AliquotaIbsUf);
            Assert.AreEqual(0.00, p2.AliquotaIbsMunicipio);
            Assert.AreEqual(0.36, p2.AliquotaCbs);

            var p3 = model.Produtos[2];
            Assert.AreEqual(200.00, p3.BaseIs);
            Assert.AreEqual(10.00, p3.AliquotaIs);
            Assert.AreEqual(20.00, p3.ValorIs);
            Assert.AreEqual(200.00, p3.BaseIpi);
        }

        [TestMethod]
        public void v4_SemReformaTributaria_MantemLeiauteAnterior()
        {
            var model = DanfeViewModelCreator.CriarDeArquivoXml(Path.Combine(InputXmlDirectoryPrefix, "v4.00/v4_ComLocalEntrega.xml"));
            Assert.IsFalse(model.UsarLeiauteReformaTributaria);
        }
    }
}
