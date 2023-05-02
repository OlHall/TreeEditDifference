using Algorithms.TreeDifference;
using Algorithms.TreeDifference.Tree;

namespace Algorithms.TreeDifference
{
    public interface IEditCost<T>
    {
        int Cost(Operation.Op editOp, Node<T>? nodeA, Node<T>? nodeB);
    }
}
