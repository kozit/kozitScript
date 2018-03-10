#use System.Invoke

class EZlib
{

	  $Init = null;
	  $Protection = null;

        _Init()
		{

		Invoke.Load("EzLib.dll");
		Invoke.Make(new Initialize, null, $Init);
		Invoke.Make(nwe Protection, null, $Protection);

		Invoke.Run($Protection, "StartAntiDebug", null);
		Invoke.Run($Protection, "AntiDump", typeof|ME);

		}

		_Bye()
		{

		Raw(^^jumpEnd^^);

		}

	    function Init(PROGRAM_ID, PROGRAM_TOKEN)
		{
		    Invoke.Run($Init ,"Init", LIST:*PROGRAM_ID,*PROGRAM_TOKEN);
		}

}
