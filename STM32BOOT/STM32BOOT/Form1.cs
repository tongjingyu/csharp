using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;
using System.IO;

namespace STM32BOOT
{
    public partial class Form1 : Form
    {

        private Usart Usart1;
        private byte DevAddr;

        StringBuilder SB = new StringBuilder();
        int RxCount=0;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            button1_Click(null, null);
            button2_Click(null, null);
            Usart1 = new Usart(serialPort1);
            DevAddr = byte.Parse(comboBox3.Text);
            comboBox3.Text = Ini.Read("DEVID");
            Read_UpDataInfor();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string[] SPNames = SerialPort.GetPortNames();
            comboBox1.Items.Clear();
            for (int i = 0; i < SPNames.Length; i++)
            {
                comboBox1.Items.Add(SPNames[i]);
            }
            if (SPNames.Length > 0)
            {
                comboBox1.Text = SPNames[0];
                button2.Enabled = true;
            }
            else button2.Enabled = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (comboBox1.Items.Count <= 0) return;
            if (button2.Text == "打开串口")
                try
                {

                    serialPort1.PortName = comboBox1.Text;
                    serialPort1.BaudRate = int.Parse(comboBox2.Text);
                    serialPort1.Open();
                    button2.Text = "关闭串口";
                    button2.ForeColor = Color.Red;
                }
                catch { MessageBox.Show("端口不存在或被占用", "警告"); }
            else
                try
                {
                    button2.Text = "打开串口";
                    serialPort1.Close();
                }
                catch (Exception E) { MessageBox.Show(E.Message); }
        }

        private void comboBox3_TextChanged(object sender, EventArgs e)
        {
            Ini.Write("DEVID", comboBox3.Text);
            if(!byte.TryParse(comboBox3.Text, out DevAddr))DevAddr=0;
        }

        private void comboBox4_TextChanged(object sender, EventArgs e)
        {
            Ini.Write("BOOTSize", comboBox3.Text);
        }



        private void timer1_Tick(object sender, EventArgs e)
        {
            //richTextBox1.Text = SB.ToString();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //byte[] Buf = new byte[1500];
            //byte[] Data = new byte[65];
            //Data[0] = 2;
            //for (int i = 0; i < 64; i++) Data[i + 1] =(byte)i;
            //int Length = ZigBeeBus.ZigBee_SendMsg(ref Buf, byte.Parse(comboBox3.Text), (byte)ACFF.SCFF_Write64Byte, Data, (byte)Data.Length);
            //ZigBeeBus.ZigBee_CheckCrc(Buf);
            //Usart1.SendData(Buf, Length, ref Buf, Length);
            byte[] Data = new byte[64];
            for (int i = 0; i < 64; i++) Data[i] = (byte)i;
            Write64Bytes(Data, 1);
        }
        private void Write1024Bytes(byte[] Data)
        {
            byte[] Tata = new byte[64];
            for (int i = 0; i < 16; i++)
            {
                for (int n = 0; n < 64; n++) Tata[n] = Data[n + i * 64];
                Write64Bytes(Tata, (byte)i);
            }
        }
        private void Write64Bytes(byte[] Data,byte Index)
        {
            byte[] Buf = new byte[1500];
            byte[] Tata = new byte[65];
            Tata[0] = Index;
            for (int i = 0; i < 64; i++) Tata[i + 1] = Data[i];
            int Length = ZigBeeBus.ZigBee_SendMsg(ref Buf, DevAddr, (byte)ACFF.SCFF_Write64Byte, Tata, (byte)Tata.Length);
            ZigBeeBus.ZigBee_CheckCrc(Buf);
            Usart1.WriteBuffer(Buf, Length);
        }
        private void button5_Click(object sender, EventArgs e)
        {
            byte[] Buf = new byte[1500];
            byte[] Data = new byte[1];
            Data[0] =byte.Parse(textBox1.Text);
            int Length = ZigBeeBus.ZigBee_SendMsg(ref Buf, DevAddr, (byte)ACFF.SCFF_Read64Byte, Data, (byte)Data.Length);
            ZigBeeBus.ZigBee_CheckCrc(Buf);
            Usart1.WriteBuffer(Buf, Length);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            byte[] Data = new byte[1024];
            for (int i = 0; i < 1024; i++) Data[i] = 3;
            Write1024Bytes(Data);
        }
        private void WritePage(byte[] Data,int Addr)
        {
            byte[] Buf = new byte[100];
            int CrcValue = Tools.GetCrc16(Data,0, 1024);
            Write1024Bytes(Data);
            Data = new byte[4];
            Data[0] =(byte)(CrcValue >> 8);
            Data[1] =(byte)(CrcValue & 0xff);
            Data[2] = (byte)(Addr >> 8);
            Data[3] = (byte)(Addr & 0xff);
            int Length = ZigBeeBus.ZigBee_SendMsg(ref Buf, DevAddr, (byte)ACFF.SCFF_Write1024Byte, Data, 4);
            ZigBeeBus.ZigBee_CheckCrc(Buf);
            Usart1.WriteBuffer(Buf, Length);
            Thread.Sleep(50);
        }
        private void button7_Click(object sender, EventArgs e)
        {
            byte[] Data = new byte[1024];
            byte[] Ref = new byte[1024];
            for (int i = 0; i < 1024; i++) Data[i] = 3;
            byte[] Buf = new byte[1500];
            Data[0] = 0x91;
            Data[1] = 0x0e;
            Data[2] = 0x00;
            Data[3] = 0x00;
            int Length = ZigBeeBus.ZigBee_SendMsg(ref Buf, DevAddr, (byte)ACFF.SCFF_Write1024Byte, Data, 4);
            ZigBeeBus.ZigBee_CheckCrc(Buf);
            Usart1.WriteBuffer(Buf, Length);
        }

