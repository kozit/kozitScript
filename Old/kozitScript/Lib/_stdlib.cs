using System;
using System.Collections.Generic;
using System.IO;

namespace kozitScript.Lib
{
    public class _stdlib : ILib
    {


        public string Name
        {
            get
            {
                return "std";
            }
        }

        public void RegisterLib()
        {

            Globals.RegisterCommand(new Command("autoload", "auto loads", new Command.command(autoload)));

            Globals.RegisterCommand(new Command("if", "", new Command.command(IF),false));
            Globals.RegisterCommand(new Command("elseif", "", new Command.command(IF), false));
            Globals.RegisterCommand(new Command("else", "", new Command.command(ELSE), false));
            Globals.RegisterCommand(new Command("func","",new Command.command(func),false));
            Globals.RegisterCommand(new Command("endfunc", "", new Command.command(endfunc), false));
            Globals.RegisterCommand(new Command("dummy", "", new Command.command(dummy), false));

            Globals.RegisterCommand(new Command("var","lets you set/get vars",new Command.command(Var)));

            Globals.RegisterCommand(new Command("print", "Prints text", new Command.command(Print)));
            Globals.RegisterCommand(new Command("cls", "clear's screen", new Command.command(cls)));
            Globals.RegisterCommand(new Command("help", "Prints out help", new Command.command(help)));
            Globals.RegisterCommand(new Command("rs", "reset the screen", new Command.command(rs)));
            Globals.RegisterCommand(new Command("info", "prints kozitScript info", new Command.command(info)));
            Globals.RegisterCommand(new Command("libraries", "print's the loaded Libraries", new Command.command(libraries)));
            Globals.RegisterCommand(new Command("run", "runs a kS file", new Command.command(run)));
             


        }

        void Var(kozitScript Script, List<string> args)
        {
            if (args[0] == "var")
            {
                if (args.Count == 2)
                {
                    Script.var[args[1].Remove(0, 1)].data = Console.ReadLine();
                }
                else if (args.Count == 4)
                {
                    if (args[2] == "=")
                    {
                        Script.var[args[1].Remove(0, 1)].data = args[2];
                    }
                    else if (args[2] == "===")
                    {
                        Script.var[args[1].Remove(0, 1)].data = args[2];
                    }
                }
                else
                {

                }
            }
            else
            {
                if (args.Count == 1)
                {
                    Script.var[args[0].Remove(0, 1)].data = Console.ReadLine();
                }
                else if (args.Count == 3)
                {
                    if (args[1] == "=")
                    {
                        Script.var[args[0].Remove(0, 1)].data = args[2];
                    }
                    else if (args[1] == "===")
                    {
                        //
                    }
                }
                else
                {
//
                }
            }
        }

        void autoload(kozitScript Script, List<string> args)
        {

            string temp = "";

            Script.Parse($"import {temp}");

        }

        void debug(kozitScript Script, List<string> args)
        {
            if (args[1] == "on")
            {
                Globals.Debug = true;
            }
            else if (args[1] == "off")
            {
                Globals.Debug = false;
            }
            else
            {
                Globals.Debug = !Globals.Debug;
            }
        }

        void dummy(kozitScript Script, List<string> args)
        {
            Script.Parse($"print unknow command :{args[0]}:;");
        }

        void endfunc(kozitScript Script, List<string> args)
        {
            if (Globals.Debug)
                Console.WriteLine( ":endfunc:" + Script.CommandData["funcC"]);
            try
            {
                Script.ScriptLine = (int)Script.CommandData["funcLine" + Script.CommandData["funcC"]];
                Script.CommandData.Remove("funcLine" + Script.CommandData["funcC"]);
                Script.CommandData["funcC"] = (int)Script.CommandData["funcC"] - 1;
            }
            catch
            { }
            }

