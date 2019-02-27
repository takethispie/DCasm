
using System;

namespace DCasm {



public class Parser {
	public const int _EOF = 0;
	public const int _ident = 1;
	public const int _number = 2;
	public const int _registerNum = 3;
	public const int maxT = 26;

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
			switch (la.kind) {
			case 7: case 8: case 9: case 10: {
				arithm(out exp);
				break;
			}
			case 5: case 6: {
				immediateLoad(out exp);
				break;
			}
			case 11: case 12: case 13: case 14: case 15: {
				data(out exp);
				break;
			}
			case 16: {
				function(out exp);
				break;
			}
			case 18: {
				call(out exp);
				break;
			}
			case 19: {
				Condition(out exp);
				break;
			}
			}
			gen.treeRoot.Childrens.Add(exp); 
		}
		Expect(0);
	}

	void arithm(out INode exp) {
		exp = new Error();
		arithmOp(out string op);
		register(out Register dest);
		register(out Register src1);
		if (la.kind == 3) {
			register(out Register src2);
			exp = ArithmFactory.Create(op, dest, src1, src2); 
		} else if (la.kind == 2) {
			constant(out Const src2);
			exp = ArithmFactory.Create(op, dest, src1, src2); 
		} else SynErr(27);
	}

	void immediateLoad(out INode exp) {
		exp = new Error(); 
		if (la.kind == 5) {
			Get();
			exp = new ImmediateLoad(false); 
		} else if (la.kind == 6) {
			Get();
			exp = new ImmediateLoad(true); 
		} else SynErr(28);
		register(out Register dest);
		constant(out Const val);
		exp.Childrens.Add(dest); exp.Childrens.Add(val); 
	}

	void data(out INode exp) {
		exp = new Error(); 
		if (la.kind == 11) {
			Get();
			register(out Register dest);
			register(out Register source);
			exp = new Move(source, dest); 
		} else if (la.kind == 12) {
			Get();
			register(out Register OutputSelection);
			if (la.kind == 3) {
				register(out Register val);
				exp = new Write(OutputSelection, val); 
			} else if (la.kind == 2) {
				constant(out Const val);
				exp = new Write(OutputSelection, val); 
			} else SynErr(29);
		} else if (la.kind == 13) {
			Get();
			register(out Register inputSelection);
			register(out Register dest);
			exp = new Read(inputSelection, dest); 
		} else if (la.kind == 14) {
			Get();
			register(out Register dest);
			register(out Register baseReg);
			constant(out Const offset);
			exp = new Load(dest, baseReg, offset); 
		} else if (la.kind == 15) {
			Get();
			register(out Register value);
			register(out Register baseReg);
			constant(out Const offset);
			exp = new Store(baseReg, offset, value); 
		} else SynErr(30);
	}

	void function(out INode function) {
		Expect(16);
		functionName(out string name);
		INode exp = new Error(); function = new Function(name); 
		while (StartOf(2)) {
			if (StartOf(3)) {
				arithm(out exp);
			} else if (la.kind == 5 || la.kind == 6) {
				immediateLoad(out exp);
			} else {
				data(out exp);
			}
			function.Childrens.Add(exp); 
		}
		Expect(17);
		function.Value = name; gen.Functions.Add(name, function); 
	}

	void call(out INode exp) {
		Expect(18);
		functionName(out string name);
		exp = new Call(name); 
	}

	void Condition(out INode node) {
		Expect(19);
		register(out Register reg1);
		ConditionOp(out string op);
		register(out Register reg2);
		Expect(20);
		call(out INode exp);
		node = new Condition(reg1, op, reg2, exp); 
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
		} else SynErr(31);
		op = t.val; 
	}

	void functionName(out string name) {
		Expect(1);
		name = t.val; 
	}

	void ConditionOp(out string op) {
		if (la.kind == 21) {
			Get();
		} else if (la.kind == 22) {
			Get();
		} else if (la.kind == 23) {
			Get();
		} else if (la.kind == 24) {
			Get();
		} else if (la.kind == 25) {
			Get();
		} else SynErr(32);
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
		{_T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x},
		{_x,_x,_x,_x, _x,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_x,_T,_T, _x,_x,_x,_x, _x,_x,_x,_x},
		{_x,_x,_x,_x, _x,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x},
		{_x,_x,_x,_x, _x,_x,_x,_T, _T,_T,_T,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x}

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
			case 5: s = "\"li\" expected"; break;
			case 6: s = "\"lui\" expected"; break;
			case 7: s = "\"add\" expected"; break;
			case 8: s = "\"sub\" expected"; break;
			case 9: s = "\"div\" expected"; break;
			case 10: s = "\"mul\" expected"; break;
			case 11: s = "\"mov\" expected"; break;
			case 12: s = "\"out\" expected"; break;
			case 13: s = "\"in\" expected"; break;
			case 14: s = "\"lw\" expected"; break;
			case 15: s = "\"sw\" expected"; break;
			case 16: s = "\"function\" expected"; break;
			case 17: s = "\"end\" expected"; break;
			case 18: s = "\"call\" expected"; break;
			case 19: s = "\"if\" expected"; break;
			case 20: s = "\"then\" expected"; break;
			case 21: s = "\">\" expected"; break;
			case 22: s = "\"<\" expected"; break;
			case 23: s = "\"==\" expected"; break;
			case 24: s = "\">=\" expected"; break;
			case 25: s = "\"<=\" expected"; break;
			case 26: s = "??? expected"; break;
			case 27: s = "invalid arithm"; break;
			case 28: s = "invalid immediateLoad"; break;
			case 29: s = "invalid data"; break;
			case 30: s = "invalid data"; break;
			case 31: s = "invalid arithmOp"; break;
			case 32: s = "invalid ConditionOp"; break;

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