using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kozitScript.Lib
{
    class stdlib : ILib
    {
        
        public string Name { get; } = "StdLib";

        public void RegisterLib()
        {
            
        }


        public class IF : Command
        {
            public override bool showinhelp { get; } = false;

            public override string Help { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
            public override string Name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

            public override void execute(kozitScript Script, List<string> args)
            {
                throw new NotImplementedException();
            }
        }

    }
}
