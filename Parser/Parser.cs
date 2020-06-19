
using System;

namespace DCasm {



public class Parser {
	public const int _EOF = 0;
	public const int _ident = 1;
	public const int _number = 2;
	public const int _registerNum = 3;
	public const int maxT = 34;

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
		} else SynErr(35);
		while (la.kind == 6) {
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
		case 11: case 12: case 13: case 14: case 15: case 16: {
			arithm(out exp);
			break;
		}
		case 9: case 10: {
			immediateLoad(out exp);
			break;
		}
		case 17: case 18: case 19: case 20: case 21: {
			data(out exp);
			break;
		}
		case 22: {
			function(out exp);
			break;
		}
		case 24: {
			Call(out exp);
			break;
		}
		case 25: {
			Condition(out exp);
			break;
		}
		case 33: {
			While(out exp);
			break;
		}
		default: SynErr(36); break;
		}
	}

	void arithm(out INode exp) {
		exp = new Error();
		arithmOp(out string op);
		register(out INode dest);
		register(out INode src1);
		if (la.kind == 3) {
			register(out INode src2);
			exp = ArithmFactory.Create(op, dest, src1, src2); 
		} else if (la.kind == 2) {
			constant(out INode src2);
			exp = ArithmFactory.Create(op, dest, src1, src2); 
		} else SynErr(37);
	}

	void immediateLoad(out INode exp) {
		exp = new Error(); 
		if (la.kind == 9) {
			Get();
			register(out INode dest);
			constant(out INode val);
			exp = new ImmediateLoad(false) { Destination = dest, DataValue = val }; 
		} else if (la.kind == 10) {
			Get();
			register(out INode dest);
			constant(out INode val);
			exp = new ImmediateLoad(true) { Destination = dest, DataValue = val }; 
		} else SynErr(38);
	}

	void data(out INode exp) {
		exp = new Error(); 
		if (la.kind == 17) {
			Get();
			register(out INode dest);
			register(out INode source);
			exp = new Move(source, dest); 
		} else if (la.kind == 18) {
			Get();
			register(out INode OutputSelection);
			if (la.kind == 3) {
				register(out INode val);
				exp = new Write(OutputSelection, val); 
			} else if (la.kind == 2) {
				constant(out INode val);
				exp = new Write(OutputSelection, val); 
			} else SynErr(39);
		} else if (la.kind == 19) {
			Get();
			register(out INode inputSelection);
			register(out INode dest);
			exp = new Read(inputSelection, dest); 
		} else if (la.kind == 20) {
			Get();
			register(out INode dest);
			register(out INode baseReg);
			register(out INode offset);
			exp = new Load(dest, baseReg, offset); 
		} else if (la.kind == 21) {
			Get();
			register(out INode value);
			register(out INode baseReg);
			register(out INode offset);
			exp = new Store(baseReg, offset, value); 
		} else SynErr(40);
	}

	void function(out INode function) {
		Expect(22);
		functionName(out string name);
		var temp = new Function(name);  
		while (StartOf(2)) {
			if (StartOf(3)) {
				arithm(out INode exp);
				temp.Children.Add(exp); 
			} else if (la.kind == 9 || la.kind == 10) {
				immediateLoad(out INode exp);
				temp.Children.Add(exp); 
			} else if (StartOf(4)) {
				data(out INode exp);
				temp.Children.Add(exp); 
			} else if (la.kind == 24) {
				Call(out INode exp);
				temp.Children.Add(exp); 
			} else {
				Condition(out INode exp);
				temp.Children.Add(exp); 
			}
		}
		Expect(23);
		temp.Value = name; temp.Children.Add(new Return(name)); function = temp; gen.Functions.Add(name, temp); 
	}

	void Call(out INode exp) {
		Expect(24);
		functionName(out string name);
		exp = new Call(name); 
	}

	void Condition(out INode node) {
		Expect(25);
		Comparaison(out INode reg1, out string op, out INode reg2);
		Expect(26);
		Block(out Block thenblock);
		node = new Condition(new Comparaison(op, reg1, reg2), thenblock); 
		if (la.kind == 27) {
			Get();
			Block(out Block elseBlock);
			node = new Condition(new Comparaison(op, reg1, reg2), thenblock, elseBlock); 
		}
	}

	void While(out INode Node) {
		Expect(33);
		Comparaison(out INode reg1, out string op, out INode reg2);
		Block(out Block exp);
		Node = new While(exp, new Comparaison(op, reg1, reg2)); 
	}

	void register(out INode node) {
		Expect(3);
		node = new Register(); node.Value = t.val; 
	}

	void constant(out INode val) {
		val = null; 
		Expect(2);
		val = new Const(t.val); 
	}

	void arithmOp(out string op) {
		op = "";
		switch (la.kind) {
		case 11: {
			Get();
			break;
		}
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
		default: SynErr(41); break;
		}
		op = t.val; 
	}

	void functionName(out string name) {
		Expect(1);
		name = t.val; 
	}

	void Comparaison(out INode reg1, out string op, out INode reg2) {
		reg1 = new Const("-1"); reg2 = new Const("-1"); 
		if (la.kind == 3) {
			register(out reg1);
		} else if (la.kind == 2) {
			constant(out reg1);
		} else SynErr(42);
		ConditionOp(out op);
		if (la.kind == 3) {
			register(out reg2);
		} else if (la.kind == 2) {
			constant(out reg2);
		} else SynErr(43);
	}

	void ConditionOp(out string op) {
		if (la.kind == 28) {
			Get();
		} else if (la.kind == 29) {
			Get();
		} else if (la.kind == 30) {
			Get();
		} else if (la.kind == 31) {
			Get();
		} else if (la.kind == 32) {
			Get();
		} else SynErr(44);
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
		{_T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x},
		{_x,_x,_x,_x, _x,_x,_x,_x, _x,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_x, _T,_T,_x,_x, _x,_x,_x,_x, _x,_T,_x,_x},
		{_x,_x,_x,_x, _x,_x,_x,_x, _x,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_x,_x, _T,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x},
		{_x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_T, _T,_T,_T,_T, _T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x},
		{_x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_T,_T,_T, _T,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x}

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
			case 9: s = "\"set\" expected"; break;
			case 10: s = "\"setupper\" expected"; break;
			case 11: s = "\"add\" expected"; break;
			case 12: s = "\"sub\" expected"; break;
			case 13: s = "\"div\" expected"; break;
			case 14: s = "\"mul\" expected"; break;
			case 15: s = "\"lsh\" expected"; break;
			case 16: s = "\"rsh\" expected"; break;
			case 17: s = "\"mov\" expected"; break;
			case 18: s = "\"out\" expected"; break;
			case 19: s = "\"in\" expected"; break;
			case 20: s = "\"load\" expected"; break;
			case 21: s = "\"store\" expected"; break;
			case 22: s = "\"function\" expected"; break;
			case 23: s = "\"end\" expected"; break;
			case 24: s = "\"call\" expected"; break;
			case 25: s = "\"if\" expected"; break;
			case 26: s = "\"then\" expected"; break;
			case 27: s = "\"else\" expected"; break;
			case 28: s = "\">\" expected"; break;
			case 29: s = "\"<\" expected"; break;
			case 30: s = "\"==\" expected"; break;
			case 31: s = "\">=\" expected"; break;
			case 32: s = "\"<=\" expected"; break;
			case 33: s = "\"while\" expected"; break;
			case 34: s = "??? expected"; break;
			case 35: s = "invalid DCasm"; break;
			case 36: s = "invalid BlockUnit"; break;
			case 37: s = "invalid arithm"; break;
			case 38: s = "invalid immediateLoad"; break;
			case 39: s = "invalid data"; break;
			case 40: s = "invalid data"; break;
			case 41: s = "invalid arithmOp"; break;
			case 42: s = "invalid Comparaison"; break;
			case 43: s = "invalid Comparaison"; break;
			case 44: s = "invalid ConditionOp"; break;

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