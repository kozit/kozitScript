namespace System
{

    class Console
    {

        function Write(input)
		{
		    Init(1, 0, *input);
		}

		function WriteLine(input)
		{
		    Init(1, 1, *input);
		}

		function ReadLine()
		{
            Init(1, 2, ^^return^^);
		}

		function Read()
		{
            Init(1, 4, ^^return^^);
		}

		function Clear()
		{
		    Init(1, 3);
		}

    }

}
