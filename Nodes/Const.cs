using System;
using System.Collections.Generic;
using System.IO;

namespace DCasm
{
    public class Const : Node {
        private int value;

        public Const(int value) {
            this.value = value;    
        }

        public int GetValue() {
            return this.value;
        }

        public void Accept(Visitor v) { v.VisitConst(this); }
    }
}