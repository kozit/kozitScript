using System;
using System.Collections.Generic;
using System.Text;

namespace kozitScript
{
    public abstract class API
    {

        public abstract string Name { get; }

        public abstract object Interrupt(byte Init, Dictionary<string, object> MEM);

    }
}
