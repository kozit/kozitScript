using System;

namespace kozitScript
{
    public class Memory
    {

        byte[] Buffer;
        int Size;
        public Memory(int Size)
        {
            this.Size = Size;
            Buffer = new byte[Size];
        }
        public int GetSize()
        {
            return Size;
        }
        public void Clear()
        {
            for (int i = 0; i < Size; i++)
                Buffer[i] = 0;
        }
        public byte Get(int Addr)
        {
            return Buffer[Addr];
        }
        public void Set(int Addr, byte Value)
        {
            Buffer[Addr] = Value;
        }

        internal void Destroy()
        {
            Buffer = null;
            Size = 0;
        }
        public byte this[int Addr]
        {
            get { return Buffer[Addr]; }
            set {
                Buffer[Addr] = value; }
        }



    }
}
