using Algorithms.TreeDifference;
using Algorithms.TreeDifference.Tree;
using System.Xml.Linq;

namespace Algorithms.XmlTreeDifference
{
    public class XmlDifference
    {
        private readonly Tree<XElement> _treeA;
        private readonly Tree<XElement> _treeB;
        private List<Operation>? _ops;

        public XmlDifference(XElement rootA, XElement rootB)
        {
            XmlTreeParser parser = new();
            _treeA = new Tree<XElement>(rootA, parser);
            _treeB = new Tree<XElement>(rootB, parser);
        }

        public int EditDifference
        {
            get
            {
                ZhangShasha<XElement, XmlUnitEditCost, XmlNodeEquality> zss = new(_treeA, _treeB);
                int ted = zss.Compare();
                _ops = zss.EditOperations();
                return ted;
            }
        }

        public List<XmlEditOperation> Operations
        {
            get
            {
                if( _ops == null )
                {
                    return new List<XmlEditOperation>();
                }
                else
                {
                    List<XmlEditOperation> ops = new();
                    foreach(Operation op in _ops)
                    {
                        if( op.EditOp != Operation.OpType.Match )
                        {
                            ops.Add(new XmlEditOperation(op, _treeA, _treeB));
                        }
                    }
                    return ops;
                }
            }
        }
    }
}