using System;
using System.Collections.Generic;
using DCasm.InstructionSet;

namespace DCasm.Visitors
{
    public class Compiler : IVisitor
    {

        private Dictionary<int, int> blockSizeCache;
        private int currentTempRegister;
        public int PC;
        public bool Verbose;
        public INode Root;
        public List<string> Program;

        public Compiler(bool dumpIntermediate) {
            Verbose = false;
            blockSizeCache = new Dictionary<int, int>();
            currentTempRegister = 0;
            PC = 0;
        }


        public void Visit(Store n)
        {
            var inst = OpCodes.OpToBinary(n.Value) + RegisterConverter.RegisterToBinary(n.Children[1]) 
            + RegisterConverter.RegisterToBinary(n.Children[0]) + ConstConverter.ConstantToBinary(n.Children[2].Value);
            Program.Add(inst);
            ConsoleWriteLine(inst);
            PC++;
        }

        public void Visit(Const n){}

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
            var inst = OpCodes.OpToBinary(n.Value) + RegisterConverter.RegisterToBinary(n.Children[0]) 
            + RegisterConverter.RegisterToBinary(n.Children[1]) 
            + n.Children[2] switch {
                Const c => ConstConverter.ConstantToBinary(c.Value),
                Register r => RegisterConverter.RegisterToBinary(r) + "00000000000",
                _ => throw new ArgumentException("wrong argument type must be constant or register")
            }
            ; 
            ConsoleWriteLine(inst);
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
            var inst = OpCodes.OpToBinary(n.Value) + RegisterConverter.RegisterToBinary(n.Children[0]) + "00000" 
            + ConstConverter.ConstantToBinary(n.Children[1].Value);
            ConsoleWriteLine(inst);
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

        private void ConsoleWrite(string str)
        {
            if (Verbose) Console.Write(str);
        }

        private void ConsoleWriteLine(string str)
        {
            if (Verbose) Console.WriteLine(str);
        }
    }
}