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
namespace Server
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

    
        protected override void OnStart(string[] args)
        {
            TagetValue.SystemType = SoftType.ST_Server;
            TagetValue.SystemLogType = LogType.LT_Error;
            Main main= new Main();
            Thread newthread = new Thread(new ThreadStart(main.MainThread));
            newthread.Start();

        }

        protected override void OnStop()
        {
            TagetValue.Run = false;
        }
    }
}
