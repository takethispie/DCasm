using System;
using System.Collections.Generic;

namespace DCasm
{
	public class Block
    {
		public static List<Block> Program = new List<Block>();
		public long StartAdress;
		public string Name;
		public SymbolTable LocalSymbols;

        public Block(string name)
        {
			Name = name;
			StartAdress = 0;
			LocalSymbols = new SymbolTable();
        }

		public void Resolve()
		{
			
		}
	}
}
