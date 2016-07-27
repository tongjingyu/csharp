using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace 广告邮件
{
    public struct SendMailInfor
    {
        public int Id;
        public int UserId;
        public string Name;
        public string Demain;
        public string PassWord;
        public string Error;
        public string Note;
    }
    class ValueBag
    {

        byte[] GetBinFromSendMail(SendMailInfor SMI)
        {
            byte[] Buf=new byte[1000];
            return Buf;
        }
    }
}
