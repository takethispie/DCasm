
using System;

namespace DCasm {



public class Parser {
	public const int _EOF = 0;
	public const int _ident = 1;
	public const int _number = 2;
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
	  undef = 0, integer = 1, boolean = 2;

	const int // object kinds
	  var = 0, proc = 1;

	public SymbolTable   tab;
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

	
	void AddOp(out Op op) {
		op = Op.ADD; 
		if (la.kind == 3) {
			Get();
		} else if (la.kind == 4) {
			Get();
			op = Op.SUB; 
		} else SynErr(29);
	}

	void Expr(out int type) {
		int type1; Op op; 
		SimExpr(out type);
		if (la.kind == 7 || la.kind == 8 || la.kind == 9) {
			RelOp(out op);
			SimExpr(out type1);
			if (type != type1) SemErr("incompatible types"); gen.Emit(op); type = boolean; 
		}
	}

	void SimExpr(out int type, out INode node) {
		int type1; Op op; INode node, left, right; 
		Term(out type);
		while (la.kind == 3 || la.kind == 4) {
			AddOp(out op);
			Term(out type1);
			if (type != integer || type1 != integer) 
			 SemErr("integer type expected");
			gen.Emit(op); 
		}
	}

	void RelOp(out Op op) {
		op = Op.EQU; 
		if (la.kind == 7) {
			Get();
		} else if (la.kind == 8) {
			Get();
			op = Op.LSS; 
		} else if (la.kind == 9) {
			Get();
			op = Op.GTR; 
		} else SynErr(30);
	}

	void Ident(out string name) {
		Expect(1);
		name = t.val; 
	}

	void MulOp(out Op op) {
		op = Op.MUL; 
		if (la.kind == 5) {
			Get();
		} else if (la.kind == 6) {
			Get();
			op = Op.DIV; 
		} else SynErr(31);
	}

	void Factor(out int type, out INode n) {
		int n; Obj obj; string name; INode n;
		type = undef; 
		if (la.kind == 1) {
			Ident(out name);
			obj = tab.Find(name); type = obj.type;
			if (obj.kind == var) {
			 if (obj.level == 0) gen.Emit(Op.LOADG, obj.adr);
			 else gen.Emit(Op.LOAD, obj.adr);
			} else SemErr("variable expected"); 
		} else if (la.kind == 2) {
			Get();
			n = int.Parse(t.val); gen.Emit(Op.CONST, n); type = integer; 
		} else if (la.kind == 4) {
			Get();
			Factor(out type);
			if (type != integer) {
			 SemErr("integer type expected"); type = integer;
			}
			gen.Emit(Op.NEG); 
		} else if (la.kind == 10) {
			Get();
			gen.Emit(Op.CONST, 1); type = boolean; 
		} else if (la.kind == 11) {
			Get();
			gen.Emit(Op.CONST, 0); type = boolean; 
		} else SynErr(32);
	}

	void ProcDecl() {
		string name; Obj obj; int adr; INode fun; 
		Expect(12);
		Ident(out name);
		obj = tab.NewObj(name, proc, undef); obj.adr = gen.pc;
		if (name == "Main") gen.progStart = gen.pc; 
		tab.OpenScope(); 
		Expect(13);
		Expect(14);
		Expect(15);
		gen.Emit(Op.ENTER, 0); adr = gen.pc - 1; 
		while (StartOf(1)) {
			if (la.kind == 25 || la.kind == 26) {
				VarDecl();
			} else {
				Stat();
			}
		}
		Expect(16);
		gen.Emit(Op.LEAVE); gen.Emit(Op.RET); tab.CloseScope(); 
	}

	void VarDecl() {
		string name; int type; 
		Type(out type);
		Ident(out name);
		tab.NewObj(name, var, type); 
		while (la.kind == 27) {
			Get();
			Ident(out name);
			tab.NewObj(name, var, type); 
		}
		Expect(18);
	}

