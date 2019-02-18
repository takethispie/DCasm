using System.Collections.Generic;
using System;

namespace DCasm
{
    public class Interpreter : IVisitor
    {
        private int pc;
        private bool gt, eq, lt;
        private Dictionary<int, int> ram;
        private int[] registers;
        private Stack<int> stack;

        public Interpreter() {
            pc = 0;
            gt = false;
            eq = false;
            lt = false;
            registers = new int[32];
            stack = new Stack<int>();
            ram = new Dictionary<int, int>();
        }

        private int GetRegisterValue(INode n) {
            var index = GetRegisterIndex(n);
            return registers[index];
        }

        private int GetRegisterIndex(INode n) {
            var reg = n.Value.Remove(0,1);
            var correctReg = int.TryParse(reg, out int regNumber);
            if(correctReg)  return regNumber;
            else throw new Exception("cannot parse register number !");
        }

        public void Visit(Root n) => n.Childrens.ForEach(x => x.Accept(this));

        public void Visit(Store n)
        {
            string valueReg = n.Childrens[0].Value.Remove(0,1);
            var correctValueReg = int.TryParse(valueReg, out int valueNum);

            var baseReg = n.Childrens[1].Value.Remove(0,1);
            var correctBaseReg = int.TryParse(baseReg, out int baseNum);

            var offset = n.Childrens[2].Value;
            var correctOffset = int.TryParse(offset, out int parsedOffset);

            if(!correctOffset || !correctValueReg || !correctBaseReg) throw new Exception("one or more arguments could not be parsed to integer !");

            var valueToStore = registers[valueNum];
            var adress = registers[baseNum];
            var storeAddress = adress + parsedOffset;

            if(!ram.ContainsKey(storeAddress)) ram.Add(storeAddress, valueToStore);    
            else ram[storeAddress] = valueToStore;
            Console.WriteLine("stored " + valueToStore + " to adress " + storeAddress);
        }

        public void Visit(Const n) => stack.Push(n.ToInt());

        public void Visit(Function n)
        {
        }

        public void Visit(Call n) {

        }

        public void Visit(Load n)
        {
            string destReg = n.Childrens[0].Value.Remove(0,1);
            var correctDestReg = int.TryParse(destReg, out int destNum);

            var baseReg = n.Childrens[1].Value.Remove(0,1);
            var correctBaseReg = int.TryParse(baseReg, out int baseNum);

            var offset = n.Childrens[2].Value;
            var correctOffset = int.TryParse(offset, out int parsedOffset);

            if(!correctOffset || !correctDestReg || !correctBaseReg) throw new Exception("one or more arguments could not be parsed to integer !");

            var destRegister = registers[destNum];
            var adress = registers[baseNum];
            var loadAddress = adress + parsedOffset;

            if(!ram.ContainsKey(loadAddress)) registers[destRegister] = 0;
            else registers[destRegister] = ram[loadAddress];
            Console.WriteLine("Loaded value " + registers[destRegister] + " from adress " + loadAddress);
            
        }

        public void Visit(Add n)
        {
            n.Childrens.ForEach(x => x.Accept(this));
            var src2 = stack.Pop();
            var src1 = stack.Pop();
            stack.Pop();
            var regNumber = GetRegisterIndex(n.Childrens[0]);
            registers[regNumber] = src1 + src2;
            Console.WriteLine(src1 + " + " + src2 + " => $" + regNumber);
            Console.WriteLine("$" + regNumber + " = " + registers[regNumber]);
        }

        public void Visit(Sub n)
        {
            n.Childrens.ForEach(x => x.Accept(this));
            var src2 = stack.Pop();
            var src1 = stack.Pop();
            stack.Pop();
            var regNumber = GetRegisterIndex(n.Childrens[0]);
            registers[regNumber] = src1 - src2;
            Console.WriteLine(src1 + " - " + src2 + " => $" + regNumber);
            Console.WriteLine("$" + regNumber + " = " + registers[regNumber]);
        }

        public void Visit(Mul n)
        {
            n.Childrens.ForEach(x => x.Accept(this));
            var src2 = stack.Pop();
            var src1 = stack.Pop();
            stack.Pop();
            var regNumber = GetRegisterIndex(n.Childrens[0]);
            registers[regNumber] = src1 * src2;
            Console.WriteLine(src1 + " * " + src2 + " => $" + regNumber);
            Console.WriteLine("$" + regNumber + " = " + registers[regNumber]);
        }

        public void Visit(Div n)
        {
            n.Childrens.ForEach(x => x.Accept(this));
            var src2 = stack.Pop();
            var src1 = stack.Pop();
            stack.Pop();
            var regNumber = GetRegisterIndex(n.Childrens[0]);
            if(src2 == 0) throw new DivideByZeroException();
            registers[regNumber] = src1 / src2;
            Console.WriteLine(src1 + " - " + src2 + " => $" + regNumber);
            Console.WriteLine("$" + regNumber + " = " + registers[regNumber]);
        }

        public void Visit(Register n) => stack.Push(GetRegisterValue(n));

        public void Visit(ImmediateLoad n)
        {
            var reg = n.Childrens[0].Value.Remove(0,1);
            var correctReg = int.TryParse(reg, out int regNumber);
            var correctValue = int.TryParse(n.Childrens[1].Value, out int value);
            if(correctReg && correctValue)  registers[regNumber] = value;
            else throw new Exception("cannot parse immediate load parameters !");
            Console.WriteLine("stored " + value + " in $" + regNumber);
        }

        public void Visit(Read n)
        {
            var inSel = n.Childrens[0].Value.Remove(0,1);
            var correctInSel = int.TryParse(inSel, out int parsedInSel);

            var destReg = n.Childrens[1].Value.Remove(0,1);
            var correctDestReg = int.TryParse(destReg, out int parsedDestReg);

            if(!correctDestReg || !correctInSel) throw new Exception("one or more argument could not be parsed to integer !");

            switch(parsedInSel) {
                case 0:
                Console.Write("IN>");
                var key = Console.Read();
                registers[parsedDestReg] = key;
                Console.Write(Environment.NewLine);
                Console.WriteLine("read " + key + " from terminal and stored it in " + parsedDestReg);
                break;
            }
        }

        public void Visit(Write n)
        {
            var outSel = n.Childrens[0].Value.Remove(0,1);
            var correctOutSel = int.TryParse(outSel, out int parsedOutSel);

            var sourceReg = n.Childrens[1].Value.Remove(0,1);
            var correctSourceReg = int.TryParse(sourceReg, out int parsedSourceReg);

            if(!correctOutSel || !correctSourceReg) throw new Exception("one or more arguments could not be parsed to integer !");

            switch(parsedOutSel) {
                case 0:
                Console.Write("OUT>");
                var val = registers[parsedSourceReg];
                Console.WriteLine(val);
                break;
            }
        }

        public void Visit(Move n) {
            var source = stack.Pop();
            var destination = stack.Pop();
            registers[destination] = registers[source];
            Console.WriteLine("$" + source + "(" + registers[source] + ") => $" + destination + "(" + registers[destination] + ")");
        }
    }
}