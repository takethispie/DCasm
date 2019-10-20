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
            var valueReg = n.Childrens[0].Value.Remove(0, 1);
            var correctValueReg = int.TryParse(valueReg, out var valueNum);

            var baseReg = n.Childrens[1].Value.Remove(0, 1);
            var correctBaseReg = int.TryParse(baseReg, out var baseNum);

            var offset = n.Childrens[2].Value;
            var correctOffset = int.TryParse(offset, out var parsedOffset);

            if (!correctOffset || !correctValueReg || !correctBaseReg)
                throw new Exception("one or more arguments could not be parsed to integer !");

            var valueToStore = registers[valueNum];
            var adress = registers[baseNum];
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
            var destReg = n.Childrens[0].Value.Remove(0, 1);
            var correctDestReg = int.TryParse(destReg, out var destNum);

            var baseReg = n.Childrens[1].Value.Remove(0, 1);
            var correctBaseReg = int.TryParse(baseReg, out var baseNum);

            var offset = n.Childrens[2].Value;
            var correctOffset = int.TryParse(offset, out var parsedOffset);

            if (!correctOffset || !correctDestReg || !correctBaseReg)
                throw new Exception("one or more arguments could not be parsed to integer !");

            var destRegister = registers[destNum];
            var adress = registers[baseNum];
            var loadAddress = adress + parsedOffset;

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
            var regNumber = GetRegisterIndex(n.Childrens[0]);
            registers[regNumber] = src1 + src2;
            ConsoleWriteLine(src1 + " + " + src2 + " => $" + regNumber);
            ConsoleWriteLine("$" + regNumber + " = " + registers[regNumber]);
        }

        public void Visit(Sub n)
        {
            n.Childrens.ForEach(x => x.Accept(this));
            var src2 = stack.Pop();
            var src1 = stack.Pop();
            stack.Pop();
            var regNumber = GetRegisterIndex(n.Childrens[0]);
            registers[regNumber] = src1 - src2;
            ConsoleWriteLine(src1 + " - " + src2 + " => $" + regNumber);
            ConsoleWriteLine("$" + regNumber + " = " + registers[regNumber]);
        }

        public void Visit(Mul n)
        {
            n.Childrens.ForEach(x => x.Accept(this));
            var src2 = stack.Pop();
            var src1 = stack.Pop();
            stack.Pop();
            var regNumber = GetRegisterIndex(n.Childrens[0]);
            registers[regNumber] = src1 * src2;
            ConsoleWriteLine(src1 + " * " + src2 + " => $" + regNumber);
            ConsoleWriteLine("$" + regNumber + " = " + registers[regNumber]);
        }

        public void Visit(Div n)
        {
            n.Childrens.ForEach(x => x.Accept(this));
            var src2 = stack.Pop();
            var src1 = stack.Pop();
            stack.Pop();
            var regNumber = GetRegisterIndex(n.Childrens[0]);
            if (src2 == 0) throw new DivideByZeroException();
            registers[regNumber] = src1 / src2;
            ConsoleWriteLine(src1 + " - " + src2 + " => $" + regNumber);
            ConsoleWriteLine("$" + regNumber + " = " + registers[regNumber]);
        }

        public void Visit(Register n)
        {
            stack.Push(GetRegisterValue(n));
        }

        public void Visit(ImmediateLoad n)
        {
            var reg = n.Childrens[0].Value.Remove(0, 1);
            var correctReg = int.TryParse(reg, out var regNumber);
            var correctValue = int.TryParse(n.Childrens[1].Value, out var value);
            if (correctReg && correctValue)
            {
                if (!n.Upper) registers[regNumber] = value;
                else
                {
                    var lowerHex = registers[regNumber].ToString("X4");
                    var upperhex = value.ToString("X4");
                    var hex = upperhex + lowerHex;
                    registers[regNumber] = (int) Convert.ToUInt32(hex, 16);
                }
            }
            else throw new Exception("cannot parse immediate load parameters !");
            ConsoleWriteLine("stored " + value + " in $" + regNumber);
        }

        public void Visit(Read n)
        {
            var inSel = n.Childrens[0].Value.Remove(0, 1);
            var correctInSel = int.TryParse(inSel, out var parsedInSel);

            var destReg = n.Childrens[1].Value.Remove(0, 1);
            var correctDestReg = int.TryParse(destReg, out var parsedDestReg);

            if (!correctDestReg || !correctInSel)
                throw new Exception("one or more argument could not be parsed to integer !");

            switch (parsedInSel)
            {
                //fully code handled input, no new lines or prefix
                case 0:
                    var key = Console.ReadLine();
                    Console.WriteLine(key);
                    registers[parsedDestReg] = key[0];
                    ConsoleWrite(Environment.NewLine);
                    ConsoleWriteLine("read " + key + " from terminal and stored it in " + parsedDestReg);
                    break;
            }
        }

        public void Visit(Write n)
        {
            var outSel = n.Childrens[0].Value.Remove(0, 1);
            var correctOutSel = int.TryParse(outSel, out var parsedOutSel);

            var sourceReg = n.Childrens[1].Value.Remove(0, 1);
            var correctSourceReg = int.TryParse(sourceReg, out var parsedSourceReg);

            if (!correctOutSel || !correctSourceReg)
                throw new Exception("one or more arguments could not be parsed to integer !");

            switch (registers[parsedOutSel])
            {
                //transparent writing (no new lines and prefix)
                case 0:
                    var val = registers[parsedSourceReg];
                    Console.Write(Convert.ToChar((short)val));
                    break;
                
                case 1:
                    val = registers[parsedSourceReg];
                    Console.Write(val);
                    break;
            }
        }

        public void Visit(Move n)
        {
            var source = stack.Pop();
            var destination = stack.Pop();
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
    }
}