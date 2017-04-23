using kozitScript;
using System;

namespace Runtime
{
    class Program
    {
        static void Main(string[] args)
        {
            kozitScript.API[] Apis = new API[] {new kozitScript.Lib.Math(), new kozitScript.Lib.System() };
            KozitScriptHost ScriptHost = new KozitScriptHost(Apis);
            Console.WriteLine(System.IO.Directory.GetCurrentDirectory() + "\\Lib\\");
            Console.WriteLine(System.IO.Path.GetDirectoryName(args[0]));
            ScriptHost.MEM["System:Paths"] = new string[2] { System.IO.Directory.GetCurrentDirectory() + "\\Lib\\", System.IO.Path.GetDirectoryName(args[0]) };

            ScriptHost.RunFile("Test.ks");

            while (true)
            { }
        }
    }
}