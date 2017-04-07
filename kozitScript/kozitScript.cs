using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kozitScript
{
    public class kozitScript
    {

        public static void InitCosmos()
        {

            if (!Globals.isinit)
            {
                Lib.LibManger.InitCosmos();
            }
        }

        public static void Init()
        {

            if (!Globals.isinit)
            {
                Lib.LibManger.Init();
            }
        }

        public Dictionary<string, object> CommandData = new Dictionary<string, object>();

        public Dictionary<string, Var> var = new Dictionary<string, Var>();

        public class Var
        {
            public object data;
            public bool isReadonly;
            public Var(object data, bool isReadonly = false)
            {
                this.data = data;
                this.isReadonly = isReadonly;
            }
        }

        public string[] Script;
        public int ScriptLine;

        public kozitScript() { common(); }

        public kozitScript(string Code, Dictionary<string, object> CommandData, Dictionary<string, Var> var)
        {
            common();
            this.CommandData = CommandData;
            this.var = var;
            Parse(Code);
        }

        public kozitScript(string Code)
        {
            common();
            Parse(Code);

        }

        public kozitScript(string Code, string args)
        {
            common();
            Parse(args + ";" + Code);
            
        }

        Command dummy;

        void common()
        {
            if (!Globals.isinit)
            {
                throw new Exception(":(");
            }
            for (int i = 0; i < Globals.Commands.Count; i++)
            {
                if ("dummy" == Globals.Commands[i].Name)
                {
                    dummy = Globals.Commands[i];
                    if(Globals.Debug)
                    Console.WriteLine("dummy set");
                    break;
                }
            }

        }

        public void Parse(string Code)
        {
            if (Code.Length < 3)
            {
                Console.WriteLine("Code not set");
                return;
            }
            try
            {
                if (!CommandData.ContainsKey("ParseC"))
                {
                    CommandData.Add("ParseC", 1);
                }
                else
                {
                    CommandData["ParseC"] = (int)CommandData["ParseC"] + 1;
                }

                if (!CommandData.ContainsKey("ParseLine" + CommandData["ParseC"]))
                {
                    CommandData.Add("ParseLine" + CommandData["ParseC"], ScriptLine);
                }

                if (!CommandData.ContainsKey("ErrorC"))
                {
                    CommandData.Add("ErrorC", 0);
                }
            }
            catch
            {

            }

            if (Globals.Debug)
                Console.WriteLine(Code);
            Code = Code.Replace(Environment.NewLine, ";");
            Script = Code.Split(';');
            
            for (ScriptLine = 0; ScriptLine < Script.Length; ScriptLine++)
            {

                //{}{}{}
                if (Globals.Debug)
                    Console.WriteLine(ScriptLine +":"+ Script.Length);
                
                List<string> t = getTokens(Script[ScriptLine]);
                if (Script[ScriptLine].StartsWith("//"))
                { }
                else if (Script[ScriptLine].StartsWith(":"))
                { }
                else if (Script[ScriptLine].Trim(' ') == "")
                { }
                else if (Script[ScriptLine].Length < 2)
                { }
                else if (Script[ScriptLine] == "")
                { }
                else if (Script[ScriptLine].StartsWith("#"))
                {
                    getCommand("func").execute(this, t);
                }
                else if (Script[ScriptLine].StartsWith("&"))
                {
                    getCommand("var").execute(this, t);
                }
                else
                {
                    if (Globals.Debug)
                        Console.WriteLine(t[0] + ":" + Script[ScriptLine]);
                    getCommand(t[0]).execute(this, t);
                }

               

            }

            if ((int)CommandData["ParseC"] != 1)
            {
                ScriptLine = (int)CommandData["ParseLine" + CommandData["ParseC"]];
            }
            CommandData.Remove("ParseLine" + CommandData["ParseC"]);
            CommandData["ParseC"] = (int)CommandData["ParseC"] - 1;

        }



        public Command getCommand(string text)
        {
            Command r = dummy;
            text = text.TrimStart(Environment.NewLine.ToCharArray());
            for (int i = 0; i < Globals.Commands.Count; i++)
            {
                if (text.ToLower() == Globals.Commands[i].Name)
                {
                    r = Globals.Commands[i];
                }
            }
            return r;//null command
        }


        public List<string> getTokens(string s)
        {
            const char split = ' ';
            const char quote = '"';
            bool isinquotes = false;
            for (int i = 0; i < var.Count;i++)
            {
                s.Replace("%" + var.Keys[i].ToString() + "%", var.Values[i].ToString());
            }
            List<string> tokens = new List<string> { "" };
            foreach (char c in s)
            {

                if (c == quote) { isinquotes = !isinquotes; }
                else if (c == split && isinquotes == false) { tokens.Add(""); }
                else { tokens[tokens.Count - 1] += c; }

            }

            return tokens;

        }

    }


    public class Command
    {

        public Command(string Command, command command, bool showinhelp = true)
        {
            this.showinhelp = showinhelp;
            executec = command;
            Name = Command;
        }

        public Command(string Command, string Help, command command, bool showinhelp = true)
        {
            this.showinhelp = showinhelp;
            this.Help = Help;
            executec = command;
            Name = Command;
        }

        public void execute(kozitScript Script, List<string> args)
        {
            executec(Script, args);
        }

        public bool showinhelp;
        public string Help { get; set; }
        public string Name { get; set; }
        public command executec;
        public delegate void command(kozitScript Script, List<string> args);


    }




}
