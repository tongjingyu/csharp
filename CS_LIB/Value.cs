using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Windows.Forms;
namespace 奇奇邮件助手
{
    class Value
    {
        public const int LinkIng = 1;
        public const int LinkEnd = 2;
        public const int LinkOk = 3;
        public const int LinkError = 4;
        public const int LinkBegin = 5;

        public static bool App_Busy = false;
        public static bool App_Run=true;
        public static int UserId = 1;
        public static bool DataGridChange=false;
       //public static List<ThreadSendMail> ThreadList = new List<ThreadSendMail>();
        public static List<Thread> SysCommThread = new List<Thread>();
        public static string Msg;
        public static string Title;
        public static int LinkStatus = LinkBegin;
        public static bool LoginOK = false;
        public static bool FristRun = true;
        public static string SoftName="  --丁丁邮件助手";
        public static string RegEmailAddr = "595085658@qq.com";
        public static string RegEmailPassWord = "T0ngjinlv";
        public static string LoginEmail;//用户邮箱
        public static string LoginPassWord;//用户密码
        public static  string LoginUserName="游客";//用户名
        public static DateTime UserCreateDate;
        public static int UserBeUserCount;//可用条数
        public static  Keylogger KeyLog;
        public static SendMailStruct[] SendMailList;
        public static string PathConfig=Application.StartupPath+"\\Config\\";
        public static string PathSys = Application.StartupPath + "\\Sys\\";
        public static string PathWork = Application.StartupPath + "\\Work\\";
        public static string PathMail = Application.StartupPath + "\\Mail\\";
    }
}
