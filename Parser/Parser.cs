
using System;

namespace DCasm; 

public class Parser {
	public const int _EOF = 0;
	public const int _ident = 1;
	public const int _number = 2;
	public const int _hexa = 3;
	public const int _registerNum = 4;
	public const int maxT = 35;

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
		if (la.kind == 5) {
			Get();
			gen.Type = FileTypeEnum.Program; 
		} else if (la.kind == 6) {
			Get();
			gen.Type = FileTypeEnum.Module; 
		} else SynErr(36);
		while (la.kind == 7) {
			Get();
			moduleName(out string name);
			if(gen.Type == FileTypeEnum.Program) gen.ImportModule(name); else throw new ArgumentException("you cannot import a module into another"); 
		}
		Block(out Block node);
		node.Children.ForEach(c => gen.RootNodes.Add(c)); 
		Expect(0);
	}

	void moduleName(out string name) {
		Expect(1);
		name = t.val; 
	}

	void Block(out Block node) {
		node = new Block(); 
		Expect(8);
		BlockUnit(out INode exp);
		node.Children.Add(exp); 
		while (StartOf(1)) {
			BlockUnit(out INode exp2);
			node.Children.Add(exp2); 
		}
		Expect(9);
	}

	void BlockUnit(out INode exp) {
		exp = new Error(); 
		switch (la.kind) {
		case 12: case 13: case 14: case 15: case 16: case 17: {
			arithm(out exp);
			break;
		}
		case 10: case 11: {
			immediateLoad(out exp);
			break;
		}
		case 18: case 19: case 20: case 21: case 22: {
			data(out exp);
			break;
		}
		case 23: {
			function(out exp);
			break;
		}
		case 25: {
			Call(out exp);
			break;
		}
		case 26: {
			Condition(out exp);
			break;
		}
		case 34: {
			While(out exp);
			break;
		}
		default: SynErr(37); break;
		}
	}

	void arithm(out INode exp) {
		exp = new Error();
		arithmOp(out string op);
		register(out INode dest);
		register(out INode src1);
		if (la.kind == 4) {
			register(out INode src2);
			exp = ArithmFactory.Create(op, dest, src1, src2); 
		} else if (la.kind == 2 || la.kind == 3) {
			constant(out INode src2);
			exp = ArithmFactory.Create(op, dest, src1, src2); 
		} else SynErr(38);
	}

	void immediateLoad(out INode exp) {
		exp = new Error(); 
		if (la.kind == 10) {
			Get();
			register(out INode dest);
			constant(out INode val);
			exp = new ImmediateLoad(false) { Destination = dest, DataValue = val }; 
		} else if (la.kind == 11) {
			Get();
			register(out INode dest);
			constant(out INode val);
			exp = new ImmediateLoad(true) { Destination = dest, DataValue = val }; 
		} else SynErr(39);
	}

	void data(out INode exp) {
		exp = new Error(); 
		if (la.kind == 18) {
			Get();
			register(out INode dest);
			register(out INode source);
			exp = new Move(source, dest); 
		} else if (la.kind == 19) {
			Get();
			register(out INode OutputSelection);
			if (la.kind == 4) {
				register(out INode val);
				exp = new Write(OutputSelection, val); 
			} else if (la.kind == 2 || la.kind == 3) {
				constant(out INode val);
				exp = new Write(OutputSelection, val); 
			} else SynErr(40);
		} else if (la.kind == 20) {
			Get();
			register(out INode inputSelection);
			register(out INode dest);
			exp = new Read(inputSelection, dest); 
		} else if (la.kind == 21) {
			Get();
			register(out INode dest);
			register(out INode baseReg);
			register(out INode offset);
			exp = new Load(dest, baseReg, offset); 
		} else if (la.kind == 22) {
			Get();
			register(out INode value);
			register(out INode baseReg);
			register(out INode offset);
			exp = new Store(baseReg, offset, value); 
		} else SynErr(41);
	}

	void function(out INode function) {
		Expect(23);
		functionName(out string name);
		var temp = new Function(name);  
		while (StartOf(2)) {
			if (StartOf(3)) {
				arithm(out INode exp);
				temp.Children.Add(exp); 
			} else if (la.kind == 10 || la.kind == 11) {
				immediateLoad(out INode exp);
				temp.Children.Add(exp); 
			} else if (StartOf(4)) {
				data(out INode exp);
				temp.Children.Add(exp); 
			} else if (la.kind == 25) {
				Call(out INode exp);
				temp.Children.Add(exp); 
			} else {
				Condition(out INode exp);
				temp.Children.Add(exp); 
			}
		}
		Expect(24);
		temp.Value = name; temp.Children.Add(new Return(name)); function = temp; gen.Functions.Add(name, temp); 
	}

	void Call(out INode exp) {
		Expect(25);
		functionName(out string name);
		exp = new Call(name); 
	}

	void Condition(out INode node) {
		Expect(26);
		Comparaison(out INode reg1, out string op, out INode reg2);
		Expect(27);
		Block(out Block thenblock);
		node = new Condition(new Comparaison(op, reg1, reg2), thenblock); 
		if (la.kind == 28) {
			Get();
			Block(out Block elseBlock);
			node = new Condition(new Comparaison(op, reg1, reg2), thenblock, elseBlock); 
		}
	}

	void While(out INode Node) {
		Expect(34);
		Comparaison(out INode reg1, out string op, out INode reg2);
		Block(out Block exp);
		Node = new While(exp, new Comparaison(op, reg1, reg2)); 
	}

	void register(out INode node) {
		Expect(4);
		node = new Register(); node.Value = t.val; 
	}

	void constant(out INode val) {
		val = null; 
		if (la.kind == 2) {
			Get();
			val = new Const(t.val, false); 
		} else if (la.kind == 3) {
			Get();
			val = new Const(t.val, true); 
		} else SynErr(42);
	}

	void arithmOp(out string op) {
		op = "";
		switch (la.kind) {
		case 12: {
			Get();
			break;
		}
		case 13: {
			Get();
			break;
		}
		case 14: {
			Get();
			break;
		}
		case 15: {
			Get();
			break;
		}
		case 16: {
			Get();
			break;
		}
		case 17: {
			Get();
			break;
		}
		default: SynErr(43); break;
		}
		op = t.val; 
	}

	void functionName(out string name) {
		Expect(1);
		name = t.val; 
	}

	void Comparaison(out INode reg1, out string op, out INode reg2) {
		reg1 = new Const("-1", false); reg2 = new Const("-1", false); 
		if (la.kind == 4) {
			register(out reg1);
		} else if (la.kind == 2 || la.kind == 3) {
			constant(out reg1);
		} else SynErr(44);
		ConditionOp(out op);
		if (la.kind == 4) {
			register(out reg2);
		} else if (la.kind == 2 || la.kind == 3) {
			constant(out reg2);
		} else SynErr(45);
	}

	void ConditionOp(out string op) {
		if (la.kind == 29) {
			Get();
		} else if (la.kind == 30) {
			Get();
		} else if (la.kind == 31) {
			Get();
		} else if (la.kind == 32) {
			Get();
		} else if (la.kind == 33) {
			Get();
		} else SynErr(46);
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
		{_T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x},
		{_x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _x,_T,_T,_x, _x,_x,_x,_x, _x,_x,_T,_x, _x},
		{_x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_x, _x,_T,_T,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x},
		{_x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _T,_T,_T,_T, _T,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x},
		{_x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_T,_T, _T,_T,_T,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x}

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
			case 3: s = "hexa expected"; break;
			case 4: s = "registerNum expected"; break;
			case 5: s = "\"program\" expected"; break;
			case 6: s = "\"module\" expected"; break;
			case 7: s = "\"import\" expected"; break;
			case 8: s = "\"{\" expected"; break;
			case 9: s = "\"}\" expected"; break;
			case 10: s = "\"set\" expected"; break;
			case 11: s = "\"setupper\" expected"; break;
			case 12: s = "\"add\" expected"; break;
			case 13: s = "\"sub\" expected"; break;
			case 14: s = "\"div\" expected"; break;
			case 15: s = "\"mul\" expected"; break;
			case 16: s = "\"lsh\" expected"; break;
			case 17: s = "\"rsh\" expected"; break;
			case 18: s = "\"mov\" expected"; break;
			case 19: s = "\"out\" expected"; break;
			case 20: s = "\"in\" expected"; break;
			case 21: s = "\"load\" expected"; break;
			case 22: s = "\"store\" expected"; break;
			case 23: s = "\"function\" expected"; break;
			case 24: s = "\"end\" expected"; break;
			case 25: s = "\"call\" expected"; break;
			case 26: s = "\"if\" expected"; break;
			case 27: s = "\"then\" expected"; break;
			case 28: s = "\"else\" expected"; break;
			case 29: s = "\">\" expected"; break;
			case 30: s = "\"<\" expected"; break;
			case 31: s = "\"==\" expected"; break;
			case 32: s = "\">=\" expected"; break;
			case 33: s = "\"<=\" expected"; break;
			case 34: s = "\"while\" expected"; break;
			case 35: s = "??? expected"; break;
			case 36: s = "invalid DCasm"; break;
			case 37: s = "invalid BlockUnit"; break;
			case 38: s = "invalid arithm"; break;
			case 39: s = "invalid immediateLoad"; break;
			case 40: s = "invalid data"; break;
			case 41: s = "invalid data"; break;
			case 42: s = "invalid constant"; break;
			case 43: s = "invalid arithmOp"; break;
			case 44: s = "invalid Comparaison"; break;
			case 45: s = "invalid Comparaison"; break;
			case 46: s = "invalid ConditionOp"; break;

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