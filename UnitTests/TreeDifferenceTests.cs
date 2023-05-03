using Algorithms.TreeDifference.Tree;
using System.Text;

namespace Algorithms.TreeDifference.Testing
{
    public static class CharTreeExtensions
    {
        public static string TreeString(this Tree<char> tree, string fmt = "")
        {
            if (tree.Root == null)
            {
                return "Empty tree";
            }
            else
            {
                return "{" + tree.Root.NodeString(fmt) + "}";
            }
        }
        public static string NodeString(this Node<char> node, string fmt)
        {
            StringBuilder sb = new();
            if(fmt == "p")
            {
                sb.Append($"{node.PostfixIndex}:");
            }
            sb.Append(node.Value);
            if( node.Children.Count > 0 )
            {
                sb.Append('{');
                for (int i= 0; i < node.Children.Count; ++i)
                {
                    if(( fmt == "p" ) && ( i > 0))
                    {
                        sb.Append(", ");
                    }
                    sb.Append(node.Children[i].NodeString(fmt));
                }
                sb.Append('}');
            }
            return sb.ToString();
        }
    }

    [TestClass]
    public class TreeDifferenceTests
    {
        #region String to Char Tree Conversion and Comparator
        public class CharTreeParser : ITreeParser<char>
        {
            public Node<char>? Parse(object input)
            {
                return Parse((string)input);
            }

            private static Node<char>? Parse(string input)
            {
                Node<char>? root = null;
                Stack<Node<char>> nodes = new();
                foreach (char ch in input)
                {
                    switch (ch)
                    {
                        case '{':
                            {
                                if (root != null)
                                {
                                    if (nodes.Count == 0)
                                    {
                                        Assert.IsNotNull(root);
                                        nodes.Push(root);
                                    }
                                    else
                                    {
                                        Node<char> node = nodes.Peek();
#pragma warning disable IDE0056 // Use index operator
                                        node = node.Children[node.Children.Count - 1];
#pragma warning restore IDE0056 // Use index operator
                                        nodes.Push(node);
                                    }
                                }
                                break;
                            }
                        case '}':
                            {
                                if (nodes.Count > 0)
                                {
                                    nodes.Pop();
                                }
                                break;
                            }
                        case ' ': break;
                        default:
                            {
                                // Any other char
                                if (nodes.Count == 0)
                                {
                                    // Root node has no parent
                                    root = new Node<char>(ch, null);
                                }
                                else
                                {
                                    // Child nodes add with parent
                                    nodes.Peek().Children.Add(new Node<char>(ch, nodes.Peek()));
                                }
                                break;
                            }
                    }
                }

                if (nodes.Count == 0)
                    return root;
                else
                    throw new Exception("Bad input");
            }
        }

        public class CharTreeEquality : ITreeNodeEquality<char>
        {
            public bool EqualTo(char a, char b)
            {
                return a == b;
            }
        }

        public class CharEditCost : AUnitEditCost<char>
        {
        }

        #endregion

        private const string zssPaperExA = "{f{d{ac{b}}e}}";
        private const string zssPaperExB = "{f{c{d{ab}}e}}";
        private const int zssPaperTED = 2;

        [DataTestMethod]
        [DataRow("{a}", 1, 1)]
        [DataRow("{a{b}}", 2, 2)]
        [DataRow("{a{bcd}}", 4, 2)]
        [DataRow(zssPaperExA, 6, 4)]
        public void ReadTree_StringParser(string input, int size, int depth)
        {
            CharTreeParser parser = new();
            Tree<char> tree = new(input, parser);
            Console.WriteLine(tree.TreeString());
            Console.WriteLine(tree.TreeString("p"));
            Assert.AreEqual(size, tree.Size);
            Assert.AreEqual(depth, tree.Depth);
        }

