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
namespace 广告邮件
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }
        private MySqlDataAdapter MysqlAdapter;
        private MySqlDataAdapter MysqlAdapter1;
        private DataSet DataBuf = new DataSet();
        public void LoadStart()
        {
            Value.Msg = File.ReadText(Application.StartupPath + "/Msg.htm");
            Value.Title = File.ReadText(Application.StartupPath + "/Title.htm");
            Thread myThread = new Thread(new ThreadStart(login.LoginThread));
            myThread.IsBackground = true;
            myThread.Start();
        }
        private void Main_Load(object sender, EventArgs e)
        {
            Tools.SetDoubleBuf(dataGridView1, true);
            Tools.SetDoubleBuf(dataGridView2, true);
            Thread myThread = new Thread(new ThreadStart(LoadStart));
            myThread.Start();
            SendMailBingding(Excel.OpenCSVFile(Application.StartupPath + "/Send.CVS"));
            ReMailBingding(Excel.OpenCSVFile(Application.StartupPath + "/Receive.CVS"));
        }

        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            Value.App_Run = false;
        }
        private void SendMailBingding(DataTable DT)
        {
            bindingSource1.DataSource = DT;
            dataGridView1.DataSource = bindingSource1;
            Tools.SetHeader(dataGridView1);
            Tools.Grid_OutoSize(dataGridView1);
        }
        private void ReMailBingding(DataTable DT)
        {
            bindingSource2.DataSource = DT;
            dataGridView2.DataSource = bindingSource2;
            Tools.SetHeader(dataGridView2);
            Tools.Grid_OutoSize(dataGridView2);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            UpDateStatus("正在连接..",100, 1);
            MySql.TryOpen();
            MysqlAdapter = new MySqlDataAdapter(DataBase.GetSendMailString(), MySql.Connect);
            MySqlCommandBuilder sb1 = new MySqlCommandBuilder(MysqlAdapter);
            Tools.ClearTable(DataBuf.Tables["Send"]);
            MysqlAdapter.Fill(DataBuf,"Send");
            SendMailBingding(DataBuf.Tables["Send"]);

            MysqlAdapter1 = new MySqlDataAdapter(DataBase.GetReceiveMailString(), MySql.Connect);
            MySqlCommandBuilder sb2 = new MySqlCommandBuilder(MysqlAdapter1);
            Tools.ClearTable(DataBuf.Tables["Receive"]);
            MysqlAdapter1.Fill(DataBuf, "Receive");
            ReMailBingding(DataBuf.Tables["Receive"]);
            UpDateStatus("加载完成",100, 100);
            button4.Enabled = true;
            MySql.TryClose();
        }
       
        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            Value.App_Run = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                MysqlAdapter.Update(DataBuf, "Send");
                MysqlAdapter1.Update(DataBuf, "Receive");
                Excel.SaveCSVFile(DataBuf.Tables["Send"], Application.StartupPath + "/Send.CVS");
                Excel.SaveCSVFile(DataBuf.Tables["Receive"], Application.StartupPath + "/Receive.CVS");
                UpDateStatus("保存数据",100, 1);
            }
            catch (Exception E)
            {
                MessageBox.Show(E.Message, "警告!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                UpDateStatus("保存失败",100, 0);
                return;
            }
            UpDateStatus("保存完成",100, 100);
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView1[e.ColumnIndex, e.RowIndex].Style.BackColor = Color.LightYellow;
            Value.DataGridChange = true;
            dataGridView1[1, e.RowIndex].Value = Value.UserId;
        }
        private void UpDateStatus(string Msg,int Max,int Val)
        {
            toolStripLabel6.Text = Msg;
            if (Val >= Max) Max = Val;
            toolStripProgressBar3.Maximum = Max;
            toolStripProgressBar3.Value = Val;
            timer1.Enabled=true;
            toolStripLabel7.Text = Val.ToString() + "%";
            if (Msg.IndexOf("失败") != -1) toolStripLabel6.ForeColor = Color.Red;
            else toolStripLabel6.ForeColor = Color.Green;
            Application.DoEvents();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MySql.CreateSqlConnet();
            UpDateStatus("正在连接..",100, 1);
            if (MySql.TryOpen())
            {
                UpDateStatus("连接成功", 100,100);
                button1.Enabled = true;
                button2.Enabled = true;
                button4.Enabled = true;
                button5.Enabled = true;
                button1_Click(null, null);
                this.Text = "用户ID:" + Value.UserId.ToString();
                button3.Enabled = true;
            }
            else
            {
                UpDateStatus("连接失败",100, 0);
                button1.Enabled = false;
                button2.Enabled = false;
                button4.Enabled = false;
                button5.Enabled = false;

            }
        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.S && e.Control)
            {
                button2_Click(null, null);    
            } 
        }

        private void dataGridView2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.L && e.Control) button1_Click(null, null);
            if (e.KeyCode == Keys.S && e.Control) button2_Click(null, null);
            
        }

        private void dataGridView2_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView2[e.ColumnIndex, e.RowIndex].Style.BackColor = Color.Gold;
            dataGridView2[1, e.RowIndex].Value = Value.UserId;
            dataGridView2[4, e.RowIndex].Value = 0;
        }

        private void dataGridView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Text = "Send";
                contextMenuStrip1.Show(new Point(MousePosition.X, MousePosition.Y));//cMSFullSelect is ContextMenuStrip
            }
        }

        private void dataGridView2_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Text = "Receive";
                contextMenuStrip1.Show(new Point(MousePosition.X, MousePosition.Y));//cMSFullSelect is ContextMenuStrip
            }
        }

        private void 导出ExcelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpDateStatus("正在导出",100, 1);
            try
            {
                Excel.SaveCSV(DataBuf.Tables[contextMenuStrip1.Text]);
            }
            catch { UpDateStatus("导出失败",100, 0); return; }
            UpDateStatus("导出成功",100, 100);
        }
        private void ResultCallBack(ThreadMailInfor TMI,int i)
        {
  
        }

        private void button4_Click(object sender, EventArgs e)
        {
            UpDateStatus("正在准备...", 100,1);
            for (int i = 0; i<Value.SysCommThread.Count; i++) Value.SysCommThread[i].Abort();
            Value.ThreadList.Clear();
            Value.SysCommThread.Clear();
            Value.threadSendMail = new ThreadSendMail[dataGridView1.RowCount];
            for (int i = 0; i < dataGridView1.RowCount-1; i++)
            {
                Value.threadSendMail[i] = new ThreadSendMail();
                Value.threadSendMail[i].TheadTMI.MI = new MailInfor();
                Value.threadSendMail[i].TheadTMI.MI.MailTo = null;
                Value.threadSendMail[i].TheadTMI.MI.MailFrom = dataGridView1[2, i].Value.ToString();
                Value.threadSendMail[i].TheadTMI.MI.Password = dataGridView1[3, i].Value.ToString();
                Value.threadSendMail[i].TheadTMI.Index = i;
                dataGridView1[5, i].Value = "启动完毕";
                Thread myThread = new Thread(new ThreadStart(Value.threadSendMail[i].ThreadFun));
                Value.SysCommThread.Add(myThread);
                myThread.IsBackground = true;
                myThread.Start();
                UpDateStatus("正在启动...", dataGridView1.RowCount, i);
            }
            UpDateStatus("全部启动", 100, 100);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < Value.SysCommThread.Count; i++)
            {
                dataGridView1[5, i].Value = "成功" + Value.threadSendMail[i].TheadTMI.SucceedCount.ToString() + "失败" + Value.threadSendMail[i].TheadTMI.FailCount.ToString()+"|"+Value.threadSendMail[i].TheadTMI.MI.MailTo;
            }
            if (Value.LinkOk)
            {
                toolStripLabel1.Text = "用户ID:" + Value.UserId.ToString();
            }
            else toolStripLabel1.Text = "正在登录"; 

        }

        private void 导入ExcelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            try
            {
                DataTable DT = Excel.OpenCSV();
                if (contextMenuStrip1.Text == "Send") dataGridView1.DataSource = DT;
                else dataGridView2.DataSource = DT;
                Tools.SetHeader(dataGridView2);
                Tools.Grid_OutoSize(dataGridView2);
                Tools.SetHeader(dataGridView1);
                Tools.Grid_OutoSize(dataGridView1);
            }
            catch (Exception E) { MessageBox.Show(E.Message); UpDateStatus("导入失败",100, 0); return; }
            UpDateStatus("导入成功",100, 100);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Content Form = new Content();
            Form.ShowDialog();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://trtos.com/?page_id=165"); 
        }

        private void 更新ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button1_Click(null, null);
        }

        private void 保存CtrlSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button2_Click(null, null);
        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }


    }
}
