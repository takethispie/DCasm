using System;
using System.Collections.Generic;

namespace DCasm
{
    public class Block
    {
        static List<Block> blocks = new List<Block>();
        static bool mainSet = false;

        /// <summary>
        /// all the instructions in the block.
        /// </summary>
        List<Instruction> content;
        public int size;
        public string name;
        public int startAdress;
        bool isMain;

        public Block()
        {
            name = "";
            size = 0;
            startAdress = 0;
            isMain = false;
            content = new List<Instruction>();
        }

        /// <summary>
        /// Adds the instruction to the block.
        /// </summary>
        /// <param name="inst">Inst.</param>
        public void addInstruction(Instruction inst)
        {
            content.Add(inst);
            size += inst.machineCodeSize;
        }
            

        /// <summary>
        /// Adds the block to the block list.
        /// </summary>
        public void addBlock()
        {
            if (name == "main")
            { 
                if (!mainSet)
                {
                    isMain = true;
                }
                else
                {
                    throw new Exception("Error: entry point already set !");
                }
            }
            blocks.Add(this);
        }


        public void resolve()
        {
            foreach (Instruction inst in this.content)
            {
                
            }
        }


        public static void preGenerate()
        {
            //find main    
            Block main = blocks.Find(x => x.isMain == true);
            main.startAdress = 0;

            //get all non-main blocks and process them
            foreach (Block b in blocks.FindAll(x => x.isMain == false))
            {
                b.resolve();
            }
        }
    }
}

