namespace DCasm;

public class Block : INode
{
    public string Value { get; set; }
    public List<INode> Children { get; set; }

    public Block() {
        Children = new List<INode>(); 
    }
}