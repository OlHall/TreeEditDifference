using Algorithms.TreeDifference.Tree;
using System.Xml.Linq;

namespace Algorithms.XmlTreeDifference
{
    internal class XmlTreeParser : ITreeParser<XElement>
    {
        private readonly bool _descendChildren;

        public XmlTreeParser(bool descendChildren)
        {
            _descendChildren = descendChildren;
        }

        public Node<XElement> Parse(object root)
        {
            return ParseItem((XElement)root, null);
        }

        private Node<XElement> ParseItem(XElement element, Node<XElement>? parent)
        {
            Node<XElement> item = new Node<XElement>(element, parent);

            if (element.HasElements)
            {
                ParseList(item, element);
            }
            return item;
        }

        private void ParseList(Node<XElement> newNode, XElement element)
        {
            foreach (XElement e in element.Elements())
            {
                if( _descendChildren )
                {
                    newNode.Children.Add(ParseItem(e, newNode));
                }
                else
                {
                    newNode.Children.Add(new Node<XElement>(e, newNode));
                }
            }
        }

    }
}