        void func(kozitScript Script, List<string> args)
        {

            if (args[0] == "func")
            {
                if (Globals.Debug)
                    Console.WriteLine("go in to the next endfunc");

                for (; Script.ScriptLine < Script.Script.Length; Script.ScriptLine++)
                {
                    if (Script.Script[Script.ScriptLine] == "endfunc")
                    {

                        if (Globals.Debug)
                            Console.WriteLine("go in to the next endfunc");
                        return;
                    }
                }

            }


            if (!Script.CommandData.ContainsKey("funcC"))
            {
                Script.CommandData.Add("funcC", 1);
            }
            else
            {
                Script.CommandData["funcC"] = (int)Script.CommandData["funcC"] + 1 ;
            }

            if (!Script.CommandData.ContainsKey("funcLine" + Script.CommandData["funcC"] ))
            {
                Script.CommandData.Add("funcLine" + Script.CommandData["funcC"], Script.ScriptLine);
            }
            string l = args[0].Remove(0, 1);
            for (Script.ScriptLine = 0; Script.ScriptLine < Script.Script.Length; Script.ScriptLine++)
            {
                if (Script.Script[Script.ScriptLine] == "func" + l)
                {
                    return;
                }
            }
        }

        void IF(kozitScript Script, List<string> args)
        {

            if (!Script.CommandData.ContainsKey("isElse"))
            {
                Script.CommandData.Add("isElse", false);
            }

            if (args.Count != 4)
            { throw new Exception(); }

            bool istrue = false;

            if (args[2] == "==")
            {
                if (args[1] == args[3])
                {
                    istrue = true;
                }
            }
            else if (args[2] == "!=")
            {
                if (args[1] != args[3])
                {
                    istrue = true;
                }
            }

            if (!istrue)
            {
                // need to fix
                for (; Script.ScriptLine < Script.Script.Length; Script.ScriptLine++)
                {
                    if (Script.Script[Script.ScriptLine] == "else")
                    {
                        ELSE(Script, args);
                        
                        break;
                    }
                    else if (Script.Script[Script.ScriptLine] == "elseif")
                    {
                        IF(Script, args);
                        
                        break;
                    }
                    else if (Script.Script[Script.ScriptLine] == "endif")
                    {
                        
                        break;
                    }
                }

            }
            else
            {
                Script.CommandData["isElse"] = false;
                
            }

        }

        void ELSE(kozitScript Script, List<string> args)
        {
            if ((bool)Script.CommandData["isElse"] == false)
            {
                
                Script.CommandData["isElse"] = true;
                return;
            }
            for (; Script.ScriptLine < Script.Script.Length; Script.ScriptLine++)
            {
                if (Script.Script[Script.ScriptLine] == "elseif")
                {
                    IF(Script, args);
                    break;
                }
                else if (Script.Script[Script.ScriptLine] == "endif")
                {
                    Script.ScriptLine++;
                    break;
                }
            }

        }

        void run(kozitScript Script, List<string> args)
        {
            if (File.Exists(args[1]))
            {
                Script.Parse(File.ReadAllText(args[1]));
            }
            else if (File.Exists(args[1] + ".ks"))
            {
                Script.Parse(File.ReadAllText(args[1] + ".ks"));
            }
            else
            {
                Script.Parse("print File not found;");
            }
        }

        void libraries(kozitScript Script, List<string> args)
        {

            Script.Parse("print loaded Libraries:");
            foreach (ILib Item in Lib.LibManger.LoadedLibraries)
            {
                Script.Parse($"print -{Item.Name}");
            }

        }


        void info(kozitScript Script, List<string> args)
        {
            //Script.Parse($"print kozitScript v{ Globals.version};print OS:{OS.getOS()}");
        }


        void rs(kozitScript Script, List<string> args)
        {
            Console.Title = $"kozitScript v{ Globals.version}";
            Script.Parse($"cls;print kozitScript v{ Globals.version}");
        }


        void cls(kozitScript Script, List<string> args)
        {
            Console.Clear();
        }


        void Print(kozitScript Script, List<string> args)
        {
            int i = 0;
            foreach (string Item in args)
            {
                if (i == 0 && Item == "print")
                { i = 1; }
                else
                {

                    Console.Write(Item + " ");

                }
                i = 1;
            }

            Console.WriteLine();
        }

        void help(kozitScript Script, List<string> args)
        {
            string c = "";
            foreach (Command Item in Globals.Commands)
            {
                if (Item.showinhelp)
                    c += $"print $> {Item.Name} : {Item.Help};";
            }
            Script.Parse(c);
        }


    }

}

