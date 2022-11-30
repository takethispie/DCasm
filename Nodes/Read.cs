namespace DCasm;

public class Read : INode
{
    public string Value { get; set; }
    public INode InputSelection { get; set; }
    public INode Destination { get; set; }

    public Read(INode inputSelection, INode destination)
    {
        Value = "in";
        InputSelection = inputSelection;
        Destination = destination;
    }

}