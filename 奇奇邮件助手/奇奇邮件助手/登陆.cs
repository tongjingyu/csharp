using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace 奇奇邮件助手
{
    public partial class 登陆 : Form
    {
        public 登陆()
        {
            InitializeComponent();
        }
        public static int aaa;
        private void button1_Click(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                Ini.Write("用户名", textBox1.Text);
                Ini.Write("用户密码", MyEncrypt.EncryptDES(textBox2.Text));
            }
            Value.LoginEmail = textBox1.Text;
            Value.LoginPassWord = textBox2.Text;
            if (login.Login(textBox1.Text, textBox2.Text))
            {
                this.Text = "登陆成功";
                Value.LoginOK = true;
                this.Close();
            }
            else this.Text = "登陆失败";
        }

        private void 登陆_Load(object sender, EventArgs e)
        {
            if (Ini.Read("记住") == "是")
            {
                checkBox1.Checked = true;
                textBox1.Text = Ini.Read("用户名");
                textBox2.Text =MyEncrypt.DecryptDES(Ini.Read("用户密码"));
            }
            if (Ini.Read("自动登陆") == "是")
            {
                checkBox2.Checked = true;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked)Ini.Write("记住", "是");
            else Ini.Write("记住", "否");
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked) Ini.Write("自动登陆", "是");
            else Ini.Write("自动登陆", "否");
        }
    }
}
