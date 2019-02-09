
using System;

namespace DCasm {



public class Parser {
	public const int _EOF = 0;
	public const int _ident = 1;
	public const int _number = 2;
	public const int _registerNum = 3;
	public const int maxT = 11;

	const bool _T = true;
	const bool _x = false;
	const int minErrDist = 2;
	
	public Scanner scanner;
	public Errors  errors;

	public Token t;    // last recognized token
	public Token la;   // lookahead token
	int errDist = minErrDist;

public CodeGenerator gen;
  
/*--------------------------------------------------------------------------*/


	public Parser(Scanner scanner) {
		this.scanner = scanner;
		errors = new Errors();
	}

	void SynErr (int n) {
		if (errDist >= minErrDist) errors.SynErr(la.line, la.col, n);
		errDist = 0;
	}

	public void SemErr (string msg) {
		if (errDist >= minErrDist) errors.SemErr(t.line, t.col, msg);
		errDist = 0;
	}
	
	void Get () {
		for (;;) {
			t = la;
			la = scanner.Scan();
			if (la.kind <= maxT) { ++errDist; break; }

			la = t;
		}
	}
	
	void Expect (int n) {
		if (la.kind==n) Get(); else { SynErr(n); }
	}
	
	bool StartOf (int s) {
		return set[s, la.kind];
	}
	
	void ExpectWeak (int n, int follow) {
		if (la.kind == n) Get();
		else {
			SynErr(n);
			while (!StartOf(follow)) Get();
		}
	}


	bool WeakSeparator(int n, int syFol, int repFol) {
		int kind = la.kind;
		if (kind == n) {Get(); return true;}
		else if (StartOf(repFol)) {return false;}
		else {
			SynErr(n);
			while (!(set[syFol, kind] || set[repFol, kind] || set[0, kind])) {
				Get();
				kind = la.kind;
			}
			return StartOf(syFol);
		}
	}

	
	void DCasm() {
		INode exp; exp = new Error(); 
		Expect(4);
		while (StartOf(1)) {
			if (StartOf(2)) {
				arithm(out exp);
			} else {
				immediateLoad(out exp);
			}
			gen.treeRoot.Childrens.Add(exp); 
		}
		Expect(0);
	}

	void arithm(out INode exp) {
		exp = new Error(); bool unsigned = false; 
		arithmOp(out string op);
		if (la.kind == 5) {
			Get();
			unsigned = true; 
		}
		register(out Register dest);
		register(out Register src1);
		if (la.kind == 3) {
			register(out Register src2);
			exp = ArithmFactory.Create(op, dest, src1, src2); 
		} else if (la.kind == 2) {
			constant(out Const src2);
			exp = ArithmFactory.Create(op, dest, src1, src2); 
		} else SynErr(12);
	}

	void immediateLoad(out INode exp) {
		Expect(6);
		register(out Register dest);
		constant(out Const val);
		exp = new ImmediateLoad(); exp.Childrens.Add(dest); exp.Childrens.Add(val); 
	}

	void register(out Register node) {
		Expect(3);
		node = new Register(); node.Value = t.val; 
	}

	void constant(out Const val) {
		Expect(2);
		val = new Const(t.val); 
	}

	void arithmOp(out string op) {
		op = "";
		if (la.kind == 7) {
			Get();
		} else if (la.kind == 8) {
			Get();
		} else if (la.kind == 9) {
			Get();
		} else if (la.kind == 10) {
			Get();
		} else SynErr(13);
		op = t.val; 
	}



	public void Parse() {
		la = new Token();
		la.val = "";		
		Get();
		DCasm();
		Expect(0);

	}
	
	static readonly bool[,] set = {
		{_T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x},
		{_x,_x,_x,_x, _x,_x,_T,_T, _T,_T,_T,_x, _x},
		{_x,_x,_x,_x, _x,_x,_x,_T, _T,_T,_T,_x, _x}

	};
} // end Parser


public class Errors {
	public int count = 0;                                    // number of errors detected
	public System.IO.TextWriter errorStream = Console.Out;   // error messages go to this stream
	public string errMsgFormat = "-- line {0} col {1}: {2}"; // 0=line, 1=column, 2=text

	public virtual void SynErr (int line, int col, int n) {
		string s;
		switch (n) {
			case 0: s = "EOF expected"; break;
			case 1: s = "ident expected"; break;
			case 2: s = "number expected"; break;
			case 3: s = "registerNum expected"; break;
			case 4: s = "\"program\" expected"; break;
			case 5: s = "\"uns\" expected"; break;
			case 6: s = "\"li\" expected"; break;
			case 7: s = "\"add\" expected"; break;
			case 8: s = "\"sub\" expected"; break;
			case 9: s = "\"div\" expected"; break;
			case 10: s = "\"mul\" expected"; break;
			case 11: s = "??? expected"; break;
			case 12: s = "invalid arithm"; break;
			case 13: s = "invalid arithmOp"; break;

			default: s = "error " + n; break;
		}
		errorStream.WriteLine(errMsgFormat, line, col, s);
		count++;
	}

	public virtual void SemErr (int line, int col, string s) {
		errorStream.WriteLine(errMsgFormat, line, col, s);
		count++;
	}
	
	public virtual void SemErr (string s) {
		errorStream.WriteLine(s);
		count++;
	}
	
	public virtual void Warning (int line, int col, string s) {
		errorStream.WriteLine(errMsgFormat, line, col, s);
	}
	
	public virtual void Warning(string s) {
		errorStream.WriteLine(s);
	}
} // Errors


public class FatalError: Exception {
	public FatalError(string m): base(m) {}
}
}