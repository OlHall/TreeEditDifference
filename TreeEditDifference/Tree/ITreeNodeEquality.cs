namespace Algorithms.TreeDifference.Tree
{
    public interface ITreeNodeEquality<T>
    {
        bool EqualTo(T a, T b);
    }
}
