using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
namespace SensorFace
{
    public partial class Download : Form
    {
        public Download()
        {
            InitializeComponent();
        }
        byte[] TxBuffer = new byte[3000];
        byte[] RxBuffer = new byte[3000];
        MB TxModBus = new MB();
        MB RxModBus = new MB();
        int Count = 0;
        private void FillValue()
        {
            ModBusClass.ModBus_Clear(ref TxModBus);
            ModBusClass.ModBus_Clear(ref RxModBus);
            ModBusClass.ModBus_Create(ref ModBusClass.DefMoBus, 2, byte.Parse("5"), MasterSlaveMode.WorkMode_Master, ModBusClass.CheakMode_Crc);//产生默认配置
            ModBusClass.ModBusCoppyCreate(ref ModBusClass.DefMoBus, ref TxModBus);
            ModBusClass.ModBusCoppyCreate(ref ModBusClass.DefMoBus, ref RxModBus);
            ModBusClass.ModBusCoppyCreate(ref ModBusClass.DefMoBus, ref Usart.ThreadRxModBusMsg);
            ModBusClass.ModBusCoppyCreate(ref ModBusClass.DefMoBus, ref Usart.ThreadTxModBusMsg);

        }
        uint FailCount = 0;
        private void ResetCom()
        {
            try
            {
                Value.serialPort1.Close();
            }
            catch { }
            try
            {
                Value.serialPort1 = new System.IO.Ports.SerialPort();
                Value.serialPort1.PortName = Ini.Read("COM");
                Value.serialPort1.BaudRate = 115200;
                Value.serialPort1.DataReceived += Port1_DataReceived;
                Value.serialPort1.Open();
            }
            catch { (new SetPort()).ShowDialog(); if (FailCount++ > 2)return;ResetCom(); }
        }
        private void 下载_Load(object sender, EventArgs e)
        {
            Value.App_Run = true;
            Value.UpdateBin = false;
            Read_UpDataInfor();
            try
            {
                ResetCom();
            }
            catch { (new SetPort()).ShowDialog(); }
            ResetCom();
            autoscanf = new Thread(烧写);
            (new Thread(DownLoad.DownLoadThread)).Start();
           button41_Click(null, null);
        }
        private string Read_UpDataInfor()
        {
            try
            {
                string name = Ini.Read("附件路径");
                comboBox1.Text = name;
                return name;
            }
            catch { }
            return "NULL";
        }
        private void button41_Click(object sender, EventArgs e)
        {
            try
            {
                while (comboBox1.Items.Count > 0) comboBox1.Items.RemoveAt(0);
                string[] files = System.IO.Directory.GetFiles(Application.StartupPath + "\\File", "*.bin");
                for (int i = 0; i < files.Length; i++)
                {
                    comboBox1.Items.Add(Path.GetFileName(files[i]));
                }
            }
            catch {  }
            comboBox1.Text = Ini.Read("附件路径");
        }
        Thread autoscanf;

