using System;
using System.Collections.Generic;
using DCasm.InstructionSet;

namespace DCasm.Translator
{
    public class Compiler
    {
        public IList<string> Program;
        private Dictionary<string, int> functionsAdress { get; set; }
        private int pc;

        public Compiler(Dictionary<string, Function> functions, bool verbose = false) {
            Program = new List<string>();
            pc = 2;
        }

        public void Compile(List<INode> nodes) {
            
            foreach (var node in nodes)
            {
                var current = pc;
                Program = Process(node);  
            }
        }

        public IList<string> Process(INode node) => node switch
        {
            Store st => Store(st, Program),
            Call call => Call(call, Program),
            Return ret => Return(ret, Program),
            IArithmeticNode n => arithmeticInstructionBuilder(n, Program),
            ImmediateLoad iLoad => ImmediateLoad(iLoad, Program),
            Load load => Load(load, Program),
            Read read => Read(read, Program),
            Write write => Write(write, Program),
            _ => throw new Exception("Unknown Error")
        };

        public IList<string> Store(Store n, IList<string> program) {
            var inst = OpCodes.OpToBinary(n.Value) + RegisterConverter.RegisterToBinary(n.BaseRegister) 
            + RegisterConverter.RegisterToBinary(n.OffsetRegister) + RegisterConverter.RegisterToBinary(n.DataValue)
            + "00000000000";
            program.Add(inst);
            return program;
        }

        public IList<string> Call (Call n, IList<string> program)
        {
            var address = functionsAdress[n.Value];
            var setInst = OpCodes.OpToBinary("set") + "00111" + "00000" + ConstConverter.ConstantToBinary(address.ToString());
            var inst = OpCodes.OpToBinary("call") + "00000" + "00111" + ConstConverter.ConstantToBinary("0");
            program.Add(setInst);
            program.Add(inst);
            return program;
        }

        public IList<string> Return (Return ret, IList<string> program) {
            var inst = OpCodes.OpToBinary("ret").PadRight(32, '0');
            program.Add(inst);
            return program;
        }

        public IList<string> Load(Load n, IList<string> program)
        {
            var inst = OpCodes.OpToBinary(n.Value) + RegisterConverter.RegisterToBinary(n.Destination) 
            + RegisterConverter.RegisterToBinary(n.BaseRegister) + RegisterConverter.RegisterToBinary(n.OffsetRegister)
            + "00000000000" ;
            program.Add(inst);
            return program;
        }

        public IList<string> arithmeticInstructionBuilder(IArithmeticNode n, IList<string> program) {
            var inst = OpCodes.OpToBinary(n.Value) + RegisterConverter.RegisterToBinary(n.Destination) 
            + RegisterConverter.RegisterToBinary(n.Left) 
            + n.Right switch {
                Const c => ConstConverter.ConstantToBinary(c.Value),
                Register r => RegisterConverter.RegisterToBinary(r) + "00000000000",
                _ => throw new ArgumentException("wrong argument type must be constant or register")
            }; 
            program.Add(inst);
            return program;
        }

        public IList<string> ImmediateLoad(ImmediateLoad n, IList<string> program)
        {
            var inst = OpCodes.OpToBinary(n.Value) + RegisterConverter.RegisterToBinary(n.Destination) + "00000" 
            + ConstConverter.ConstantToBinary(n.DataValue.Value);
            program.Add(inst);
            return program;
        }

        public IList<string> Read(Read n, IList<string> program) {
            var inst = OpCodes.OpToBinary(n.Value) + RegisterConverter.RegisterToBinary(n.Destination)
            + RegisterConverter.RegisterToBinary(n.InputSelection);
            inst = inst.PadRight(32, '0');
            program.Add(inst);
            return program;
        }

        public IList<string> Write(Write n, IList<string> program)
        {
            var inst = OpCodes.OpToBinary(n.Value) + "00000" + RegisterConverter.RegisterToBinary(n.OutputSelection)
            + RegisterConverter.RegisterToBinary(n.DataValue);
            inst = inst.PadRight(32, '0');
            program.Add(inst);
            return program;
        }

