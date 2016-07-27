using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
namespace 广告邮件
{
    class Value
    {
        public static bool App_Run=true;
        public static int UserId = 1;
        public static bool DataGridChange=false;
        public static List<ThreadSendMail> ThreadList = new List<ThreadSendMail>();
        public static List<Thread> SysCommThread = new List<Thread>();
        public static ThreadSendMail[] threadSendMail;
        public static string Msg;
        public static string Title;
        public static bool LinkOk = false;
        public static bool FristRun = true;
    }
}
