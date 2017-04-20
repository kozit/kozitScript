using System;
using System.Collections.Generic;
using System.Text;

namespace kozitScript
{
    public abstract class API
    {

        public abstract string Name { get; }

        public abstract void Interrupt(byte Init);

    }
}
