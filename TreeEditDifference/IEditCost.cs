using Algorithms.TreeDifference;
using Algorithms.TreeDifference.Tree;

namespace Algorithms.TreeDifference
{
    public interface IEditCost<T>
    {
        int Cost(Operation.OpType editOp, Node<T>? nodeA, Node<T>? nodeB);
    }
}
