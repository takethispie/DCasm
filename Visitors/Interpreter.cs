using System;
using System.Collections.Generic;

namespace DCasm
{
    public class Interpreter : IVisitor
    {
        public Dictionary<string, INode> Functions;
        private bool gt, eq, lt;
        private int pc;
        private readonly Dictionary<int, int> ram;
        private readonly int[] registers;
        public INode root;
        private readonly Stack<int> stack;

        public bool verbose;

        public Interpreter(Dictionary<string, INode> functions)
        {
            pc = 0;
            gt = false;
            eq = false;
            lt = false;
            registers = new int[32];
            stack = new Stack<int>();
            ram = new Dictionary<int, int>();
            Functions = functions;
            verbose = true;
        }

        public void Visit(Store n)
        {
            var valueReg = GetRegisterIndex(n.Childrens[0]);
            var baseReg = GetRegisterIndex(n.Childrens[1]);
            var offset = n.Childrens[2].Value;
            var correctOffset = int.TryParse(offset, out var parsedOffset);

            if (!correctOffset) throw new Exception("one or more arguments could not be parsed to integer !");

            var valueToStore = registers[valueReg];
            var adress = registers[baseReg];
            var storeAddress = adress + parsedOffset;

            if (!ram.ContainsKey(storeAddress)) ram.Add(storeAddress, valueToStore);
            else ram[storeAddress] = valueToStore;
            ConsoleWriteLine("stored " + valueToStore + " to adress " + storeAddress);
        }

        public void Visit(Const n)
        {
            stack.Push(n.ToInt());
        }

        public void Visit(Function n)
        {
            ConsoleWriteLine("defined function with name: " + n.Value);
        }

        public void Visit(Call n)
        {
            ConsoleWriteLine("calling " + n.Value);
            if (Functions.ContainsKey(n.Value))
            {
                Functions[n.Value].Childrens.ForEach(x => x.Accept(this));
                ConsoleWriteLine("return from " + n.Value);
            }
            else throw new ArgumentException("Function not found !");
        }

        public void Visit(Load n)
        {
            var destReg = GetRegisterIndex(n.Childrens[0]);
            var baseReg = GetRegisterIndex(n.Childrens[1]);
            var offset = GetRegisterIndex(n.Childrens[2]);

            var destRegister = registers[destReg];
            var adress = registers[baseReg];
            var loadAddress = adress + offset;

            if (!ram.ContainsKey(loadAddress)) registers[destRegister] = 0;
            else registers[destRegister] = ram[loadAddress];
            ConsoleWriteLine("Loaded value " + registers[destRegister] + " from adress " + loadAddress);
        }

        public void Visit(Add n)
        {
            n.Childrens.ForEach(x => x.Accept(this));
            var src2 = stack.Pop();
            var src1 = stack.Pop();
            stack.Pop();
            var destReg = GetRegisterIndex(n.Childrens[0]);
            registers[destReg] = src1 + src2;
            ConsoleWriteLine(src1 + " + " + src2 + " => $" + destReg);
            ConsoleWriteLine("$" + destReg + " = " + registers[destReg]);
        }

        public void Visit(Sub n)
        {
            n.Childrens.ForEach(x => x.Accept(this));
            var src2 = stack.Pop();
            var src1 = stack.Pop();
            stack.Pop();
            var destReg = GetRegisterIndex(n.Childrens[0]);
            registers[destReg] = src1 - src2;
            ConsoleWriteLine(src1 + " - " + src2 + " => $" + destReg);
            ConsoleWriteLine("$" + destReg + " = " + registers[destReg]);
        }

        public void Visit(Mul n)
        {
            n.Childrens.ForEach(x => x.Accept(this));
            var src2 = stack.Pop();
            var src1 = stack.Pop();
            stack.Pop();
            var destReg = GetRegisterIndex(n.Childrens[0]);
            registers[destReg] = src1 * src2;
            ConsoleWriteLine(src1 + " * " + src2 + " => $" + destReg);
            ConsoleWriteLine("$" + destReg + " = " + registers[destReg]);
        }

