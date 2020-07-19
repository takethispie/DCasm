using System;
using System.Collections.Generic;

namespace DCasm
{
    public class Interpreter
    {
        private Dictionary<string, Function> functions { get; set; }

        public Interpreter(Dictionary<string, Function> functions, bool verbose = false) {
            this.functions = functions;
        }

        public void Compile(List<INode> nodes) {
            
            foreach (var node in nodes)
            {
                Process(node);  
            }
        }

        private void Process(INode node)
        {
            switch (node)
            {
                case Store st: 

                    break;
                    
                default:
                    break;
            }
            
        }

    }
}