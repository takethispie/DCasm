using System;
namespace DCasm
{
	public class ArithmCommand : ICommand
    {
		//expr command
		ICommand left,right;

        public ICommand Next { get; set; }

        public bool IsFinal { get; set; }

        public void Execute()
		{
			
		}

        void ICommand.Execute()
        {
            throw new NotImplementedException();
        }
    }
}
