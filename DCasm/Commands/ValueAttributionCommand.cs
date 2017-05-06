using System;
namespace DCasm
{
	public class ValueAttributionCommand : ICommand
    {
		public ICommand Next { get; set; }

		public bool IsFinal { get; set; }

        public ValueAttributionCommand()
        {
        }

		public void Execute()
		{
			
		}
	}
}
