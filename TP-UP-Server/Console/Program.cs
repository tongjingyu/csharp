using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            TagetValue.SystemType = SoftType.ST_Console;
            Main main = new Main();
         //  WebServer main = new WebServer();
            Thread newthread = new Thread(new ThreadStart(main.MainThread));
            newthread.Start();
        }
    }
}
