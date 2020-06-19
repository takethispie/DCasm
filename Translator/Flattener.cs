using System.Collections.Generic;
using DCasm.Visitors;

namespace DCasm.Translator {

    public class Flattener {

        public List<INode> FlatProgram;


        public Flattener(Dictionary<string, Function> functions) {
            FlatProgram = new List<INode>();
            foreach (var (key, value) in functions) {
            }
        }
    }

}