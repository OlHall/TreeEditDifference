using Algorithms.TreeDifference.Tree;
using System.Xml.Linq;

namespace Algorithms.XmlTreeDifference
{
    internal class XmlNodeEquality : ITreeNodeEquality<XElement>
    {
        public bool Exact(XElement a, XElement b)
        {
            return true;
        }

        public bool EqualTo(XElement a, XElement b)
        {
            // Fundamental... different element?
            if (a.Name != b.Name)
            {
                return false;
            }

            // Compare attributes
            Dictionary<string, string> aAttrs = new();
            foreach(XAttribute attr in a.Attributes())
            {
                aAttrs.Add(attr.Name.ToString(), attr.Value);
            }
            Dictionary<string, string> bAttrs = new();
            foreach (XAttribute attr in b.Attributes())
            {
                bAttrs.Add(attr.Name.ToString(), attr.Value);
            }

            if( aAttrs.Count != bAttrs.Count)
            {
                return false;
            }

            foreach(string attr in aAttrs.Keys)
            {
                if(bAttrs.ContainsKey(attr))
                {
                    if (aAttrs[attr] != bAttrs[attr] )
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            // Is there any immediate content, and is it different?
            if ((!a.HasElements && !b.HasElements) && (a.Value != b.Value))
            {
                return false;
            }

            // Close enough!
            return true;
        }
    }
}
