using System;
namespace DCasm
{
	public class ExprCommand : ICommand
    {
		int Type;
		string Val;

		public ICommand Next { get; set; }

		public bool IsFinal { get; set; }

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
