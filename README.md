# DCasm
DC32 compiler for my homemade CPU architecture

##compilation steps

### Block creation
Each block has a dynamic array (List) wich contains the instruction inside the block 
the first step add the instructions, formatted to be easily processed, to the block instruction list
when the compiler reach the end of a block it adds it to the blocks list.

see l40 - l50 in DCasm.atg : global block grammar
see l80 - l95 in Block.cs  : addBlock() function
see l57 - 106 in DCasm.atg : instruction grammar


### Pre-Generation

#### I
Occurs when all blocks are added, the compiler will take a block, set it's start adress according to the Main block wich is set first and resolve labels adress.
Doing so, there is no need to set the adress of each instruction (this might change in the future )

#### II
the compiler resolve any block call as it has now the adress of any block in the source code


### Code Generation
to be implemented


## How to test
>do path/to/source/file 

currently there is no output file generation

 

