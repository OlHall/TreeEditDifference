using Algorithms.TreeDifference.Tree;

namespace Algorithms.TreeDifference
{
    public abstract class AUnitEditCost<T> : IEditCost<T>
    {
        public virtual int Cost(Operation.Op editOp, Node<T>? nodeA, Node<T>? nodeB)
        {
#pragma warning disable IDE0066 // Convert switch statement to expression
            switch (editOp)
            {
                case Operation.Op.Delete:
                case Operation.Op.Insert:
                case Operation.Op.Change:
                    return 1;
                default:
                    return 0;
            }
#pragma warning restore IDE0066 // Convert switch statement to expression
        }
    }
}
