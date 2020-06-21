namespace DCasm
{
    public static class ArithmFactory
    {
        public static INode Create(string op, INode dest, INode src1, INode src2) => src2 switch
        {
            Const v => Create(op, dest, src1, src2, true),
            Register r => Create(op, dest, src1, src2, false),
            _ => new Error("Wrong argument type")
        };

        private static INode Create(string op, INode dest, INode src1, INode src2, bool isImmediate)
        {
            var node = GetOperand(op, true);
            node.Children.Add(dest);
            node.Children.Add(src1);
            node.Children.Add(src2);
            return node;
        }

        public static INode GetOperand(string op, bool immediate) => op switch {
            "add" => new Add(immediate ? "addi" : "add"),
            "sub" => new Sub(immediate ? "subi" : "sub"),
            "mul" => new Mul(immediate ? "muli" : "mul"),
            "div" => new Div(immediate ? "divi" : "div"),
            _ => new Error()
        };
    }
}