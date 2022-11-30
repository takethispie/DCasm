namespace DCasm;

public class Mul : IArithmeticNode
{
    public Mul(bool isImmediate)
    {
        Value = isImmediate ? "muli" : "mul";
    }

    public INode Left { get; set; }
    public INode Right { get; set; }
    public INode Destination { get; set; }

    public string Value { get; set; }
}