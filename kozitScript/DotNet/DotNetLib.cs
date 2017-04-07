using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kozitScript.DotNet
{
    public class DotNetLib : Lib.ILib
    {
        public string Name
        {
            get
            {
                return "DotNet(!Cosmos)";
            }
        }

        public void RegisterLib()
        {
            Globals.Commands.Add(new Command("title",new Command.command(Title)));
        }

        static void Title(kozitScript kS, List<string> args)
        {
            Console.Title = args[1];
        }
            

    }
}