        private void Write(ACFF Mode, byte[] Data, int Length)
        {
            Random ra = new Random();
            FillValue();
            TxModBus.MsgFlag = (byte)Mode;
            TxModBus.DataFlag = 0x01;
            TxModBus.DataLength = Length;
            TxModBus.MsgLength = Length + 3;
            int TxLength = ModBusClass.ModBus_CreateMsg(ref TxBuffer, ref TxModBus, (int)Mode, ra.Next(0, 255), 0x91, Data, Length);
            try
            {
                Value.serialPort1.Write(TxBuffer, 0, TxLength);
            }
            catch { ResetCom(); }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "烧写")
            {
                ResetCom();
                comboBox1.Enabled = false;
                autoscanf = new Thread(烧写);
                autoscanf.Start();
                Count = 0;
                button1.Text = "停止";
                richTextBox1.Focus();
                richTextBox1.Text = "";
                progressBar1.Value = 0;
                Value.App_Run = true;
            }
            else
            {
                button1.Text = "烧写";
                autoscanf.Abort();
                Value.App_Run = false;
                comboBox1.Enabled = true;
            }
        }
        void Port1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            try
            {
                Thread.Sleep(20);
                int n = Value.serialPort1.BytesToRead;
                byte[] buf = new byte[n];
                Value.serialPort1.Read(buf, 0, n);
                Value.ReviceData.Add(buf);
                BeginInvoke(new MethodInvoker(delegate()
                {
                    if (buf.Length > 4)
                    {
                        ModBusClass.ModBus_Expend(buf, buf.Length, ref RxModBus);
                        if (RxModBus.ErrorFlag == ModBusClass.ModBus_Ok)
                        {
                            richTextBox1.SelectionColor = Color.Green;
                            string Msg = Tools.GetStringFromByte(RxModBus.Data);
                            Msg = Msg.Replace("NULL", "");
                            richTextBox1.AppendText(Msg);
                            if (Msg.IndexOf("OK") > 0) Count++;
                        }
                    }
                }));
            }
            catch (Exception E) { MessageBox.Show(E.Message); }
        }
        private void Erase(uint len)
        {
            BeginInvoke(new MethodInvoker(delegate()
            {
                richTextBox1.SelectionColor = Color.Blue;
                richTextBox1.AppendText("开始擦除....\r\n");
            }));
            byte[] Tata = new byte[4];
            Tools.ByteFromU32(len, ref Tata, 0, 0);
            Write(ACFF.SCFF_EraseFlase, Tata, Tata.Length);
        }
        private void Write1024(byte[] Buf, int Page, int Size)
        {
            byte[] Tata = new byte[Size + 1];
            Tata[0] = (byte)Page;
            for (int i = 0; i < Size; i++)
            {
                Tata[i + 1] = Buf[i + Page * 1024];
            }
            Write(ACFF.SCFF_Write1024Byte, Tata, Tata.Length);
            BeginInvoke(new MethodInvoker(delegate()
            {
                richTextBox1.SelectionColor = Color.Blue;
                richTextBox1.AppendText("写入页"+Page);
            }));
        }
        private void 烧写()
        {
           
            int PageSize;
            uint LostCount = 0;
            UInt32 Error = 0; 
            FileInfo fi;
            uint len;
            byte[] buffer;
            try
            {
                fi = new FileInfo(Application.StartupPath + "\\File\\" +Ini.Read("附件路径"));
                len = (uint)fi.Length;
                FileStream fs=fi.OpenRead();
                buffer = new byte[len];
                fs.Read(buffer, 0, (int)len);
                fs.Close();
            }
            catch (Exception E)
            {
                MessageBox.Show(E.Message);
                return;
            }
           
            PageSize = (int)len / 1024;
       Ai:
            Count = 0;
            while (Value.App_Run)
            {
                button4_Click(null, null);
                for (int z = 0; z < 5; z++)
                {
                    Thread.Sleep(10);
                    if (Count > 0) goto Ci;
                }
            }
        Ci:
            Count = 0;
           Erase((uint)fi.Length);
           Thread.Sleep(2000);
                for (int z = 0; z < 5300;z++ )
                {
                    Thread.Sleep(1);
                    if (Count > 0) goto Bi;
                }
                goto Ai;
        Bi:
            Thread.Sleep(1000);
            Count = 0;
            int i = 0;
            while (Value.App_Run)
            {
                if (i == Count)
                {
                    Error = 0;
                    if (len >= 1024)
                    {
                        Write1024(buffer, i, 1024); len -= 1024;
                        LostCount = 1024;
                        goto B;
                    }
                    if (len < 1024) if (len > 0)
                        {
                            Write1024(buffer, i, (int)len); len = 0;
                            LostCount = len;
                            goto B;
                        }
                    if (len == 0) break;
                B:
                    i++;
                }
                else Error++;
                if (Error == 100) { i--; len += LostCount; }
                BeginInvoke(new MethodInvoker(delegate()
                {
                    progressBar1.Maximum = PageSize+1;
                    progressBar1.Value = i;
                }));
                Thread.Sleep(10);
            }
            BeginInvoke(new MethodInvoker(delegate()
            {
                richTextBox1.SelectionColor = Color.Blue;
                richTextBox1.AppendText("烧写完成!\r\n");
                button1.Text = "烧写";
            }));
            button3_Click(null, null);
            Thread.Sleep(100);
            button3_Click(null, null);
            BeginInvoke(new MethodInvoker(delegate()
            {
                richTextBox1.SelectionColor = Color.Blue;
                richTextBox1.AppendText("程序开始执行!\r\n");
                button1.Text = "烧写";
                Value.queue.Enqueue(DateTime.Now.ToString() + "固件下载LOG:"+richTextBox1.Text);
                comboBox1.Enabled = true;
            }));
           
        }



  
        private void button3_Click(object sender, EventArgs e)
        {
            byte[] Tata = new byte[0];
            Write(ACFF.SCFF_GotoApplication, Tata, Tata.Length);
        }
        private void button4_Click(object sender, EventArgs e)
        {
            byte[] Tata = new byte[0];
            Write(ACFF.SCFF_GoToBootLoader, Tata, Tata.Length);
        }

        private void 下载_FormClosed(object sender, FormClosedEventArgs e)
        {
            Value.serialPort1.DataReceived -= Port1_DataReceived;
            Value.serialPort1.Dispose();
        }

        private void 下载_FormClosing(object sender, FormClosingEventArgs e)
        {
            Value.serialPort1.DataReceived -= Port1_DataReceived;
            Value.serialPort1.Dispose();
            autoscanf.Abort();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Ini.Write("附件路径",comboBox1.Text);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string FilePath = "File/";
            if (Value.UpdateBin)
            {
                comboBox1.Focus();
                button41_Click(null, null);
                timer1.Enabled = false;
                Value.UpdateBin = false;
                richTextBox1.AppendText("固件更新至最新\r\n");
                string str = File.ReadAllText(FilePath + "List.txt", System.Text.Encoding.GetEncoding("utf-8"));
                richTextBox1.AppendText(str);
            }
        }

    }
}
