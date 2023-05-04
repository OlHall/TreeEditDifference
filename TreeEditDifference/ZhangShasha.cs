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
        private List<Operation>[,] _operations;

        public ZhangShasha(Tree<T> treeA, Tree<T> treeB)
        {
            _treeA = treeA;
            _treeB = treeB;
            _check = new E();
            _cost = new C();
            _treeDistance = new int[_treeA.Size + 1, _treeB.Size + 1];
            _operations = new List<Operation>[_treeA.Size + 1, _treeB.Size + 1];
        }

        public int Compare()
        {
            // Set up the dynamic programming table
            _treeDistance = new int[_treeA.Size + 1, _treeB.Size + 1];
            _operations = new List<Operation>[_treeA.Size + 1, _treeB.Size + 1];

            // Initialise the operations table prior to solutions
            for (int i = 0; i <= _treeA.Size; ++i)
            {
                for (int j = 0; j <= _treeB.Size; ++j)
                {
                    _operations[i, j] = new List<Operation>();
                }
            }

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
            DumpOperations(_treeA.Size, _treeB.Size);
#endif
            return _treeDistance[_treeA.Size, _treeB.Size];
        }

        public List<Operation> EditOperations(bool full=false)
        {
            if( full)
            {
                return _operations[_treeA.Size, _treeB.Size];
            }
            else
            {
                List<Operation> operations = new();
                foreach(Operation op in _operations[_treeA.Size, _treeB.Size])
                {
                    if( op.EditOp != Operation.OpType.Match )
                    {
                        operations.Add(op);
                    }
                }
                return operations;
            }
        }

        private int TreeDistance(int i, int j)
        {
            // Initialise the forest dynamic programming table
            // Most implementations create a full size table, which seems wasteful
            // The bottom right corner of this table maps on to i,j of the tree distance table
            int treeSizeA = _treeA.SizeOfSubtreeAt(i);
            int treeSizeB = _treeB.SizeOfSubtreeAt(j);

            int fdALeft = _treeA[i].LeftMostIndex;
            int fdBLeft = _treeB[j].LeftMostIndex;

            // Create the forest distance tables and initialise with edit distances/ops
            // This isn't the algorithm - just prep
            int[,] fd = new int[treeSizeA + 1, treeSizeB + 1];
            List<Operation>[,] pOps = new List<Operation>[treeSizeA + 1, treeSizeB + 1];

            fd[0, 0] = 0;
            pOps[0, 0] = new List<Operation>();

            for (int m = 1; m <= treeSizeA; ++m)
            {
                fd[m, 0] = fd[m-1, 0] + _cost.Cost(Operation.OpType.Delete, null, null);
#pragma warning disable IDE0028 // Simplify collection initialization
                pOps[m, 0] = new List<Operation>(pOps[m-1,0]);
#pragma warning restore IDE0028 // Simplify collection initialization
                pOps[m, 0].Add(new Operation(Operation.OpType.Delete, fdALeft + m - 1, 0));
            }

            for (int n = 1; n <= treeSizeB; ++n)
            {
                fd[0, n] = fd[0, n - 1] + _cost.Cost(Operation.OpType.Insert, null, null);
#pragma warning disable IDE0028 // Simplify collection initialization
                pOps[0,n] = new List<Operation>(pOps[0,n-1]);
#pragma warning restore IDE0028 // Simplify collection initialization
                pOps[0, n].Add(new Operation(Operation.OpType.Insert, 0, fdBLeft + n - 1));
            }

            // Indicies into smaller fdist table
            for (int fdi = 1, m = fdALeft; m <= i; ++m, ++fdi)
            {
                for (int fdj = 1, n = fdBLeft; n <= j; ++n, ++fdj)
                {
                    // Work out the various costs
                    // The top adjacent cell plus delete cost
                    int costDel = fd[fdi - 1, fdj] + _cost.Cost(Operation.OpType.Delete, _treeA[m], null);
                    // The left adjacent cell plus insert cost
                    int costIns = fd[fdi, fdj - 1] + _cost.Cost(Operation.OpType.Insert, null, _treeB[n]);

                    // Test to see the nodes being compared fall within the same sub-tree in each tree
                    if ((_treeA.HasMatchingLeftMost(m,i)) && (_treeB.HasMatchingLeftMost(n,j)))
                    {
#if DEBUG
                        Console.WriteLine($"Left Node Match => [{m},{n}]");
#endif
                        bool match = _check.EqualTo(_treeA[m].Value, _treeB[n].Value);
                        int costChg = fd[fdi - 1, fdj - 1] + (match ? 0 : _cost.Cost(Operation.OpType.Change, _treeA[m], _treeB[n]));
                        fd[fdi, fdj] = Math.Min(Math.Min(costDel, costIns), costChg);
                        _treeDistance[m, n] = fd[fdi, fdj];

                        // This is not part of Zhang-Shasha but provides the edit script for the least cost different
                        // taken from a superb Python implementation
                        // https://github.com/timtadh/zhang-shasha
                        Operation.OpType op = ZhangShasha<T, C, E>.SelectOperation(costDel, costIns, costChg, match);
                        switch( op )
                        {
                            case Operation.OpType.Delete:
                                {
#pragma warning disable IDE0028 // Simplify collection initialization
                                    pOps[fdi, fdj] = new List<Operation>(pOps[fdi - 1, fdj]);
#pragma warning restore IDE0028 // Simplify collection initialization
                                    pOps[fdi, fdj].Add(new Operation(op, m, 0));
                                    break;
                                }
                            case Operation.OpType.Insert:
                                {
#pragma warning disable IDE0028 // Simplify collection initialization
                                    pOps[fdi, fdj] = new List<Operation>(pOps[fdi, fdj-1]);
#pragma warning restore IDE0028 // Simplify collection initialization
                                    pOps[fdi, fdj].Add(new Operation(op, 0, n));
                                    break;
                                }
                            case Operation.OpType.Change:
                            case Operation.OpType.Match:
                                {
#pragma warning disable IDE0028 // Simplify collection initialization
                                    pOps[fdi, fdj] = new List<Operation>(pOps[fdi - 1, fdj - 1]);
#pragma warning restore IDE0028 // Simplify collection initialization
                                    pOps[fdi, fdj].Add(new Operation(op, m, n));
                                    break;
                                }
                        }
                        _operations[m, n] = pOps[fdi, fdj];
                    }
                    // If not, then use the result from a previously calculated sub-problem
                    else
                    {
#if DEBUG
                        Console.WriteLine($"Left Node Pre Calc => [{_treeA[m].LeftMostIndex-1},{_treeB[n].LeftMostIndex-1}] => [{_treeA[m].LeftMostIndex - fdALeft},{_treeB[n].LeftMostIndex - fdBLeft}]");
#endif
                        int costChg = fd[_treeA[m].LeftMostIndex - fdALeft, _treeB[n].LeftMostIndex - fdBLeft] + _treeDistance[m, n];
                        fd[fdi, fdj] = Math.Min(Math.Min(costDel, costIns), costChg);

                        // This is not part of Zhang-Shasha but provides the edit script for the least cost different
                        // taken from a superb Python implementation
                        // https://github.com/timtadh/zhang-shasha
                        bool match = _check.EqualTo(_treeA[m].Value, _treeB[n].Value);
                        Operation.OpType op = ZhangShasha<T, C, E>.SelectOperation(costDel, costIns, costChg, match);
                        switch( op )
                        {
                            case Operation.OpType.Delete:
                                {
#pragma warning disable IDE0028 // Simplify collection initialization
                                    pOps[fdi, fdj] = new List<Operation>(pOps[fdi - 1, fdj]);
#pragma warning restore IDE0028 // Simplify collection initialization
                                    pOps[fdi, fdj].Add(new Operation(op, m, 0));
                                    break;
                                }
                            case Operation.OpType.Insert:
                                {
#pragma warning disable IDE0028 // Simplify collection initialization
                                    pOps[fdi, fdj] = new List<Operation>(pOps[fdi, fdj - 1]);
#pragma warning restore IDE0028 // Simplify collection initialization
                                    pOps[fdi, fdj].Add(new Operation(op, 0, n));
                                    break;
                                }
                            case Operation.OpType.Change:
                            case Operation.OpType.Match:
                                {
                                    pOps[fdi, fdj] = new List<Operation>(pOps[_treeA[m].LeftMostIndex - fdALeft, _treeB[n].LeftMostIndex - fdBLeft]);
                                    pOps[fdi, fdj].AddRange(_operations[m,n]);
                                    break;
                                }
                        }
                    }
                }
            }

#if DEBUG
            DumpTable(_treeA[i].LeftMostIndex, _treeB[j].LeftMostIndex, i, j, fd, "fd");
#endif

            return _treeDistance[i, j];
        }

        /// <summary>
        /// We decide which operation is the lowest cost
        /// </summary>
        /// <param name="costDel">The cost of a deletion</param>
        /// <param name="costIns">The cost of an insertion</param>
        /// <param name="costChg">The cost of a change</param>
        /// <param name="match">A change might be a match</param>
        /// <returns>The lowest cost operation</returns>
        private static Operation.OpType SelectOperation(int costDel, int costIns, int costChg, bool match)
        {
            if ((costChg <= costDel) && (costChg <= costIns))
            {
                return match ? Operation.OpType.Match : Operation.OpType.Change;
            }
            else if (costDel < costIns)
            {
                return Operation.OpType.Delete;
            }
            else
            {
                return Operation.OpType.Insert;
            }
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
                    try
                    {
                        nodeLabel = Convert.ToChar(_treeB[sj - 1 + n].Value);
                    }
                    catch
                    {
                        nodeLabel = '?';
                    }
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
                    try
                    {
                        nodeLabel = Convert.ToChar(_treeA[si - 1 + m].Value);
                    }
                    catch
                    {
                        nodeLabel = '?';
                    }
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

        private void DumpOperations(int i, int j)
        {
            Console.WriteLine("Operations:");
            foreach( Operation op in _operations[i,j] )
            {
                Console.WriteLine(op.ToString());
            }
        }
#endif
    }
}
