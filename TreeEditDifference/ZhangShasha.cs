using Algorithms.TreeDifference.Tree;

namespace Algorithms.TreeDifference
{
    public class ZhangShasha<T, C, E>
        where C : IEditCost<T>, new()
        where E : ITreeNodeEquality<T>, new()
    {
        private readonly Tree<T> _treeA;
        private readonly Tree<T> _treeB;
        private readonly C _cost;
        private readonly E _check;

        private int[,] _treeDistance;

        public ZhangShasha(Tree<T> treeA, Tree<T> treeB)
        {
            _treeA = treeA;
            _treeB = treeB;
            _check = new E();
            _cost = new C();
            _treeDistance = new int[0,0];
        }

        public int Compare()
        {
            // Set up the dynamic programming table
            _treeDistance = new int[_treeA.Size + 1, _treeB.Size + 1];

            // Solve the sub-problems for each keyroot
            for (int i = 0; i < _treeA.Keyroots.Count; ++i)
            {
                for (int j = 0; j < _treeB.Keyroots.Count; ++j)
                {
                    TreeDistance(_treeA.Keyroots[i], _treeB.Keyroots[j]);
                }
            }

#if DEBUG
            DumpTable(1, 1, _treeA.Size, _treeB.Size, _treeDistance, "TD");
#endif
            return _treeDistance[_treeA.Size, _treeB.Size];
        }

        public Operation[] EditOperations
        {
            get
            {
                throw new NotImplementedException("I'll tackle this once the tree distance is correct!");
            }
        }

        private int TreeDistance(int i, int j)
        {
            // Initialise the forest dynamic programming table
            // Most implementations create a full size table, which seems wasteful
            // The bottom right corner of this table maps on to i,j of the tree distance table
            int treeSizeA = _treeA.SizeOfSubtreeAt(i);
            int treeSizeB = _treeB.SizeOfSubtreeAt(j);

            int[,] fd = new int[treeSizeA+1, treeSizeB+1];

            // Set up the 0th rows/columns with delete/insert costs
            for (int m = 1; m < treeSizeA+1; ++m)
            {
                fd[m, 0] = fd[m - 1, 0] + _cost.Cost(Operation.Op.Delete, null, null);
            }
            for (int n = 1; n < treeSizeB+1; ++n)
            {
                fd[0, n] = fd[0, n - 1] + _cost.Cost(Operation.Op.Insert, null, null);
            }

            // Indicies into smaller fdist table
            for (int fdi = 1, m = _treeA[i].LeftMostIndex; m <= i; ++m, ++fdi)
            {
                for (int fdj = 1, n = _treeB[j].LeftMostIndex; n <= j; ++n, ++fdj)
                {
                    // Work out the various costs
                    // The top adjacent cell plus delete cost
                    int costDel = fd[fdi - 1, fdj] + _cost.Cost(Operation.Op.Delete, _treeA[m], null);
                    // The left adjacent cell plus insert cost
                    int costIns = fd[fdi, fdj - 1] + _cost.Cost(Operation.Op.Insert, null, _treeB[n]);
                    // The top-left diagonal cell plus change cost (if not equal)
                    int costTL = fd[fdi - 1, fdj - 1];

                    // Test to see the nodes being compared fall within the same sub-tree in each tree
                    if ((_treeA.HasMatchingLeftMost(m,i)) && (_treeB.HasMatchingLeftMost(n,j)))
                    {
#if DEBUG
                        Console.WriteLine($"Left Node Match => [{m},{n}]");
#endif

                        int costChg = costTL +
                            (_check.EqualTo(_treeA[m].Value, _treeB[n].Value ) ? 0 : _cost.Cost(Operation.Op.Change, _treeA[m], _treeB[n]));
                        fd[fdi, fdj] = Math.Min(Math.Min(costDel, costIns), costChg);
                        _treeDistance[m, n] = fd[fdi, fdj];
                    }
                    // If not, then use the result from a previously calculated sub-problem
                    else
                    {
                        fd[fdi, fdj] = Math.Min(Math.Min(costDel, costIns), costTL + _treeDistance[m,n]);
                        Console.WriteLine($"TD[{m},{n}] = {fd[fdi, fdj]}");
                    }
                }
            }

#if DEBUG
            DumpTable(_treeA[i].LeftMostIndex, _treeB[j].LeftMostIndex, i, j, fd, "fd");
#endif

            return _treeDistance[i, j];
        }

#if DEBUG
        private void DumpTable(int si, int sj, int i, int j, int[,] table, string label = "  ")
        {
            Console.WriteLine("--");
            Console.WriteLine("SubTree A(i) = " + i + ", B(j) = " + j);

            Console.Write(label + "  ");
            for (int n = 0; n < table.GetLength(1); ++n)
            {
                char nodeLabel = ' ';
                if (n > 0)
                {
                    nodeLabel = Convert.ToChar(_treeB[sj - 1 + n].Value);
                }
                Console.Write( $"  {nodeLabel}" );
            }
            Console.WriteLine();

            Console.Write("    ");
            for (int n = 0; n < table.GetLength(1); n++)
            {
                Console.Write(string.Format("{0}", n == 0 ? " ^^" : $" {n:00}"));
            }
            Console.WriteLine();

            for (int m = 0; m < table.GetLength(0); m++)
            {
                char nodeLabel = ' ';
                if (m > 0)
                {
                    nodeLabel = Convert.ToChar(_treeA[si - 1 + m].Value);
                }
                Console.Write($"{nodeLabel}");
                Console.Write(string.Format("{0}", m == 0 ? " ^^" : $" {m:00}"));

                Console.ForegroundColor = ConsoleColor.Blue;
                for (int n = 0; n < table.GetLength(1); n++)
                {
                    if (table[m, n] == int.MaxValue)
                    {
                        Console.Write($" ..");
                    }
                    else
                    {
                        Console.Write($" {table[m, n]:00}");
                    }
                }
                Console.ResetColor();
                Console.WriteLine();
            }
            Console.WriteLine();
        }
#endif
    }
}
