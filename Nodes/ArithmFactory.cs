namespace DCasm
{
    public static class ArithmFactory
    {
        public static INode Create(string op, Register dest, Register src1, Register src2)
        {
            var node = GetOperand(op, false);
            node.Children.Add(dest);
            node.Children.Add(src1);
            node.Children.Add(src2);
            return node;
        }

        public static INode Create(string op, Register dest, Register src1, Const src2)
        {
            var node = GetOperand(op, true);
            node.Children.Add(dest);
            node.Children.Add(src1);
            node.Children.Add(src2);
            return node;
        }

        public static INode GetOperand(string op, bool immediate)
        {
            switch (op)
            {
                case "add":
                    return new Add(immediate ? "addi" : "add");

                case "sub":
                    return new Sub(immediate ? "subi" : "sub");

                case "mul":
                    return new Mul(immediate ? "muli" : "mul");

                case "div":
                    return new Div(immediate ? "divi" : "div");

                default:
                    return new Error();
            }
        }
    }
}