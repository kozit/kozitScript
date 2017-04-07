using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kozitScript.Lib
{
    public interface ILib
    {

        void RegisterLib();
        string Name { get; }

    }
}
