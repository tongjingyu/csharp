using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Data.OleDb;
using System.IO;
using System.Diagnostics;
using System.Web;
using System.Net;
using NS_DataSave;
namespace 奇奇邮件助手
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }
        
        private delegate void SetProgressBar2ValueCallBack(int value);
        private int FromMailIndex = 0;
        private void Form1_Load(object sender, EventArgs e)
        {
            Tools.SetDoubleBuf(dataGridView1, true);
            Tools.Grid_OutoSize(dataGridView1);
            this.webBrowser1.DocumentText = string.Empty;
            this.webBrowser1.Document.ExecCommand("EditMode", false, null);
            this.webBrowser1.Document.ExecCommand("LiveResize", false, null);
            webBrowser1.Url = new Uri(Value.PathMail + Ini.Read("HtmlPath"));
            textBox1.Text = Ini.Read("MailTitle");
            fileSystemWatcher1.Path = Value.PathMail;
            textBox2.Text = Ini.Read("附件路径");
            if (Tools.GetFileSize(Value.PathWork+ Ini.Read("MailPath")) > 10000) MessageBox.Show("任务文件过大，避免卡机，建议启动后手动加载!", "提示");
            else DataSave.GetDataFromFile(Value.PathWork + Ini.Read("MailPath"), dataGridView1);
            if (Ini.Read("自动登陆") == "是") toolStripLabel2.Text = "正在登陆";
            Ini.Write("RecordPath", "Record.DLL");
            ThreadSend TS = new ThreadSend(null, 0);//加载消息
            TS.OnReceivedData += new ThreadSend.ReceivedData(Thread_OnReceivedData);
            new Thread(new ThreadStart(TS.Func)).Start();
           
        }

        void Thread_OnReceivedData(int Index, string Msg)
        {
            if (this.InvokeRequired)
            {
                MIThreadSend.ReceivedData NR = Thread_OnReceivedData;
                this.Invoke(NR, new object[2] { Index, Msg });
            }
            else
            {
                switch (Index)
                {
                    case 0:
                        string[] MsgAry=Msg.Split('|');
                        linkLabel1.Text = MsgAry[0];
                        linkLabel1.Tag = MsgAry[1];

                        ThreadSend TS1 = new ThreadSend(null, 1);//加载消息
                        TS1.OnReceivedData += new ThreadSend.ReceivedData(Thread_OnReceivedData);
                        new Thread(new ThreadStart(TS1.Func)).Start();
                        break;
                    case 1:
                        toolStripLabel2.Text = Msg;
                        if(Msg.IndexOf("成功")>-1)登陆LToolStripMenuItem.Text = "注销(&E)";
                        ThreadSend TS2 = new ThreadSend(null, 2);//加载消息
                        TS2.OnReceivedData += new ThreadSend.ReceivedData(Thread_OnReceivedData);
                        new Thread(new ThreadStart(TS2.Func)).Start();
                        break;
                    case 2:
                        button2.Enabled = true;
                        button3.Enabled = true;
                        break;
                    case 3:
                        MessageBox.Show(Msg);
                        break;
                    default: break;
                }
            }
        }
        private void 自定义CToolStripMenuItem_Click(object sender, EventArgs e)
        {
            共享 Form = new 共享();
            Form.ShowDialog();
        }

        private void 打开OToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog Dlg = new OpenFileDialog();
            Dlg.InitialDirectory = Value.PathWork;
            Dlg.DefaultExt = "CSV";
            Dlg.Filter = "CSV文件(*.CSV)|*.CSV";
            if (Dlg.ShowDialog() == DialogResult.Cancel) return;
            DataSave.GetDataFromFile(Dlg.FileName, dataGridView1);
            Ini.Write("MailPath", Path.GetFileName(Dlg.FileName));
            this.Text = Dlg.FileName + Value.SoftName+"("+Value.LoginUserName+")";
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            //File.WriteText("Title.DLL",textBox1.Text);
            Ini.Write("MailTitle", textBox1.Text);
        }

        private void 撤消UToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(Application.StartupPath + "/WizHtmlEditor/WizHtmlEditor.exe", Value.PathMail+Ini.Read("HtmlPath"));
        }

        private void button5_Click(object sender, EventArgs e)
        {
            OpenFileDialog Dlg = new OpenFileDialog();
            Dlg.DefaultExt = "*.*";
            Dlg.Filter = "*.*文件(*.*.*)|*.*.*";
            if (Dlg.ShowDialog() == DialogResult.Cancel) return;
            textBox2.Text = Dlg.FileName;
            Ini.Write("附件路径", Dlg.FileName);
        }

        private void fileSystemWatcher1_Changed(object sender, FileSystemEventArgs e)
        {
            webBrowser1.Url = new Uri(Value.PathMail+Ini.Read("HtmlPath"));
        }

        private void 打开邮件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog Dlg = new OpenFileDialog();
            Dlg.InitialDirectory = Value.PathMail;
            Dlg.DefaultExt = "Htm";
            Dlg.Filter = "Html文件(*.Htm)|*.Htm";
            if (Dlg.ShowDialog() == DialogResult.Cancel) return;
            webBrowser1.Url = new Uri(Dlg.FileName);
            Ini.Write("HtmlPath", Path.GetFileName(Dlg.FileName));
        }

        private void 保存邮件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog Dlg = new SaveFileDialog();
            Dlg.InitialDirectory = Value.PathMail;
            Dlg.DefaultExt = "Htm";
            Dlg.Filter = "Html文件(*.Htm)|*.Htm";
            if (Dlg.ShowDialog() == DialogResult.Cancel) return;
            FileMan.WritePathFileText(Dlg.FileName, webBrowser1.DocumentText.ToString());
        }

        private void 另存为AToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog Dlg = new SaveFileDialog();
            Dlg.InitialDirectory = Value.PathWork;
            Dlg.DefaultExt = "CSV";
            Dlg.Filter = "CSV文件(*.CSV)|*.CSV";
            if (Dlg.ShowDialog() == DialogResult.Cancel) return;
            DataSave.SetDataToFile(dataGridView1, Dlg.FileName);
            Ini.Write("MailPath", Path.GetFileName(Dlg.FileName));

            this.Text = Dlg.FileName + Value.SoftName + "(" + Value.LoginUserName + ")";
        }

        private void 保存SToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataSave.SetDataToFile(dataGridView1,Ini.Read("MailPath"));
        }

        private void 新建NToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog Dlg = new SaveFileDialog();
            Dlg.InitialDirectory = Value.PathWork;
            Dlg.DefaultExt = "CSV";
            Dlg.Filter = "CSV文件(*.CSV)|*.CSV";
            if (Dlg.ShowDialog() == DialogResult.Cancel) return;
            Ini.Write("MailPath", Dlg.FileName);
            Tools.DGVClear(dataGridView1);
            this.Text = Dlg.FileName + Value.SoftName + "(" + Value.LoginUserName + ")";
        }

        private void 打印PToolStripMenuItem_Click(object sender, EventArgs e)
        {
            webBrowser1.ShowPrintDialog();
        }

        private void 打印预览VToolStripMenuItem_Click(object sender, EventArgs e)
        {
            webBrowser1.ShowPrintPreviewDialog();
        }

        private void 退出XToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void 关于AToolStripMenuItem_Click(object sender, EventArgs e)
        {
            关于 Form = new 关于();
            Form.ShowDialog();
        }
        private void CallBack(string message)
        {
            //主线程报告信息,可以根据这个信息做判断操作,执行不同逻辑.
            MessageBox.Show(message);
        }
        void NewWork_OnReceivedData(int Index,string Msg)
        {
            if (this.InvokeRequired)
            {
                MIThreadSend.ReceivedData NR = NewWork_OnReceivedData;
                this.Invoke(NR, new object[2] { Index, Msg });
            }
            else
            {
                if (Msg.IndexOf("成功") > -1)
                {
                    dataGridView1.Rows[Index].DefaultCellStyle.BackColor = Color.Aquamarine;
                    System.Media.SystemSounds.Hand.Play();
                }
                else
                {
                    dataGridView1.Rows[Index].DefaultCellStyle.BackColor = Color.Red;
                    System.Media.SystemSounds.Beep.Play();
                }
               // dataGridView1.ClearSelection();
                dataGridView1.CurrentCell = dataGridView1.Rows[Index].Cells[0];
               // dataGridView1.FirstDisplayedScrollingRowIndex = Index;
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (Value.SendMailList != null)
            {
                if (FromMailIndex < (Value.SendMailList.Length-1)) FromMailIndex++;
                else FromMailIndex = 0;
                MailInfor MI = new MailInfor();
                MI.MailFrom = Value.SendMailList[FromMailIndex].Mail;
                MI.Password = Value.SendMailList[FromMailIndex].Password;
                MI.MailTo = dataGridView1.SelectedRows[0].Cells[1].Value.ToString();
                MI.Title = textBox1.Text;
                MI.Msg = webBrowser1.DocumentText.ToString();
                MI.AddPath = Ini.Read("附件路径");
                MIThreadSend MITS = new MIThreadSend(MI, dataGridView1.SelectedRows[0].Index);
                MITS.OnReceivedData += new MIThreadSend.ReceivedData(NewWork_OnReceivedData);
                new Thread(new ThreadStart(MITS.Send)).Start();
            }
        }

        private void qq群成员导出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            qq群成员导出 Form = new qq群成员导出();
            //frm_GetQQ Form = new frm_GetQQ();
            Form.ShowDialog();
        }

        private void html编辑器ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(Application.StartupPath + "/WizHtmlEditor/WizHtmlEditor.exe");
        }

        private void 版本检测UToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("iexplore.exe", "http://www.trtos.com/?page_id=165");  
        }

        private void 粘贴ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            webBrowser1.Url = null;
            this.webBrowser1.DocumentText = string.Empty;
            this.webBrowser1.Document.ExecCommand("EditMode", false, null);
            this.webBrowser1.Document.ExecCommand("LiveResize", false, null);
        }

        private void qQToolStripMenuItem_Click(object sender, EventArgs e)
        {
            QQ邮箱通信录转换 Form = new QQ邮箱通信录转换();
            Form.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
  
            if (button3.Text == "顺序发送")
            {
                timer1.Enabled = true;
                button3.Text = "停止发送";
                SendIndex = dataGridView1.SelectedCells[0].RowIndex;
            }
            else
            {
                timer1.Enabled = false;
                button3.Text = "顺序发送";
            }
        }
        int SendIndex = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {

            if (Value.SendMailList != null)
            {
                if (FromMailIndex < (Value.SendMailList.Length - 1)) FromMailIndex++;
                else FromMailIndex = 0;
                if (SendIndex > (dataGridView1.RowCount - 2)) { SendIndex = 0; timer1.Enabled = false; button3.Text = "顺序发送"; }
                toolStripProgressBar3.Maximum = dataGridView1.RowCount;
                MailInfor MI = new MailInfor();

                MI.MailFrom = Value.SendMailList[FromMailIndex].Mail;
                MI.Password = Value.SendMailList[FromMailIndex].Password;
                MI.MailTo = dataGridView1[1, SendIndex].Value.ToString();
                MI.Title = textBox1.Text;
                MI.Msg = webBrowser1.DocumentText.ToString();
                MI.AddPath = Ini.Read("附件路径");
                MIThreadSend MITS = new MIThreadSend(MI, SendIndex);
                MITS.OnReceivedData += new MIThreadSend.ReceivedData(NewWork_OnReceivedData);
                new Thread(new ThreadStart(MITS.Send)).Start();
                toolStripProgressBar3.Value = SendIndex++;
            }
                
        }

        private void 登陆LToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (登陆LToolStripMenuItem.Text.IndexOf("登陆")>-1)
            {
                登陆 Form = new 登陆();
                Form.ShowDialog();
                if (Form.Text.IndexOf("成功") > -1)
                {
                    toolStripLabel2.Text = Form.Text;
                    登陆LToolStripMenuItem.Text = "注销(&E)";
                    Value.LoginOK = true;
                }
            }
            else
            {
                Value.LoginOK = false;
                登陆LToolStripMenuItem.Text = "登陆(&E)";
                toolStripLabel2.Text = "未登录";
                Value.LoginUserName = "游客";
            }
        }

        private void 注册RToolStripMenuItem_Click(object sender, EventArgs e)
        {
            注册 Form = new 注册();
            Form.ShowDialog();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            //toolStripLabel6.Text = MySql.Connect.State.ToString();
            this.Text = Ini.Read("MailPath") + Value.SoftName + "(" + Value.LoginUserName + ")";
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            Ini.Write("附件路径", textBox2.Text);
        }
        private void dataGridView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) contextMenuStrip1.Show(MousePosition);
        }

        private void 保存ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Value.App_Busy = true;
            DataSave.SetDataToFile(dataGridView1, Ini.Read("MailPath"));
            Value.App_Busy = false;
        }

        private void 打开ToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            打开OToolStripMenuItem_Click(null, null);
        }

        private void Main_FormClosing_1(object sender, FormClosingEventArgs e)
        {
            if (Value.App_Busy)
            {
                MessageBox.Show("程序正忙,关闭会导致数据丢失！", "警告");
                e.Cancel = true;
                return;
            }
            Value.App_Run = false;
        }

        private void 意见反馈RToolStripMenuItem_Click(object sender, EventArgs e)
        {
            意见反馈 Form = new 意见反馈();
            Form.ShowDialog();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (linkLabel1.Tag != null)
            {
                string Link = linkLabel1.Tag.ToString();
                if (Link.Length > 3) System.Diagnostics.Process.Start(Link);
            }
        }

        private void webBrowser2_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            string Msg = Tools.StripHTML(webBrowser2.DocumentText.ToString());
            try
            {
                Msg = Msg.Substring(Msg.IndexOf("本机"), 24);
                ThreadSend TS = new ThreadSend(Msg, 3);//加载消息
                TS.OnReceivedData += new ThreadSend.ReceivedData(Thread_OnReceivedData);
                new Thread(new ThreadStart(TS.Func)).Start();
                webBrowser2.Url = null;
            }
            catch { }
        }
    }
}
