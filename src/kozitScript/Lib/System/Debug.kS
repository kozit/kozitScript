#use System.IO 

namespace System
{
    class Debug
    {

	    $Debugger.Runnig = false;
		$Debugger.FilePath;

		function Init(LogPath)
		{
		
		    $Debugger.FilePath = *LogPath;

		}

		function Start()
		{
		
		    $Debugger.Runnig = true;

		}

		function Stop()
		{
		
		    $Debugger.Runnig = false;

		}

		function Log(Data)
		{
		
		    if($Debugger.Runnig)
			{

			    File.AppendAllText($Debugger.FilePath, *Data);
				
			}

		}
		
    }
}
