using System.Collections.Generic;
using DCasm.Visitors;

namespace DCasm.Translator {

    public class Flattener : IVisitor {

        public List<INode> FlatProgram;


        public Flattener(Dictionary<string, INode> functions) {
            FlatProgram = new List<INode>();
            foreach (var (key, value) in functions) {
                value.Children.ForEach(child => child.Accept(this));
            }
        }

        public void Visit(Store n) {
            var dest = n.Children[0];
            var value = n.Children[1];
            n.Children = new List<INode>();
            FlatProgram.Add(n);
            dest.Accept(this);
            value.Accept(this);
        }


        public void Visit(Const n) {
            FlatProgram.Add(n);
        }


        public void Visit(Function n) {
            var body = n.Children;
            n.Children = new List<INode>();
            FlatProgram.Add(n);
            body.ForEach(line => line.Accept(this));
            FlatProgram.Add(new Return(n.Value));
        }


        public void Visit(Call n) {
            FlatProgram.Add(n);
        }


        public void Visit(Load n) {
            var dest = n.Children[0];
            var baseReg = n.Children[1];
            var offset = n.Children[2];
            n.Children = new List<INode>();
            FlatProgram.Add(n);
            dest.Accept(this);
            baseReg.Accept(this);
            offset.Accept(this);
        }


        public void Visit(Add n) {
            throw new System.NotImplementedException();
        }


        public void Visit(Sub n) {
            throw new System.NotImplementedException();
        }


        public void Visit(Mul n) {
            throw new System.NotImplementedException();
        }


        public void Visit(Div n) {
            throw new System.NotImplementedException();
        }


        public void Visit(Register n) {
            throw new System.NotImplementedException();
        }


        public void Visit(ImmediateLoad n) {
            throw new System.NotImplementedException();
        }


        public void Visit(Read n) {
            throw new System.NotImplementedException();
        }


        public void Visit(Write n) {
            throw new System.NotImplementedException();
        }


        public void Visit(Move n) {
            throw new System.NotImplementedException();
        }


        public void Visit(Condition n) {
            throw new System.NotImplementedException();
        }


        public void Visit(Block n) {
            throw new System.NotImplementedException();
        }


        public void Visit(While n) {
            throw new System.NotImplementedException();
        }


        public void Visit(Comparaison comparaison) {
            throw new System.NotImplementedException();
        }


        public void Visit(Return ret) {
            throw new System.NotImplementedException();
        }

    }

}