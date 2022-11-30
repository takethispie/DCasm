namespace DCasm;

public class LeftShift : IArithmeticNode
{

    public string Value { get; set; }
    public INode Destination { get; set;}
    public INode Left { get; set; }
    public INode Right { get; set; }

    public LeftShift(bool imm) {
        Value = imm ? "lshi" : "lsh"; 
    }

}