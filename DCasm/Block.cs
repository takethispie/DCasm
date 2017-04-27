using System;
using System.Collections.Generic;

namespace DCasm
{
    public class Block
    {
        static int totalSize = 0;
        static public List<Block> blocks = new List<Block>();
        static bool mainSet = false;
        static Dictionary<string,int> blockTable = new Dictionary<string, int>();
        static int GlobalSize = 0;

        public delegate void ResolutionEvent(string name, int adress);
        public event ResolutionEvent onBlockRes;

        public List<string> blockQueue;


        /// <summary>
        /// all the instructions in the block.
        /// </summary>
        public List<Instruction> content;

        /// <summary>
        /// number of instruction in the block.
        /// </summary>
        public int size;

        /// <summary>
        /// block name, can be empty.
        /// </summary>
        public string name;

        /// <summary>
        /// The block start adress.
        /// </summary>
        public int startAdress;

        /// <summary>
        /// true if the block is the entry point of the program
        /// </summary>
        bool isMain;

        public static void Init()
        {
            totalSize = 0;
            blocks.Clear();
            mainSet = false;
            blockTable.Clear();
            GlobalSize = 0;
        }

        public Block()
        {
            name = "";
            size = 0;
            startAdress = 0;
            isMain = false;
            content = new List<Instruction>();
            blockQueue = new List<string>();
        }

        /// <summary>
        /// Adds the instruction to the block.
        /// </summary>
        /// <param name="inst">Inst.</param>
        public void addInstruction(Instruction inst)
        {
            content.Add(inst);
            size++;
            totalSize++;
        }

        /// <summary>
        /// Adds the block to the block list.
        /// </summary>
        public void addBlock()
        {
            if (name == "main") {
                if (!mainSet) {
                    this.isMain = true;
                    mainSet = true;
                }
                else
                    throw new Exception("Error: entry point already set !");
            }
            blocks.Add(this);
        }


        public void addBlockRef(string name)
        {
            blockQueue.Add(name);
        }


        /// <summary>
        /// resolve call adress, block adress
        /// befor instructions are converted to machine code
        /// </summary>
        public static void preGenerate()
        {
            #warning TO-DO add patching of call and label (adding mup to translate to 32bit adress)

            //set adress of each block
            patch();
            ResolveBlocksCall();
        }


        /// <summary>
        /// Patch adresses
        /// </summary>
        static void patch()
        {
            GlobalSize = 0;
            Console.WriteLine(Environment.NewLine + "Pre Generation:" + Environment.NewLine);

            //find main
            Block main = blocks.Find(x => x.isMain == true);
            GlobalSize += main.size;
            Console.WriteLine("Main".PadRight(20) + GlobalSize.ToString().PadLeft(10,'0') + " Lines | start adress: 0");

            //get all non-main blocks and process them
			//set their start adress
            foreach (Block b in blocks.FindAll(x => x.isMain == false)) {
                b.startAdress = GlobalSize;
                Console.WriteLine(b.name.PadRight(20) + b.size.ToString().PadLeft(10,'0') + " Lines | start adress: " + GlobalSize);
                GlobalSize += b.size;
                blockTable.Add(b.name, b.startAdress);
            }
        }
        

        /// <summary>
        /// Resolves blocks call
        /// </summary>
        static void ResolveBlocksCall()
        {
           foreach (Block b in blocks) {
                foreach (string n in b.blockQueue) {
                     Block target = blocks.Find(x => x.name == n);
                     if (target != null) {
                        #if DEBUG
                            Console.WriteLine("resolving block call: " + target.name + " @ " + target.startAdress);
                        #endif
                        b.onBlockRes(target.name,target.startAdress);
                     }
                }
           }
       }

    }
}
