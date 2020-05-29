using System.Collections.Generic;

namespace DCasm.Visitors
{
    public class Compiler : IVisitor
    {

        private Dictionary<int, int> blockSizeCache;
        public int PC;
        public INode Root;
        public List<string> Program;

        public Compiler() {
            blockSizeCache = new Dictionary<int, int>();
            PC = 0;
        }


        public void Visit(Store n)
        {
            PC++;
            
        }

        public void Visit(Const n)
        {
        }

        public void Visit(Function n)
        {
        }

        public void Visit(Call n)
        {
        }

        public void Visit(Load n)
        {
            PC++;
        }

        public void Visit(Add n)
        {
            PC++;
        }

        public void Visit(Sub n)
        {
            PC++;
        }

        public void Visit(Mul n)
        {
            PC++;
        }

        public void Visit(Div n)
        {
            PC++;
        }

        public void Visit(Register n)
        {
        }

        public void Visit(ImmediateLoad n)
        {
            PC++;
        }

        public void Visit(Read n)
        {
            PC++;
        }

        public void Visit(Write n)
        {
            PC++;
        }

        public void Visit(Move n)
        {
            PC++;
        }

        public void Visit(Condition n)
        {
        }

        public void Visit(Block n)
        {
        }

        public void Visit(While n)
        {
        }

        public void Visit(Comparaison comparaison)
        {
        }
    }
}