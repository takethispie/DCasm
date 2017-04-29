
using System;

namespace DCasm {



public class Parser {
	public const int _EOF = 0;
	public const int _hex = 1;
	public const int _reg = 2;
	public const int _ident = 3;
	public const int _number = 4;
	public const int _string = 5;
	public const int _semicolon = 6;
	public const int maxT = 28;

	const bool _T = true;
	const bool _x = false;
	const int minErrDist = 2;
	
	public Scanner scanner;
	public Errors  errors;

	public Token t;    // last recognized token
	public Token la;   // lookahead token
	int errDist = minErrDist;

const int // types
	  undef = 0, integer = 1, boolean = 2, symbol = 3;

	const int // object kinds
	  var = 0, proc = 1;

    public CodeGenerator gen;
    public int adress = 0;
    public I_ISA CurrentISA;

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
		if (la.kind == 13) {
			ProcDecl();
		} else if (la.kind == 25 || la.kind == 26) {
			VarDecl();
		} else SynErr(29);
		while (la.kind == 13 || la.kind == 25 || la.kind == 26) {
			if (la.kind == 13) {
				ProcDecl();
			} else {
				VarDecl();
			}
		}
		Expect(0);
	}

	void ProcDecl() {
		string name; int adr; 
		Expect(13);
		Ident(out name);
		Expect(14);
		Expect(15);
		Expect(16);
		while (StartOf(1)) {
			if (la.kind == 25 || la.kind == 26) {
				VarDecl();
			} else {
				Stat();
			}
		}
		Expect(17);
	}

	void VarDecl() {
		string name; int type; 
		Type(out type);
		Ident(out name);
		while (la.kind == 27) {
			Get();
			Ident(out name);
		}
		Expect(6);
	}

	void AddOp(out string op) {
		op = "ADD"; 
		if (la.kind == 7) {
			Get();
		} else if (la.kind == 8) {
			Get();
			op = "SUB"; 
		} else SynErr(30);
	}

	void Expr(out int type) {
		int type1; string op; 
		SimExpr(out type);
		if (la.kind == 18 || la.kind == 19 || la.kind == 20) {
			RelOp(out op);
			SimExpr(out type1);
			
		}
	}

	void SimExpr(out int type) {
		int type1; string op; 
		Term(out type);
		while (la.kind == 7 || la.kind == 8) {
			AddOp(out op);
			Term(out type1);
			
		}
	}

	void RelOp(out string op) {
		op = "EQU"; 
		if (la.kind == 18) {
			Get();
		} else if (la.kind == 19) {
			Get();
			op = "LSS"; 
		} else if (la.kind == 20) {
			Get();
			op = "GTR"; 
		} else SynErr(31);
	}

	void Factor(out int type, out string value) {
		int n; string name; 
		type = undef; value = "";
		if (la.kind == 3) {
			Ident(out name);
			value = name; type = symbol;
		} else if (la.kind == 4) {
			Get();
			value = t.val; type = integer; 
		} else if (la.kind == 8) {
			Get();
			Factor(out type, out value);
			value = "-" + t.val; /*if (type != integer) { SemErr("integer type expected"); type = integer; }*/
		} else if (la.kind == 9) {
			Get();
			value = t.val; type = boolean; 
		} else if (la.kind == 10) {
			Get();
			value = t.val; type = boolean; 
		} else SynErr(32);
	}

	void Ident(out string name) {
		Expect(3);
		name = t.val; 
	}

	void MulOp(out string op) {
		op = "MUL"; 
		if (la.kind == 11) {
			Get();
		} else if (la.kind == 12) {
			Get();
			op = "DIV"; 
		} else SynErr(33);
	}

	void Stat() {
		int type; string name; int adr, adr2, loopstart; 
		if (la.kind == 3) {
			Ident(out name);
			if (la.kind == 21) {
				Get();
				
				SimExpr(out type);
				Expect(6);
				
			} else if (la.kind == 14) {
				Get();
				Expect(15);
				Expect(6);
				
			} else SynErr(34);
		} else if (la.kind == 22) {
			Get();
			Expect(14);
			Expr(out type);
			Expect(15);
			
			Stat();
			if (la.kind == 23) {
				Get();
				Stat();
			}
		} else if (la.kind == 24) {
			Get();
			Expect(14);
			Expr(out type);
			Expect(15);
			
			Stat();
		} else if (la.kind == 16) {
			Get();
			while (StartOf(1)) {
				if (StartOf(2)) {
					Stat();
				} else {
					VarDecl();
				}
			}
			Expect(17);
		} else SynErr(35);
	}

	void Term(out int type) {
		int type1; string op; string val1 = ""; string val2 = "";
		Factor(out type,out val1);
		while (la.kind == 11 || la.kind == 12) {
			MulOp(out op);
			Factor(out type1, out val2);
			Term term = new Term(new Factor(val1,type),op,new Factor(val2,type)); 
		}
	}

	void Type(out int type) {
		type = undef; 
		if (la.kind == 25) {
			Get();
			type = integer; 
		} else if (la.kind == 26) {
			Get();
			type = boolean; 
		} else SynErr(36);
	}



	public void Parse() {
		la = new Token();
		la.val = "";		
		Get();
		DCasm();
		Expect(0);

	}
	
	static readonly bool[,] set = {
		{_T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x},
		{_x,_x,_x,_T, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _T,_x,_x,_x, _x,_x,_T,_x, _T,_T,_T,_x, _x,_x},
		{_x,_x,_x,_T, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _T,_x,_x,_x, _x,_x,_T,_x, _T,_x,_x,_x, _x,_x}

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
			case 1: s = "hex expected"; break;
			case 2: s = "reg expected"; break;
			case 3: s = "ident expected"; break;
			case 4: s = "number expected"; break;
			case 5: s = "string expected"; break;
			case 6: s = "semicolon expected"; break;
			case 7: s = "\"+\" expected"; break;
			case 8: s = "\"-\" expected"; break;
			case 9: s = "\"true\" expected"; break;
			case 10: s = "\"false\" expected"; break;
			case 11: s = "\"*\" expected"; break;
			case 12: s = "\"/\" expected"; break;
			case 13: s = "\"void\" expected"; break;
			case 14: s = "\"(\" expected"; break;
			case 15: s = "\")\" expected"; break;
			case 16: s = "\"{\" expected"; break;
			case 17: s = "\"}\" expected"; break;
			case 18: s = "\"==\" expected"; break;
			case 19: s = "\"<\" expected"; break;
			case 20: s = "\">\" expected"; break;
			case 21: s = "\"=\" expected"; break;
			case 22: s = "\"if\" expected"; break;
			case 23: s = "\"else\" expected"; break;
			case 24: s = "\"while\" expected"; break;
			case 25: s = "\"int\" expected"; break;
			case 26: s = "\"bool\" expected"; break;
			case 27: s = "\",\" expected"; break;
			case 28: s = "??? expected"; break;
			case 29: s = "invalid DCasm"; break;
			case 30: s = "invalid AddOp"; break;
			case 31: s = "invalid RelOp"; break;
			case 32: s = "invalid Factor"; break;
			case 33: s = "invalid MulOp"; break;
			case 34: s = "invalid Stat"; break;
			case 35: s = "invalid Stat"; break;
			case 36: s = "invalid Type"; break;

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