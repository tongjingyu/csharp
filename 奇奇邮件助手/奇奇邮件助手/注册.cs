using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
namespace 奇奇邮件助手
{
    public partial class 注册 : Form
    {
        public 注册()
        {
            InitializeComponent();
        }
        Random ra = new Random();
        private string RegCode;
        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "注册")
            {
                if (textBox2.Text != textBox5.Text)
                {
                    MessageBox.Show("两次密码不一样，请再次输入!", "提示");
                    return;
                }
                RegCode = Tools.GetRandom(8);
                MailInfor MI = new MailInfor();
                MI.MailFrom = Value.RegEmailAddr;
                MI.Password = Value.RegEmailPassWord;
                MI.MailTo = textBox1.Text;
                MI.Title = "丁丁邮箱助手 邮箱验证！";
                MI.Msg = "您的验证码：" + RegCode + "</br>请填写到注册窗口验证信息栏。(仅使用本次有效)";
                Mail.SendMail(MI);
                button1.Text = "验证";
                textBox4.Enabled = true;
                MessageBox.Show("请查看邮箱！", "提示");
            }
            else if (button1.Text == "验证")
            {
                try
                {
                    if (textBox4.Text == RegCode)
                    {
                        button1.Text = "注册完成";
                        Thread myThread = new Thread(new ThreadStart(UserRegister));
                        myThread.IsBackground = true;
                        myThread.Start();
                    }
                }
                catch { MessageBox.Show("请检查输入！", "警告"); return; }
                
            }
        }
        private void UserRegister()
        {
            DataTable DT = MySql.GetDataBase(DataBase.GetRecordCount("dd_users where LoginEmail='" + textBox1.Text + "'"));
            if (int.Parse(DT.Rows[0][0].ToString()) > 0)
            {
                MessageBox.Show("该账号已经被注册！", "提示");
                return;
            }
            MySql.SqlCommand(DataBase.GetJoinRegUser(textBox1.Text, textBox3.Text, textBox2.Text));
            MessageBox.Show("注册成功，请使用"+textBox1.Text+"作为账号登陆！", "提示");
        }
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://mail" + Tools.GetHost(textBox1.Text));
        }
    }
}
