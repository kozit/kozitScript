using System;
using System.Collections.Generic;
using System.Text;

namespace kozitScript.Lib
{
    class System : API
    {
        public override string Name { get { return "System"; } }

        public override object Interrupt(byte Init, Dictionary<string, object> MEM)
        {
            switch (Init)
            {
                case 0:
                    Console.Write(MEM["*1"]);
                    break;
                case 1:
                    if (!(bool)MEM["*1"])
                    {
                        return Console.ReadLine();
                    }
                    else
                    {
                        return Console.ReadKey();
                    }
            }
            return "";
        }
    }
}
