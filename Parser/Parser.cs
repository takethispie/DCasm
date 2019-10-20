using System;
using System.IO;

namespace DCasm
{
    public class Parser
    {
        public const int _EOF = 0;
        public const int _ident = 1;
        public const int _number = 2;
        public const int _registerNum = 3;
        public const int maxT = 29;

        private const bool _T = true;
        private const bool _x = false;
        private const int minErrDist = 2;

        private static readonly bool[,] set =
        {
            {
                _T, _x, _x, _x, _x, _x, _x, _x, _x, _x, _x, _x, _x, _x, _x, _x, _x, _x, _x, _x, _x, _x, _x, _x, _x, _x,
                _x, _x, _x, _x, _x
            },
            {
                _x, _x, _x, _x, _x, _x, _x, _T, _T, _T, _T, _T, _T, _T, _T, _T, _T, _T, _T, _x, _T, _T, _x, _x, _x, _x,
                _x, _x, _x, _x, _x
            },
            {
                _x, _x, _x, _x, _x, _x, _x, _T, _T, _T, _T, _T, _T, _T, _T, _T, _T, _T, _x, _x, _T, _T, _x, _x, _x, _x,
                _x, _x, _x, _x, _x
            },
            {
                _x, _x, _x, _x, _x, _x, _x, _x, _x, _T, _T, _T, _T, _x, _x, _x, _x, _x, _x, _x, _x, _x, _x, _x, _x, _x,
                _x, _x, _x, _x, _x
            },
            {
                _x, _x, _x, _x, _x, _x, _x, _x, _x, _x, _x, _x, _x, _T, _T, _T, _T, _T, _x, _x, _x, _x, _x, _x, _x, _x,
                _x, _x, _x, _x, _x
            }
        };

        private int errDist = minErrDist;
        public Errors errors;

        public CodeGenerator gen;
        public Token la; // lookahead token

        public Scanner scanner;

        public Token t; // last recognized token

/*--------------------------------------------------------------------------*/


        public Parser(Scanner scanner)
        {
            this.scanner = scanner;
            errors = new Errors();
        }

        private void SynErr(int n)
        {
            if (errDist >= minErrDist) errors.SynErr(la.line, la.col, n);
            errDist = 0;
        }

        public void SemErr(string msg)
        {
            if (errDist >= minErrDist) errors.SemErr(t.line, t.col, msg);
            errDist = 0;
        }

        private void Get()
        {
            for (;;)
            {
                t = la;
                la = scanner.Scan();
                if (la.kind <= maxT)
                {
                    ++errDist;
                    break;
                }

                la = t;
            }
        }

        private void Expect(int n)
        {
            if (la.kind == n) Get();
            else SynErr(n);
        }

        private bool StartOf(int s)
        {
            return set[s, la.kind];
        }

        private void ExpectWeak(int n, int follow)
        {
            if (la.kind == n)
            {
                Get();
            }
            else
            {
                SynErr(n);
                while (!StartOf(follow)) Get();
            }
        }


        private bool WeakSeparator(int n, int syFol, int repFol)
        {
            var kind = la.kind;
            if (kind == n)
            {
                Get();
                return true;
            }

            if (StartOf(repFol))
            {
                return false;
            }

            SynErr(n);
            while (!(set[syFol, kind] || set[repFol, kind] || set[0, kind]))
            {
                Get();
                kind = la.kind;
            }

            return StartOf(syFol);
        }


        private void DCasm()
        {
            if (la.kind == 4)
            {
                Get();
                gen.Type = FileTypeEnum.Program;
            }
            else if (la.kind == 5)
            {
                Get();
                gen.Type = FileTypeEnum.Module;
            }
            else
            {
                SynErr(30);
            }

            while (la.kind == 6)
            {
                Get();
                moduleName(out var name);
                if (gen.Type == FileTypeEnum.Program) gen.ImportModule(name);
                else throw new ArgumentException("you cannot nest module import");
            }

            while (StartOf(1))
                switch (la.kind)
                {
                    case 9:
                    case 10:
                    case 11:
                    case 12:
                    {
                        arithm(out var exp);
                        gen.rootNodes.Add(exp);
                        break;
                    }
                    case 7:
                    case 8:
                    {
                        immediateLoad(out var exp);
                        gen.rootNodes.Add(exp);
                        break;
                    }
                    case 13:
                    case 14:
                    case 15:
                    case 16:
                    case 17:
                    {
                        data(out var exp);
                        gen.rootNodes.Add(exp);
                        break;
                    }
                    case 18:
                    {
                        function(out var exp);
                        gen.rootNodes.Add(exp);
                        break;
                    }
                    case 20:
                    {
                        call(out var exp);
                        gen.rootNodes.Add(exp);
                        break;
                    }
                    case 21:
                    {
                        Condition(out var exp);
                        gen.rootNodes.Add(exp);
                        break;
                    }
                }
            Expect(0);
        }

