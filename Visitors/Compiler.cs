using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using DCasm.InstructionSet;

namespace DCasm.Visitors
{
    public class Compiler : IVisitor
    {

        private readonly Dictionary<string, int> functionsAdress;
        private int tempOffset;
        public int PC;
        private bool Verbose;
        public INode root;
        public readonly List<string> Program;

        public Compiler(Dictionary<string, INode> functions, bool dumpIntermediate, bool verbose = false) {
            Verbose = verbose;
            functionsAdress = new Dictionary<string, int>();
            Program = new List<string>();
            tempOffset = 0;
            PC = 2;
            var adressInst = OpCodes.OpToBinary("set") + "00001" + "00000" + "{0}";
            Program.Add(adressInst);
            var inst = OpCodes.OpToBinary("jmp") + "00000" + "00001" + ConstConverter.ConstantToBinary("0");
            Program.Add(inst);
            ConsoleWriteLine(functions.Count.ToString());
            foreach(var (key, value) in functions) {
                functionsAdress.Add(key, PC);
                value.Children.ForEach(child => child.Accept(this));
            }
            // update init jump to resolved adress
            var item = Program[0];
            item = string.Format(item, ConstConverter.ConstantToBinary(PC.ToString()));
            Program[0] = item;
            ConsoleWriteLine("start of program: " + PC);
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
            var inst = OpCodes.OpToBinary("call") + "00000" + "00111" + ConstConverter.ConstantToBinary("0");
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
            var inst = OpCodes.OpToBinary(n.Value) + "00000" + RegisterConverter.RegisterToBinary(n.Children[0])
            + RegisterConverter.RegisterToBinary(n.Children[1]);
            inst = inst.PadRight(32, '0');
            Program.Add(inst);
            ConsoleWriteLine(inst);
            PC++;
        }

        public void Visit(Write n)
        {
            var inst = OpCodes.OpToBinary(n.Value) + "00000" + RegisterConverter.RegisterToBinary(n.Children[0])
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
            int startAddress = PC;
            int jmpOutEndIfAddress = 0;

            var comp = n.Children[0];
            comp.Accept(this); 
            
            
            //if
            var block = n.Children[1];
            block.Accept(this);

            if(n.HasElseCall) {
                //add jump out of if after if execution
                var loadAddress = OpCodes.OpToBinary("set") + "00001" + "00000" + "{0}";
                Program.Add(loadAddress);
                PC++;
                var jumpOutIfInst = OpCodes.OpToBinary("jmp") + "00000" + "00001" + ConstConverter.ConstantToBinary("0"); 
                jmpOutEndIfAddress = PC - 1;    
                Program.Add(jumpOutIfInst);
                PC++;      

                 //patches the jump to else
                var jumpOutInst = Program[startAddress + tempOffset + 1];
                jumpOutInst = string.Format(jumpOutInst, ConstConverter.ConstantToBinary(PC.ToString()));
                Program[startAddress + tempOffset + 1] = jumpOutInst;

                var thenBlock = n.Children[2];
                thenBlock.Accept(this);

                var endIfJmp = Program[jmpOutEndIfAddress];
                endIfJmp = string.Format(endIfJmp, ConstConverter.ConstantToBinary(PC.ToString()));
                Program[jmpOutEndIfAddress] = endIfJmp;
            } else {
                //patches the jump out of if address
                var jumpOutInst = Program[startAddress + tempOffset + 1];
                jumpOutInst = string.Format(jumpOutInst, ConstConverter.ConstantToBinary(PC.ToString()));
                Program[startAddress + tempOffset + 1] = jumpOutInst;
            }
        }

        public void Visit(Block n) => n.Children.ForEach(child => child.Accept(this));

        public void Visit(While n)
        {
            var startAddress = PC;

            var comp = n.Children[0];
            comp.Accept(this);            
            
            var block = n.Children[1];
            block.Accept(this);

            var setJmpAdress = new ImmediateLoad(false, "$3", startAddress.ToString());
            setJmpAdress.Accept(this);
            
            var loopInst = OpCodes.OpToBinary("jmp") + "00000" + "00011"
            + ConstConverter.ConstantToBinary("0");
            Program.Add(loopInst);
            PC++;

            //patches the jump out of while address
            var jumpOutInst = Program[startAddress + tempOffset + 1];
            jumpOutInst = string.Format(jumpOutInst, ConstConverter.ConstantToBinary(PC.ToString()));
            Program[startAddress + tempOffset + 1] = jumpOutInst;
        }

        public void Visit(Comparaison comparaison)
        {
            switch (comparaison.Children[1]) {
                case Const c:
                    var reg = new ImmediateLoad(false, "$1", c.Value);
                    reg.Accept(this);
                    comparaison.Children[1] = new Register { Value = "$1"};
                    tempOffset++;
                    break;
            }
            
            switch (comparaison.Children[0]) {
                case Const c:
                    var reg = new ImmediateLoad(false, "$2", c.Value);
                    reg.Accept(this);
                    comparaison.Children[0] = new Register { Value = "$2"};
                    tempOffset++;
                    break;
            }
            
            var comp = OpCodes.OpToBinary("comp") 
            + "00000" 
            + RegisterConverter.RegisterToBinary(comparaison.Children[0]) 
            + RegisterConverter.RegisterToBinary(comparaison.Children[1])
            + "00000000000";
            Program.Add(comp);
            ConsoleWriteLine(comp);
            PC++;

            var setEndJumpAddressInst = OpCodes.OpToBinary("set") 
            + "01001" + "00000" + "{0}";
            Program.Add(setEndJumpAddressInst);
            ConsoleWriteLine(setEndJumpAddressInst);
            PC++;
            
            var initInst = new ImmediateLoad(false, "$8", (PC + 3).ToString());
            initInst.Accept(this);

            var op = comparaison.Value switch {
                ">" => "jgt",
                "<" => "jlt",
                "==" => "jeq",
                "<=" => "jle",
                ">=" => "jge",
                "!=" => "jne",
                _ => throw new ArgumentException("invalid comparison operator")
            };
            var conditionInst = OpCodes.OpToBinary(op) + "00000" + "01000" 
            + ConstConverter.ConstantToBinary("0");
            Program.Add(conditionInst);
            ConsoleWriteLine(conditionInst);
            PC++;

            var jumpOutInst = OpCodes.OpToBinary("jmp") + "00000" + "01001" 
            + ConstConverter.ConstantToBinary("0");
            Program.Add(jumpOutInst);
            ConsoleWriteLine(jumpOutInst);
            PC++;
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