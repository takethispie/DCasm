namespace DCasm;

public class RightShift : IArithmeticNode
{

    public string Value { get; set; }
    public INode Destination { get; set;}
    public INode Left { get; set; }
    public INode Right { get; set; }

    public RightShift(bool imm) {
        Value = imm ? "rshi" : "rsh"; 
    }
}