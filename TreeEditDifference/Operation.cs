﻿namespace Algorithms.TreeDifference
{
    public class Operation
    {
        public Operation(OpType op, int postOrderA, int postOrderB )
        {
            EditOp = op;
            PostOrderIndex1 = postOrderA;
            PostOrderIndex2 = postOrderB;
        }

        public enum OpType {
            Insert,
            Delete,
            Change,
            Match,
            Similar    // A special case where a node is essentially a match, but worthy of further investigation
        }

        public OpType EditOp = OpType.Match;
        public int PostOrderIndex1;
        public int PostOrderIndex2;

        public override string ToString()
        {
            char opCh = 'x';
            switch(EditOp)
            {
                case OpType.Insert: opCh = 'I'; break;
                case OpType.Delete: opCh = 'D'; break;
                case OpType.Change: opCh = 'C'; break;
                case OpType.Match: opCh = 'M'; break;
                case OpType.Similar: opCh = 'S'; break;
            }
            return $"{opCh}:{PostOrderIndex1:00}->{PostOrderIndex2:00}";
        }
    }
}
