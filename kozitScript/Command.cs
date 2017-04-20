using System;
using System.Collections.Generic;
using System.Text;

namespace kozitScript
{
    public abstract class Command
    {

        public abstract string Name { get; }
        public abstract void execute(Script Script, List<string> args);

    }
}
