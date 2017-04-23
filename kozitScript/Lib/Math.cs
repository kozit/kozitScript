using s = System;
using System.Collections.Generic;
using System.Text;

namespace kozitScript.Lib
{
    public class Math : API
    {
        public override string Name { get { return "Math"; } }

        public override object Interrupt(byte Init, Dictionary<string, object> MEM)
        {
            switch (Init)
            {
                //Parse
                case 0:
                    return int.Parse((string)MEM["*1"]);
                                                                                                                  
            }
            return "";
        }
    }
}
