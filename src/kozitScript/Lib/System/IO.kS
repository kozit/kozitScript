namespace System.IO
{
    class File
    {

	function Delete(path)
	{
	    Init(2, 0, *path);
    }

	function WriteAllText(path, Content)
	{
		Init(2, 1, *path, *Content);
	}

	function WriteAllBytes(path, Content)
	{
		Init(2, 2, *path, *Content);
	}

	function AppendAllText(path, Content)
	{
	    Init(2, 3, *path, *Content);
	}

	function Exists(path)
	{
	    Init(2, 7, *path);
	    if
	    {
			return 1;
		}
		else
		{
			return 0;
		}

    }

	}

	class Directory
	{

	    function Delete(path)
		{
		    Init(2, 4, *path);
		}

	    function Create(path)
		{
		    Init(2, 5, *path);
		}

		function Exists(path)
	    {
	        Init(2, 6, *path);
			if
			{
				return 1;
			}
			else
			{
				return 0;
			}
		}

	}

}
