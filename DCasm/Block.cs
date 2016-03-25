using System;
using System.Collections.Generic;

namespace DCasm
{
    public class Block
    {
        static List<Block> blocks = new List<Block>();

        /// <summary>
        /// all the instructions in the block.
        /// </summary>
        List<Instruction> content;
        public int size;
        public string name;
        public int startAdress;

        public Block()
        {
            name = "";
            size = 0;
            startAdress = 0;
        }

        /// <summary>
        /// Adds the instruction to the block.
        /// </summary>
        /// <param name="inst">Inst.</param>
        public void addInstruction(Instruction inst)
        {
            content.Add(inst);
            size++;
        }
            

        /// <summary>
        /// Adds the block to the block list.
        /// </summary>
        public void addBlock()
        {
            blocks.Add(this);
        }
    }
}

