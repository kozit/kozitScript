using System;
using System.Collections.Generic;
using System.Text;

namespace kozitScript
{
    public class Script
    {

        public string[] Code;
        
        public string Path { get; }
        
        private Dictionary<string, object> MEM = new Dictionary<string, object>();

        private KozitScriptHost kozitScriptHost;

        public Script(KozitScriptHost kozitScriptHost,string Path)
        {
            Code = System.IO.File.ReadAllText(Path).Split(';');

            this.kozitScriptHost = kozitScriptHost;

            this.Path = Path;

            string Func = "";
            string NameSpace = "";
            for (int i = 0; i != Code.Length;i++)
            {

                if (Code[i].Trim(' ').StartsWith("Namespace"))
                {

                    NameSpace = Code[i].Remove(0, 9).Trim(' ');
                }


                if (Code[i].Trim(' ').StartsWith("End Namespace"))
                {

                    NameSpace = "";

                }
                
                if (Code[i].Trim(' ').StartsWith("Func"))
                {

                    Func = Code[i].Remove(0,4).Trim(' ');
                    MEM.Add("Func:" + Func + ":Start", i);
                    
                }


                if (Code[i].Trim(' ').StartsWith("End Func"))
                {

                    MEM.Add("Func:" + Func + ":End", i);
                    kozitScriptHost.MEM.Add("Func:" + NameSpace + Func, this.getFunc(Func));
                    Func = "";

                }


            }

        }

        public string[] getFunc(string Func)
        {
            List<string> r = new List<string>();
            if (MEM["Func:" + Func + ":Start"] != null)
            {
                for (int i = int.Parse(MEM["Func:" + Func + ":Start"].ToString()); i < int.Parse(MEM["Func:" + Func + ":Start"].ToString() + 1); i++)
                {

                    r.Add(Code[i]);

                }
            }
            return r.ToArray();
        }
        
    }
}