	void Stat() {
		int type; string name; Obj obj;
		int adr, adr2, loopstart; 
		switch (la.kind) {
		case 1: {
			Ident(out name);
			obj = tab.Find(name);
			if (la.kind == 17) {
				Get();
				if (obj.kind != var) SemErr("cannot assign to procedure"); 
				Expr(out type);
				Expect(18);
				if (type != obj.type) SemErr("incompatible types");
				if (obj.level == 0) gen.Emit(Op.STOG, obj.adr);
				else gen.Emit(Op.STO, obj.adr); 
			} else if (la.kind == 13) {
				Get();
				Expect(14);
				Expect(18);
				if (obj.kind != proc) SemErr("object is not a procedure"); gen.Emit(Op.CALL, obj.adr); 
			} else SynErr(33);
			break;
		}
		case 19: {
			Get();
			Expect(13);
			Expr(out type);
			Expect(14);
			if (type != boolean) SemErr("boolean type expected");
			gen.Emit(Op.FJMP, 0); adr = gen.pc - 2; 
			Stat();
			if (la.kind == 20) {
				Get();
				gen.Emit(Op.JMP, 0); adr2 = gen.pc - 2; adr = adr2; 
				Stat();
			}
			
			break;
		}
		case 21: {
			Get();
			loopstart = gen.pc; 
			Expect(13);
			Expr(out type);
			Expect(14);
			if (type != boolean) SemErr("boolean type expected");
			gen.Emit(Op.FJMP, 0); adr = gen.pc - 2; 
			Stat();
			gen.Emit(Op.JMP, loopstart); 
			break;
		}
		case 22: {
			Get();
			Ident(out name);
			Expect(18);
			obj = tab.Find(name);
			if (obj.type != integer) SemErr("integer type expected");
			gen.Emit(Op.READ);
			if (obj.level == 0) gen.Emit(Op.STOG, obj.adr);
			else gen.Emit(Op.STO, obj.adr); 
			break;
		}
		case 23: {
			Get();
			Expr(out type);
			Expect(18);
			if (type != integer) SemErr("integer type expected"); gen.Emit(Op.WRITE); 
			break;
		}
		case 15: {
			Get();
			while (StartOf(1)) {
				if (StartOf(2)) {
					Stat();
				} else {
					VarDecl();
				}
			}
			Expect(16);
			break;
		}
		default: SynErr(34); break;
		}
	}

	void Term(out int type, out INode n) {
		int type1; Op op; INode n, left, right; 
		Factor(out type, out INode left);
		n = factory.Create(op.toString());  
		while (la.kind == 5 || la.kind == 6) {
			MulOp(out op);
			Factor(out type1, out INode right);
			if (type != integer || type1 != integer)  SemErr("integer type expected"); gen.Emit(op); 
		}
	}

	void DCasm() {
		string name; 
		Expect(24);
		Ident(out name);
		tab.OpenScope(); 
		Expect(15);
		while (la.kind == 12 || la.kind == 25 || la.kind == 26) {
			if (la.kind == 25 || la.kind == 26) {
				VarDecl();
			} else {
				ProcDecl();
			}
		}
		Expect(16);
		tab.CloseScope(); if (gen.progStart == -1) SemErr("main function never defined"); 
	}

	void Type(out int type) {
		type = undef; 
		if (la.kind == 25) {
			Get();
			type = integer; 
		} else if (la.kind == 26) {
			Get();
			type = boolean; 
		} else SynErr(35);
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
		{_x,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_T, _x,_x,_x,_T, _x,_T,_T,_T, _x,_T,_T,_x, _x,_x},
		{_x,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_T, _x,_x,_x,_T, _x,_T,_T,_T, _x,_x,_x,_x, _x,_x}

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
			case 3: s = "\"+\" expected"; break;
			case 4: s = "\"-\" expected"; break;
			case 5: s = "\"*\" expected"; break;
			case 6: s = "\"/\" expected"; break;
			case 7: s = "\"==\" expected"; break;
			case 8: s = "\"<\" expected"; break;
			case 9: s = "\">\" expected"; break;
			case 10: s = "\"true\" expected"; break;
			case 11: s = "\"false\" expected"; break;
			case 12: s = "\"void\" expected"; break;
			case 13: s = "\"(\" expected"; break;
			case 14: s = "\")\" expected"; break;
			case 15: s = "\"{\" expected"; break;
			case 16: s = "\"}\" expected"; break;
			case 17: s = "\"=\" expected"; break;
			case 18: s = "\";\" expected"; break;
			case 19: s = "\"if\" expected"; break;
			case 20: s = "\"else\" expected"; break;
			case 21: s = "\"while\" expected"; break;
			case 22: s = "\"read\" expected"; break;
			case 23: s = "\"write\" expected"; break;
			case 24: s = "\"program\" expected"; break;
			case 25: s = "\"int\" expected"; break;
			case 26: s = "\"bool\" expected"; break;
			case 27: s = "\",\" expected"; break;
			case 28: s = "??? expected"; break;
			case 29: s = "invalid AddOp"; break;
			case 30: s = "invalid RelOp"; break;
			case 31: s = "invalid MulOp"; break;
			case 32: s = "invalid Factor"; break;
			case 33: s = "invalid Stat"; break;
			case 34: s = "invalid Stat"; break;
			case 35: s = "invalid Type"; break;

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