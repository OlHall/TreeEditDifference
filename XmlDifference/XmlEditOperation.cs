using Algorithms.TreeDifference;
using Algorithms.TreeDifference.Tree;
using System.Text;
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
                return $"{_op.EditOp}: '{FormattedElement(_treeANode)}'\n     => '{FormattedElement(_treeBNode)}'";
            }
            else if( _treeANode != null )
            {
                return $"{_op.EditOp}: '{FormattedElement(_treeANode)}'";
            }
            else if (_treeBNode != null)
            {
                return $"{_op.EditOp}: '{FormattedElement(_treeBNode)}'";
            }
            else
            {
                return $"{_op.EditOp} - no nodes?";
            }
        }

        private static string FormattedElement(XElement elem)
        {
            if (elem.NodeType == System.Xml.XmlNodeType.Element)
            {
                StringBuilder sb = new("<" + elem.Name);
                foreach ( XAttribute attr in elem.Attributes())
                {
                    sb.Append($" {attr.Name}=\"{attr.Value}\"");
                }
                if (elem.HasElements)
                {
                    sb.Append('>');
                }
                else
                {
                    if (string.IsNullOrEmpty(elem.Value))
                    {
                        sb.Append(" />");
                    }
                    else
                    {
                        sb.Append($">{elem.Value}</{elem.Name}>");
                    }
                }
                return sb.ToString();
            }
            else
            {
                return "<TODO: Not an element?>";
            }
        }
    }
}
