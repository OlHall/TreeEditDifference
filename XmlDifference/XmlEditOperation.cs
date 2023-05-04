using Algorithms.TreeDifference;
using Algorithms.TreeDifference.Tree;
using System.Xml.Linq;

namespace Algorithms.XmlTreeDifference
{
    public class XmlEditOperation
    {
        private readonly Operation _op;
        private readonly XElement? _treeANode;
        private readonly XElement? _treeBNode;
        public XmlEditOperation(Operation op, Tree<XElement> treeA, Tree<XElement> treeB)
        {
            _op = op;
            _treeANode = op.PostOrderIndex1 > 0 ? treeA[op.PostOrderIndex1].Value : null;
            _treeBNode = op.PostOrderIndex2 > 0 ? treeB[op.PostOrderIndex2].Value : null;
        }

        public override string ToString()
        {
            if(( _treeANode != null) && (_treeBNode != null))
            {
                return $"{_op.EditOp}:\n  '{_treeANode}'\n  => '{_treeBNode}'";
            }
            else if( _treeANode != null )
            {
                return $"{_op.EditOp}:\n  '{_treeANode}'";
            }
            else if (_treeANode != null)
            {
                return $"{_op.EditOp}:\n  '{_treeBNode}'";
            }
            else
            {
                return $"{_op.EditOp} - no nodes?";
            }
        }
    }
}
