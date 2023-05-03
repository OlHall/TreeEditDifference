using Algorithms.TreeDifference.Tree;
using System.Xml.Linq;

namespace Algorithms.XmlTreeDifference
{
    internal class XmlTreeParser : ITreeParser<XElement>
    {
        public Node<XElement> Parse(object root)
        {
            return ParseItem((XElement)root, null);
        }

        private Node<XElement> ParseItem(XElement element, Node<XElement>? parent)
        {
            if (element.HasElements)
            {
                return ParseList(element, parent);
            }
            else
            {
                return new Node<XElement>(element, parent);
            }
        }

        private Node<XElement> ParseList(XElement element, Node<XElement>? parent)
        {
            Node<XElement> node = new(element, parent);
            foreach (XElement e in element.Elements())
            {
                node.Children.Add(ParseItem(e, node));
            }
            return node;
        }

    }
}
