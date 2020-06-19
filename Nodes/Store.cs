using System.Collections.Generic;
using DCasm.Visitors;

namespace DCasm
{
    public class Store : INode
    {
        public Store(INode baseReg, INode offsetReg, INode value)
        {
            Value = "store";
            DataValue = value;
            BaseRegister = baseReg;
            OffsetRegister = offsetReg;
        }

        public string Value { get; set; }
        public INode DataValue { get; set; }
        public INode BaseRegister { get; set; }
        public INode OffsetRegister { get; set; }
    }
}