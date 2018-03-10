using System;
using System.Collections.Generic;
using System.Linq;
using KsIL;

namespace kozitScript
{
    class Program
    {

        public static byte[] KsILCode = new byte[] { {{KsILCODE}} };



        static KsILVM KsIL;
        
        static void Main(string[] args)
        {

            KsIL = new KsILVM({{Memory}});

            KsIL.Load(KsILCode);

            {{RAM}}

            KsIL.AutoTick();

        }
    }
}
