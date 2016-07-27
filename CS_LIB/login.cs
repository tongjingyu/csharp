using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Data.OleDb;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Web;
using System.Net;
namespace 奇奇邮件助手
{
    class login
    {

        public static bool Login(string Name,string Password)
        {
            int TryCount = 5;
            DataTable DT;
            if (Value.App_Run)
            {
                while (TryCount-- > 0)
                {
                    try
                    {
                        DT = MySql.GetDataBase(DataBase.GetLoginInfor(Name, Password));
                    }
                    catch { MessageBox.Show("网络错误!", "登陆失败"); return false; }
                    try
                    {
                        Value.LoginUserName = DT.Rows[0][0].ToString();
                        Value.UserCreateDate = DateTime.Parse(DT.Rows[0][1].ToString());
                        Value.UserBeUserCount = int.Parse(DT.Rows[0][2].ToString());
                        MessageBox.Show("欢迎 " + Value.LoginUserName, "登陆成功");
                        return true;
                    }catch{MessageBox.Show("用户名或密码错误!", "登陆失败"); return false;}
                }
                return false;
            }
            return false;
        }
        public static void LoginThread()
        {
            while (Value.App_Run)
            {
                while (Value.App_Run) { Thread.Sleep(10); if (Value.LinkStatus == Value.LinkOk)break; }
                if (Ini.Read("记住") == "是")if(Value.App_Run)
                {
                    Value.LoginEmail = Ini.Read("用户名");
                    Value.LoginPassWord = MyEncrypt.DecryptDES(Ini.Read("用户密码"));
                    if (Login(Value.LoginEmail, Value.LoginPassWord))
                    {
                        Value.LoginOK = true;
                        return;
                    }

                }
            }
        }
        public static void LoadStart()
        {
            Thread myThread1 = new Thread(new ThreadStart(MySql.LoginThread));
            myThread1.Start();
            //Thread myThread2 = new Thread(new ThreadStart(LoginThread));
            //myThread2.Start();
        }
        public static void LoadWebPage()
        {
            //try
            //{
            //    WebClient MyWebClient = new WebClient();
            //    MyWebClient.Credentials = CredentialCache.DefaultCredentials;//获取或设置用于向Internet资源的请求进行身份验证的网络凭据
            //    Byte[] pageData = MyWebClient.DownloadData("http://www.baidu.com/baidu?wd=ip&tn=cnopera&ie=gb2312"); //从指定网站下载数据
            //    string pageHtml = Encoding.Default.GetString(pageData);  //如果获取网站页面采用的是GB2312，则使用这句            
            //    MessageBox.Show(pageHtml);
            //}
            //catch (WebException webEx) { MessageBox.Show(webEx.Message); }
            WebBrowser web = new WebBrowser(); 
            web.Navigate("http://www.xjflcp.com/ssc/"); 
            web.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(web_DocumentCompleted);
        }
        
       public static void web_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            WebBrowser web = (WebBrowser)sender;
            HtmlElementCollection ElementCollection = web.Document.GetElementsByTagName("Table");
            MessageBox.Show(ElementCollection.ToString());
        }
    }
   public class LoginReg
    {
        public string Msg;
        public  LoginReg(string Msg)
        {
            this.Msg = Msg;
        }
        public void Func()
        {
            MessageBox.Show(Msg);
        }
    }
}