        [DataTestMethod]
        [DataRow("{e{fg{i}}}", "{e{fg{h}}}", 1)] // Simple failure
        [DataRow("{f{d{ac{b}}e}}", "{f{ab{c}}}", 4)] // 09 - Temp\tree-similarity-master\build\Debug>ted.exe "string" "{f{d{ac{b}}e}}" "{f{ab{c}}}"
        [DataRow("{a{b{cd}e{fg{i}}}}", "{a{b{cd}e{fg{h}}}}", 1)]
        [DataRow("{d}", "{g{h}}", 2)]
        [DataRow("{f{a{hc{i}}e}}","{f{a{dc{b}}e}}",2)] // Another (broken!) example  - https://pythonhosted.org/zss/
        [DataRow(zssPaperExA, zssPaperExB, zssPaperTED)]  // Distance: 2 (main example used in the Zhang-Shasha paper)
        public void CharTree_EditDistance(string inputA, string inputB, int treeDist)
        {
            CharTreeParser parser = new();
            Tree<char> treeA = new(inputA, parser);
            Tree<char> treeB = new(inputB, parser);

            Console.WriteLine("Tree A");
            Console.WriteLine(treeA.DumpTree(1, "-"));
            Console.WriteLine(treeA.DumpKeyRoots());
            Console.WriteLine("Tree B");
            Console.WriteLine(treeB.DumpTree(1, "-"));
            Console.WriteLine(treeB.DumpKeyRoots());

            ZhangShasha<char, CharEditCost, CharTreeEquality> algo = new(treeA, treeB);
            Assert.AreEqual(treeDist, algo.Compare());
        }

        [TestMethod]
        public void CharTree_KeyRoots_exPaper()
        {
            CharTreeParser parser = new();
            Tree<char> treeA = new(zssPaperExA, parser);
            Tree<char> treeB = new(zssPaperExB, parser);

            Console.WriteLine("Tree A");
            Console.WriteLine(treeA.DumpTree(1, "-"));
            Console.WriteLine(treeA.DumpKeyRoots());
            Console.WriteLine("Tree B");
            Console.WriteLine(treeB.DumpTree(1, "-"));
            Console.WriteLine(treeB.DumpKeyRoots());

            CollectionAssert.AreEqual(new int[] { 3, 5, 6 }, treeA.Keyroots);
            CollectionAssert.AreEqual(new int[] { 2, 5, 6 }, treeB.Keyroots);
        }

        [TestMethod]
        public void CharTree_KeyRoots_NotBroken()
        {
            CharTreeParser parser = new();
            Tree<char> treeA = new("{a{b{c d} e{f g{i}}}", parser);
            Tree<char> treeB = new("{a{b{c d} e{f g{h}}}", parser);

            Console.WriteLine("Tree A");
            Console.WriteLine(treeA.DumpTree(1, "-"));
            Console.WriteLine(treeA.DumpKeyRoots());
            Console.WriteLine("Tree B");
            Console.WriteLine(treeB.DumpTree(1, "-"));
            Console.WriteLine(treeB.DumpKeyRoots());

            CollectionAssert.AreEqual(new int[] { 2, 6, 7, 8 }, treeA.Keyroots);
            CollectionAssert.AreEqual(new int[] { 2, 6, 7, 8 }, treeB.Keyroots);
        }

        [TestMethod]
        public void CharTree_Operations_exPaper()
        {
            CharTreeParser parser = new();
            Tree<char> treeA = new(zssPaperExA, parser);
            Tree<char> treeB = new(zssPaperExB, parser);
            ZhangShasha<char, CharEditCost, CharTreeEquality> algo = new(treeA, treeB);

            int ted = algo.Compare();
            List<Operation> ops = algo.EditOperations();
            Assert.AreEqual(zssPaperTED, ted);
            Assert.AreEqual(zssPaperTED, ops.Count);
            Assert.AreEqual(Operation.OpType.Delete, ops[0].EditOp);
            Assert.AreEqual(Operation.OpType.Insert, ops[1].EditOp);
        }
    }
}
