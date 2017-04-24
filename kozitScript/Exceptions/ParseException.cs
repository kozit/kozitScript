using System;
using System.Collections.Generic;
using System.Text;

namespace kozitScript.Exceptions
{
    class ParseException : Exception
    {

        public ParseException(string Func, int Line, string Exception) : base(Func + ":" + Line + ":" + Exception)
        { }

    }
}