          private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] Buffer_Get = null;
            try
            {
                int N = this.serialPort1.BytesToRead;
                Buffer_Get = new byte[N];
                this.serialPort1.Read(Buffer_Get, 0, N);
                RxCount += N;
                SB.Clear();
                this.Invoke((EventHandler)(delegate
                {
                    foreach (byte Buf in Buffer_Get)
                    {
                        SB.Append(Buf.ToString("X2") + " ");
                    }

                    this.richTextBox1.AppendText(SB.ToString());
                    Application.DoEvents();
                }));
            }
            catch { }
            finally
            {
            }
        }

     
        private void DownLoad_Thread()
        {
            for(int z=0;z<10;z++)
            {
                RxCount = 0;
                button12_Click(null, null);
                for (int a = 0; a < 10; a++)
                {
                    Thread.Sleep(100);
                    if (RxCount > 5) { z+=1; }
                }
                if (RxCount < 5) z--;
            }
            FileInfo fi = new FileInfo(Ini.Read("附件路径"));
            uint len =(uint) fi.Length;
            FileStream fs = new FileStream(Ini.Read("附件路径"), FileMode.Open);
            byte[] buffer = new byte[len];
            fs.Read(buffer, 0, (int)len); fs.Close();
            fs.Close();
            uint PageSize = len / 1024;
            if ((len % 1024) > 0) PageSize++;
            for (int z = 0; z < PageSize; z++)
            {
                this.Invoke((EventHandler)(delegate
                {
                    this.progressBar1.Maximum =(int)(PageSize-1);
                    this.progressBar1.Value = z;
                    Application.DoEvents();
                }));
                byte[] Data = new byte[1024];
                for (int i = 0; i < 1024; i++)
                {
                    try { Data[i] = buffer[1024 * z + i]; } catch { Data[i] = 0xff; }
                }
                RxCount = 0;
                WritePage(Data,z);
                for (int a = 0; a < 10; a++)
                {
                    if (RxCount > 5) {  break; }
                    Thread.Sleep(10);
                }
                if (RxCount < 5) z--;
            }
            button3_Click(null, null);
            button3_Click(null, null);
            this.Invoke((EventHandler)(delegate
            {
                this.progressBar1.Value = 0;
                button9.Text = "写入文件";
                Application.DoEvents();
            }));
        }
        Thread ServiceThread;
        private void button9_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = null;
            if (button9.Text == "写入文件")
            {
                ServiceThread = new Thread(new ThreadStart(DownLoad_Thread));
                ServiceThread.Start();
                button9.Text = "停止写入";
            }
            else { ServiceThread.Abort(); button9.Text = "写入文件"; }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            byte[] Data = new byte[1];
            byte[] Buf = new byte[1500];
            int Length = ZigBeeBus.ZigBee_SendMsg(ref Buf, DevAddr, (byte)ACFF.SCFF_GoToBootLoader, Data, 0);
            ZigBeeBus.ZigBee_CheckCrc(Buf);
            Usart1.WriteBuffer(Buf, Length);
            Thread.Sleep(50);
        }
        private string Read_UpDataInfor()
        {
            try
            {
                string name = Ini.Read("附件路径");
                richTextBox1.Text = "文件路径:" + name+"\r\n";
                FileInfo FI = new FileInfo(name);
                richTextBox1.AppendText("文件大小:" +FI.Length.ToString()+" Bytes");
                return name;
            }
            catch { }
            return "NULL";
        }
        private void button10_Click(object sender, EventArgs e)
        {
            OpenFileDialog Dlg = new OpenFileDialog();
            Dlg.DefaultExt = Read_UpDataInfor();
            Dlg.Filter = "bin文件(*.bin)|*.bin|All文件(*.*)|*.*";
            if (Dlg.ShowDialog() == DialogResult.Cancel) return;
            Ini.Write("附件路径", Dlg.FileName);
            //richTextBox1.Text = Dlg.FileName;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            byte[] Buf = new byte[1500];
            byte[] Tata = new byte[2];
            Tata=Tools.Int16ToByte(int.Parse(textBox2.Text));
            int Length = ZigBeeBus.ZigBee_SendMsg(ref Buf, DevAddr, (byte)ACFF.SCFF_Read1024Byte, Tata, (byte)Tata.Length);
            ZigBeeBus.ZigBee_CheckCrc(Buf);
            Usart1.WriteBuffer(Buf, Length);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            byte[] Buf = new byte[1500];
            byte[] Tata = new byte[8];
            FileInfo fi = new FileInfo(Ini.Read("附件路径"));
            uint len = (uint)fi.Length;
            Tools.ByteFromU32(len, ref Tata, 0, 0);
            Tata[4] = 0x19;
            Tata[5] = 0x89;
            Tata[6] = 0x11;
            Tata[7] = 0x06;
            int Length = ZigBeeBus.ZigBee_SendMsg(ref Buf, DevAddr, (byte)ACFF.SCFF_EraseFlase, Tata, (byte)Tata.Length);
            ZigBeeBus.ZigBee_CheckCrc(Buf);
            Usart1.WriteBuffer(Buf, Length);
        }
    }
}
