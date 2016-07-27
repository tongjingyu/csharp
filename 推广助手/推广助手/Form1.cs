using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace 推广助手
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        int SucceedCount = 0;
        int FailCount = 0;
        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox4.Text = openFileDialog1.FileName;
            } 
        }
        private void SendMail(string Title, string Msg)
        {

            System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient();
            client.Host = "smtp.163.com";//使用163的SMTP服务器发送邮件
            client.UseDefaultCredentials = true;
            client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
            client.Credentials = new System.Net.NetworkCredential(textBox2.Text,textBox3.Text);//163的SMTP服务器需要用163邮箱的用户名和密码作认证，如果没有需要去163申请个, 
            //这里假定你已经拥有了一个163邮箱的账户，用户名为abc，密码为*******
            System.Net.Mail.MailMessage Message = new System.Net.Mail.MailMessage();
            Message.From = new System.Net.Mail.MailAddress(textBox2.Text+"@163.com");//这里需要注意，163似乎有规定发信人的邮箱地址必须是163的，而且发信人的邮箱用户名必须和上面SMTP服务器认证时的用户名相同
            Message.To.Add("admin@arm9.info");//将邮件发送给
            Message.To.Add(textBox1.Text+"@qq.com");//将邮件发送给QQ邮箱
            Message.Subject = Title;
            Message.Body = Msg;
            Message.SubjectEncoding = System.Text.Encoding.UTF8;
            Message.BodyEncoding = System.Text.Encoding.UTF8;
            Message.Priority = System.Net.Mail.MailPriority.High;
            Message.IsBodyHtml = true; client.Send(Message);//因为上面用的用户名abc作SMTP服务器认证，所以这里发信人的邮箱地址也应该写为abc@163.comMessage.To.Add("123456@gmail.com");
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            button2_Click(null, null);
            timer1.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            try
            {
                StreamReader sr = new StreamReader(textBox4.Text);
                string content = sr.ReadToEnd();
                SendMail("您本月订阅的消息已发送，请查收！", content);
                SucceedCount++; label5.Text = "发送计数:" + SucceedCount;
                if (checkBox1.Checked == true)
                {
                    int i= int.Parse(textBox1.Text);
                    i++;
                    textBox1.Text =i.ToString();
                }
            }

            catch { FailCount++; }
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                FileInfo file = new FileInfo(textBox4.Text);
                if (!file.Exists)
                {
                    MessageBox.Show("jiancha");
                    return;
                }
            }
            catch (Exception E)
            {
                MessageBox.Show(E.Message);
                return;
            }
            if (button3.Text == "开始发送")
            {
                timer1.Enabled = true;
                button3.Text = "停止发送";
            }
            else
            {
                timer1.Enabled = false;
                button3.Text = "开始发送";
            }
        }
    }
}
