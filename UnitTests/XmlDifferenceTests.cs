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
        private static XDocument _doc3 = new();
        private static XDocument _doc4 = new();

        [ClassInitialize]
#pragma warning disable IDE0060 // Remove unused parameter
        public static void Setup(TestContext context)
#pragma warning restore IDE0060 // Remove unused parameter
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
            using (Stream? xmlDoc = Assembly.GetExecutingAssembly().GetManifestResourceStream("Algorithms.TreeDifference.Testing.TestData.Document3.xml"))
            {
                if (xmlDoc != null)
                    _doc3 = XDocument.Load(xmlDoc);
            }
            using (Stream? xmlDoc = Assembly.GetExecutingAssembly().GetManifestResourceStream("Algorithms.TreeDifference.Testing.TestData.Document4.xml"))
            {
                if (xmlDoc != null)
                    _doc4 = XDocument.Load(xmlDoc);
            }
        }

        [TestMethod]
        public void CompareXml_Same()
        {
            Assert.IsNotNull(_doc1.Root);
            XmlDifference diff = new(_doc1.Root, _doc1.Root);
            int ted = diff.EditDifference;
            DumpEditOps(diff.Operations);
            Assert.AreEqual(0, ted);
        }

        [TestMethod]
        public void CompareXml_Nodes_Different()
        {
            Assert.IsNotNull(_doc1.Root);
            Assert.IsNotNull(_doc2.Root);
            XmlDifference diff = new(_doc1.Root, _doc2.Root);
            int ted = diff.EditDifference;
            DumpEditOps(diff.Operations);
            Assert.AreEqual(2, ted);
        }

        [TestMethod]
        public void CompareXml_ChangedContent_Different()
        {
            Assert.IsNotNull(_doc2.Root);
            Assert.IsNotNull(_doc3.Root);
            XmlDifference diff = new(_doc2.Root, _doc3.Root);
            int ted = diff.EditDifference;
            DumpEditOps(diff.Operations);
            Assert.AreEqual(1, ted);
        }

        [TestMethod]
        public void CompareXml_ChangedAttr_Different()
        {
            Assert.IsNotNull(_doc3.Root);
            Assert.IsNotNull(_doc4.Root);
            XmlDifference diff = new(_doc3.Root, _doc4.Root);
            int ted = diff.EditDifference;
            DumpEditOps(diff.Operations);
            Assert.AreEqual(1, ted);
        }

        private static void DumpEditOps(List<XmlEditOperation> ops)
        {
            Console.WriteLine("XML Edit Operations");
            if (ops.Count == 0)
            {
                Console.WriteLine("  No Edits");
            }
            else
            {
                foreach (XmlEditOperation op in ops)
                {
                    Console.WriteLine(op);
                }
            }
        }
    }
}
