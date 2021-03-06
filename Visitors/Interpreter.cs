using System;
using System.Collections.Generic;

namespace DCasm.Visitors
{
    public class Interpreter : IVisitor
    {
        public Dictionary<string, INode> Functions;
        private readonly Dictionary<int, int> ram;
        private readonly int[] registers;
        public INode root;
        private readonly Stack<int> stack;
        public bool verbose;

        public Interpreter(Dictionary<string, INode> functions)
        {
            registers = new int[32];
            stack = new Stack<int>();
            ram = new Dictionary<int, int>();
            Functions = functions;
        }

        public void Visit(Store n)
        {
            var valueReg = Utils.GetRegisterIndex(n.Children[0]);
            var baseReg = Utils.GetRegisterIndex(n.Children[1]);
            var offsetReg = Utils.GetRegisterIndex(n.Children[2]);


            var valueToStore = registers[valueReg];
            var adress = registers[baseReg];
            var offset = registers[offsetReg];
            var storeAddress = adress + offset;

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
                Functions[n.Value].Children.ForEach(x => x.Accept(this));
                ConsoleWriteLine("return from " + n.Value + Environment.NewLine);
            }
            else throw new ArgumentException("Function not found !");
        }

        public void Visit(Return ret) {
            ConsoleWriteLine("return from " + ret.Value + Environment.NewLine);
        }

        public void Visit(Load n)
        {
            var destReg = Utils.GetRegisterIndex(n.Children[0]);
            var baseReg = Utils.GetRegisterIndex(n.Children[1]);
            var offsetReg = Utils.GetRegisterIndex(n.Children[2]);

            var destRegister = registers[destReg];
            var adress = registers[baseReg];
            var offset = registers[offsetReg];
            var loadAddress = adress + offset;

            if (!ram.ContainsKey(loadAddress)) registers[destRegister] = 0;
            else registers[destRegister] = ram[loadAddress];
            ConsoleWriteLine("Loaded value " + registers[destRegister] + " from adress " + loadAddress);
        }

        public void Visit(Add n)
        {
            n.Children.ForEach(x => x.Accept(this));
            var src2 = stack.Pop();
            var src1 = stack.Pop();
            stack.Pop();
            var destReg = Utils.GetRegisterIndex(n.Children[0]);
            registers[destReg] = src1 + src2;
            ConsoleWriteLine(src1 + " + " + src2 + " => $" + destReg);
            ConsoleWriteLine("$" + destReg + " = " + registers[destReg]);
        }

        public void Visit(Sub n)
        {
            n.Children.ForEach(x => x.Accept(this));
            var src2 = stack.Pop();
            var src1 = stack.Pop();
            stack.Pop();
            var destReg = Utils.GetRegisterIndex(n.Children[0]);
            registers[destReg] = src1 - src2;
            ConsoleWriteLine(src1 + " - " + src2 + " => $" + destReg);
            ConsoleWriteLine("$" + destReg + " = " + registers[destReg]);
        }

        public void Visit(Mul n)
        {
            n.Children.ForEach(x => x.Accept(this));
            var src2 = stack.Pop();
            var src1 = stack.Pop();
            stack.Pop();
            var destReg = Utils.GetRegisterIndex(n.Children[0]);
            registers[destReg] = src1 * src2;
            ConsoleWriteLine(src1 + " * " + src2 + " => $" + destReg);
            ConsoleWriteLine("$" + destReg + " = " + registers[destReg]);
        }

        public void Visit(Div n)
        {
            n.Children.ForEach(x => x.Accept(this));
            var src2 = stack.Pop();
            var src1 = stack.Pop();
            stack.Pop();
            var destReg = Utils.GetRegisterIndex(n.Children[0]);
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
            var destReg = Utils.GetRegisterIndex(n.Children[0]);
            var correctValue = int.TryParse(n.Children[1].Value, out var value);
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
            var inSel = Utils.GetRegisterIndex(n.Children[0]);
            var destReg = Utils.GetRegisterIndex(n.Children[1]);

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
            var outSel = Utils.GetRegisterIndex(n.Children[0]);
            var sourceReg = Utils.GetRegisterIndex(n.Children[1]);

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
            var source = Utils.GetRegisterIndex(n.Children[0]);
            var destination = Utils.GetRegisterIndex(n.Children[1]);
            registers[destination] = registers[source];
            ConsoleWriteLine("$" + source + "(" + registers[source] + ") => $" + destination + "(" +
                             registers[destination] + ")");
        }

        public void Visit(Condition n)
        {
            void ExecuteElse()
            {
                if (!n.HasElseCall) return;
                if (n.Children.Count < 3) throw new ArgumentException("should have 3 arguments !");
                n.Children[3].Accept(this);
            }
            //visit comparaison
            n.Children[0].Accept(this);
            if(stack.Pop() == 1) n.Children[1].Accept(this);
            else ExecuteElse();
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
            var index = Utils.GetRegisterIndex(n);
            return registers[index];
        }

        public void Visit(Block n) => n.Children.ForEach(l => l.Accept(this));
        
        public void Visit(While n) {
            ConsoleWriteLine("entering while");
            n.Children[0].Accept(this);
            var condition = stack.Pop() == 1;
            ConsoleWriteLine("initial condition is: " + condition);
            while (condition) {
                n.Children[1].Accept(this);
                //we do the comparison at the end 
                n.Children[0].Accept(this);
                condition = stack.Pop() == 1;
                ConsoleWriteLine("condition is: " + condition);
            }
            
        }

        public void Visit(Comparaison n) {
            n.Children[0].Accept(this);
            n.Children[1].Accept(this);
            var right = stack.Pop();
            var left = stack.Pop();
            ConsoleWriteLine(left + " " + n.Value + " " + right);
            var compResult = n.Value switch {
                ">" => left > right,
                "==" => left == right,
                "<" => left < right,
                ">=" => left >= right,
                "<=" => left <= right,
                _ => throw new ArgumentException("incorrect comparison operator")
            };
            stack.Push(compResult ? 1 : 0);
        }

    }
}