using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
namespace SensorFace
{
    public partial class Supper_Face : Form
    {



        byte[] TxBuffer = new byte[100];
        byte[] RxBuffer = new byte[100];
        int TxBufferSize;
        MB TxModBus = new MB();
        MB RxModBus = new MB();
        public Supper_Face()
        {
            try
            {
                InitializeComponent();
            }
            catch { }
            
        }
        private void SetUpDataGridView()
        {
            Tools.DGVInit(dataGridView1);
            this.dataGridView1.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellDoubleClick);
            this.dataGridView1.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView1_CellMouseDown);
            this.dataGridView1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataGridView1_KeyDown);
            this.dataGridView1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.dataGridView1_KeyPress);
        }
        private void FillValue()
        {
            ModBusClass.ModBus_Clear(ref TxModBus);//ModBusClass.HostAddr
            ModBusClass.ModBus_Clear(ref RxModBus);
            ModBusClass.ModBus_Create(ref ModBusClass.DefMoBus, 2, byte.Parse("5"), MasterSlaveMode.WorkMode_Master, ModBusClass.CheakMode_Crc);//产生默认配置
            ModBusClass.ModBusCoppyCreate(ref ModBusClass.DefMoBus, ref TxModBus);
            ModBusClass.ModBusCoppyCreate(ref ModBusClass.DefMoBus, ref RxModBus);
            ModBusClass.ModBusCoppyCreate(ref ModBusClass.DefMoBus, ref Usart.ThreadRxModBusMsg);
            ModBusClass.ModBusCoppyCreate(ref ModBusClass.DefMoBus, ref Usart.ThreadTxModBusMsg);

        }
     
        private void datagridViewEnter(int index,bool Action)
        {
            string Msg = dataGridView1.Rows[index].Cells[0].Value.ToString();
            int Index = Msg.IndexOf(":");
            if (Index > 0)
            {
                string Data = Msg.Substring(Index + 1, Msg.Length - Index - 1);

                if (Action)
                {
                    GetInput form = new GetInput(Data, Msg);
                    DialogResult R = form.ShowDialog();
                    if (R == DialogResult.OK)
                    {
                        byte[] Data1 = (new byte[] { 0x01, (byte)index }).Concat(System.Text.Encoding.GetEncoding("GB2312").GetBytes(form.Text)).Concat(new byte[] { 0x00 }).ToArray();
                        Msg = SendReadSensor(ACFF.SCFF_COMMenuList, Data1, Data1.Length);
                        if (Msg == "NULL") { MessageBox.Show("连接失败"); return; }
                        dataGridView1.Rows[index].Cells[0].Value = index + "、" + Msg;
                       
                    }
                }
                else
                {
                    byte[] Data1 = (new byte[] { 0x01, (byte)index }).Concat(System.Text.Encoding.GetEncoding("GB2312").GetBytes(Data)).Concat(new byte[] { 0x00 }).ToArray();
                    Msg = SendReadSensor(ACFF.SCFF_COMMenuList, Data1, Data1.Length);
                    if (Msg == "NULL") { MessageBox.Show("连接失败"); return; }
                    dataGridView1.Rows[index].Cells[0].Value = index + "、" + Msg;
                }
            }
            else
            {
                if (Action)
                { 
                Msg = SendReadSensor(ACFF.SCFF_COMMenuList, new byte[] { 0x01, (byte)index }, 2);
                if (Msg == "NULL") { MessageBox.Show("连接失败"); return; }
                MessageBox.Show(Msg,"响应");
                }
            }
        }
        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            string Text = "";
            if (e.RowIndex < 0) return;
            Application.DoEvents();
            datagridViewEnter(e.RowIndex,true);
            if (dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().IndexOf("保存") > 0)
            {
                for (int i = 0; i < (dataGridView1.Rows.Count - 1); i++)
                {
                    Text += dataGridView1.Rows[i].Cells[0].Value.ToString();
                }
                Value.queue.Enqueue(DateTime.Now.ToString()+"保存"+ ":" + Text);
            }
            Application.DoEvents();
        }
        private bool dataGridViewGet(int index)
        {
            string Msg = SendReadSensor(ACFF.SCFF_COMMenuList, new byte[] { 0x02, (byte)index }, 2);
            if (Msg.IndexOf("ERROR") >= 0) return false;
            if (dataGridView1.Rows.Count < (index + 2))
            {
                if (Msg == "NULL") return false;
                dataGridView1.Rows.Add();
            }
            if (Msg == "NULL") return false;
            if (index >= 0) this.dataGridView1.Rows[index].Cells[0].Value =index+"、"+ Msg;
            Application.DoEvents();
            return true;
        }
        private void dataGridView1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if(e.Button==MouseButtons.Right)
            {
                frmMain_Load(null, null);
                contextMenuStrip1.Show(MousePosition);
                return;
            }
            int i = 0;
            if (e.RowIndex < 0)
            {
                while (dataGridView1.RowCount > 1) dataGridView1.Rows.RemoveAt(0);
                while (dataGridViewGet(i++)) ;
                return;
            }
            dataGridViewGet(e.RowIndex);
        }

        private void dataGridView1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                if (this.dataGridView1.CurrentRow.Index != this.dataGridView1.Rows.Count)
                {
                    this.dataGridView1.CurrentCell = this.dataGridView1.Rows[this.dataGridView1.CurrentRow.Index - 1].Cells[0];
                    datagridViewEnter(dataGridView1.SelectedRows[0].Cells[0].RowIndex,true);
                }
            }

        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Down || e.KeyCode == Keys.Up)
            {
                dataGridViewGet(dataGridView1.SelectedRows[0].Cells[0].RowIndex);
               if(e.KeyCode == Keys.Down)if (dataGridView1.SelectedRows[0].Cells[0].Value == null)
                {
                    dataGridView1.CurrentCell = dataGridView1.Rows[0].Cells[0]; 
                }
            }
            if ((e.KeyCode == Keys.F5) || (e.KeyCode == Keys.R)) toolStripMenuItem1_Click(null, null);
            if (e.KeyCode == Keys.W) 写入配置ToolStripMenuItem_Click(null, null);
            if (e.KeyCode == Keys.L) 加载配置ToolStripMenuItem_Click(null, null);
            if (e.KeyCode == Keys.S) 导出配置ToolStripMenuItem_Click(null, null);
            if (e.KeyCode == Keys.O) 更多ToolStripMenuItem_Click(null, null);
            if(e.KeyCode==Keys.T)同步时间ToolStripMenuItem_Click(null,null);
            if(e.KeyCode==Keys.P)打印二维码ToolStripMenuItem_Click(null,null);
            if (e.KeyCode == Keys.C) 设置串口ToolStripMenuItem_Click(null, null);
            if (e.KeyCode == Keys.U) 更新固件ToolStripMenuItem_Click(null, null);
            if (e.KeyCode == Keys.H) 帮助ToolStripMenuItem_Click(null, null);
        }

          private string SendReadSensor(ACFF Mode,byte[] Data,int Length)
        {
            try { 
            Random ra = new Random();
            FillValue();
            TxModBus.MsgFlag = (byte)Mode;
            TxModBus.DataFlag = 0x01;
            TxModBus.DataLength = Length;
            TxModBus.MsgLength = Length+3;
            int TxLength = ModBusClass.ModBus_CreateMsg(ref TxBuffer, ref TxModBus, (int)Mode, ra.Next(0, 255), 0x91, Data,Length);
            TxBufferSize = TxLength;
            RxBuffer = new byte[512];
                int RxLength = Usart.SendData(Value.serialPort1, TxBuffer, TxLength, ref RxBuffer, 100);
            ModBusClass.ModBus_Expend(RxBuffer, RxLength, ref RxModBus);
                if (RxModBus.ErrorFlag != ModBusClass.ModBus_Ok)return "ERROR";
            return Tools.GetStringFromByte(RxModBus.Data);
            }
            catch { try { Value.serialPort1.Open(); } catch { }; return "NULL"; }
        }
        private void UpLoad()
          {
              try
              {
                  Process p = new Process();
                  p.StartInfo.UseShellExecute = false;
                  p.StartInfo.FileName = @"上传程序.exe";
                  p.StartInfo.CreateNoWindow =true;
                  p.Start();
              }
              catch { }
          }
        private void frmMain_Load(object sender, EventArgs e)
        {
            string COM=Ini.Read("COM");
            List<string> list = new List<string>();
            string[] SPNames = SerialPort.GetPortNames();
            for (int i = 0; i < SPNames.Length; i++)
            {
                list.Add(SPNames[i]);
            }
            ToolStripMenuItem myItem = new ToolStripMenuItem();
            myItem.Text = "选择串口";
            while (设置串口ToolStripMenuItem.DropDownItems.Count > 0) 设置串口ToolStripMenuItem.DropDownItems.RemoveAt(0);
            foreach (string item in list)
            {
                ToolStripMenuItem mi = new ToolStripMenuItem(item);
                mi.Text = item;
                if (item == COM) mi.Image = Properties.Resources.Image1;
                mi.Click += new EventHandler(CustomItem_Click);
                设置串口ToolStripMenuItem.DropDownItems.Add(mi);
            }
        }

        private void CustomItem_Click(object sender, EventArgs e)
        {
            Ini.Write("COM", ((ToolStripMenuItem)sender).Text);
            Tools.ResetCom();
        }

        private void Supper_Face_Load(object sender, EventArgs e)
          {
            Value.App_Run = true;
            Value.serialPort1 = new SerialPort();
              SetUpDataGridView();
              try
              {
                  Value.serialPort1.PortName = Ini.Read("COM");
              }
              catch { (new SetPort()).ShowDialog(); }
             Tools.ResetCom();
              (new Thread(MySql.LoginThread)).Start();
             (new Thread(UpLoad)).Start();
            try
            {
                this.Width = int.Parse(Ini.Read("窗口宽度"));
                this.Height = int.Parse(Ini.Read("窗口高度"));
            }
            catch { }
          }

        private void 更多ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            try
            {
                Value.serialPort1.Close();
            }catch{ }
            Sensor form = new Sensor();
            form.ShowDialog();
            this.Show();
            try
            {
                Value.serialPort1.PortName = Ini.Read("COM");
                Value.serialPort1.Open();
            }
            catch { }
        }
        ToolStripMenuItem AddContextMenu(string text, ToolStripItemCollection cms, EventHandler callback)
        {
            if (text == "-")
            {
                ToolStripSeparator tsp = new ToolStripSeparator();
                cms.Add(tsp);
                return null;
            }
            else if (!string.IsNullOrEmpty(text))
            {
                ToolStripMenuItem tsmi = new ToolStripMenuItem(text);
                tsmi.Tag = text + "TAG";
                if (callback != null) tsmi.Click += callback;
                cms.Add(tsmi);

                return tsmi;
            }

            return null;
        }
      
        private void 设置串口ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetPort from = new SetPort();
            from.ShowDialog();
            Tools.ResetCom();
        }

        private void 导出配置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Tools.CreateCvsFromDGV(dataGridView1);

        }

        private void 写入配置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for(int i=0;i<dataGridView1.RowCount-1;i++)
            {
                dataGridView1.CurrentCell = dataGridView1.Rows[i].Cells[0];
                datagridViewEnter(i, false);
                Application.DoEvents();
            }
        }

        private void 加载配置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Tools.CreateCvsToDGV(dataGridView1);
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            int i = 0;
            while (dataGridView1.RowCount > 1) dataGridView1.Rows.RemoveAt(0);
            while (dataGridViewGet(i++)) ;
        }

        private void 帮助ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (new Help()).Show();
        }

        private void Supper_Face_DragDrop(object sender, DragEventArgs e)
        {
            string path = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            MessageBox.Show(path);  
        }

        private void dataGridView1_DragDrop(object sender, DragEventArgs e)
        {
            string path = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            Tools.CreateCvsToDGV(dataGridView1, path);
            写入配置ToolStripMenuItem_Click(null, null);
        }

        private void dataGridView1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Link; 
            else e.Effect = DragDropEffects.None;
        }
  
        private void Supper_Face_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                Value.App_Run = false;
                Application.Exit();
                Application.ExitThread();
                Value.serialPort1.Close();
                Process p = new Process();
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.FileName = @"更新程序.exe";
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.Arguments = "http://www.trtos.com/file/";
                p.Start();
            }
            catch { }
        }

        private void 同步时间ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            byte[] TData = Tools.DateTimeToBytes(DateTime.Now);
            string Msg = SendReadSensor(ACFF.SCFF_SetDateTime, TData, TData.Length);
            MessageBox.Show(Msg,"响应");
        }
     
        private void 打印二维码ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string Num = Tools.GetOnlyID(dataGridView1);
            
                二维码 form = new 二维码(Num);
                form.ShowDialog();
            }
            catch (Exception E) { MessageBox.Show(E.Message); }
        }

        private void 更新固件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            { Value.serialPort1.Close(); }
            catch { }
            this.Hide();
            Download form = new Download();
            form.ShowDialog();
            this.Show();
            Tools.ResetCom();
        }

        private void Supper_Face_FormClosing(object sender, FormClosingEventArgs e)
        {
            Ini.Write("窗口宽度", this.Width.ToString());
            Ini.Write("窗口高度", this.Height.ToString());
        }

        private void 关于ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About from = new About();
            from.ShowDialog();
        }
    }
}
