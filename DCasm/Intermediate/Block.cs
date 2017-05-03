using System;
using System.Collections.Generic;

namespace DCasm
{
	public class Block : Node
    {
		public static List<Block> Program = new List<Block>();
		public long StartAdress;
		public List<Node> Childs;
		public string Name;
		public SymbolTable LocalSymbols;

        public Block(string name)
        {
			Name = name;
			Childs = new List<Node>();
			StartAdress = 0;
			LocalSymbols = new SymbolTable();
        }

		public override void Resolve()
		{
			
		}
	}
}