        public IList<string> Move(Move n, IList<string> program) {
            var inst = OpCodes.OpToBinary("mov") + RegisterConverter.RegisterToBinary(n.Destination)
            + RegisterConverter.RegisterToBinary(n.Source);
            inst = inst.PadRight(32, '0');
            program.Add(inst);
            return program;
        }

        public IList<string> Condition(Condition n, IList<string> program)
        {
            int startAddress = program.Count;
            int jmpOutEndIfAddress = 0;

            var comp = n.Comparaison;
            Process(comp);
            var tempOffset = program.Count - 2;
            
            
            
            //if
            var block = n.Then;
            Process(block);

            if(n.HasElseCall) {
                //add jump out of if after if execution
                var loadAddress = OpCodes.OpToBinary("set") + "00001" + "00000" + "{0}";
                program.Add(loadAddress);
                var jumpOutIfInst = OpCodes.OpToBinary("jmp") + "00000" + "00001" + ConstConverter.ConstantToBinary("0"); 
                jmpOutEndIfAddress = program.Count - 1;    
                program.Add(jumpOutIfInst);

                 //patches the jump to else
                var jumpOutInst = Program[startAddress + tempOffset + 1];
                jumpOutInst = string.Format(jumpOutInst, ConstConverter.ConstantToBinary(program.Count.ToString()));
                Program[startAddress + tempOffset + 1] = jumpOutInst;

                var thenBlock = n.Then;
                Process(thenBlock);

                var endIfJmp = program[jmpOutEndIfAddress];
                endIfJmp = string.Format(endIfJmp, ConstConverter.ConstantToBinary(program.Count.ToString()));
                program[jmpOutEndIfAddress] = endIfJmp;
            } else {
                //patches the jump out of if address
                var jumpOutInst = Program[startAddress + tempOffset + 1];
                jumpOutInst = string.Format(jumpOutInst, ConstConverter.ConstantToBinary(program.Count.ToString()));
                Program[startAddress + tempOffset + 1] = jumpOutInst;
            }
            return program;
        }

        public void Visit(Block n) => n.Children.ForEach(child => Process(child));

        public IList<string> While(While n, IList<string> program)
        {
            var startAddress = program.Count;

            var comp = n.Children[0];
            Process(comp);           
            
            var block = n.Children[1];
            Process(block);

            var setJmpAdress = new ImmediateLoad(false, "$3", startAddress.ToString());
            Process(setJmpAdress);
            
            var loopInst = OpCodes.OpToBinary("jmp") + "00000" + "00011"
            + ConstConverter.ConstantToBinary("0");
            Program.Add(loopInst);

            //patches the jump out of while address
            var jumpOutInst = Program[startAddress + 1];
            jumpOutInst = string.Format(jumpOutInst, ConstConverter.ConstantToBinary(program.Count.ToString()));
            Program[startAddress + 1] = jumpOutInst;
            return program;
        }

        public IList<string> Comparaison(Comparaison comparaison, IList<string> program)
        {
            switch (comparaison.Right) {
                case Const c:
                    var reg = new ImmediateLoad(false, "$1", c.Value);
                    Process(reg);
                    comparaison.Right = new Register { Value = "$1"};
                    break;
            }
            
            switch (comparaison.Left) {
                case Const c:
                    var reg = new ImmediateLoad(false, "$2", c.Value);
                    Process(reg);
                    comparaison.Left = new Register { Value = "$2"};
                    break;
            }
            
            var comp = OpCodes.OpToBinary("comp") 
            + "00000" 
            + RegisterConverter.RegisterToBinary(comparaison.Left) 
            + RegisterConverter.RegisterToBinary(comparaison.Right)
            + "00000000000";
            program.Add(comp);

            var setEndJumpAddressInst = OpCodes.OpToBinary("set") 
            + "01001" + "00000" + "{0}";
            program.Add(setEndJumpAddressInst);
            
            var initInst = new ImmediateLoad(false, "$8", (program.Count + 3).ToString());
            Process(initInst);

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
            program.Add(conditionInst);

            var jumpOutInst = OpCodes.OpToBinary("jmp") + "00000" + "01001" 
            + ConstConverter.ConstantToBinary("0");
            program.Add(jumpOutInst);
            return program;
        }

    }
}