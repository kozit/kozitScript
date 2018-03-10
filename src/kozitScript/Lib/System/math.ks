namespace System
{

    class Math
    {

		function Fac(i)
		{
		
		if(e == 0)
	    {
			return 1;
	    }

		output = 1;

		for(ii = 0; i > ii; ii++)
		{
		
		    output *= ii;

		}
		
		return output;

		}

        function Pow(b, e)
		{
			
			output = 0;

			if(e == 1)
			{
			return 0;
			}
			else if(e == 0)
			{
			return 1;
			}

			for(i = e; i > 0; i--)
			{
			
			temp = b * i;
			output += temp

			}

			return output;

		}

    }

}
