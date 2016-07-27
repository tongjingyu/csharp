using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace 串口服务
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread ServiceThread = new Thread(new ThreadStart(MainThread.Serial1Server));
            ServiceThread.Start();
        }
    }
}
