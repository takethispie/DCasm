
namespace DCasm.Visitors
{
    public interface IVisitor
    {
        void Visit(Store n);
        void Visit(Const n);
        void Visit(Function n);
        void Visit(Call n);
        void Visit(Load n);
        void Visit(Add n);
        void Visit(Sub n);
        void Visit(Mul n);
        void Visit(Div n);
        void Visit(Register n);
        void Visit(ImmediateLoad n);
        void Visit(Read n);
        void Visit(Write n);
        void Visit(Move n);
        void Visit(Condition n);
        void Visit(Block n);
        void Visit(While @while);

    }
}