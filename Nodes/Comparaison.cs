using System.Collections.Generic;
using DCasm.Visitors;

namespace DCasm; 
public class Comparaison : INode {

    public string Value { get; set; }
    public INode Left, Right;

    public Comparaison(string op, INode left, INode right) {
        Value = op;
        Left = left;
        Right = right;
    }


}