        private void moduleName(out string name)
        {
            Expect(1);
            name = t.val;
        }

        private void arithm(out INode exp)
        {
            exp = new Error();
            arithmOp(out var op);
            register(out var dest);
            register(out var src1);
            if (la.kind == 3)
            {
                register(out var src2);
                exp = ArithmFactory.Create(op, dest, src1, src2);
            }
            else if (la.kind == 2)
            {
                constant(out var src2);
                exp = ArithmFactory.Create(op, dest, src1, src2);
            }
            else
            {
                SynErr(31);
            }
        }

        private void immediateLoad(out INode exp)
        {
            exp = new Error();
            if (la.kind == 7)
            {
                Get();
                exp = new ImmediateLoad(false);
            }
            else if (la.kind == 8)
            {
                Get();
                exp = new ImmediateLoad(true);
            }
            else
            {
                SynErr(32);
            }

            register(out var dest);
            constant(out var val);
            exp.Childrens.Add(dest);
            exp.Childrens.Add(val);
        }

        private void data(out INode exp)
        {
            exp = new Error();
            if (la.kind == 13)
            {
                Get();
                register(out var dest);
                register(out var source);
                exp = new Move(source, dest);
            }
            else if (la.kind == 14)
            {
                Get();
                register(out var OutputSelection);
                if (la.kind == 3)
                {
                    register(out var val);
                    exp = new Write(OutputSelection, val);
                }
                else if (la.kind == 2)
                {
                    constant(out var val);
                    exp = new Write(OutputSelection, val);
                }
                else
                {
                    SynErr(33);
                }
            }
            else if (la.kind == 15)
            {
                Get();
                register(out var inputSelection);
                register(out var dest);
                exp = new Read(inputSelection, dest);
            }
            else if (la.kind == 16)
            {
                Get();
                register(out var dest);
                register(out var baseReg);
                constant(out var offset);
                exp = new Load(dest, baseReg, offset);
            }
            else if (la.kind == 17)
            {
                Get();
                register(out var value);
                register(out var baseReg);
                constant(out var offset);
                exp = new Store(baseReg, offset, value);
            }
            else
            {
                SynErr(34);
            }
        }

        private void function(out Function function)
        {
            Expect(18);
            functionName(out var name);
            function = new Function(name);
            while (StartOf(2))
                if (StartOf(3))
                {
                    arithm(out var exp);
                    function.Childrens.Add(exp);
                }
                else if (la.kind == 7 || la.kind == 8)
                {
                    immediateLoad(out var exp);
                    function.Childrens.Add(exp);
                }
                else if (StartOf(4))
                {
                    data(out var exp);
                    function.Childrens.Add(exp);
                }
                else if (la.kind == 20)
                {
                    call(out var exp);
                    function.Childrens.Add(exp);
                }
                else
                {
                    Condition(out var exp);
                    function.Childrens.Add(exp);
                }

            Expect(19);
            function.Value = name;
            gen.Functions.Add(name, function);
        }

        private void call(out Call exp)
        {
            Expect(20);
            functionName(out var name);
            exp = new Call(name);
        }

        private void Condition(out Condition node)
        {
            Expect(21);
            register(out var reg1);
            ConditionOp(out var op);
            register(out var reg2);
            Expect(22);
            call(out var thenCall);
            node = new Condition(reg1, op, reg2, thenCall);
            if (la.kind == 23)
            {
                Get();
                call(out var elseCall);
                node = new Condition(reg1, op, reg2, thenCall, elseCall);
            }
        }

        private void register(out Register node)
        {
            Expect(3);
            node = new Register();
            node.Value = t.val;
        }

