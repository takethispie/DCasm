using System;
using System.Collections.Generic;
using System.IO;
using DCasm.Visitors;

namespace DCasm;

public class CodeGenerator
{
    private readonly Parser parser;
    public Dictionary<string, Function> Functions;
    public List<INode> RootNodes;
    private readonly Scanner scanner;
    private Compiler compiler;


    public CodeGenerator(string fileName)
    {
        scanner = new Scanner(fileName);
        parser = new Parser(scanner) {gen = this};
        RootNodes = new List<INode>();
        Functions = new Dictionary<string, Function>();
        compiler = new Compiler(false);
    }

    public CodeGenerator(Stream stream)
    {
        scanner = new Scanner(stream);
        parser = new Parser(scanner) {gen = this};
        RootNodes = new List<INode>();
        Functions = new Dictionary<string, Function>();
        compiler = new Compiler(false);
    }

    public FileTypeEnum Type { get; set; }
    public int ErrorCount => parser.errors.count;

    public IEnumerable<string> Compile() {
        compiler.Compile(RootNodes);
        var hexProgram = new List<string>();
        foreach (var binary in compiler.Program) {
            var hex = Utils.BinaryToHex(binary);
            if(true) Console.WriteLine(hex);
            hexProgram.Add(hex);
        }

        return hexProgram;
    }

    public void Interpret() {
    }


    public void Parse() => parser.Parse();

    public void ImportModule(string name)
    {
        if (!File.Exists(name)) throw new FileNotFoundException(name + " Not found");
        var generator = new CodeGenerator(name);
        generator.Parse();
        compiler.ImportModule(generator.RootNodes);
        Console.WriteLine("imported " + name);
    }
}