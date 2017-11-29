using System;
using System.Collections.Generic;

namespace  DCasm {
    public class Visitor {
        public virtual void VisitStore(Node n){}
        public virtual void VisitGlobStore(Node n){}
        public virtual void VisitConst(Node n){}
        public virtual void VisitCall(Node n){}
        public virtual void VisitFunction(Node n){}
        public virtual void VisitLoad(Node n){}
        public virtual void VisitGlobLoad(Node n){}
        public virtual void VisitAdd(Node n){}
        public virtual void VisitSub(Node n){}
        public virtual void VisitMul(Node n){}
        public virtual void VisitDiv(Node n){}
        public virtual void VisitNeg(Node n){}
        public virtual void VisitEqu(Node n){}
        public virtual void VisitLSS(Node n){}
        public virtual void VisitGTR(Node n){}
        public virtual void VisitRet(Node n){}
        public virtual void VisitRead(Node n){}
        public virtual void VisitWrite(Node n){}
    }
}