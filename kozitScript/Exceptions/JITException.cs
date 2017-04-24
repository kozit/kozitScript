using System;
using System.Collections.Generic;
using System.Text;

namespace kozitScript.Exceptions
{
    class JITException : Exception
    {

        public JITException(string Func, int Line, string Exception) : base(Func + ":" + Line + ":" + Exception)
        {

        }

    }
}
