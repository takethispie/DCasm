using System.Collections.Generic;
using DCasm.Visitors;

namespace DCasm
{
    public class Return : INode
    {
        public string Value { get; set; }

        public Return(string functionName) {
            Value = functionName;
        }
    }
}