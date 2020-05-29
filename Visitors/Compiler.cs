using System;
using System.Collections.Generic;
using DCasm.InstructionSet;

namespace DCasm.Visitors
{
    public class Compiler : IVisitor
    {

        private readonly Dictionary<string, int> functionsAdress;
        private int currentTempRegister;
        public int PC;
        public bool Verbose;
        public INode root;
        public List<string> Program;

        public Compiler(Dictionary<string, INode> functions, bool dumpIntermediate, bool verbose = false) {
            Verbose = verbose;
            functionsAdress = new Dictionary<string, int>();
            currentTempRegister = 0;
            Program = new List<string>();
            PC = 2;
            ConsoleWriteLine(functions.Count.ToString());
            foreach(var (key, value) in functions) {
                functionsAdress.Add(key, PC);
                value.Children.ForEach(child => child.Accept(this));
            }
        }


        public void Visit(Store n)
        {
            var inst = OpCodes.OpToBinary(n.Value) + RegisterConverter.RegisterToBinary(n.Children[1]) 
            + RegisterConverter.RegisterToBinary(n.Children[0]) + RegisterConverter.RegisterToBinary(n.Children[2])
            + "00000000000" ;
            Program.Add(inst);
            ConsoleWriteLine(inst);
            PC++;
        }

        public void Visit(Const n){}

        public void Visit(Function n){}

        public void Visit(Call n)
        {
            var address = functionsAdress[n.Value];
            var setInst = OpCodes.OpToBinary("set") + "00111" + "00000" + ConstConverter.ConstantToBinary(address.ToString());
            Program.Add(setInst);
            PC++;
            ConsoleWriteLine(setInst);
            var inst = OpCodes.OpToBinary("call") + "00111" + "00000" + ConstConverter.ConstantToBinary("0");
            Program.Add(inst);
            PC++;
            ConsoleWriteLine(inst);
        }

        public void Visit(Return ret) {
            var inst = OpCodes.OpToBinary("ret").PadRight(32, '0');
            Program.Add(inst);
            ConsoleWriteLine(inst);
            PC++;
        }

        public void Visit(Load n)
        {
            var inst = OpCodes.OpToBinary(n.Value) + RegisterConverter.RegisterToBinary(n.Children[1]) 
            + RegisterConverter.RegisterToBinary(n.Children[0]) + RegisterConverter.RegisterToBinary(n.Children[2])
            + "00000000000" ;
            Program.Add(inst);
            ConsoleWriteLine(inst);
            PC++;
        }

        private string arithmeticInstructionBuilder(INode n) {
            var inst = OpCodes.OpToBinary(n.Value) + RegisterConverter.RegisterToBinary(n.Children[0]) 
            + RegisterConverter.RegisterToBinary(n.Children[1]) 
            + n.Children[2] switch {
                Const c => ConstConverter.ConstantToBinary(c.Value),
                Register r => RegisterConverter.RegisterToBinary(r) + "00000000000",
                _ => throw new ArgumentException("wrong argument type must be constant or register")
            }; 
            return inst;
        }

        public void Visit(Add n)
        {
            var inst = arithmeticInstructionBuilder(n);
            Program.Add(inst);
            ConsoleWriteLine(inst);
            PC++;
        }

        public void Visit(Sub n)
        {
            var inst = arithmeticInstructionBuilder(n);
            Program.Add(inst);
            ConsoleWriteLine(inst);
            PC++;
        }

        public void Visit(Mul n)
        {
            var inst = arithmeticInstructionBuilder(n);
            Program.Add(inst);
            ConsoleWriteLine(inst);
            PC++;
        }

        public void Visit(Div n)
        {
            var inst = arithmeticInstructionBuilder(n);
            Program.Add(inst);
            ConsoleWriteLine(inst);
            PC++;
        }

        public void Visit(Register n) { }

        public void Visit(ImmediateLoad n)
        {
            var inst = OpCodes.OpToBinary(n.Value) + RegisterConverter.RegisterToBinary(n.Children[0]) + "00000" 
            + ConstConverter.ConstantToBinary(n.Children[1].Value);
            Program.Add(inst);
            ConsoleWriteLine(inst);
            PC++;
        }

        public void Visit(Read n) {
            var inst = OpCodes.OpToBinary(n.Value) + RegisterConverter.RegisterToBinary(n.Children[0])
            + RegisterConverter.RegisterToBinary(n.Children[1]);
            inst = inst.PadRight(32, '0');
            Program.Add(inst);
            ConsoleWriteLine(inst);
            PC++;
        }

        public void Visit(Write n)
        {
            var inst = OpCodes.OpToBinary(n.Value) + RegisterConverter.RegisterToBinary(n.Children[0])
            + RegisterConverter.RegisterToBinary(n.Children[1]);
            inst = inst.PadRight(32, '0');
            Program.Add(inst);
            ConsoleWriteLine(inst);
            PC++;
        }

        public void Visit(Move n) {
            var inst = OpCodes.OpToBinary("mov") + RegisterConverter.RegisterToBinary(n.Children[0])
            + RegisterConverter.RegisterToBinary(n.Children[1]);
            inst = inst.PadRight(32, '0');
            Program.Add(inst);
            ConsoleWriteLine(inst);
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