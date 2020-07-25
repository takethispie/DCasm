namespace DCasm
{
    public interface IArithmeticNode : INode
    {
        INode Left { get; set; }
        INode Right { get; set; }
        INode Destination { get; set; }
    }
}