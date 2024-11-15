using Algorithms.TreeDifference;
using Algorithms.TreeDifference.Tree;
using System.Text;
using System.Xml.Linq;

namespace Algorithms.XmlTreeDifference
{
    public class XmlEditOperation
    {
        private readonly Operation _op;
        public Operation Op => _op;

        private readonly XElement? _treeANode;
        public XElement? TreeANode => _treeANode;
        private readonly XElement? _treeBNode;
        public XElement? TreeBNode => _treeBNode;

        public XmlEditOperation(Operation op, Tree<XElement> treeA, Tree<XElement> treeB)
        {
            _op = op;
            _treeANode = op.PostOrderIndex1 > 0 ? treeA[op.PostOrderIndex1].Value : null;
            _treeBNode = op.PostOrderIndex2 > 0 ? treeB[op.PostOrderIndex2].Value : null;
        }

        public string PathA => PathBuilder(_treeANode);
        public string PathB => PathBuilder(_treeBNode);

        private string PathBuilder(XElement? node)
        {
            if(node == null)
            {
                return string.Empty;              
            }
            else
            {
                return PathBuilder(node.Parent) + "/" + node.Name.LocalName;
            }
        }

        public override string ToString()
        {
            if(( _treeANode != null) && (_treeBNode != null))
            {
                if (_op.EditOp == Operation.OpType.Match)
                {
                    return $"{_op.EditOp}: '{FormattedElement(_treeANode)}'";
                }
                else
                {
                    return $"{_op.EditOp}: '{FormattedElement(_treeANode)}' => '{FormattedElement(_treeBNode)}'";
                }
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

        public string AttributeDiffs
        {
            get
            {
                StringBuilder sb = new StringBuilder($"{_op.EditOp}: Attributes differ -");
                if ((_treeANode != null) && (_treeBNode != null))
                {
                    Dictionary<string, string> aAttrs = new();
                    foreach (XAttribute attr in _treeANode.Attributes())
                    {
                        aAttrs.Add(attr.Name.ToString(), attr.Value);
                    }
                    Dictionary<string, string> bAttrs = new();
                    foreach (XAttribute attr in _treeBNode.Attributes())
                    {
                        bAttrs.Add(attr.Name.ToString(), attr.Value);
                    }

                    foreach (string attr in aAttrs.Keys)
                    {
                        if (!bAttrs.ContainsKey(attr))
                        {
                            sb.Append($" -{attr}");
                        }
                        else
                        {
                            if (aAttrs[attr] != bAttrs[attr])
                            {
                                sb.Append($" {attr}:{aAttrs[attr]}!={bAttrs[attr]}");
                            }
                            bAttrs.Remove(attr);
                        }
                    }

                    foreach (string attr in bAttrs.Keys)
                    {
                        sb.Append($" +{attr}");
                    }

                }
                else
                {
                    sb.Append(" Something wrong... empty nodes?");
                }
                return sb.ToString();
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
