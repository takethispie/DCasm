using DCasm.InstructionSet;

namespace DCasm;

public class Compiler
{
    public IList<string> Program;
    private Dictionary<string, int> functionsAdress;
    private Dictionary<int, string> lazyFunctionCall;
    private int tempAddress;

    public Compiler(bool verbose = false) {
        Program = new List<string>();
        functionsAdress = new Dictionary<string, int>();
        lazyFunctionCall = new Dictionary<int, string>();
        var adressInst = OpCodes.OpToBinary("set") + "00001" + "00000" + "{0}";
        Program.Add(adressInst);
        var inst = OpCodes.OpToBinary("jmp") + "00000" + "00001" + ConstConverter.ConstantToBinary("0");
        Program.Add(inst);


        // update init jump to resolved adress
    }

    public void Compile(List<INode> nodes) {
        var funcs = nodes.Where(node => node.GetType() == typeof(Function)).ToList();
        funcs.ForEach(func => Program = ProcessFunctions(func, Program));

        var item = Program[0];
        item = string.Format(item, ConstConverter.ConstantToBinary((Program.Count).ToString()));
        Program[0] = item;

        var insts = nodes.Except(funcs);
        foreach (var node in insts) {
            Program = Process(node, Program);  
        }
    }

    public void ImportModule(List<INode> nodes) {
        foreach(var node in nodes ) {
            Program = ProcessFunctions(node, Program);
        }
    }

    public IList<string> ProcessFunctions(INode node, IList<string> program) => node switch {
        Function f => Function(f, program),
        _ => program
    };

    public IList<string> Process(INode node, IList<string> program) => node switch
    {
        Store st => Store(st, program),
        Call call => Call(call, program),
        Return ret => Return(ret, program),
        IArithmeticNode n => arithmeticInstructionBuilder(n, program),
        ImmediateLoad iLoad => ImmediateLoad(iLoad, program),
        Load load => Load(load, program),
        Read read => Read(read, program),
        Write write => Write(write, program),
        Const constant => program,
        Function f => program,
        Register reg => program,
        Block bl => program,
        Condition cond => Condition(cond, program),
        Comparaison comp => Comparaison(comp, program),
        While w => While(w, program),
        _ => throw new Exception("Instruction type does not exists")
    };

    private IList<string> Store(Store n, IList<string> program) {
        var inst = OpCodes.OpToBinary(n.Value) + RegisterConverter.RegisterToBinary(n.BaseRegister) 
        + RegisterConverter.RegisterToBinary(n.OffsetRegister) + RegisterConverter.RegisterToBinary(n.DataValue)
        + "00000000000";
        program.Add(inst);
        return program;
    }

    private IList<string> Function(Function n, IList<string> program) {
        if(functionsAdress.ContainsKey(n.Value)) throw new Exception("Function already exists");
        functionsAdress.Add(n.Value, program.Count);

        var calls = lazyFunctionCall.Where(pair => pair.Value == n.Value).ToList();
        calls.ForEach(pair => {
            var inst = program[pair.Key];
            inst = string.Format(inst, ConstConverter.ConstantToBinary((program.Count).ToString()));
            program[pair.Key] = inst;
            lazyFunctionCall.Remove(pair.Key);
        });
        
        n.Children.ForEach(child => {
            program = Process(child, program);
        });
        return program;
    }

    private IList<string> Call (Call n, IList<string> program)
    {
        if(functionsAdress.ContainsKey(n.Value)) {
            var address  = functionsAdress[n.Value];
            var setInst = OpCodes.OpToBinary("set") + "00111" + "00000" + ConstConverter.ConstantToBinary(address.ToString());
            program.Add(setInst);
        } else {
            program.Add(OpCodes.OpToBinary("set") + "00111" + "00000" + "{0}");
            lazyFunctionCall.Add(program.Count - 1, n.Value);
        }
        var inst = OpCodes.OpToBinary("call") + "00000" + "00111" + ConstConverter.ConstantToBinary("0");
        program.Add(inst);
        return program;
    }

    private IList<string> Return (Return ret, IList<string> program) {
        var inst = OpCodes.OpToBinary("ret").PadRight(32, '0');
        program.Add(inst);
        return program;
    }

    private IList<string> Load(Load n, IList<string> program)
    {
        var inst = OpCodes.OpToBinary(n.Value) + RegisterConverter.RegisterToBinary(n.Destination) 
        + RegisterConverter.RegisterToBinary(n.BaseRegister) + RegisterConverter.RegisterToBinary(n.OffsetRegister)
        + "00000000000" ;
        program.Add(inst);
        return program;
    }

    private IList<string> arithmeticInstructionBuilder(IArithmeticNode n, IList<string> program) {
        var inst = OpCodes.OpToBinary(n.Value) + RegisterConverter.RegisterToBinary(n.Destination) 
        + RegisterConverter.RegisterToBinary(n.Left) 
        + n.Right switch {
            Const c => ConstConverter.ConstantToBinary(c.Value),
            Register r => RegisterConverter.RegisterToBinary(r) + "00000000000",
            _ => throw new ArgumentException("wrong argument type must be constant or register")
        }; 
        program.Add(inst);
        return program;
    }

    private IList<string> ImmediateLoad(ImmediateLoad n, IList<string> program)
    {
        var inst = OpCodes.OpToBinary(n.Value) + RegisterConverter.RegisterToBinary(n.Destination) + "00000" 
        + ConstConverter.ConstantToBinary(n.DataValue.Value);
        program.Add(inst);
        return program;
    }

