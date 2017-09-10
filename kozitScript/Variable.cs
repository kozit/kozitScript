using System.Collections.Generic;

namespace kozitScript
{
    public class VariableManager
    {
        Variable[] Variables;
        Memory Memory;
        public int FreeAddr;

        public VariableManager(int MemorySize)
        {
            Variables = new Variable[MemorySize / 4];
            Memory = new Memory(MemorySize);
            FreeAddr = 0;
            Init(16);
        }

        void Init(int n)
        {
            for (int i = 0; i < n; i++)
            {
                AMem(i, FreeAddr, 4);
            }
        }

        public byte[] GetRange(int Start, int End)
        {
            int now = Start;
            List<byte> get = new List<byte>();
            while (now < Start + End)
                get.Add(Memory.Get(now++));
            return get.ToArray();
        }

        public void AMemRemove(int clear)
        {
            if (Variables[clear] != null)
            {
                AMemRemove(clear);
                Variables[clear] = null;
            }
        }
        public void AMemClear(int clear)
        {
            if (Variables[clear] != null)
            {
                int now = Variables[clear].Address;
                while (now <= Variables[clear].Address + Variables[clear].Size)
                    Memory.Set(now++, 0x0);
            }
        }

        internal byte[][] Get(int v)
        {
            byte[][] ret = new byte[v][];
            for (int i = 0; i < v; i++)
                ret[i] = AMem(i);
            return ret;
        }

        public void AMem(int set, int addr, int size)
        {
            if (Variables[set] != null)
            {
                return;
            }
            FreeAddr += size;
            Variables[set] = new Variable(addr, size);
        }

        public void AMem(int set, byte[] value)
        {
            if (Variables[set] != null)
            {
                try
                {
                    for (int i = 0; i < value.Length; i++)
                        Memory.Set(i + Variables[set].Address, value[i]);
                }
                catch
                {
                    
                }
            }
            else
            {
                
            }
        }

        public byte[] AMem(int get)
        {
            if (Variables[get] != null)
            {
                return GetRange(Variables[get].Address, Variables[get].Size);
            }
            return new byte[1];
        }

        public void AMem(int dest, int source)
        {
            if (Variables[dest] != null && Variables[source] != null)
            {
                Variables[dest] = new Variable(Variables[source].Address, Variables[source].Size);
                return;
            }
            
        }

        public void Destroy()
        {
            Variables = null;
            Memory.Destroy();
        }

    }
    public class Variable
    {
        public int Address;
        public int Size;
        public Variable(int addr, int size)
        {
            Address = addr;
            Size = size;
        }
    }
}
