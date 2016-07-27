using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace 广告邮件
{
    class SendMail
    {
        private MailInfor MI;
        public MailInfor Addr
        {
            get { return MI; }
            set { MI = value; }
        }
        public SendMail(MailInfor Mi)
        {
            MI = Mi;
        }
        public void ThreadFun()
        {
            while (Value.App_Run)
            {
                Random re = new Random();
                MI.Msg = re.Next().ToString();
                Mail.SendMail(MI);
                int i=10;
                while (i-->0)
                {
                    System.Threading.Thread.Sleep(1000);
                    if (!Value.App_Run) return;
                }
            }
        }


    }
}
