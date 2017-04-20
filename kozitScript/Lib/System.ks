Namespace System;

Func Console.Write *1 ;
 Init Core 0;
End Func;

Func Console.Read *1 ;
 Init Core 1; 
End Func;

Func Console.ReadKey *1 ;
 *2 = true;
 Init Core 1; 
End Func;

End Namespace;