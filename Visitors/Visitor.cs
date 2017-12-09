using System;
using System.Collections.Generic;

namespace DCasm
{
    public abstract class Visitor
    {
        public abstract void VisitRoot(Node n);
        public abstract void VisitStore(Node n);
        public abstract void VisitGlobStore(Node n);
        public abstract void VisitConst(Node n);
        public abstract void VisitCall(Node n);
        public abstract void VisitFunction(Node n);
        public abstract void VisitLoad(Node n);
        public abstract void VisitGlobLoad(Node n);
        public abstract void VisitAdd(Node n);
        public abstract void VisitSub(Node n);
        public abstract void VisitMul(Node n);
        public abstract void VisitDiv(Node n);
        public abstract void VisitNeg(Node n);
        public abstract void VisitEqu(Node n);
        public abstract void VisitLSS(Node n);
        public abstract void VisitGTR(Node n);
        public abstract void VisitRead(Node n);
        public abstract void VisitWrite(Node n);
    }
}