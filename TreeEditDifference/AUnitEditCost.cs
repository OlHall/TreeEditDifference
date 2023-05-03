using Algorithms.TreeDifference.Tree;

namespace Algorithms.TreeDifference
{
    public abstract class AUnitEditCost<T> : IEditCost<T>
    {
        public virtual int Cost(Operation.OpType editOp, Node<T>? nodeA, Node<T>? nodeB)
        {
#pragma warning disable IDE0066 // Convert switch statement to expression
            switch (editOp)
            {
                case Operation.OpType.Delete:
                case Operation.OpType.Insert:
                case Operation.OpType.Change:
                    return 1;
                default:
                    return 0;
            }
#pragma warning restore IDE0066 // Convert switch statement to expression
        }
    }
}
