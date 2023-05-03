using Algorithms.TreeDifference;
using Algorithms.TreeDifference.Tree;
using System.Xml.Linq;

namespace Algorithms.XmlTreeDifference
{
    public class XmlDifference
    {
        private Tree<XElement> _treeA;
        private Tree<XElement> _treeB;

        public XmlDifference(XElement rootA, XElement rootB)
        {
            XmlTreeParser parser = new XmlTreeParser();
            _treeA = new Tree<XElement>(rootA, parser);
            _treeB = new Tree<XElement>(rootB, parser);
        }

        public int EditDifference
        {
            get
            {
                ZhangShasha<XElement, XmlUnitEditCost, XmlNodeEquality> zss = new(_treeA, _treeB);
                return zss.Compare();
            }
        }

//        public List<Operation>
    }
}