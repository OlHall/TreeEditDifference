namespace Algorithms.TreeDifference
{
    public class Operation
    {
        public enum Op {
            Insert,
            Delete,
            Change,
            NoOp
        }

        public Op EditOp = Op.NoOp;
        public int N1;
        public int N2;

        public override string ToString()
        {
            char opCh = 'x';
            switch(EditOp)
            {
                case Op.Insert: opCh = 'I'; break;
                case Op.Delete: opCh = 'D'; break;
                case Op.Change: opCh = 'C'; break;
            }
            return $"{opCh} {N1:00}->{N2:00}";
        }
    }
}
