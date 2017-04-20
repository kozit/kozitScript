
using System.Collections.Generic;

namespace kozitScript
{
   public class Globals
    {

        public class SystemINFO
        {

            public static string Name = "null";
            public static bool isx86 = true;

        }

        public static string version = "0007";//base 62 0-9a-zA-Z
        public static string version_Name = "Sydney";
        public static string version_Build = "01";//base 62 0-9a-zA-Z  add 1 to vresion when ZZ and go back to 00

        public static List<Command> Commands = new List<Command>();

        public static bool Debug = false;
        public static bool isinit = false;

        public static void RegisterCommand(Command command)
        {
            if (command != null)
            {
                Globals.Commands.Add(command);
            }
        }

        public static void overrideCommand(string Command, Command comm)
        {
            for (int i = 0; i < Commands.Count; i++)
            {
                if (Command.ToLower() == Commands[i].Name)
                {
                    Commands[i] = comm;
                    break;
                }
            }
        }
        

    }
}
