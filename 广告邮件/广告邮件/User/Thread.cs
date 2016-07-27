using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace 广告邮件
{
    public struct ThreadMailInfor
    {
        public MailInfor MI;
        public int FailCount;
        public int SucceedCount;
        public int SendCount;
        public int Cycle;
        public int Index;
        public string Msg;
    }
    public delegate void CallBackDelegate(ThreadMailInfor TMI,int i);

    class ThreadSendMail
    {
        public  ThreadMailInfor TheadTMI;
        public void ThreadFun()
        {
            System.Threading.Thread.Sleep(TheadTMI.Index*300);
            while (Value.App_Run)
            {
                Random re = new Random();
                TheadTMI.MI.Msg = Value.Msg + "<br><br>发送时间:" + DateTime.Now.ToString();
                TheadTMI.MI.Title = Value.Title+re.Next(100).ToString();
                TheadTMI.MI.MailTo = re.Next().ToString() + "@qq.com";
                try
                {
                    Mail.SendMail(TheadTMI.MI);
                    TheadTMI.SucceedCount++;
                    TheadTMI.Msg = "发送成功";
                }
                catch (Exception E) { TheadTMI.Msg = E.Message; TheadTMI.FailCount++; }
                TheadTMI.SendCount++;
                int i = 100;
                while (i-- > 0)
                {
                    System.Threading.Thread.Sleep(100);
                    
                    if (!Value.App_Run) return;
                }
            }
        }
    }
    
}
