using System;
namespace DCasm
{
    public class CommandBuilder
    {
        public CommandBuilder()
        {
        }

		public ICommand CreateArith(ICommand expr)
		{
			ValueAttributionCommand valCom;
			return null;
		}

		public ICommand CreateArith(ICommand expr,ICommand rightExpr)
		{
			ArithmCommand arCo;
			return null;   
		}
    }
}
