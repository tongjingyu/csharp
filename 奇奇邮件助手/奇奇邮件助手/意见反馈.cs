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
    public partial class 意见反馈 : Form
    {
        public 意见反馈()
        {
            InitializeComponent();

        }
        bool RRturn = false;
        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (RRturn)
            {
                MessageBox.Show("请勿重复提交!", "提示");
                return;
            }
            if (richTextBox1.Text.Length < 10)
            {
                MessageBox.Show("反馈内容长度不低于10个字符", "提醒");
                return;
            }
            if (textBox1.Text.Length < 5)
            {
                MessageBox.Show("请正确输入联系方式!", "提醒");
                return;
            }
            if (textBox2.Text.Length < 1)
            {
                MessageBox.Show("请留下大名!", "提醒");
                return;
            }
            try
            {
                MySql.SqlCommand(DataBase.GetJoinLeave(richTextBox1.Text, textBox1.Text, textBox2.Text));
                MessageBox.Show("提交成功!", "非常感谢");
                RRturn = true;
            }
            catch(Exception E) { MessageBox.Show(E.Message, "失败"); }
        }
       
    }
}
