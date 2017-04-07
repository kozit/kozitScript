
namespace kozitScriptwin
{
    class Program
    {
        static void Main(string[] args)
        {

            kozitScript.kozitScript.Init();
            kozitScript.kozitScript r = new kozitScript.kozitScript();

            while (true)
            {

                r.Parse(System.Console.ReadLine());

            }
                        
        }
    }
}
