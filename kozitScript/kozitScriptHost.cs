using System;
using System.Collections.Generic;

namespace kozitScript
{
    public class KozitScriptHost
    {
        public Dictionary<string, object> MEM = new Dictionary<string, object>();


        public object this[object ob, string Name]
        {
            set { MEM.Add( "Func:" + Name,value); }
        }

        public KozitScriptHost()
        {
            MEM.Add("API:System", new Lib.System());
        }


        public void Run(string Path)
        {

            for (int i = 0; i <11;i++)
            {
                MEM.Add("*" + i, null);
            }

            Include(Path);
            Parse("Main");
            MEM = new Dictionary<string, object>();
        }


        internal void Parse(string Func)
        {
            string[] Code = (string[])MEM["Func:" + Func];
            MEM.Add("","");
            for (int i = 1; i < Code.Length; i++)
            {
                string[] Tokens = getTokens(Code[i]);
                //Srart if Else Endif Elseif
                if (Tokens[0] == "if")
                {
                    if (Tokens.Length == 2)
                    {
                        if (Tokens[1][1] == '!')
                        {
                            if ((bool)MEM[Tokens[1].Remove(0, 2)] == false)
                            {

                            }
                            else
                            {
                                int ii = i;
                                while (Code[ii] != "Else" | Code[ii] != "Elseif")
                                {
                                    ii++;
                                }
                                if (Code[ii] == "Elseif")
                                {
                                    ii--;
                                }

                                i = ii;

                            }
                        }
                        else
                        {

                            if ((bool)MEM[Tokens[1].Remove(0, 1)] == true)
                            {

                            }
                            else
                            {
                                int ii = i;
                                while (Code[ii] != "Else" | Code[ii] != "Elseif")
                                {
                                    ii++;
                                }
                                if (Code[ii] == "Elseif")
                                {
                                    ii--;
                                }

                                i = ii;

                            }

                        }

                    }
                    else if (Tokens.Length == 4)
                    {



                    }
                    else
                    {

                    }

                }
                else if (Code[i] == "Else")
                {
                    int ii = i;
                    while (Code[ii] != "Endif" | Code[ii] != "Elseif")
                    {
                        ii++;
                    }

                    if (Code[ii] == "Elseif")
                    {
                        ii--;
                    }

                    i = ii;
                    //Srart if Else Endif Elseif
                }
                else if (Tokens[0] == "Init")
                {
                    API temp = (API)MEM["API:" + Tokens[1]];
                    MEM["*0"] = temp.Interrupt(byte.Parse(Tokens[2]), MEM);
                }
                else if (Tokens[0].StartsWith("$"))
                {
                    if (MEM[Tokens[1]] != null)
                    {
                        MEM[Tokens[1]] = Tokens[1];
                    }
                    else
                    {
                        MEM.Add(Tokens[1], Tokens[1]);
                    }
                }
                else
                {

                    if (MEM[Tokens[0]] != null)
                    {
                        int a = 1;
                        // why do i need to do this C# why
                        string[] t = (string[])MEM[Tokens[0]];
                        foreach (string I in t[0].Split(','))
                        {
                            if (MEM[I] != null)
                            {
                                MEM[I] = Tokens[a];
                            }
                            else
                            {
                                MEM.Add(I, Tokens[a]);
                            }
                            a++;
                        }
                        t = null;
                        Parse(Tokens[0]);
                    }

                }

            }



        }

        public string[] getTokens(string s)
        {
            const char split = ' ';
            const char quote = '"';
            bool isinquotes = false;

            
            List<string> tokens = new List<string> { "" };

            foreach (char c in s)
            {

                if (c == quote) { isinquotes = !isinquotes; }
                else if (c == split && isinquotes == false) { tokens.Add(""); }
                else { tokens[tokens.Count - 1] += c; }

            }

            return tokens.ToArray();

        }


        public void Include(string Path)
        {
            string[] Temp = System.IO.File.ReadAllText(Path).Split(';');
            

            string Func = "";
            string NameSpace = "";
            for (int i = 0; i != Temp.Length; i++)
            {

                if (Temp[i].StartsWith("#include"))
                {
                    Include(getTokens(Temp[i])[1]);
                }

                if (Temp[i].Trim(' ').StartsWith("Namespace"))
                {

                    NameSpace = Temp[i].Remove(0, 9).Trim(' ');
                }


                if (Temp[i].Trim(' ').StartsWith("End Namespace"))
                {

                    NameSpace = "";

                }

                if (Temp[i].Trim(' ').StartsWith("Func"))
                {

                    Func = Temp[i].Remove(0, 4).Trim(' ');
                    MEM.Add("Func:" + Func + ":Start", i);

                }


                if (Temp[i].Trim(' ').StartsWith("End Func"))
                {

                    MEM.Add("Func:" + Func + ":End", i);

                    List<string> r = new List<string>();
                    if (MEM["Func:" + Func + ":Start"] != null)
                    {
                        string args = getTokens(Temp[int.Parse((string)MEM["Func:" + Func + ":Start"])]).ToString().Remove(0,5 + Func.Length);
                        

                        r.Add( args );
                        for (int ii = int.Parse((string)MEM["Func:" + Func + ":Start"]) + 1; ii < int.Parse(MEM["Func:" + Func + ":Start"].ToString() + 1); ii++)
                        {

                            r.Add(Temp[ii]);

                        }
                    }
                    
                    MEM.Add("Func:" + NameSpace + Func, r.ToArray());
                    MEM.Remove("Func:" + Func + ":End");
                    MEM.Remove("Func:" + Func + ":Start");
                    Func = "";

                }


            }


        }

    }
}