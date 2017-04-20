using System;
using System.Collections.Generic;

namespace kozitScript
{
    public class KozitScriptHost
    {
        public Dictionary<string, object> MEM = new Dictionary<string, object>();

        List<Script> Scripts = new List<Script>();

        public object this[object ob, string Name]
        {
            set { MEM.Add( "Func:" + Name,value); }
        }

        public KozitScriptHost()
        {
            
        }


        public void Compile(string Path)
        {
            string[] Script = System.IO.File.ReadAllText(Path).Split(';');
            

                string funcTemp = ""; 

                for (int i = 0; i < Script.Length; i++)
                {

                    if (Script[i].StartsWith("#include"))
                    {
                        Include(getTokens(Script[i])[1]);
                    }

                    if (Script[i].Trim(' ').StartsWith("Func"))
                    {

                        funcTemp = Script[i].Remove(0, 4).Trim(' ');
                        MEM.Add("Func:" + funcTemp + ":Start", i);

                    }


                    if (Script[i].Trim(' ').StartsWith("End Func"))
                    {

                        MEM.Add("Func:" + funcTemp + ":End", i);

                        List<string> r = new List<string>();
                        if (MEM["Func:" + funcTemp + ":Start"] != null)
                        {
                            for (int ii = int.Parse(MEM["Func:" + funcTemp + ":Start"].ToString()); ii < int.Parse(MEM["Func:" + funcTemp + ":Start"].ToString() + 1); ii++)
                            {

                                r.Add(Script[ii]);

                            }
                        }

                        MEM.Add("Func:" + funcTemp, r.ToArray());
                        MEM.Remove("Func:" + funcTemp + ":End");
                        MEM.Remove("Func:" + funcTemp + ":Start");
                        funcTemp = "";

                    }



                }


            MEM.Add("System:FL",1);
            Parse("Main");

        }


        internal void Parse(string Func)
        {
            string[] Code = (string[])MEM["Func:" + Func];
            MEM.Add("","");
            for (int i = 0; i < Code.Length; i++)
            {
                string[] Tokens = getTokens(Code[i]);

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
                        for (int ii = int.Parse(MEM["Func:" + Func + ":Start"].ToString()); ii < int.Parse(MEM["Func:" + Func + ":Start"].ToString() + 1); ii++)
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