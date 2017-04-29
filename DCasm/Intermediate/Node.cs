using System;
namespace DCasm
{
	public abstract class Node 
	{
		/// <summary>
		/// final intermediate langage representation for this node
		/// </summary>
		public string Final;

		/// <summary>
		/// resolve this node to create the final IL
		/// </summary>
		public abstract void Resolve();
	}
}
