namespace DCasm;

public class Add : IArithmeticNode
{
    public INode Left { get; set; }
    public INode Right { get; set; }
    public INode Destination { get; set; }

    public bool Unsigned { get; set; }
    public string Value { get; set; }
    
    public Add(bool isImmediate)
    {
        Value = isImmediate ? "addi" : "add";
    }


}