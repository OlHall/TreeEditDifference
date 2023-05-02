using System.Text;

namespace Algorithms.TreeDifference.Tree
{
    /// <summary>
    /// A generic tree structure for difference calculations
    /// </summary>
    /// <typeparam name="T">The data type of the nodes</typeparam>
    public class Tree<T>
    {
        public readonly List<int> Keyroots = new();

        private readonly Node<T>? _root;

        private readonly Dictionary<int, Node<T>> _postfixNodes = new();

        /// <summary>
        /// Convenience accessor for postfix nodes
        /// </summary>
        /// <param name="i">The postfix index</param>
        /// <returns>The node at the postfix index</returns>
        public Node<T> this[int i]
        {
            get
            {
                CheckIndexBounds(i);
                return _postfixNodes[i];
            }
        }

        public Tree(object source, ITreeParser<T> parser)
        {
            _root = parser.Parse(source);
            if( _root != null)
            {
                TraverseAndComputePostfix(_root, 1);
                ComputeKeyroots();
            }
        }

        private void CheckIndexBounds(int nodeIndex)
        {
            if ((_root == null) || (nodeIndex <= 0) && (nodeIndex > _root.PostfixIndex))
            {
                throw new ArgumentOutOfRangeException(nameof(nodeIndex), "Subtree root indicies must be between 1 and the size of the tree (inclusive)");
            }
        }

        public Node<T>? Root => _root;

        #region Informational

        public int Size => _root != null ? _root.PostfixIndex : 0;
        public int Depth => _root != null ? _root.TreeDepth : 0;

        #endregion

        #region Sub-Trees

        public int SizeOfSubtreeAt(int subtreeRoot)
        {
            CheckIndexBounds(subtreeRoot);
            Node<T> subRoot = _postfixNodes[subtreeRoot];
            return subRoot.PostfixIndex - subRoot.LeftMostIndex + 1;
        }

        public IEnumerable<int> NodeIndicesForSubtreeAt(int subtreeRoot)
        {
            CheckIndexBounds(subtreeRoot);
            Node<T> subRoot = _postfixNodes[subtreeRoot];
            List<int> nodes = new();
            for (int i=subRoot.LeftMostIndex; i<=subRoot.PostfixIndex; i++ )
            {
                nodes.Add(i);
            }
            return nodes;
        }

        public int FirstIndexForSubtreeAt(int subtreeRoot)
        {
            CheckIndexBounds(subtreeRoot);
            return _postfixNodes[subtreeRoot].LeftMostIndex;
        }

        public bool HasMatchingLeftMost(int i, int m)
        {
            return _postfixNodes[i].LeftMostIndex == _postfixNodes[m].LeftMostIndex;
        }

        #endregion

        #region Postfix & Keyroots
        private int TraverseAndComputePostfix(Node<T> node, int postfixIndex)
        {
            foreach (Node<T> child in node.Children)
            {
                postfixIndex = TraverseAndComputePostfix(child, postfixIndex) + 1;
            }
            node.PostfixIndex = postfixIndex;
            _postfixNodes.Add(node.PostfixIndex, node);

            // Assign a left-most leaf to each node
            if (node.Children.Count == 0)
            {
                node.LeftMostIndex = node.PostfixIndex;
            }
            else
            {
                node.LeftMostIndex = node.Children[0].LeftMostIndex;
            }
            return postfixIndex;
        }

        /// <summary>
        /// Working backwards along the postfix nodes, the first occurance of each left-most is the key root
        /// </summary>
        private void ComputeKeyroots()
        {
            HashSet<int> rootSet = new();
            for(int i=_postfixNodes.Count; i>0; --i)
            {
                if (!rootSet.Contains(_postfixNodes[i].LeftMostIndex))
                {
                    rootSet.Add(_postfixNodes[i].LeftMostIndex);
                    Keyroots.Insert(0,i);
                }
            }
        }

        #endregion

        public string DumpKeyRoots()
        {
            StringBuilder sb = new("keyroots(T) = {");
            for(int i = 0; i < Keyroots.Count; ++i)
            {
                sb.Append(Keyroots[i]);
                if( i == Keyroots.Count - 1)
                {
                    sb.AppendLine("}");
                }
                else
                {
                    sb.Append(", ");
                }
            }
            return sb.ToString();
        }

        #region
        public string DumpTree(int indent, string pre)
        {
            if (_root == null)
            {
                return "Empty tree";
            }
            else
            {
                StringBuilder sb = new();
                DumpNode(sb, _root, indent, pre);
                return sb.ToString();
            }
        }

        private void DumpNode(StringBuilder sb, Node<T> node, int indent, string pre )
        {
            sb.AppendLine($"{new string(' ', indent)}{pre}p[{node.PostfixIndex}] l={node.LeftMostIndex} {node.Value}");
            foreach(Node<T> child in node.Children)
            {
                DumpNode(sb, child, indent + 1, pre);
            }
        }
        #endregion
    }
}
