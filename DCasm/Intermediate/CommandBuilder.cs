using System;
namespace DCasm
{
    public class CommandBuilder
    {
        public CommandBuilder()
        {
        }

		public ICommand CreateArith(ICommand expr,ICommand rightExpr)
		{
			if(rightExpr != null) {
				Console.WriteLine("right expression is not null");
			}
			return null;
		}
    }
}
