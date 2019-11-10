
using System;

namespace DCasm {



public class Parser {
	public const int _EOF = 0;
	public const int _ident = 1;
	public const int _number = 2;
	public const int _registerNum = 3;
	public const int maxT = 32;

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
		if (la.kind == 4) {
			Get();
			gen.Type = FileTypeEnum.Program; 
		} else if (la.kind == 5) {
			Get();
			gen.Type = FileTypeEnum.Module; 
		} else SynErr(33);
		while (la.kind == 6) {
			Get();
			moduleName(out string name);
			if(gen.Type == FileTypeEnum.Program) gen.ImportModule(name); else throw new ArgumentException("you cannot import a module into another"); 
		}
		block(out Block node);
		node.Children.ForEach(c => gen.RootNodes.Add(c)); 
		Expect(0);
	}

	void moduleName(out string name) {
		Expect(1);
		name = t.val; 
	}

	void block(out Block node) {
		node = new Block(); 
		Expect(7);
		BlockUnit(out INode exp);
		node.Children.Add(exp); 
		while (StartOf(1)) {
			BlockUnit(out INode exp2);
			node.Children.Add(exp2); 
		}
		Expect(8);
	}

	void BlockUnit(out INode exp) {
		exp = new Error(); 
		switch (la.kind) {
		case 11: case 12: case 13: case 14: {
			arithm(out exp);
			break;
		}
		case 9: case 10: {
			immediateLoad(out exp);
			break;
		}
		case 15: case 16: case 17: case 18: case 19: {
			data(out exp);
			break;
		}
		case 20: {
			function(out exp);
			break;
		}
		case 22: {
			Call(out exp);
			break;
		}
		case 23: {
			Condition(out exp);
			break;
		}
		case 31: {
			While(out exp);
			break;
		}
		default: SynErr(34); break;
		}
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
		} else SynErr(35);
	}

	void immediateLoad(out INode exp) {
		exp = new Error(); 
		if (la.kind == 9) {
			Get();
			exp = new ImmediateLoad(false); 
		} else if (la.kind == 10) {
			Get();
			exp = new ImmediateLoad(true); 
		} else SynErr(36);
		register(out Register dest);
		constant(out Const val);
		exp.Children.Add(dest); exp.Children.Add(val); 
	}

	void data(out INode exp) {
		exp = new Error(); 
		if (la.kind == 15) {
			Get();
			register(out Register dest);
			register(out Register source);
			exp = new Move(source, dest); 
		} else if (la.kind == 16) {
			Get();
			register(out Register OutputSelection);
			if (la.kind == 3) {
				register(out Register val);
				exp = new Write(OutputSelection, val); 
			} else if (la.kind == 2) {
				constant(out Const val);
				exp = new Write(OutputSelection, val); 
			} else SynErr(37);
		} else if (la.kind == 17) {
			Get();
			register(out Register inputSelection);
			register(out Register dest);
			exp = new Read(inputSelection, dest); 
		} else if (la.kind == 18) {
			Get();
			register(out Register dest);
			register(out Register baseReg);
			constant(out Const offset);
			exp = new Load(dest, baseReg, offset); 
		} else if (la.kind == 19) {
			Get();
			register(out Register value);
			register(out Register baseReg);
			constant(out Const offset);
			exp = new Store(baseReg, offset, value); 
		} else SynErr(38);
	}

	void function(out INode function) {
		Expect(20);
		functionName(out string name);
		function = new Function(name); 
		while (StartOf(2)) {
			if (StartOf(3)) {
				arithm(out INode exp);
				function.Children.Add(exp); 
			} else if (la.kind == 9 || la.kind == 10) {
				immediateLoad(out INode exp);
				function.Children.Add(exp); 
			} else if (StartOf(4)) {
				data(out INode exp);
				function.Children.Add(exp); 
			} else if (la.kind == 22) {
				Call(out INode exp);
				function.Children.Add(exp); 
			} else {
				Condition(out INode exp);
				function.Children.Add(exp); 
			}
		}
		Expect(21);
		function.Value = name; gen.Functions.Add(name, function); 
	}

	void Call(out INode exp) {
		Expect(22);
		functionName(out string name);
		exp = new Call(name); 
	}

	void Condition(out INode node) {
		Expect(23);
		Comparaison(out Register reg1, out string op, out Register reg2);
		Expect(24);
		block(out Block thenblock);
		node = new Condition(new Comparaison(op, reg1, reg2), thenblock); 
		if (la.kind == 25) {
			Get();
			block(out Block elseBlock);
			node = new Condition(new Comparaison(op, reg1, reg2), thenblock, elseBlock); 
		}
	}

	void While(out INode Node) {
		Expect(31);
		Comparaison(out Register reg1, out string op, out Register reg2);
		Expect(7);
		Call(out INode exp);
		Expect(8);
		Node = new While(exp, new Comparaison(op, reg1, reg2)); 
	}

	void register(out Register node) {
		Expect(3);
		node = new Register(); node.Value = t.val; 
	}

	void constant(out Const val) {
		val = null; 
		Expect(2);
		val = new Const(t.val); 
	}

	void arithmOp(out string op) {
		op = "";
		if (la.kind == 11) {
			Get();
		} else if (la.kind == 12) {
			Get();
		} else if (la.kind == 13) {
			Get();
		} else if (la.kind == 14) {
			Get();
		} else SynErr(39);
		op = t.val; 
	}

	void functionName(out string name) {
		Expect(1);
		name = t.val; 
	}

	void Comparaison(out Register reg1, out string op, out Register reg2) {
		register(out reg1);
		ConditionOp(out op);
		register(out reg2);
	}

	void ConditionOp(out string op) {
		if (la.kind == 26) {
			Get();
		} else if (la.kind == 27) {
			Get();
		} else if (la.kind == 28) {
			Get();
		} else if (la.kind == 29) {
			Get();
		} else if (la.kind == 30) {
			Get();
		} else SynErr(40);
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
		{_T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x},
		{_x,_x,_x,_x, _x,_x,_x,_x, _x,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_x,_T,_T, _x,_x,_x,_x, _x,_x,_x,_T, _x,_x},
		{_x,_x,_x,_x, _x,_x,_x,_x, _x,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _x,_x,_T,_T, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x},
		{_x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_T, _T,_T,_T,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x},
		{_x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_T, _T,_T,_T,_T, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x}

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
			case 5: s = "\"module\" expected"; break;
			case 6: s = "\"import\" expected"; break;
			case 7: s = "\"{\" expected"; break;
			case 8: s = "\"}\" expected"; break;
			case 9: s = "\"li\" expected"; break;
			case 10: s = "\"lui\" expected"; break;
			case 11: s = "\"add\" expected"; break;
			case 12: s = "\"sub\" expected"; break;
			case 13: s = "\"div\" expected"; break;
			case 14: s = "\"mul\" expected"; break;
			case 15: s = "\"mov\" expected"; break;
			case 16: s = "\"out\" expected"; break;
			case 17: s = "\"in\" expected"; break;
			case 18: s = "\"lw\" expected"; break;
			case 19: s = "\"sw\" expected"; break;
			case 20: s = "\"function\" expected"; break;
			case 21: s = "\"end\" expected"; break;
			case 22: s = "\"call\" expected"; break;
			case 23: s = "\"if\" expected"; break;
			case 24: s = "\"then\" expected"; break;
			case 25: s = "\"else\" expected"; break;
			case 26: s = "\">\" expected"; break;
			case 27: s = "\"<\" expected"; break;
			case 28: s = "\"==\" expected"; break;
			case 29: s = "\">=\" expected"; break;
			case 30: s = "\"<=\" expected"; break;
			case 31: s = "\"while\" expected"; break;
			case 32: s = "??? expected"; break;
			case 33: s = "invalid DCasm"; break;
			case 34: s = "invalid BlockUnit"; break;
			case 35: s = "invalid arithm"; break;
			case 36: s = "invalid immediateLoad"; break;
			case 37: s = "invalid data"; break;
			case 38: s = "invalid data"; break;
			case 39: s = "invalid arithmOp"; break;
			case 40: s = "invalid ConditionOp"; break;

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