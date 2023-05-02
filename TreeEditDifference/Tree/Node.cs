namespace Algorithms.TreeDifference.Tree
{
    public class Node<T>
    {
        private readonly Node<T>? _parent;

        public T Value { get; set; }
        public int PostfixIndex { get; set; }
        public int LeftMostIndex { get; set; }
        public List<Node<T>> Children = new();

        #region Informational
        public int TreeSize
        {
            get
            {
                int size = 1;
                foreach (Node<T> node in Children)
                {
                    size += node.TreeSize;
                }
                return size;
            }
        }

        public int TreeDepth
        {
            get
            {
                int depth = 0;
                foreach (Node<T> node in Children)
                {
                    depth = Math.Max(depth,node.TreeDepth);
                }
                return depth+1;
            }
        }

        public int Depth
        {
            get
            {
                if (_parent == null)
                {
                    return 1;
                }
                else
                {
                    return _parent.Depth+1;
                }
            }
        }

        #endregion

        public Node(T value, Node<T>? parent)
        {
            _parent = parent;
            Value = value;
        }
    }
}
