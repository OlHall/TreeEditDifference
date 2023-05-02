namespace Algorithms.TreeDifference.Tree
{
    /// <summary>
    /// The Visitor pattern for tree building
    /// </summary>
    /// <typeparam name="T">The sub-type carried by tree nodes</typeparam>
    public interface ITreeParser<T>
    {
        /// <summary>
        /// Take source input and generate an ordered tree
        /// </summary>
        /// <param name="source">The source object</param>
        /// <returns>The root node for the resulting tree</returns>
        Node<T>? Parse(object source);
    }
}
