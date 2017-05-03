
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
	public const int maxT = 20;

	const bool _T = true;
	const bool _x = false;
	const int minErrDist = 2;
	
	public Scanner scanner;
	public Errors  errors;

	public Token t;    // last recognized token
	public Token la;   // lookahead token
	int errDist = minErrDist;

const int undef = 0, ident = 1, number = 2, hex = 3;

    public CodeGenerator gen;
    public Block currentBlock;
    public int adress = 0;
    public I_ISA CurrentISA;
    public CommandBuilder CBuild = new CommandBuilder();

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
		block();
		while (la.kind == 3) {
			block();
		}
		Expect(0);
	}

	void block() {
		Expect(3);
		currentBlock = new Block(t.val); 
		Expect(7);
		Expect(8);
		while (StartOf(1)) {
			core();
		}
		Expect(9);
		Block.Program.Add(currentBlock); 
	}

	void core() {
		string value = "null"; int type = -1; 
		switch (la.kind) {
		case 3: {
			arithm();
			break;
		}
		case 19: {
			varDecl();
			break;
		}
		case 10: {
			Get();
			Expect(3);
			break;
		}
		case 11: {
			Get();
			break;
		}
		case 12: {
			Get();
			expr(out type, out value);
			break;
		}
		case 13: {
			Get();
			Expect(3);
			break;
		}
		default: SynErr(21); break;
		}
	}

	void arithm() {
		string value = "null"; string value2 = ""; int type = -1; 
		Expect(3);
		if(!currentBlock.LocalSymbols.SymbolExist(t.val)) { SemErr("the variable name does not exists !"); } 
		Expect(14);
		expr(out type, out value);
		if (StartOf(2)) {
			if (la.kind == 15) {
				Get();
			} else if (la.kind == 16) {
				Get();
			} else if (la.kind == 17) {
				Get();
			} else {
				Get();
			}
			expr(out type, out value2);
		}
		Expect(6);
	}

	void varDecl() {
		Expect(19);
		Expect(3);
		currentBlock.LocalSymbols.Add(t.val,SymbolType.Var); /*use factory to construct instruction builder*/ 
		Expect(6);
	}

	void expr(out int type, out string value) {
		type = undef; value = "";
		if (la.kind == 3) {
			Get();
			type = ident; value = t.val; 
		} else if (la.kind == 4) {
			Get();
			type = number; value = t.val; 
		} else if (la.kind == 1) {
			Get();
			type = hex; value = t.val; 
		} else SynErr(22);
	}



	public void Parse() {
		la = new Token();
		la.val = "";		
		Get();
		DCasm();
		Expect(0);

	}
	
	static readonly bool[,] set = {
		{_T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x},
		{_x,_x,_x,_T, _x,_x,_x,_x, _x,_x,_T,_T, _T,_T,_x,_x, _x,_x,_x,_T, _x,_x},
		{_x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_T, _T,_T,_T,_x, _x,_x}

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
			case 7: s = "\"->\" expected"; break;
			case 8: s = "\"{\" expected"; break;
			case 9: s = "\"}\" expected"; break;
			case 10: s = "\"call\" expected"; break;
			case 11: s = "\"return\" expected"; break;
			case 12: s = "\"out\" expected"; break;
			case 13: s = "\"in\" expected"; break;
			case 14: s = "\"=\" expected"; break;
			case 15: s = "\"+\" expected"; break;
			case 16: s = "\"-\" expected"; break;
			case 17: s = "\"*\" expected"; break;
			case 18: s = "\"/\" expected"; break;
			case 19: s = "\"var\" expected"; break;
			case 20: s = "??? expected"; break;
			case 21: s = "invalid core"; break;
			case 22: s = "invalid expr"; break;

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