    private IList<string> Read(Read n, IList<string> program) {
        var inst = OpCodes.OpToBinary(n.Value) + RegisterConverter.RegisterToBinary(n.Destination)
        + RegisterConverter.RegisterToBinary(n.InputSelection);
        inst = inst.PadRight(32, '0');
        program.Add(inst);
        return program;
    }

    private IList<string> Write(Write n, IList<string> program)
    {
        var inst = OpCodes.OpToBinary(n.Value) + "00000" + RegisterConverter.RegisterToBinary(n.OutputSelection)
        + RegisterConverter.RegisterToBinary(n.DataValue);
        inst = inst.PadRight(32, '0');
        program.Add(inst);
        return program;
    }

    private IList<string> Move(Move n, IList<string> program) {
        var inst = OpCodes.OpToBinary("mov") + RegisterConverter.RegisterToBinary(n.Destination)
        + RegisterConverter.RegisterToBinary(n.Source);
        inst = inst.PadRight(32, '0');
        program.Add(inst);
        return program;
    }

    private IList<string> Condition(Condition n, IList<string> program)
    {
        int startAddress = program.Count;
        int jmpOutEndIfAddress = 0;

        var comp = n.Comparaison;
        program = Process(comp, program);
        var tempOffset = program.Count - 2;
        
        
        
        //if
        var block = n.Then;
        program = Process(block, program);
        var blockOffset = program.Count - tempOffset;

        if(n.HasElseCall) {
            //add jump out of if after if execution
            var loadAddress = OpCodes.OpToBinary("set") + "00001" + "00000" + "{0}";
            program.Add(loadAddress);
            var jumpOutIfInst = OpCodes.OpToBinary("jmp") + "00000" + "00001" + ConstConverter.ConstantToBinary("0"); 
            jmpOutEndIfAddress = program.Count - 1;    
            program.Add(jumpOutIfInst);

             //patches the jump to else
            var jumpOutInst = Program[startAddress + tempOffset];
            jumpOutInst = string.Format(jumpOutInst, ConstConverter.ConstantToBinary(program.Count.ToString()));
            Program[startAddress + tempOffset] = jumpOutInst;

            var thenBlock = n.Then;
            program = Process(thenBlock, program);

            var endIfJmp = program[jmpOutEndIfAddress];
            endIfJmp = string.Format(endIfJmp, ConstConverter.ConstantToBinary(program.Count.ToString()));
            program[jmpOutEndIfAddress] = endIfJmp;
        } else {
            //patches the jump out of if address
            var jumpOutInst = Program[tempOffset - blockOffset];
            jumpOutInst = string.Format(jumpOutInst, ConstConverter.ConstantToBinary(program.Count.ToString()));
            Program[tempOffset - blockOffset] = jumpOutInst;
        }
        return program;
    }

    private IList<string> Block(Block n, IList<string> program) {

        n.Children.ForEach(child => program = Process(child, program));
        return program;
    } 

    private IList<string> While(While n, IList<string> program)
    {
        var startAddress = program.Count;

        var comp = n.Comparaison;
        program = Process(comp, program);           
        
        var block = n.Block;
        program = Block(block, program);

        var setJmpAdress = new ImmediateLoad(false, "$3", startAddress.ToString());
        program = Process(setJmpAdress, program);
        
        var loopInst = OpCodes.OpToBinary("jmp") + "00000" + "00011"
        + ConstConverter.ConstantToBinary("0");
        Program.Add(loopInst);

        //patches the jump out of while address
        var jumpOutInst = Program[tempAddress];
        jumpOutInst = string.Format(jumpOutInst, ConstConverter.ConstantToBinary(program.Count.ToString()));
        program[tempAddress] = jumpOutInst;
        return program;
    }

    private IList<string> Comparaison(Comparaison comparaison, IList<string> program)
    {
        switch (comparaison.Right) {
            case Const c:
                var reg = new ImmediateLoad(false, "$1", c.Value);
                program = Process(reg, program);
                comparaison.Right = new Register { Value = "$1"};
                break;
        }
        
        switch (comparaison.Left) {
            case Const c:
                var reg = new ImmediateLoad(false, "$2", c.Value);
                program = Process(reg, program);
                comparaison.Left = new Register { Value = "$2"};
                break;
        }
        
        var comp = OpCodes.OpToBinary("comp") 
        + "00000" 
        + RegisterConverter.RegisterToBinary(comparaison.Left) 
        + RegisterConverter.RegisterToBinary(comparaison.Right)
        + "00000000000";
        program.Add(comp);

        var setEndJumpAddressInst = OpCodes.OpToBinary("set") 
        + "01001" + "00000" + "{0}";
        tempAddress = program.Count;
        program.Add(setEndJumpAddressInst);
        
        var initInst = new ImmediateLoad(false, "$8", (program.Count + 3).ToString());
        program = Process(initInst, program);

        var op = comparaison.Value switch {
            ">" => "jgt",
            "<" => "jlt",
            "==" => "jeq",
            "<=" => "jle",
            ">=" => "jge",
            "!=" => "jne",
            _ => throw new ArgumentException("invalid comparison operator")
        };
        var conditionInst = OpCodes.OpToBinary(op) + "00000" + "01000" 
        + ConstConverter.ConstantToBinary("0");
        program.Add(conditionInst);

        var jumpOutInst = OpCodes.OpToBinary("jmp") + "00000" + "01001" 
        + ConstConverter.ConstantToBinary("0");
        program.Add(jumpOutInst);
        return program;
    }

}