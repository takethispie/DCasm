namespace DCasm;

public class Load : INode
{
    public Load(INode dest, INode baseReg, INode offset)
    {
        Value = "load";
        Destination = dest;
        BaseRegister = baseReg;
        OffsetRegister = offset;
    }

    public string Value { get; set; }
    public INode Destination { get; set; }
    public INode BaseRegister { get; set; }
    public INode OffsetRegister { get; set; }
}