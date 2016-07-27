using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Windows.Forms;

namespace 奇奇邮件助手
{
    public struct MailInfor
    {
        public string MailFrom;
        public string Password;
        public string Host;
        public string MailTo;
        public string Msg;
        public string Title;
        public string AddPath;
    }
    

    public class MIThreadSend
    {

        public MailInfor MI;
        public int Index;
        public delegate void ReceivedData(int Index, string Msg);           //创建委托  用于更新界面调试信息        
        public event ReceivedData OnReceivedData;
        public delegate void CallBackDelegate(string message);
        public MIThreadSend(MailInfor MI,int Index)
        {
            this.MI = MI;
            this.Index=Index;
        }
        public void Send()
        {

            try
            {
                MI.Host = Tools.MailGetHost(MI.MailFrom);
                MailMessage mm = new MailMessage(MI.MailFrom, MI.MailTo);
                mm.BodyEncoding = System.Text.Encoding.UTF8;
                mm.SubjectEncoding = System.Text.Encoding.UTF8;
                mm.Subject = MI.Title;
                mm.Body = MI.Msg;
                if (System.IO.File.Exists(MI.AddPath)) mm.Attachments.Add(new Attachment(MI.AddPath));
                mm.Priority = System.Net.Mail.MailPriority.High;
                mm.IsBodyHtml = true;
                SmtpClient sc = new SmtpClient(MI.Host);
                sc.Credentials = new System.Net.NetworkCredential(MI.MailFrom, MI.Password);
                sc.Send(mm);
                OnReceivedData(Index, "发送成功");
            }
            catch (Exception E)
            {
                MI.Msg = E.Message;
                OnReceivedData(Index, "发送失败");
            }
        }

    }
    class Mail
    {

        public static void SendMail(string Title, string Msg)
        {

            System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient();
            client.Host = "smtp.live.cn";//使用163的SMTP服务器发送邮件
            client.UseDefaultCredentials = true;
            client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
            client.Credentials = new System.Net.NetworkCredential("sobt@live.cn", "TRTOSabc123");//163的SMTP服务器需要用163邮箱的用户名和密码作认证，如果没有需要去163申请个, 
            //这里假定你已经拥有了一个163邮箱的账户，用户名为abc，密码为*******
            System.Net.Mail.MailMessage Message = new System.Net.Mail.MailMessage();
            Message.From = new System.Net.Mail.MailAddress("sobt@live.cn");//这里需要注意，163似乎有规定发信人的邮箱地址必须是163的，而且发信人的邮箱用户名必须和上面SMTP服务器认证时的用户名相同
            Message.To.Add("179990830@qq.com");//将邮件发送给QQ邮箱
            Message.Subject = Title;
            Message.Body = Msg;
            Message.SubjectEncoding = System.Text.Encoding.UTF8;
            Message.BodyEncoding = System.Text.Encoding.UTF8;
            Message.Priority = System.Net.Mail.MailPriority.High;
            Message.IsBodyHtml = true; 
            client.Send(Message);//因为上面用的用户名abc作SMTP服务器认证，所以这里发信人的邮箱地址也应该写为abc@163.comMessage.To.Add("123456@gmail.com");
        }
        //public static void SendMail(string Title, string Msg)
        //{
        //    //SendMail("smtp.163.com", "tongjinlv", "TONGJINLV", "@163.com", "179990830@qq.com", Title, Msg);
        //    SendMail("smtp.163.com", "tongjinlv", "TONGJINLV", "@163.com", "179990830@qq.com", Title, Msg);
        //}
        public static void SendMail(string Host,string SendName,string Password,string Demain,string Target, String Title, String Msg)
        {
            System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient();
            client.Host =Host;//使用163的SMTP服务器发送邮件
            client.UseDefaultCredentials = true;
            client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
            client.Credentials = new System.Net.NetworkCredential(SendName, Password);//163的SMTP服务器需要用163邮箱的用户名和密码作认证，如果没有需要去163申请个, 
            //这里假定你已经拥有了一个163邮箱的账户，用户名为abc，密码为*******
            System.Net.Mail.MailMessage Message = new System.Net.Mail.MailMessage();
            Message.From = new System.Net.Mail.MailAddress(SendName+Demain);//这里需要注意，163似乎有规定发信人的邮箱地址必须是163的，而且发信人的邮箱用户名必须和上面SMTP服务器认证时的用户名相同
            Message.To.Add(Target);//将邮件发送给
            Message.Subject = Title;
            Message.Body = Msg;
            Message.SubjectEncoding = System.Text.Encoding.UTF8;
            Message.BodyEncoding = System.Text.Encoding.UTF8;
            Message.Priority = System.Net.Mail.MailPriority.High;
            Message.IsBodyHtml = true; client.Send(Message);
        }

        public static bool SendMail(MailInfor MI)
        {
            try
            {
                MI.Host = Tools.MailGetHost(MI.MailFrom);
                MessageBox.Show(MI.MailFrom + MI.MailTo);
                MailMessage mm = new MailMessage(MI.MailFrom, MI.MailTo);
                mm.BodyEncoding = System.Text.Encoding.UTF8;
                mm.SubjectEncoding = System.Text.Encoding.UTF8;
                mm.Subject = MI.Title;
                mm.Body = MI.Msg;
                mm.Priority = System.Net.Mail.MailPriority.High;
                mm.IsBodyHtml = true;
                SmtpClient sc = new SmtpClient(MI.Host);
                sc.Credentials = new System.Net.NetworkCredential(MI.MailFrom, MI.Password);
                sc.Send(mm);
            }
            catch (Exception E) { MessageBox.Show(E.Message); return false;}
            return true;
        }

        public static bool SendMail(MailInfor MI,string Path)
        {
            try
            {
                MI.Host = Tools.MailGetHost(MI.MailFrom);
                MailMessage mm = new MailMessage(MI.MailFrom, MI.MailTo);
                mm.BodyEncoding = System.Text.Encoding.UTF8;
                mm.SubjectEncoding = System.Text.Encoding.UTF8;
                mm.Subject = MI.Title;
                mm.Body = MI.Msg;
                if(System.IO.File.Exists(Path))mm.Attachments.Add(new Attachment(Path));
                mm.Priority = System.Net.Mail.MailPriority.High;
                mm.IsBodyHtml = true;
                SmtpClient sc = new SmtpClient(MI.Host);
                sc.Credentials = new System.Net.NetworkCredential(MI.MailFrom, MI.Password);
                sc.Send(mm);
            }
            catch { return false; }
            return true;
        }
    }
}
