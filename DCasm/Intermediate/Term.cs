using System;
namespace DCasm
{
	public class Term : Node
	{
		public Factor left;
		public Factor right;
		string op;

		public Term(Factor l, string op, Factor r)
		{
			left = l;
			right = r;
			this.op = op;
			Console.WriteLine(l.value + " " + op + " " + r.value);
		}

		public override void Resolve()
		{
			left.Resolve();
			if (right != null)
				right.Resolve();
			switch (op)
			{
				case "+":
					break;

				case "-":
					break;
			}
		}
	}
}
