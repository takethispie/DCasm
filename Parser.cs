
using System;

namespace DCasm {



public class Parser {
	public const int _EOF = 0;
	public const int _ident = 1;
	public const int _number = 2;
	public const int _registerNum = 3;
	public const int maxT = 13;

	const bool _T = true;
	const bool _x = false;
	const int minErrDist = 2;
	
	public Scanner scanner;
	public Errors  errors;

	public Token t;    // last recognized token
	public Token la;   // lookahead token
	int errDist = minErrDist;

public CodeGenerator gen;
  public NodeFactory factory;
  
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
		Expect(4);
		while (StartOf(1)) {
			arithm(out INode exp);
			gen.treeRoot.Childrens.Add(exp); 
		}
		Expect(0);
	}

	void arithm(out INode exp) {
		exp = factory.Create("Error"); 
		if (StartOf(2)) {
			arithmOp(out string op);
			register(out INode dest);
			register(out INode src1);
			register(out INode src2);
			exp = factory.Create(op); exp.Childrens.Add(dest); exp.Childrens.Add(src1); exp.Childrens.Add(src2); 
		} else if (StartOf(3)) {
			arithmImOp(out string op);
			register(out INode dest);
			register(out INode src1);
			constant(out INode imm);
			exp = factory.Create(op); exp.Childrens.Add(dest); exp.Childrens.Add(src1); exp.Childrens.Add(imm); 
		} else SynErr(14);
	}

	void register(out INode node) {
		Expect(3);
		node = factory.Create("register"); node.Value = t.val; 
	}

	void constant(out INode val) {
		Expect(2);
		val = new Const(t.val); 
	}

	void arithmOp(out string op) {
		if (la.kind == 5) {
			Get();
		} else if (la.kind == 6) {
			Get();
		} else if (la.kind == 7) {
			Get();
		} else if (la.kind == 8) {
			Get();
		} else SynErr(15);
		op = t.val; 
	}

	void arithmImOp(out string op) {
		if (la.kind == 9) {
			Get();
		} else if (la.kind == 10) {
			Get();
		} else if (la.kind == 11) {
			Get();
		} else if (la.kind == 12) {
			Get();
		} else SynErr(16);
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
		{_T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x},
		{_x,_x,_x,_x, _x,_T,_T,_T, _T,_T,_T,_T, _T,_x,_x},
		{_x,_x,_x,_x, _x,_T,_T,_T, _T,_x,_x,_x, _x,_x,_x},
		{_x,_x,_x,_x, _x,_x,_x,_x, _x,_T,_T,_T, _T,_x,_x}

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
			case 5: s = "\"add\" expected"; break;
			case 6: s = "\"sub\" expected"; break;
			case 7: s = "\"div\" expected"; break;
			case 8: s = "\"mul\" expected"; break;
			case 9: s = "\"addi\" expected"; break;
			case 10: s = "\"subu\" expected"; break;
			case 11: s = "\"divu\" expected"; break;
			case 12: s = "\"mulu\" expected"; break;
			case 13: s = "??? expected"; break;
			case 14: s = "invalid arithm"; break;
			case 15: s = "invalid arithmOp"; break;
			case 16: s = "invalid arithmImOp"; break;

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