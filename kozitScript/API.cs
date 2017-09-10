using System;
using System.Collections.Generic;
using System.Text;

namespace kozitScript
{
    public abstract class API
    {
        public byte[] ID = new byte[1] { 0x00 };
        public abstract byte[] Interrupt(byte Current, byte[] Data);
    }
}
