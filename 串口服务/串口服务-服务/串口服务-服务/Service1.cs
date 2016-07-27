using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
namespace 串口服务
{
    public partial class Service1 : ServiceBase
    {
        Thread ServiceThread;
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            ServiceThread = new Thread(new ThreadStart(MainThread.Serial1Server));
            ServiceThread.Start();
        }

        protected override void OnStop()
        {
            ServiceThread.Abort();
        }
    }
}
