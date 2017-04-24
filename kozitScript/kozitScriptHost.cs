using System;
using System.Collections.Generic;

namespace kozitScript
{
    public class KozitScriptHost
    {
        public Dictionary<string, object> MEM = new Dictionary<string, object>();

#if DEBUG
        bool Debug = true;
#else
        bool Debug = fales;
#endif

        public object this[string Name]
        {
            set { if (MEM.ContainsKey(Name )) { MEM.Add(Name, value); } else { MEM[Name] = value; } }
            get { if (MEM.ContainsKey(Name )) { return MEM[Name]; } else { return ""; } }
        }

        public KozitScriptHost(API[] Apis)
        {
            foreach (API Item in Apis)
            {
                MEM.Add("API:" + Item.Name,Item);
            }
            for (int i = 0; i < 11; i++)
            {
                MEM.Add("*" + i, null);
            }
        }

        public void Run(string Func)
        {
            Parse(Func);
        }

        public void RunFile(string Path)
        {
            LoadFile(Path);
            Parse("Main");
        }

        public void LoadFile(string Path)
        {

            Include(Path);
                        
        }

        public void Compile(string Path)
        {
            Include(Path);

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
            string mPath = Path;
            if (MEM.ContainsKey("System:Paths")) {
                string[] Paths = (string[])MEM["System:Paths"];
                if(Debug)
                Console.WriteLine(":::");
                for (int i = 0; i < Paths.Length; i++)
                {
                    
                    if (System.IO.File.Exists(Paths[i] + Path))
                    {
                        if (Debug)
                            Console.WriteLine(Paths[i] + Path + "|||");
                        mPath = Paths[i] + Path;
                        break;
                    }
                    else if (System.IO.File.Exists(Paths[i] + "\\" + Path))
                    {
                        if (Debug)
                            Console.WriteLine(Paths[i] + Path + "|||");
                        mPath = Paths[i] + "\\" + Path;
                        break;
                    }
                }
            }
            string[] Temp = System.IO.File.ReadAllText(mPath).Split(';');
            

            string Func = "";
            string NameSpace = "";
            for (int i = 0; i < Temp.Length; i++)
            {
                string line = Temp[i].Trim(' ');
                if (Debug)
                    if(line != Environment.NewLine)
                    Console.WriteLine(line);

                if (line.StartsWith("#include"))
                {
                    if (Debug)
                        Console.WriteLine("include");
                    Include(getTokens(Temp[i])[1]);
                }

                else if (line.StartsWith("#$"))
                {
                    if (Debug)
                        Console.WriteLine("Prevar");

                    MEM.Add(getTokens(line.Remove(0, 1))[0], getTokens(line.Remove(0, 1))[1]);
                }

                else if (line.StartsWith("Namespace"))
                {
                    if (Debug)
                        Console.WriteLine("NameSpace Start");

                    NameSpace = line.Remove(0, 9).Trim(' ');
                }

                else if (line.StartsWith("End Namespace"))
                {
                    if (Debug)
                        Console.WriteLine("NameSpace End");
                    NameSpace = "";

                }

                else if (line.StartsWith("Func"))
                {
                    if (Debug)
                    {
                        Console.WriteLine("Func Start");
                        Console.WriteLine(":" + line.Remove(0, 4).Trim(' ') + ":");
                        Console.WriteLine(":|" + line.Remove(0, 4) + "|:");
                    }
                    Func = Temp[i].Remove(0, 4).Trim(' ');
                    MEM.Add("Func:" + Func + ":Start", i);

                }

                else if (line.StartsWith("End Func"))
                {
                    if (Debug)
                        Console.WriteLine("Func End");
                    MEM.Add("Func:" + Func + ":End", i);

                    List<string> r = new List<string>();

                    if (MEM.ContainsKey(("Func:" + Func + ":Start")))
                    {
                        string args = getTokens(Temp[int.Parse((string)MEM["Func:" + Func + ":Start"])]).ToString().Remove(0, 5 + Func.Length);
                        
                        r.Add(args);
                        r.Add("");
                        for (int ii = int.Parse((string)MEM["Func:" + Func + ":Start"]) + 1; ii < int.Parse(MEM["Func:" + Func + ":Start"].ToString() + 1); ii++)
                        {
                            string line1 = Temp[ii].Trim(' ');
                            if (!line1.StartsWith(":"))
                            {
                                r.Add(Temp[ii]);
                            }

                            else if (line1 == "")
                            { }

                            else if (line1.StartsWith("//"))
                            { }

                            else
                            {
                                r[1] += line1.Remove(0, 1) + ":" + ii + ";";
                            }

                        }
                    }

                    MEM.Add("Func:" + NameSpace + Func, r.ToArray());
                    //MEM.Remove("Func:" + Func + ":End");
                    //MEM.Remove("Func:" + Func + ":Start");
                    Func = "";

                    if (Debug)
                    {
                        Console.WriteLine("|||||||||");
                        Console.WriteLine(Func);
                        Console.WriteLine(NameSpace);
                    }

                }


            }


        }
        
