
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
	public const int maxT = 31;

	const bool _T = true;
	const bool _x = false;
	const int minErrDist = 2;
	
	public Scanner scanner;
	public Errors  errors;

	public Token t;    // last recognized token
	public Token la;   // lookahead token
	int errDist = minErrDist;

public Generator gen;
    public Block currentBlock;

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
		while (la.kind == 7) {
			block();
		}
		Expect(0);
		Block.preGenerate();
	}

	void block() {
		Expect(7);
		currentBlock = new Block();
		if (StartOf(1)) {
			core();
			while (StartOf(1)) {
				core();
			}
		} else if (la.kind == 30) {
			softcfg();
		} else SynErr(32);
		Expect(8);
		if (la.kind == 9) {
			Get();
			Expect(3);
			Console.WriteLine(t.val); currentBlock.name = t.val;
		}
		currentBlock.addBlock();
	}

	void core() {
		Instruction inst = new Instruction("");
		switch (la.kind) {
		case 10: {
			Get();
			inst = new Instruction("mov");
			Expect(2);
			inst.addArg(t.kind,t.val); 
			Expect(11);
			if (la.kind == 1) {
				Get();
			} else if (la.kind == 4) {
				Get();
			} else SynErr(33);
			inst.addArg(t.kind,t.val); inst.create(); 
			break;
		}
		case 12: case 13: case 14: case 15: case 16: {
			if (la.kind == 12) {
				Get();
			} else if (la.kind == 13) {
				Get();
			} else if (la.kind == 14) {
				Get();
			} else if (la.kind == 15) {
				Get();
			} else {
				Get();
			}
			inst = new Instruction(t.val);
			Expect(2);
			inst.addArg(t.kind,t.val); 
			Expect(11);
			Expect(2);
			inst.addArg(t.kind,t.val); 
			Expect(11);
			Expect(2);
			inst.addArg(t.kind,t.val);
			break;
		}
		case 17: case 18: case 19: case 20: case 21: {
			if (la.kind == 17) {
				Get();
			} else if (la.kind == 18) {
				Get();
			} else if (la.kind == 19) {
				Get();
			} else if (la.kind == 20) {
				Get();
			} else {
				Get();
			}
			inst = new Instruction(t.val); 
			Expect(2);
			inst.addArg(t.kind,t.val); 
			Expect(11);
			Expect(2);
			inst.addArg(t.kind,t.val);
			break;
		}
		case 22: case 23: case 24: case 25: case 26: {
			if (la.kind == 22) {
				Get();
			} else if (la.kind == 23) {
				Get();
			} else if (la.kind == 24) {
				Get();
			} else if (la.kind == 25) {
				Get();
			} else {
				Get();
			}
			inst = new Instruction(t.val);
			Expect(2);
			inst.addArg(t.kind,t.val);
			break;
		}
		case 27: {
			Get();
			Expect(3);
			inst = new Instruction("mov"); inst.addArg(t.kind,t.val); inst.create(); currentBlock.addInstruction(inst);
			inst = new Instruction("call"); inst.addArg(t.kind,t.val); 
			break;
		}
		case 28: {
			Get();
			inst = new Instruction(t.val);
			break;
		}
		case 29: {
			Get();
			Expect(3);
			inst = new Instruction("label"); inst.addArg(t.kind,t.val);
			break;
		}
		default: SynErr(34); break;
		}
		inst.create(); currentBlock.addInstruction(inst);
	}

	void softcfg() {
		Expect(30);
		Expect(7);
		Console.WriteLine("softcore config");
		Expect(8);
	}



	public void Parse() {
		la = new Token();
		la.val = "";		
		Get();
		DCasm();
		Expect(0);

	}
	
	static readonly bool[,] set = {
		{_T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x},
		{_x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_T,_x, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_x,_x, _x}

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
			case 7: s = "\"{\" expected"; break;
			case 8: s = "\"}\" expected"; break;
			case 9: s = "\"->\" expected"; break;
			case 10: s = "\"mov\" expected"; break;
			case 11: s = "\",\" expected"; break;
			case 12: s = "\"add\" expected"; break;
			case 13: s = "\"sub\" expected"; break;
			case 14: s = "\"mul\" expected"; break;
			case 15: s = "\"div\" expected"; break;
			case 16: s = "\"mup\" expected"; break;
			case 17: s = "\"comp\" expected"; break;
			case 18: s = "\"gpo\" expected"; break;
			case 19: s = "\"gpi\" expected"; break;
			case 20: s = "\"lw\" expected"; break;
			case 21: s = "\"sw\" expected"; break;
			case 22: s = "\"jmp\" expected"; break;
			case 23: s = "\"bra\" expected"; break;
			case 24: s = "\"jgt\" expected"; break;
			case 25: s = "\"jeq\" expected"; break;
			case 26: s = "\"jlt\" expected"; break;
			case 27: s = "\"call\" expected"; break;
			case 28: s = "\"return\" expected"; break;
			case 29: s = "\"lbl\" expected"; break;
			case 30: s = "\"soft\" expected"; break;
			case 31: s = "??? expected"; break;
			case 32: s = "invalid block"; break;
			case 33: s = "invalid core"; break;
			case 34: s = "invalid core"; break;

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