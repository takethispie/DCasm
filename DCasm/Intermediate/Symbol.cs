using System;
namespace DCasm
{
    public class Symbol
    {
		public string val;
		public int type;

		public Symbol(string v, int t)
        {
			val = v;
			type = t;
        }
    }
}
