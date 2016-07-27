using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;      
using System.Threading;
using NS_DataSave;
namespace 奇奇邮件助手
{
    public partial class qq群成员导出 : Form
    {
        public qq群成员导出()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox1.Text=Clipboard.GetText().ToString();
        }
        private void button2_Click(object sender, EventArgs e)
        {

            string Msg = richTextBox1.Text;
            string[] Mm = Msg.Split('\n');
            string[] dd=new string[9000];
            int Index=0;
            StringBuilder SB=new StringBuilder();
            for(int i=0;i<Mm.Length;i++)
            {
                bool Ok = true;
                if (Mm[i].Length <10)Ok=false ;
                if(Mm[i].IndexOf('(')<0)Ok=false;
                if (Mm[i].IndexOf(')')<0) Ok = false;
                if(Ok)dd[Index++] = Mm[i];
            }
            for (int i = 0; i < Index; i++)
            {
                dataGridView1.Rows.Add();
                StringBuilder Name = new StringBuilder();
                bool NameTrue = true;
                bool QQTrue=false;
                StringBuilder QQ = new StringBuilder();
                for (int n = 0; n < dd[i].Length; n++)
                {
                    if (dd[i][n] == '(') NameTrue = false;
                    if (NameTrue) Name.Append(dd[i][n]);
                    
                    if (dd[i][n] == ')') QQTrue = false;
                    if (QQTrue) QQ.Append(dd[i][n]);
                    if (dd[i][n] == '(') QQTrue = true;
                }
                dataGridView1[0, i].Value = Name;
                dataGridView1[1, i].Value = QQ+"@qq.com";
                Application.DoEvents();
                button2.Text="完成 "+i*100/Index+"%"; 
            }
            button2.Text = "提取分析";
            MessageBox.Show("共收集成员【" + dataGridView1.RowCount.ToString() + "】");
        }

        private void qq群成员导出_Load(object sender, EventArgs e)
        {
            Tools.Grid_OutoSize(dataGridView1);
            Tools.SetDoubleBuf(dataGridView1, true);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SaveFileDialog Dlg = new SaveFileDialog();
            Dlg.DefaultExt = "CSV";
            Dlg.InitialDirectory = Value.PathWork;
            Dlg.Filter = "CSV文件(*.CSV)|*.CSV";
            if (Dlg.ShowDialog() == DialogResult.Cancel) return;
            DataSave.SetDataToFile(dataGridView1, Dlg.FileName);
            Ini.Write("MailPath", Dlg.FileName);
            this.Text = Dlg.FileName + Value.SoftName;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string Msg = richTextBox1.Text;
            string[] Mm = Msg.Split('\n');
           string MMSG="";
           int MinLength=5;
           try
           {
               MinLength = int.Parse(textBox1.Text);
           }
           catch (Exception E)
           {
               MessageBox.Show(E.Message);
           }
            for(int i=0;i<Mm.Length;i++)
            {
                if (Mm[i].Length > MinLength) MMSG += Mm[i]+'\n';
            }
            richTextBox1.Text = MMSG;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string Msg = richTextBox1.Text;
            string[] Mm = Msg.Split('\n');
            string MMSG = "";
            for (int i = 0; i < Mm.Length; i++)
            {
                bool Ok = false;
                for(int n=0;n<textBox2.Text.Length;n++)
                {
                   if (Mm[i].IndexOf(textBox2.Text[n])!=-1)Ok=true;
                }
               if(Ok)MMSG += Mm[i] + '\n';
            }
            richTextBox1.Text = MMSG;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string Msg = richTextBox1.Text;
            string[] Mm = Msg.Split('\n');
            string MMSG = "";
            for (int i = 0; i < Mm.Length; i++)
            {
               string Temp1=Mm[i].Replace('(', ',');
               string Temp2 = Temp1.Replace(")", "@qq.com");
                MMSG += Temp2 + '\n';
            }
            richTextBox1.Text = MMSG;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            DataSave.GetDataFromTextbox(richTextBox1.Text, dataGridView1);
            MessageBox.Show("共收集成员【" + (dataGridView1.RowCount-1).ToString() + "】");
        }
    }
}
