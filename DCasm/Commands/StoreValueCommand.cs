using System;
namespace DCasm
{
	public class StoreValueCommand : ICommand
    {

		public ICommand Next { get; set; }

		public bool IsFinal { get; set; }
		
        public StoreValueCommand()
        {
        }

		public void Execute()
		{
			
		}
	}
}
