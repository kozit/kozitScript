using System;
using System.Collections.Generic;
using System.Text;

namespace kozitScript
{
    public class Instruction
    {

        public OpCode Op
        {
            get; private set;
        }

        public byte[] Param
        {
            get; private set;
        }

        public Instruction(OpCode Op, byte[] Param)
        {
            this.Op = Op;
            this.Param = Param;
        }

    }
}
