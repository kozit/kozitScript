using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace kozitScript
{
    public class TreeGenerator
    {

        public static int FindEnd(int start, string code, char endCode = ';')
        {

            return FindEnd(start, code, endCode);

        }

        public static int FindEnd(int start, string code, string endCode)
        {

            int r = 0;
            for (int i = start; i < code.Length; i++)
            {
                if (code.Substring(i, endCode.Length) == endCode)
                {
                    r = i + endCode.Length;
                    break;
                }
            }

            return r - start;

        }

        public static Function MakeFunction(string Code)
        {

            Function r = new Function();
            // 0 == While
            // 1 == FOR
            // 5 == IF
            // 6 == ELSEIF
            // 7 == ENDIF
            List<int> OPTYPE = new List<int>();
            List<int> OPENB = new List<int>();
            int CLOSEB = 0;

            for (int i = 0; i < Code.Length; i++)
            {

                if (Code[i] == '{')
                {

                    OPENB[0] += 1;

                }
                else if (Code[i] == '}')
                {

                    CLOSEB++;
                    if (CLOSEB == OPENB[0])
                    {

                        if (OPTYPE[0] == 0)
                        {
                            Instruction temp = new Instruction() { CodeSize = 1 };
                            temp.Op.Type = "ENDWHILE";
                            for (int InstructionsIndex = r.Instructions.Capacity; InstructionsIndex > 0; InstructionsIndex--)
                            {

                                if (r.Instructions[InstructionsIndex].Op.Type == "WHILE")
                                {
                                    temp.Op.Arguments = new List<string>() { InstructionsIndex.ToString() };
                                    break;
                                }

                            }

                            r.Instructions.Add(temp);

                        }
                        OPTYPE.RemoveAt(0);
                    }

                    OPENB[1] += OPENB[0];
                    OPENB.RemoveAt(0);
                }
                else if (Code.Substring(i, 4) == "Init")
                {

                    Instruction temp = new Instruction() { CodeSize = 1 };

                    temp.Op = new Operator();
                    temp.Op.Type = "INIT";
                    temp.Op.Arguments.AddRange(Code.Substring(i + 5, FindEnd(i + 5, Code) - 1).Split(','));

                    r.Instructions.Add(temp);
                    i += FindEnd(i, Code);

                }
                else if (Code.Substring(i, 5) == "While")
                {

                    Instruction temp = new Instruction() { CodeSize = 1 };

                    temp.Op = new Operator
                    {
                        Type = "WHILE"
                    };
                    temp.Op.Arguments.AddRange(Code.Substring(i + 6, FindEnd(i + 6, Code, '{') - 1).Split(' '));

                    r.Instructions.Add(temp);
                    i += FindEnd(i, Code) + 5;


                }
                else if (Code[i] == '*')
                {

                    Instruction temp = new Instruction() { CodeSize = 1 };
                    string[] data = Code.Substring(i + 1, FindEnd(i + 1, Code)).Split('=');
                    temp.Var = new Variable
                    {
                        isString = true,
                        Name = data[0].Trim(),
                        value = data[1].Trim(),
                        VariableType = VariableType.Functions
                    };

                    r.Instructions.Add(temp);
                    i += FindEnd(i, Code);

                }
                else if (Code[i] == '$')
                {

                    Instruction temp = new Instruction() { CodeSize = 1 };
                    string[] data = Code.Substring(i + 1, FindEnd(i + 1, Code)).Split('=');
                    temp.Var = new Variable
                    {
                        isString = true,
                        Name = data[0].Trim(),
                        value = data[1].Trim(),
                        VariableType = VariableType.Global
                    };

                    r.Instructions.Add(temp);
                    i += FindEnd(i, Code);

                }
                else if (Code[i] == '#')
                {
                    i += FindEnd(i, Code, Environment.NewLine);
                }
                else if (Code.Substring(i, 2) == "/*")
                {
                    i += FindEnd(i, Code, "*/");
                }
                else if (Code[i] == ' ')
                {
                }
                else if (Code[i] == ';')
                {
                }
                else
                {

                    Instruction temp = new Instruction() { CodeSize = 1 };
                    temp.Func = new Execute();
                    int tempi = FindEnd(i, Code, '(');
                    temp.Func.Function = Code.Substring(i, tempi - 1);
                    temp.Func.Arguments.AddRange(Code.Substring(tempi + i, FindEnd(tempi + i, Code, ')')).Split(','));
                    temp.CodeSize += temp.Func.Arguments.Count;

                    r.Instructions.Add(temp);

                }
            }

            return r;

        }

        public static ParseTree MakeTree(string Path)
        {

            string Code = File.ReadAllText(Path);
            
            List<ParseTree> Libs = new List<ParseTree>();
            
            string useRegex = "#use ([a-zA-Z]+);";


            MatchCollection matches = Regex.Matches(Code, useRegex);

            foreach (Match match in matches)
            {

                ParseTree parseTree = null;

                string libpath = match.Value;

                if (File.Exists(libpath))
                {
                    parseTree = MakeTree(libpath);
                }
                else if (File.Exists(libpath + ".Ks"))
                {
                    parseTree = MakeTree(libpath + ".Ks");
                }
                else if (File.Exists(Environment.CurrentDirectory + libpath))
                {
                    parseTree = MakeTree(Environment.CurrentDirectory + libpath);
                }
                else if (File.Exists(Environment.CurrentDirectory + libpath + ".Ks"))
                {
                    parseTree = MakeTree(Environment.CurrentDirectory + libpath + ".Ks");
                }
                else
                {
                    Console.WriteLine($"Lib not found:{libpath}");
                }
                
                if (parseTree != null)
                {
                    Libs.Add(parseTree);
                }
                
            }
            
            ParseTree r = new ParseTree();

            foreach (ParseTree Lib in Libs)
            {

                foreach (Function Item in Lib.Functions)
                {

                    if (!r.Functions.Contains(Item))
                    {

                        r.Functions.Add(Item);

                    }

                }
                
            }
            
            return r;

        }

    }
}
