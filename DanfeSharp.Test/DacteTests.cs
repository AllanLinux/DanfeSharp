using System.IO;
using System.Linq;
using DanfeSharp.CTe;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DanfeSharp.Test
{
    [TestClass]
    public class DacteTests
    {
        public readonly string OutputDirectory = Path.Combine("Output", "CTe");
        public readonly string InputDirectory = Path.Combine("Xml", "CTe");

        public DacteTests()
        {
            if (!Directory.Exists(OutputDirectory))
                Directory.CreateDirectory(OutputDirectory);
        }

        private DacteViewModel Ler(string arquivo) => DacteViewModelCreator.CriarDeArquivoXml(Path.Combine(InputDirectory, arquivo));

        private int Gerar(DacteViewModel model, string nomePdf, bool comLogo = false)
        {
            using (var dacte = new Dacte(model))
            {
                if (comLogo) dacte.AdicionarLogoImagem(Path.Combine("Logos", "JPG", "Horizontal.jpg"));
                dacte.Gerar();
                int folhas = dacte.PdfDocument.PageCount;
                dacte.Salvar(Path.Combine(OutputDirectory, nomePdf));
                return folhas;
            }
        }

        [TestMethod]
        public void Rodoviario() => Assert.AreEqual(1, Gerar(Ler("cte_rodoviario.xml"), "cte_rodoviario.pdf", true));

        [TestMethod]
        public void MuitosDocumentos_ContinuaEmOutrasFolhas() => Assert.IsTrue(Gerar(Ler("cte_muitos_documentos.xml"), "cte_muitos_documentos.pdf") > 1);

        [TestMethod]
        public void Rodoviario_Modelo()
        {
            var m = Ler("cte_rodoviario.xml");

            Assert.AreEqual(4567, m.Numero);
            Assert.AreEqual("RODOVIÁRIO", m.DescricaoModal);
            Assert.AreEqual("12345678", m.Rntrc);
            Assert.AreEqual("Curitiba - PR", m.InicioPrestacao);
            Assert.AreEqual("Sao Paulo - SP", m.TerminoPrestacao);
            Assert.AreEqual("Empresa Exemplo Ltda", m.Remetente.RazaoSocial);
            Assert.AreEqual("Cliente Exemplo S/A", m.Destinatario.RazaoSocial);
            Assert.AreEqual("REMETENTE", m.TomadorPapel);
            Assert.AreSame(m.Remetente, m.Tomador);
            Assert.IsNull(m.Expedidor);

            Assert.AreEqual(1500.00, m.ValorTotalServico);
            Assert.AreEqual(5, m.Componentes.Count);
            Assert.AreEqual(48500.00, m.ValorCarga);
            Assert.AreEqual(4, m.Quantidades.Count);
            Assert.AreEqual("KG", m.Quantidades[0].Unidade);

            Assert.AreEqual("00 - TRIBUTAÇÃO NORMAL", m.IcmsSituacaoTributaria);
            Assert.AreEqual(180.00, m.IcmsValor);
            Assert.IsTrue(m.PossuiIbsCbs);
            Assert.AreEqual(13.50, m.CbsValor);
            Assert.AreEqual(0.90, m.CbsAliquota);

            Assert.AreEqual(3, m.Documentos.Count);
            Assert.AreEqual("NF-e", m.Documentos[0].Tipo);
            Assert.AreEqual("001/000001234", m.Documentos[0].SerieNumero);
            StringAssert.StartsWith(m.ProtocoloAutorizacao, "141260000077001");
            StringAssert.Contains(m.QrCode, "chCTe=");
            StringAssert.Contains(m.Observacoes, "Pedido: PED-2026-0042");
        }

        [TestMethod]
        public void MuitosDocumentos_Modelo()
        {
            var m = Ler("cte_muitos_documentos.xml");

            Assert.AreEqual(80, m.Documentos.Count);
            Assert.AreEqual("AÉREO", m.DescricaoModal);
            Assert.AreEqual("OUTROS", m.TomadorPapel);
            Assert.AreEqual("Operador Logistico Exemplo S/A", m.Tomador.RazaoSocial);
            Assert.AreEqual("Sao Paulo", m.Tomador.Municipio);
            Assert.AreEqual("Armazem Expedidor Ltda", m.Expedidor.RazaoSocial);
            Assert.AreEqual("Centro de Distribuicao Recebedor Ltda", m.Recebedor.RazaoSocial);
            Assert.IsFalse(m.PossuiIbsCbs);
            CollectionAssert.Contains(m.InformacoesModal.Select(c => c.Key).ToList(), "aereo/tarifa/vTarifa");
        }
    }
}
