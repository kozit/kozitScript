namespace System
{
    class Invoke
    {

	  function Run(Class, Method, Constructor)
		{
		    Init(0, *Class, *Constructor, *Method);
		}

		function Run(Class, Method, Constructor)
		{
		    Init(1, *Class, *Constructor, *Method, ^^return^^);
		}

		function Run(Class, Method, Constructor)
		{
		    Init(2, *Class, *Constructor, *Method, ^^return^^);
		}

		function Load(Path)
		{

		}

    }
}
