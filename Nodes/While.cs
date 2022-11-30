using System;
using System.Collections.Generic;
using DCasm.Visitors;

namespace DCasm;

public class While : INode
{
    public string Value { get; set; }
    public Comparaison Comparaison { get; set; }
    public Block Block { get; set; }


    public While(Block block, Comparaison comp) {
        Comparaison = comp;
        Block = block;
    }

}