        internal void Parse(string Func)
        {
            string[] Code = { ""};
            try
            {
                Code = (string[])MEM["Func:" + Func];
            }
            catch
            {
                new Exceptions.ParseException(Func,-1,"Func not Found");
            }

            string[] GOTO = Code[1].Split(';');

            //MEM.Add("","");
            for (int i = 2; i < Code.Length; i++)
            {
                string[] Tokens = getTokens(Code[i]);
                //Srart if Else Endif Elseif
                if (Tokens[0] == "if")
                {
                    if (Tokens.Length == 2)
                    {
                        if (Tokens[1][1] == '!')
                        {
                            if ((bool)MEM[Tokens[1].Remove(0, 2)] == true)
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

                            if ((bool)MEM[Tokens[1].Remove(0, 1)] == false)
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

                        if (Tokens[2] == "==")
                        {

                            if (Tokens[1] != Tokens[3])
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
                        else if (Tokens[2] == "!=")
                        {

                            if (Tokens[1] == Tokens[3])
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
                    else
                    {
                        new Exceptions.ParseException(Func, i, "IF Tokens Length wrong");
                    }

                }

                else if (Tokens[0] == "Else")
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
                //Srart if Else Endif Elseif

                else if (Tokens[0] == "Init")
                {
                    API temp = (API)MEM["API:" + Tokens[1]];
                    MEM["*0"] = temp.Interrupt(byte.Parse(Tokens[2]), MEM);
                }

                else if (Tokens[0] == "Return")
                {
                    MEM["*0"] = Tokens[1];
                    break;
                }

                else if (Tokens[0].StartsWith("$"))
                {
                    if (MEM.ContainsKey(Tokens[1]))
                    {
                        MEM[Tokens[0]] = Tokens[1];
                    }
                    else
                    {
                        MEM.Add(Tokens[0], Tokens[1]);
                    }
                }

                else if (Tokens[0].StartsWith("*"))
                {
                    MEM[Tokens[0]] = Tokens[1];
                }

                else if (Tokens[0] == "Goto")
                {
                    bool isNumeric = int.TryParse(Tokens[1], out i);
                    if(!isNumeric)
                    for (int ii = 0; ii < GOTO.Length; ii++)
                    {
                        if (GOTO[ii].Split(':')[0] == Tokens[1])
                        {

                                isNumeric = int.TryParse(GOTO[ii].Split(':')[1], out i);
                                if (!isNumeric)
                                    new Exceptions.ParseException(Func, i, "Token 2 invalid");

                        }
                    }

                }

                else
                {

                    if (MEM.ContainsKey(Tokens[0]))
                    {
                        int a = 1;
                        // why do i need to do this C# why
                        string[] t = (string[])MEM[Tokens[0]];
                        foreach (string I in t[0].Split(','))
                        {
                            if (MEM.ContainsKey(I))
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
                        try
                        {
                            Parse(Tokens[0]);
                        }
                        catch
                        {
                        }
                    }
                    else
                    {
                        new Exceptions.ParseException(Func, i, "GOTO");
                    }

                }

            }



        }


    }
}