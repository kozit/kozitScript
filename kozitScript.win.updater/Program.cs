using System;

using System.Net;


namespace kozitScript.win.updater
{
    class Program
    {
        static WebClient WC = new WebClient();
        static void Main(string[] args)
        {
            WC.DownloadFileAsync(new Uri("http://pnksky.xyz/kozitScript/new.zip"), "/temp.zip");
            WC.DownloadProgressChanged += Update;
        }

        static void Update(object sender, DownloadProgressChangedEventArgs e)
        {
            Console.Clear();
            Console.WriteLine(e.BytesReceived.ToString() + "/" + e.TotalBytesToReceive.ToString());
        }

    }
}
