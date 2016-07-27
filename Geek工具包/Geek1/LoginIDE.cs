using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Threading;
using System.Net.Sockets;
namespace Geek1
{
    class LoginIDE
    {
        public static DateTime GetStandardTime()
        {
            DateTime dt = new System.DateTime();
            WebClient MyWebClient = new WebClient();
            MyWebClient.Credentials = CredentialCache.DefaultCredentials;//获取或设置用于向Internet资源的请求进行身份验证的网络凭据
            Byte[] pageData = MyWebClient.DownloadData("http://www.hko.gov.hk/cgi-bin/gts/time5a.pr?a=2"); //从指定网站下载数据
            string pageHtml = Encoding.Default.GetString(pageData);  //如果获取网站页面采用的是GB2312，则使用这句            
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(pageHtml.Remove(0, 2) + "0000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }
        public static void Create()
        {
            Thread t = new Thread(Connect);
            t.Start();
        }
        public static void Connect()
        {
            int Count = 0;
            DateTime DT = new System.DateTime();
            DateTime ReDT = new System.DateTime(2016, 4, 27);
            while(Value.Run)
            {
                try { 
                DT = GetStandardTime();
                }
                catch { MessageBox.Show("测试阶段请连接网络使用本软件，\r\n以便获取使用许可!"); Value.Run = false; Application.Exit(); }
                if (DateTime.Compare(DT, ReDT)> 0)
                {
                    Count++;
                    if (Count > 2) Application.Exit();
                    MessageBox.Show("本版本已过期请停止使用！\r\n在一段时间后自动关闭！");
                }
                for (int i = 0; i<1000; i++)
                {
                    Thread.Sleep(100);
                    if (Value.Run == false) return;
                }
               
            }
        }
    }
}
