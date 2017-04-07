using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using kozitScript;

namespace kozitScript.Lib
{
    public class LibManger
    {
        
        public static List<ILib> LoadedLibraries = new List<ILib>();

        

        /// <summary>
        /// Use This For Cosmos not Init()
        /// </summary>
        public static void InitCosmos()
        {

            if (!Globals.isinit)
            {
                //RegisterLib(new Lib.IOlib());
                RegisterLib(new Lib.stdlib());
                
                //RegisterLib(new Lib.MathLib());
                Globals.RegisterCommand(new Command("import", "inports a lib", new Command.command(ImportCosmos)));
                Globals.isinit = true;
            }
        }

        public static void Init()
        {

            if (!Globals.isinit)
            {
                RegisterLib(Assembly.GetExecutingAssembly());

                Globals.Commands.Add(new Command("import", "inports a lib", new Command.command(Import)));
                Globals.isinit = true;
            }
        }

        public static void RegisterLib(ILib Lib)
        {
            foreach (ILib Item in LoadedLibraries)
            {

                if (Item == Lib)
                {
                    return;
                }

            }
            Lib.RegisterLib();
            LoadedLibraries.Add(Lib);

        }

        public static void RegisterLib(Assembly FilePath)
        {
            var instances = from t in FilePath.GetTypes()
                            where t.GetInterfaces().Contains(typeof(ILib))
                                     && t.GetConstructor(Type.EmptyTypes) != null
                            select Activator.CreateInstance(t) as ILib;

            foreach (var instance in instances)
            {
                LoadedLibraries.Add(instance);
                instance.RegisterLib();
            }
        }

        public static void ImportCosmos(kozitScript Script, List<string> args)
        {
            Script.Parse(File.ReadAllText(args[1]));
        }

        public static void Import(kozitScript Script, List<string> args)
        {
            if (File.Exists(args[1]))
            {
                if (args[1].EndsWith(".dll"))
                {

                    RegisterLib(Assembly.LoadFile(args[1]));

                }
                else if (args[1].EndsWith(".ks"))
                {

                    Script.Parse(File.ReadAllText(args[1]));

                }

                else if (args[1].EndsWith(".cs"))
                {

                }
            }
            else if (args[1].EndsWith("/"))
            {
                if (File.Exists(args[1] + "init.ks"))
                {
                    Script.Parse(File.ReadAllText(args[1] + "init.ks"));
                }
                else
                {
                    Script.Parse("print file not found;");
                }
            }
            else
            {
                Script.Parse("print file not found;");
            }
        }

    }
}
