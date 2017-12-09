using System;
using System.Collections.Generic;

namespace DCasm
{
    public class PrintVisitor : Visitor
    {
        protected PrintVisitor()
        {
        }

        public override void VisitAdd(Node n)
        {
            if(n.Childrens.Count > 1) {
                n.Childrens[0].Accept(this);
                Console.WriteLine("+");
                n.Childrens[1].Accept(this);
            }
        }

        public override void VisitCall(Node n)
        {
            Console.WriteLine("Calling function: " + ((Function)n).Name);
        }

        public override void VisitConst(Node n)
        {
            Console.WriteLine(((Const)n).GetValue());
        }

        public override void VisitDiv(Node n)
        {
            throw new NotImplementedException();
        }

        public override void VisitEqu(Node n)
        {
            throw new NotImplementedException();
        }

        public override void VisitFunction(Node n)
        {
            throw new NotImplementedException();
        }

        public override void VisitGlobLoad(Node n)
        {
            throw new NotImplementedException();
        }

        public override void VisitGlobStore(Node n)
        {
            throw new NotImplementedException();
        }

        public override void VisitGTR(Node n)
        {
            throw new NotImplementedException();
        }

        public override void VisitLoad(Node n)
        {
            throw new NotImplementedException();
        }

        public override void VisitLSS(Node n)
        {
            throw new NotImplementedException();
        }

        public override void VisitMul(Node n)
        {
            throw new NotImplementedException();
        }

        public override void VisitNeg(Node n)
        {
            throw new NotImplementedException();
        }

        public override void VisitRead(Node n)
        {
            throw new NotImplementedException();
        }

        public override void VisitRoot(Node n)
        {
            n.Childrens.ForEach(child => child.Accept(this));
        }

        public override void VisitStore(Node n)
        {
            throw new NotImplementedException();
        }

        public override void VisitSub(Node n)
        {
            throw new NotImplementedException();
        }

        public override void VisitWrite(Node n)
        {
            throw new NotImplementedException();
        }
    }
}