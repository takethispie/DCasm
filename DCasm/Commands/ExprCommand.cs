using System;
namespace DCasm
{
	public class ExprCommand : ICommand
    {
		int Type;
		string Val;

        public ExprCommand(int type, string val)
        {
			Type = type;
			Val = val;
        }

		public void Execute()
		{
			
		}
	}
}
