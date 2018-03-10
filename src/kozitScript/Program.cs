using System;
using System.Diagnostics;

namespace kozitScript
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch SW = new Stopwatch();
            SW.Start();

            CodeGenerator CG;

            string Path = "";
            string OutPath = "";

            if (args.Length == 1)
            {
                Path = args[0];
                OutPath = "Output.KsIL";
            }
            else
            {

                for (int i = 0; i < args.Length; i++)
                {

                    if (args[i] == "-i")
                    {
                        Path = args[i + 1];
                    }
                    else if (args[i] == "-o")
                    {
                        OutPath = args[i + 1];
                    }

                }

            }

            CG = new CodeGenerator(TreeGenerator.MakeTree(Path));


            System.IO.File.WriteAllBytes(OutPath, CG.Output.ToArray());

            SW.Stop();

            Console.WriteLine("Build Time" + SW.Elapsed);

        }
    }
}
