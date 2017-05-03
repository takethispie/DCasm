using System;
namespace DCasm
{
    public class Symbol
    {
		public SymbolType Type;
		public int Adress;

		public Symbol(SymbolType type)
		{
			Type = type;
			Adress = -1;
		}
    }
}
