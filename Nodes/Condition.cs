using System.Collections.Generic;
using DCasm.Visitors;

namespace DCasm;

public class Condition : INode
{
    public bool HasElseCall { get; set; }
    public INode Then, Else;
    public Comparaison Comparaison;
    public string Value { get; set; }

    public Condition(Comparaison comp, INode thenInstructions)
    {
        Comparaison = comp;
        Then = thenInstructions;
        HasElseCall = false;
    }

    public Condition(Comparaison comp, INode thenInstructions, INode elseInstructions)
    {
        Comparaison = comp;
        Then = thenInstructions;
        Else = elseInstructions;
        HasElseCall = true;
    }
}