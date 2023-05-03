using Algorithms.XmlTreeDifference;
using System.Reflection;
using System.Xml.Linq;

namespace Algorithms.TreeDifference.Testing
{
    [TestClass]
    public class XmlDifferenceTests
    {
        private static XDocument _doc1 = new();
        private static XDocument _doc2 = new();

        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            using (Stream? xmlDoc = Assembly.GetExecutingAssembly().GetManifestResourceStream("Algorithms.TreeDifference.Testing.TestData.Document1.xml"))
            {
                if (xmlDoc != null)
                    _doc1 = XDocument.Load(xmlDoc);
            }
            using (Stream? xmlDoc = Assembly.GetExecutingAssembly().GetManifestResourceStream("Algorithms.TreeDifference.Testing.TestData.Document2.xml"))
            {
                if (xmlDoc != null)
                    _doc2 = XDocument.Load(xmlDoc);
            }
        }

        [TestMethod]
        public void CompareXml_Same()
        {
            Assert.IsNotNull(_doc1.Root);
            XmlDifference diff = new(_doc1.Root, _doc1.Root);
            Assert.AreEqual(0, diff.EditDifference);
        }

        [TestMethod]
        public void CompareXml_Nodes_Different()
        {
            Assert.IsNotNull(_doc1.Root);
            Assert.IsNotNull(_doc2.Root);
            XmlDifference diff = new(_doc1.Root, _doc2.Root);
            Assert.AreEqual(2, diff.EditDifference);
        }
    }
}
