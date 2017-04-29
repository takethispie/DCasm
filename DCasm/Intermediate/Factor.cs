using System;
namespace DCasm
{
	public class Factor : Node
    {
		public int type;
		public string value;

        public Factor(string value, int type)
        {
			this.value = value;
			this.type = type;
        }

		public override void Resolve()
		{
			
		}
	}
}
