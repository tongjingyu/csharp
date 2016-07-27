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
using System.Net.Sockets;
namespace 同步时间
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public static DateTime GetStandardTime()
        {
            DateTime dt=new System.DateTime();
            try
            {
                WebClient MyWebClient = new WebClient();
                MyWebClient.Credentials = CredentialCache.DefaultCredentials;//获取或设置用于向Internet资源的请求进行身份验证的网络凭据
                Byte[] pageData = MyWebClient.DownloadData("http://www.hko.gov.hk/cgi-bin/gts/time5a.pr?a=2"); //从指定网站下载数据
                string pageHtml = Encoding.Default.GetString(pageData);  //如果获取网站页面采用的是GB2312，则使用这句            
                DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                long lTime = long.Parse(pageHtml.Remove(0, 2) + "0000");
                TimeSpan toNow = new TimeSpan(lTime);
                return dtStart.Add(toNow);   
            }
            catch (Exception E) { MessageBox.Show(E.Message); }
            return dt;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DateTime DT = GetStandardTime();
            label1.Text = DT.ToString("yyyy年MM月dd日");
            if (DateTime.Compare(DT, DateTime.Now.AddMonths(-1)) > 0) MessageBox.Show("过期");
        }
    }
}
