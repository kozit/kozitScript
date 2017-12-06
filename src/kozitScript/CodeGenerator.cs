using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kozitScript
{
    public class CodeGenerator
    {

        public List<byte> Output = new List<byte>();

        public ParseTree Tree = new ParseTree();


        public Dictionary<string, List<byte>> Functions = new Dictionary<string, List<byte>>();
        public Dictionary<string, int> Functionspos = new Dictionary<string, int>();

        public Dictionary<string, int> Variables = new Dictionary<string, int>();

        public CodeGenerator(string Path)
        {
            
            for (int i = 0; i < Tree.Functions.Count; i++)
            {

                for (int ii = 0; ii < Tree.Functions[i].Instructions.Count; ii++)
                {

                    Variables.Add(Tree.Functions[i].Instructions[ii].Var.Name, Variables.Count * 4);

                }

            }
            int offset = 2 + Tree.Main.Instructions.Count;
            for (int i = 0; i < Tree.Functions.Count; i++)
            {
                                
                Functions.Add(Tree.Functions[i].Name, new List<byte>());
                Functionspos.Add(Tree.Functions[i].Name, offset);

                offset += Tree.Functions[i].Instructions.Count + 1;

            }

            Output.Add(0x02);
            Functions.Add("Main", new List<byte>());
            Functionspos.Add("Main", 2);
            Fill("Main", 15, (Variables.Count * 4) + 8, 0x03);

            MakeFunction(Tree.Main);

            for (int i = 0; i < Tree.Functions.Count; i++)
            {

                MakeFunction(Tree.Functions[i]);

            }


            for (int i = 0; i < Functions.Count; i++)
            {

                Output.AddRange(Functions.ElementAt(i).Value);


            }

            System.IO.File.WriteAllBytes("/Output.KsIL", Output.ToArray());

        }

        public void MakeFunction(Function Function)
        {
            
            for (int i = 0; i < Function.Instructions.Count; i++)
            {

                Instruction instruction = Function.Instructions[i];

                if (instruction.Func != null)
                {
                   // Functions[Function.Name].AddRange();
                }

            }

            Functions[Function.Name].Add(0x21);
            Footer(Function.Name);
                        
        }

        public void IF(string Function, string right, string Operator, string left, int jumpto)
        {

            int Location0, Location1;
            if (right.StartsWith('$'))
            {

                right.Remove(0, 1);
                Location0 = Variables[right];

            }
            else
            {

                Location0 = Variables.Count * 4;
                DynamicStore(Function, (Variables.Count * 4), right);

            }

            if (left.StartsWith('$'))
            {

                left.Remove(0, 1);
                Location1 = Variables[left];

            }
            else
            {

                Location1 = (Variables.Count * 4) + 4;
                DynamicStore(Function, (Variables.Count * 4) + 4, left);

            }

            if (Operator.Contains("="))
            {

                TestEqual(Function, Location0, Location1);

            }
            else
            {

                TestGreaterThan(Function, Location0, Location1);

            }

            if (Operator == "!=")
            {

                JumpIfFalse(Function, jumpto);

            }
            else if (Operator == "=")
            {

                JumpIfTrue(Function, jumpto);

            }
            else if (Operator == "<")
            {

                JumpIfFalse(Function, jumpto);

            }
            else if (Operator == ">")
            {

                JumpIfTrue(Function, jumpto);

            }

        }

        public void ELSE(string Function, int jumpto)
        {

            Jump(Function, jumpto);

        }

        public void SetVariable(string Function, string Variable, string Data)
        {

            Clear(Function, MakeRead(Variables[Variable]));
            DynamicStore(Function, Variables[Variable], Data);

        }

        #region "Base Commands"
        public void Init(string Function, Int16 code, string[] Data)
        {

            Functions[Function].Add(0x00);
            Functions[Function].AddRange(MakeSafe(BitConverter.GetBytes(code)));

            for (int i = 0; i < Data.Length; i++)
            {

                Functions[Function].AddRange(MakeSafe(GetArgument(Data[0])));

            }

            Footer(Function);
        }

        public void Store(string Function, int Location, string Data)
        {

            Functions[Function].Add(0x01);
            byte[] data = Encoding.UTF8.GetBytes(Data);
            Functions[Function].AddRange(MakeSafe(BitConverter.GetBytes(data.Length)));
            Functions[Function].AddRange(data);
            Functions[Function].AddRange(MakeSafe(BitConverter.GetBytes(Location)));
            Footer(Function);

        }

        public void DynamicStore(string Function, int Location, string Data)
        {

            Functions[Function].Add(0x02);
            byte[] data = Encoding.UTF8.GetBytes(Data);
            Functions[Function].AddRange(MakeSafe(BitConverter.GetBytes(data.Length)));
            Functions[Function].AddRange(data);
            Functions[Function].AddRange(MakeSafe(BitConverter.GetBytes(Location)));
            Footer(Function);

        }

        public void ReadInto(string Function, int Location0, int Location1)
        {

            Functions[Function].Add(0x03);
            Functions[Function].AddRange(MakeSafe(BitConverter.GetBytes(Location0)));
            Functions[Function].AddRange(MakeSafe(BitConverter.GetBytes(Location1)));
            Footer(Function);

        }

        public void DynamicReadInto(string Function, int Location0, int Location1)
        {

            Functions[Function].Add(0x04);
            Functions[Function].AddRange(MakeSafe(BitConverter.GetBytes(Location0)));
            Functions[Function].AddRange(MakeSafe(BitConverter.GetBytes(Location1)));
            Footer(Function);

        }

        public void Fill(string Function, int Location0, int Location1, byte fill)
        {

            Functions[Function].Add(0x05);
            Functions[Function].AddRange(MakeSafe(BitConverter.GetBytes(Location0)));
            Functions[Function].AddRange(MakeSafe(BitConverter.GetBytes(Location1)));
            Functions[Function].Add(fill);
            Footer(Function);

        }
               
        public void Clear(string Function, int Location)
        {

            Clear(Function, MakeSafe(BitConverter.GetBytes(Location)));
         
        }
        public void Clear(string Function, byte[] Location)
        {

            Functions[Function].Add(0x06);
            Functions[Function].AddRange(Location);
            Footer(Function);


        }

        public void TestEqual(string Function, int Location0, int Location1)
        {

            Functions[Function].Add(0x10);
            Functions[Function].AddRange(MakeSafe(BitConverter.GetBytes(Location0)));
            Functions[Function].AddRange(MakeSafe(BitConverter.GetBytes(Location1)));
            Footer(Function);

        }

        public void TestGreaterThan(string Function, int Location0, int Location1)
        {

            Functions[Function].Add(0x11);
            Functions[Function].AddRange(MakeSafe(BitConverter.GetBytes(Location0)));
            Functions[Function].AddRange(MakeSafe(BitConverter.GetBytes(Location1)));
            Footer(Function);

        }

        public void JumpIfTrue(string Function, int Location)
        {

            Functions[Function].Add(0x12);
            Functions[Function].AddRange(MakeSafe(BitConverter.GetBytes(Location)));
            Footer(Function);

        }

        public void JumpIfFalse(string Function, int Location)
        {

            Functions[Function].Add(0x13);
            Functions[Function].AddRange(MakeSafe(BitConverter.GetBytes(Location)));
            Footer(Function);

        }

        public void Jump(string Function, int Location)
        {

            Functions[Function].Add(0x20);
            Functions[Function].Add(0x01);
            Functions[Function].AddRange(MakeSafe(BitConverter.GetBytes(Location)));
            Footer(Function);

        }

        public void Return(string Function)
        {
            Functions[Function].Add(0x21);
            Footer(Function);

        }

        public void Add(string Function, int Location0, int Location1, byte fill)
        {

            Functions[Function].Add(0x30);
            Functions[Function].AddRange(MakeSafe(BitConverter.GetBytes(Location0)));
            Functions[Function].AddRange(MakeSafe(BitConverter.GetBytes(Location1)));
            Footer(Function);

        }

        public void Subtract(string Function, int Location0, int Location1, byte fill)
        {

            Functions[Function].Add(0x31);
            Functions[Function].AddRange(MakeSafe(BitConverter.GetBytes(Location0)));
            Functions[Function].AddRange(MakeSafe(BitConverter.GetBytes(Location1)));
            Footer(Function);

        }

        public void Multiply(string Function, int Location0, int Location1, byte fill)
        {

            Functions[Function].Add(0x32);
            Functions[Function].AddRange(MakeSafe(BitConverter.GetBytes(Location0)));
            Functions[Function].AddRange(MakeSafe(BitConverter.GetBytes(Location1)));
            Footer(Function);

        }

        public void Divide(string Function, int Location0, int Location1, byte fill)
        {

            Functions[Function].Add(0x33);
            Functions[Function].AddRange(MakeSafe(BitConverter.GetBytes(Location0)));
            Functions[Function].AddRange(MakeSafe(BitConverter.GetBytes(Location1)));
            Footer(Function);

        }

        #endregion

        public byte[] MakeList(List<string> Data)
        {
            List<byte> r = new List<byte>();

            r.AddRange(MakeSafe(BitConverter.GetBytes(Data.Count)));

            for (int i = 0; i < Data.Count; i++)
            {

                r.AddRange(MakeSafe(BitConverter.GetBytes(Data[i].Length)));

            }

            for (int i = 0; i < Data.Count; i++)
            {

                r.AddRange(MakeSafe(Encoding.UTF8.GetBytes(Data[i])));

            }
            
            return r.ToArray();

        }

        public byte[] MakeSafe(byte[] Data)
        {

            List<byte> r = new List<byte>();

            if (Data[0] == 0xFF)
            {
                r.Add(0xF1);
            }
            else if (Data[0] == 0xFE)
            {
                r.Add(0xF1);
            }
            else if (Data[0] == 0xF1)
            {
                r.Add(0xF1);
            }

            r.AddRange(Data);

            if (Data[Data.Length - 3] == 0x00 && Data[Data.Length - 2] == 0xFF)
            {
                r.Add(0x33);
            }

            return r.ToArray();

        }

        public byte[] MakeRead(int Location)
        {

            List<byte> r = new List<byte>();

            r.Add(0xFF);
            r.AddRange(BitConverter.GetBytes(Location));

            return r.ToArray();

        }

        public byte[] GetArgument(string Data)
        {

            List<byte> r = new List<byte>();


            if (Data.StartsWith("INT"))
            {
                Data.Remove(0, 3);
                r.AddRange(MakeSafe(BitConverter.GetBytes(int.Parse(Data))));
            }
            else if (Data.StartsWith("BYTE"))
            {
                Data.Remove(0, 4);
                r.AddRange(MakeSafe(new byte[] { byte.Parse(Data) }));
            }
            else if (Data.StartsWith("STR"))
            {
                Data.Remove(0, 3);
                r.AddRange(MakeSafe(Encoding.UTF8.GetBytes(Data)));
            }
            else if (Data.StartsWith("$"))
            {
                Data.Remove(0, 1);
                r.AddRange(MakeRead(Variables[Data]));
            }
            else
            {
                r.AddRange(MakeSafe(Encoding.UTF8.GetBytes(Data)));
            }

            return r.ToArray();

        }

        public void Footer(string Function)
        {
            Functions[Function].Add(0x00);
            Functions[Function].Add(0xFF);
            Functions[Function].Add(0x00);
            Functions[Function].Add(0xFF);
        }

    }
}
