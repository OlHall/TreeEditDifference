using Algorithms.TreeDifference.Tree;
using System.Xml.Linq;

namespace Algorithms.XmlTreeDifference
{
    internal class XmlNodeEquality : ITreeNodeEquality<XElement>
    {
        public bool EqualTo(XElement a, XElement b)
        {
            return a.Name == b.Name;
        }
    }
}
