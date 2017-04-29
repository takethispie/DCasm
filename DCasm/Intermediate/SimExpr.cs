using System;
namespace DCasm
{
	public class SimExpr : Node
    {
		public Term left;
		public Term right;
		public string op;

		public SimExpr(Term l, string op, Term r)
		{
			left = l;
			right = r;
			this.op = op;
			// Console.WriteLine()
		}

		public override void Resolve()
		{
			left.Resolve();
			if (right != null)
				right.Resolve();
			switch (op)
			{
				case "*":
					break;

				case "/":
					break;
			}
		}
	}
}
