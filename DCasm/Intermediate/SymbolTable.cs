using System;
using System.Collections.Generic;
namespace DCasm
{
	public enum SymbolType
	{
		Var,
		Block
	}

    public class SymbolTable
    {
		public Dictionary<string, Symbol> Symbols;

		public void Add(string key, SymbolType value)
		{
			Symbols.Add(key, new Symbol(value));
		}

		public bool SymbolExist(string key)
		{
			return Symbols.ContainsKey(key);
		}

        public SymbolTable()
        {
			Symbols = new Dictionary<string, Symbol>();
        }
    }
}
