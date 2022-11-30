namespace DCasm;

public class Call : INode
{
    public Call(string name)
    {
        Value = name;
    }

    public string Value { get; set; }
}