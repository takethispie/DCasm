using System.Collections.Generic;
using DCasm.Visitors;

namespace DCasm
{
    public class LeftShift : INode
    {

        public string Value { get; set; }
        public INode Destination { get; set;}
        public INode Left { get; set; }
        public INode Right { get; set; }

        public LeftShift(bool imm) {
            Value = imm ? "lshifti" : "lshift"; 
        }

    }
}