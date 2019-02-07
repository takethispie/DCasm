namespace DCasm
{
    public class NodeFactory
    {
        public INode Create(string item) {
            switch (item)
            {
                case "register":
                return new Register();

                case "add":
                return new Add();

                case "sub":
                return new Sub();

                case "div":
                return new Div();

                case "mul":
                return new Mul();

                default:
                return new Error();
            }
        }
    }
}