        private void constant(out Const val)
        {
            Expect(2);
            val = new Const(t.val);
        }

        private void arithmOp(out string op)
        {
            op = "";
            if (la.kind == 9)
                Get();
            else if (la.kind == 10)
                Get();
            else if (la.kind == 11)
                Get();
            else if (la.kind == 12)
                Get();
            else SynErr(35);
            op = t.val;
        }

        private void functionName(out string name)
        {
            Expect(1);
            name = t.val;
        }

        private void ConditionOp(out string op)
        {
            if (la.kind == 24)
                Get();
            else if (la.kind == 25)
                Get();
            else if (la.kind == 26)
                Get();
            else if (la.kind == 27)
                Get();
            else if (la.kind == 28)
                Get();
            else SynErr(36);
            op = t.val;
        }


        public void Parse()
        {
            la = new Token();
            la.val = "";
            Get();
            DCasm();
            Expect(0);
        }
    } // end Parser


    public class Errors
    {
        public int count; // number of errors detected
        public string errMsgFormat = "-- line {0} col {1}: {2}"; // 0=line, 1=column, 2=text
        public TextWriter errorStream = Console.Out; // error messages go to this stream

        public virtual void SynErr(int line, int col, int n)
        {
            string s;
            switch (n)
            {
                case 0:
                    s = "EOF expected";
                    break;
                case 1:
                    s = "ident expected";
                    break;
                case 2:
                    s = "number expected";
                    break;
                case 3:
                    s = "registerNum expected";
                    break;
                case 4:
                    s = "\"program\" expected";
                    break;
                case 5:
                    s = "\"module\" expected";
                    break;
                case 6:
                    s = "\"import\" expected";
                    break;
                case 7:
                    s = "\"li\" expected";
                    break;
                case 8:
                    s = "\"lui\" expected";
                    break;
                case 9:
                    s = "\"add\" expected";
                    break;
                case 10:
                    s = "\"sub\" expected";
                    break;
                case 11:
                    s = "\"div\" expected";
                    break;
                case 12:
                    s = "\"mul\" expected";
                    break;
                case 13:
                    s = "\"mov\" expected";
                    break;
                case 14:
                    s = "\"out\" expected";
                    break;
                case 15:
                    s = "\"in\" expected";
                    break;
                case 16:
                    s = "\"lw\" expected";
                    break;
                case 17:
                    s = "\"sw\" expected";
                    break;
                case 18:
                    s = "\"function\" expected";
                    break;
                case 19:
                    s = "\"end\" expected";
                    break;
                case 20:
                    s = "\"call\" expected";
                    break;
                case 21:
                    s = "\"if\" expected";
                    break;
                case 22:
                    s = "\"then\" expected";
                    break;
                case 23:
                    s = "\"else\" expected";
                    break;
                case 24:
                    s = "\">\" expected";
                    break;
                case 25:
                    s = "\"<\" expected";
                    break;
                case 26:
                    s = "\"==\" expected";
                    break;
                case 27:
                    s = "\">=\" expected";
                    break;
                case 28:
                    s = "\"<=\" expected";
                    break;
                case 29:
                    s = "??? expected";
                    break;
                case 30:
                    s = "invalid DCasm";
                    break;
                case 31:
                    s = "invalid arithm";
                    break;
                case 32:
                    s = "invalid immediateLoad";
                    break;
                case 33:
                    s = "invalid data";
                    break;
                case 34:
                    s = "invalid data";
                    break;
                case 35:
                    s = "invalid arithmOp";
                    break;
                case 36:
                    s = "invalid ConditionOp";
                    break;

                default:
                    s = "error " + n;
                    break;
            }

            errorStream.WriteLine(errMsgFormat, line, col, s);
            count++;
        }

        public virtual void SemErr(int line, int col, string s)
        {
            errorStream.WriteLine(errMsgFormat, line, col, s);
            count++;
        }

        public virtual void SemErr(string s)
        {
            errorStream.WriteLine(s);
            count++;
        }

        public virtual void Warning(int line, int col, string s)
        {
            errorStream.WriteLine(errMsgFormat, line, col, s);
        }

        public virtual void Warning(string s)
        {
            errorStream.WriteLine(s);
        }
    } // Errors


    public class FatalError : Exception
    {
        public FatalError(string m) : base(m)
        {
        }
    }
}