        public void Visit(Div n)
        {
            n.Childrens.ForEach(x => x.Accept(this));
            var src2 = stack.Pop();
            var src1 = stack.Pop();
            stack.Pop();
            var destReg = GetRegisterIndex(n.Childrens[0]);
            if (src2 == 0) throw new DivideByZeroException();
            registers[destReg] = src1 / src2;
            ConsoleWriteLine(src1 + " - " + src2 + " => $" + destReg);
            ConsoleWriteLine("$" + destReg + " = " + registers[destReg]);
        }

        public void Visit(Register n)
        {
            stack.Push(GetRegisterValue(n));
        }

        public void Visit(ImmediateLoad n)
        {
            var destReg = GetRegisterIndex(n.Childrens[0]);
            var correctValue = int.TryParse(n.Childrens[1].Value, out var value);
            if (correctValue)
            {
                if (!n.Upper) registers[destReg] = value;
                else
                {
                    var lowerHex = registers[destReg].ToString("X4");
                    var upperhex = value.ToString("X4");
                    var hex = upperhex + lowerHex;
                    registers[destReg] = (int) Convert.ToUInt32(hex, 16);
                }
            }
            else throw new Exception("cannot parse immediate load parameters !");
            ConsoleWriteLine("stored " + value + " in $" + destReg);
        }

        public void Visit(Read n)
        {
            var inSel = GetRegisterIndex(n.Childrens[0]);
            var destReg = GetRegisterIndex(n.Childrens[1]);

            switch (registers[inSel])
            {
                //fully code handled input, no new lines or prefix
                case 0:
                    var input = Console.ReadLine();
                    registers[destReg] = input[0];
                    ConsoleWrite(Environment.NewLine);
                    ConsoleWriteLine("read " + input + " from terminal and stored it in " + destReg);
                    break;
            }
        }

        public void Visit(Write n)
        {
            var outSel = GetRegisterIndex(n.Childrens[0]);
            var sourceReg = GetRegisterIndex(n.Childrens[1]);

            switch (registers[outSel])
            {
                //transparent writing (no new lines and prefix)
                case 0:
                    var val = registers[sourceReg];
                    Console.Write(Convert.ToChar((short)val));
                    break;
                
                case 1:
                    val = registers[sourceReg];
                    Console.Write(val);
                    break;
            }
        }

        public void Visit(Move n)
        {
            var source = GetRegisterIndex(n.Childrens[0]);
            var destination = GetRegisterIndex(n.Childrens[1]);
            registers[destination] = registers[source];
            ConsoleWriteLine("$" + source + "(" + registers[source] + ") => $" + destination + "(" +
                             registers[destination] + ")");
        }

        public void Visit(Condition n)
        {
            void ExecuteElse()
            {
                if (!n.HasElseCall) return;
                if (n.Childrens.Count < 3) throw new ArgumentException("should have 3 arguments !");
                n.Childrens[3].Accept(this);
            }

            n.Childrens[0].Accept(this);
            n.Childrens[1].Accept(this);
            var right = stack.Pop();
            var left = stack.Pop();
            ConsoleWriteLine(left + " " + n.Op + " " + right);
            switch (n.Op)
            {
                case ">":
                    if (left > right) n.Childrens[2].Accept(this);
                    else ExecuteElse();
                    break;

                case "==":
                    if (left == right) n.Childrens[2].Accept(this);
                    else ExecuteElse();
                    break;

                case "<":
                    if (left < right) n.Childrens[2].Accept(this);
                    else ExecuteElse();
                    break;

                case ">=":
                    if (left >= right) n.Childrens[2].Accept(this);
                    else ExecuteElse();
                    break;

                case "<=":
                    if (left <= right) n.Childrens[2].Accept(this);
                    else ExecuteElse();
                    break;
            }
        }

        private void ConsoleWrite(string str)
        {
            if (verbose) Console.Write(str);
        }

        private void ConsoleWriteLine(string str)
        {
            if (verbose) Console.WriteLine(str);
        }

        private int GetRegisterValue(INode n)
        {
            var index = GetRegisterIndex(n);
            return registers[index];
        }

        private int GetRegisterIndex(INode n)
        {
            var reg = n.Value.Remove(0, 1);
            var correctReg = int.TryParse(reg, out var regNumber);
            if (correctReg) return regNumber;
            throw new Exception("cannot parse register number !");
        }

        public void Visit(Block n) => n.Childrens.ForEach(l => l.Accept(this));
    }
}