using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace kozitScript
{
    public class CodeGenerator
    {

        public List<byte> Output = new List<byte>();

        public ParseTree Tree = new ParseTree();
        
        public Dictionary<string, List<byte>> Functions = new Dictionary<string, List<byte>>();
        public Dictionary<string, List<string>> FunctionsArg = new Dictionary<string, List<string>>();
        public Dictionary<string, int> Functionspos = new Dictionary<string, int>();
        public Dictionary<string, int> Variables = new Dictionary<string, int>();

        public int VariablesOffset = 0;

        public static Dictionary<string, string> Globalconstant = new Dictionary<string, string>();

        public CodeGenerator(ParseTree Tree)
        {

            this.Tree = Tree;
            VariablesOffset = 0;
            for (int i = 0; i < Tree.Functions.Count; i++)
            {

                for (int ii = 0; ii < Tree.Functions[i].Instructions.Count; ii++)
                {

                    Variables.Add(Tree.Functions[i].Instructions[ii].Var.Name, VariablesOffset);
                    VariablesOffset += 5;

                }

            }
            int offset = 2;

            for (int i = 0; i < Tree.Functions.Count; i++)
            {
                                
                Functions.Add(Tree.Functions[i].Name, new List<byte>());
                Functionspos.Add(Tree.Functions[i].Name, offset);
                int offsetby = 0;

                for (int ii = 0; ii < Tree.Functions[i].Instructions.Count; ii++)
                {

                    offsetby += Tree.Functions[i].Instructions[ii].CodeSize;

                }

                offset += offsetby + 1;

            }

            //hard codded 32bit mode
            Output.Add(0x02);

            //hard codded JUMP to Global constant under all the functions
            Output.Add(0x20);
            Output.Add(0x00);
            Output.AddRange(MakeSafe(BitConverter.GetBytes(offset + 1)));
            Output.Add(0x00);
            Output.Add(0x00);
            Output.Add(0xFF);
            Output.Add(0x00);
            Output.Add(0xFF);

            //hard codded FILL

            Output.Add(0x05);
            Output.AddRange(MakeSafe(BitConverter.GetBytes(15)));
            Output.AddRange(MakeSafe(BitConverter.GetBytes(VariablesOffset + 10)));
            Output.Add(0x03);
            Output.Add(0x00);
            Output.Add(0xFF);
            Output.Add(0x00);
            Output.Add(0xFF);
                        
            for (int i = 0; i < Tree.Functions.Count; i++)
            {

                MakeFunction(Tree.Functions[i]);

            }
            
            for (int i = 0; i < Functions.Count; i++)
            {

                Output.AddRange(Functions.ElementAt(i).Value);
                
            }

            for (int i = 0; i < Globalconstant.Count; i++)
            {

                if (Variables.TryGetValue(Globalconstant.ElementAt(i).Key, out int point))
                {

                    Output.Add(0x02);
                    byte[] data = Encoding.UTF8.GetBytes(Globalconstant.ElementAt(i).Value);
                    Output.AddRange(MakeSafe(BitConverter.GetBytes(data.Length)));
                    Output.AddRange(data);
                    Output.AddRange(MakeSafe(BitConverter.GetBytes(point)));
                    Output.Add(0x00);
                    Output.Add(0x00);
                    Output.Add(0xFF);
                    Output.Add(0x00);
                    Output.Add(0xFF);

                }

            }
            
            //hard codded JUMP to Main
            Output.Add(0x20);
            Output.Add(0x00);
            Output.AddRange(MakeSafe(BitConverter.GetBytes(Functionspos["Main"])));
            Output.Add(0x00);
            Output.Add(0x00);
            Output.Add(0xFF);
            Output.Add(0x00);
            Output.Add(0xFF);


        }

        public void MakeFunction(Function Function)
        {

            List<string> FunctionVariables = new List<string>();

            string FunctionName = Function.Name;
            int Functionoffset = Functionspos[FunctionName];

            Console.WriteLine("Making Function:" + FunctionName);

            for (int i = 0; i < Function.Instructions.Count; i++)
            {

                Instruction instruction = Function.Instructions[i];

                if (instruction.Func != null)
                {

                    for (int Variable = 0; Variable  < instruction.Func.Arguments.Count; Variable ++)
                    {

                        //SetVariable(FunctionName, new Variable() { value = instruction.Func.Arguments[i], Name = instruction.Func.Arguments.ToArray(), VariableType = VariableType.Functions});

                    }
                    
                    Jump(FunctionName, Functionspos[instruction.Func.Function]);

                }
                else if (instruction.Op != null)
                {

                    string OPCODE = instruction.Op.Type;

                    List<string> OpArguments = instruction.Op.Arguments;

                    if (OPCODE == "IF")
                    {
                        int line = 0;
                        for (int ii = i; ii < Function.Instructions.Count; ii++)
                        {

                            if (Function.Instructions[ii].Op != null)
                            {

                                if (Function.Instructions[ii].Op.Type == "ELSE")
                                {
                                    line = ii;
                                }
                                else if (Function.Instructions[ii].Op.Type == "ELSEIF")
                                {
                                    line = ii;
                                }
                                else if (Function.Instructions[ii].Op.Type == "ENDIF")
                                {
                                    line = ii;
                                }


                            }

                        }

                        IF(FunctionName, OpArguments[0], OpArguments[1], OpArguments[2], Functionoffset + line);

                    }
                    else if (OPCODE == "ELSEIF")
                    {

                        int line = 0;
                        for (int ii = i; ii < Function.Instructions.Count; ii++)
                        {

                            if (Function.Instructions[ii].Op != null)
                            {

                                if (Function.Instructions[ii].Op.Type == "ELSE")
                                {
                                    line = ii;
                                }
                                else if (Function.Instructions[ii].Op.Type == "ENDIF")
                                {
                                    line = ii;
                                }

                            }

                        }

                        IF(FunctionName, OpArguments[0], OpArguments[1], OpArguments[2], Functionoffset + line);

                    }
                    else if (OPCODE == "WHILE")
                    {

                        int Offset = 0;
                        for (int ii = i; ii < Function.Instructions.Count; ii++)
                        {

                            Offset += Function.Instructions[ii].CodeSize;
                                if (Function.Instructions[ii].Op.Type == "ENDWHILE")
                                {
                                Offset = ii;
                                    break;
                                }

                        }

                        IF(FunctionName, OpArguments[0], OpArguments[1], OpArguments[2], Functionoffset);

                    }
                    else if (OPCODE == "RAW")
                    {

                        List<byte> RAW = new List<byte>();

                        string[] Hexcodes = OpArguments[0].Split(' ');

                        for (int hexoffset = 0; hexoffset < Hexcodes.Length; hexoffset++)
                        {

                            RAW.Add(byte.Parse(Hexcodes[hexoffset], NumberStyles.HexNumber, CultureInfo.InvariantCulture.NumberFormat));

                        }

                        Functions[FunctionName].AddRange(RAW.ToArray());

                    }
                    else if (OPCODE == "INT")
                    {

                        List<string> Data = new List<string>();

                        Data = OpArguments;
                        Data.RemoveAt(0);

                        Init(FunctionName, Int16.Parse(OpArguments[0]), Data.ToArray());

                    }

                }
                else if (instruction.Var != null)
                {

                        SetVariable(FunctionName, instruction.Var);

                }

            }

            Functions[Function.Name].Add(0x21);
            Footer(Function.Name);
                        
        }

        public void RETURN(string Function, string Variable, string Returnvar)
        {

            if (Variable.StartsWith('$'))
            {
                Variable = Variable.Remove(0, 1);
            }
            else
            {

                Variable = Function + "." + Variable;

            }

            Return(Function);

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

                Location0 = Variables.Count * 5;
                DynamicStore(Function, (Variables.Count * 5), right);

            }

            if (left.StartsWith('$'))
            {

                left.Remove(0, 1);
                Location1 = Variables[left];

            }
            else
            {

                Location1 = (Variables.Count * 5) + 5;
                DynamicStore(Function, (Variables.Count * 5) + 5, left);

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

            Jump(Function, jumpto, 0x00);

        }

        public void SetVariable(string Function, Variable Variable)
        {

            string VariableName = "";

            if (Variable.VariableType == VariableType.Global)
            {

                VariableName = Variable.Name;

            }
            else if (Variable.VariableType == VariableType.Functions)
            {
                
                VariableName = Function + "." + Variable.Name;

            }
            else if (Variable.VariableType == VariableType.Globalconstant)
            {

                VariableName = "GC." + Variable.Name;

            }

            if (Variable.isList)
            {

                SetVariable(Function, VariableName, MakeList((List<string>) Variable.value));

            }
            else if (Variable.isString)
            {

                SetVariable(Function, VariableName, (string) Variable.value);

            }
            else if (Variable.isNum)
            {

                SetVariable(Function, VariableName, BitConverter.GetBytes((int) Variable.value));

            }


        }

        public void SetVariable(string Function, string Variable, byte[] Data)
        {

            DynamicStore(Function, Variables[Variable], Data);

        }

        public void SetVariable(string Function, string Variable, string Data)
        {

            SetVariable(Function, Variable, Encoding.UTF8.GetBytes(Data));

        }

        public void KillVariable(string Function, string Variable)
        {

            Clear(Function, MakeRead(Variables[Variable]));

        }

        #region "Base Commands"

        public void Init(string Function, Int16 code, string[] Data)
        {

            Functions[Function].Add(0x00);
            Functions[Function].AddRange(MakeSafe(BitConverter.GetBytes(code)));

            for (int i = 0; i < Data.Length; i++)
            {

                Functions[Function].AddRange(MakeSafe(GetArgument(Data[i])));

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

            byte[] data = Encoding.UTF8.GetBytes(Data);
            DynamicStore(Function, MakeSafe(BitConverter.GetBytes(Location)), data);

        }

        public void DynamicStore(string Function, int Location, byte[] Data)
        {

            DynamicStore(Function, MakeSafe(BitConverter.GetBytes(Location)), Data);

        }

        public void DynamicStore(string Function, byte[] Location, byte[] Data)
        {

            Functions[Function].Add(0x02);
            
            Functions[Function].AddRange(MakeSafe(BitConverter.GetBytes(Data.Length)));
            Functions[Function].AddRange(Data);
            Functions[Function].AddRange(Location);
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

        public void Fill(string Function, string Location0, string Location1, byte fill)
        {

            Functions[Function].Add(0x05);
            Functions[Function].AddRange(GetArgument(Location0));
            Functions[Function].AddRange(GetArgument(Location1));
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

        public void Jump(string Function, int Location, byte Return = 0x01)
        {

            Functions[Function].Add(0x20);
            Functions[Function].Add(Return);
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

        #region "Utills"

        public byte[] MakeList(List<string> Data)
        {

            return MakeList(Data.ToArray());

        }

        public byte[] MakeList(string[] Data)
        {
            List<byte> r = new List<byte>();

            r.AddRange(MakeSafe(BitConverter.GetBytes(Data.Length)));

            for (int i = 0; i < Data.Length; i++)
            {

                r.AddRange(BitConverter.GetBytes(Encoding.UTF8.GetBytes(Data[i]).Length));

            }

            for (int i = 0; i < Data.Length; i++)
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
            else if (Data.StartsWith("LIST"))
            {
                Data.Remove(0, 4);
                r.AddRange(MakeSafe(MakeList(Data.Split(','))));

            }
            else if (Data.StartsWith("$"))
            {
                Data.Remove(0, 1);
                r.Add(0xFF);
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

            if (Functions[Function].ElementAt( int.Parse(Functions.Count.ToString()) - 2) == 0x00 && Functions[Function].ElementAt(int.Parse(Functions.Count.ToString()) - 1) == 0xFF)
            {
                Functions[Function].Add(0x33);
            }

            Functions[Function].Add(0x00);
            Functions[Function].Add(0xFF);
            Functions[Function].Add(0x00);
            Functions[Function].Add(0xFF);
        }

        #endregion

    }
}
