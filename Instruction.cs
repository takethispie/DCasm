using System;
using System.Collections.Generic;

namespace DCasm {

    public enum DataType {
        OP = 1,
        DATA = 2
    }

    public interface ICodeFragment {
        DataType type { get; set; }
        string value { get; set; }
    }

    public class Instruction { 
        public long position;
        private List<ICodeFragment> fragments;

        public void AddFragment(ICodeFragment fragmentToAdd) {
            fragments.Add(fragmentToAdd);
        }
    }
}