using System.IO;
using System.Linq;
using DanfeSharp.Eventos;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DanfeSharp.Test
{
    [TestClass]
    public class EventoTests
    {
        public readonly string OutputDirectory = Path.Combine("Output", "Eventos");
        public readonly string InputDirectory = Path.Combine("Xml", "Eventos");
        public readonly string NFeXml = Path.Combine("Xml", "NFe", "v4.00", "v4_ReformaTributaria.xml");

        public EventoTests()
        {
            if (!Directory.Exists(OutputDirectory))
                Directory.CreateDirectory(OutputDirectory);
        }

        private EventoViewModel Ler(string arquivo, bool comNFe = false) => EventoViewModelCreator.CriarDeArquivoXml(Path.Combine(InputDirectory, arquivo), comNFe ? NFeXml : null);

        private void Gerar(EventoViewModel model, string nomePdf, bool comLogo = false)
        {
            using (var doc = new DocumentoEvento(model))
            {
                if (comLogo) doc.AdicionarLogoImagem(Path.Combine("Logos", "JPG", "Quadrado.jpg"));
                doc.Gerar();
                doc.Salvar(Path.Combine(OutputDirectory, nomePdf));
            }
        }

        [TestMethod]
        public void CartaCorrecao() => Gerar(Ler("cce.xml"), "cce.pdf");

        [TestMethod]
        public void CartaCorrecao_ComNFeELogo() => Gerar(Ler("cce.xml", true), "cce_com_nfe.pdf", true);

        [TestMethod]
        public void Cancelamento() => Gerar(Ler("cancelamento.xml", true), "cancelamento.pdf");

        [TestMethod]
        public void EventoGenerico() => Gerar(Ler("evento_generico.xml"), "evento_generico.pdf");

        [TestMethod]
        public void CartaCorrecao_Modelo()
        {
            var m = Ler("cce.xml", true);

            Assert.AreEqual("110110", m.TipoEvento);
            Assert.IsTrue(m.IsCartaCorrecao);
            Assert.AreEqual("CARTA DE CORREÇÃO ELETRÔNICA", m.Titulo);
            Assert.AreEqual(1, m.Sequencia);
            StringAssert.Contains(m.Correcao, "leia-se Pedido 1243");
            StringAssert.StartsWith(m.CondicaoUso, "A Carta de Correcao e disciplinada");
            Assert.AreEqual(135, m.CodigoStatus);
            Assert.AreEqual("141260000099001", m.ProtocoloEvento);
            Assert.AreEqual("55", m.Modelo);
            Assert.AreEqual("001", m.Serie);
            Assert.AreEqual("000001234", m.Numero);
            Assert.AreEqual("Empresa Exemplo Ltda", m.Documento.Emitente.RazaoSocial);
        }

        [TestMethod]
        public void Cancelamento_Modelo()
        {
            var m = Ler("cancelamento.xml");

            Assert.AreEqual("CANCELAMENTO", m.Titulo);
            Assert.AreEqual("141260000012345", m.ProtocoloDocumento);
            StringAssert.StartsWith(m.Justificativa, "Erro na digitacao");
            Assert.IsNull(m.Documento);
        }

        [TestMethod]
        public void EventoGenerico_Modelo()
        {
            var m = Ler("evento_generico.xml");

            Assert.AreEqual("112110", m.TipoEvento);
            StringAssert.StartsWith(m.Titulo, "INFORMACAO DE EFETIVO PAGAMENTO");
            CollectionAssert.AreEqual(new[] { "cOrgaoAutor", "tpAutor", "verAplic", "indQuitacao" }, m.OutrosCampos.Select(c => c.Key).ToArray());
            Assert.AreEqual("1", m.OutrosCampos.Single(c => c.Key == "indQuitacao").Value);
        }
    }
}
