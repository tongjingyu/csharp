using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
namespace 串口显示屏
{
    public partial class Form1 : Form
    {
        public Form1()
        {
           
            CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
        }
        MB ModBus = new MB();
        MB RXModBus = new MB();
        Bitmap img;
        Graphics G;
        int LCD_XSIZE = 480, LCD_YSIZE = 272;
        int ReadLine = 0;
        bool ScreenWriteTrue=true;
        private void Form1_Load(object sender, EventArgs e)
        {
            button1_Click(null,null);
            button2_Click(null, null);
            ModBusClass.ModBus_Create(ref ModBus, 0x01, 0x94, 0x7e, 0x7f);
            img = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            G = Graphics.FromImage(img);
            toolStripProgressBar1.Maximum = LCD_XSIZE;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string[] SPNames = SerialPort.GetPortNames();
            comboBox1.Items.Clear();
            for (int i = 0; i < SPNames.Length; i++)
            {
                comboBox1.Items.Add(SPNames[i]);
                pictureBox2.Image = Properties.Resources.Ok;
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
            if (button2.Text == "打开串口")
                try
                {

                    serialPort1.PortName = comboBox1.Text;
                    serialPort1.BaudRate = int.Parse(comboBox2.Text);
                    serialPort1.Open();
                    button2.Text = "关闭串口";
                    button2.ForeColor = Color.White;
                    button2.BackColor = Color.Red;
                    pictureBox3.Image = Properties.Resources.Open;
                }
                catch (Exception E) { MessageBox.Show("端口不存在或被占用", "警告"); }
            else
                try
                {
                    button2.Text = "打开串口";
                    button2.ForeColor = button1.ForeColor;
                    button2.BackColor = button1.BackColor;
                    serialPort1.Close();
                    pictureBox3.Image = Properties.Resources.Close;
                }
                catch (Exception E) { MessageBox.Show(E.Message); }
        }
        private void ReadLineFromStruct()
        {
            if ((RXModBus.DataLength / 2) == LCD_YSIZE)
            {
                for (int y = 0; y < LCD_YSIZE; y++) img.SetPixel(ReadLine, y, Tools.GetColorFromTFT(Tools.ByteToInt16(RXModBus.Data, y * 2)));
            }
            pictureBox1.Image = img;
        }
        private void ReadStructFromBuffer()
        {
            int n = serialPort1.BytesToRead;
            byte[] Buffer = new byte[1000];
            serialPort1.Read(Buffer, 0, n);
            ModBusClass.ModBus_CreatStruct(ref RXModBus, Buffer);
        }
        private byte[] ReadPictureLine()
        {
            byte[] Buf=new byte[LCD_YSIZE*2+2];
            Buf[0] =(byte) (ReadLine >> 8);
            Buf[1] =(byte)( ReadLine & 0xff);
            for (int y = 0; y < LCD_YSIZE; y++)
            {
                Color Temp = img.GetPixel(ReadLine, y);
                byte[] TempBuf = Tools.Int16ToByte(Tools.GetTftFromColor(Temp));
                Buf[y*2+2]=TempBuf[0];
                Buf[y*2+1+2]=TempBuf[1];
            }
            return Buf;
        }
        private void Read_ScreenLine()
        {
            byte[] Buf = new byte[100];
            ModBus.TargetAddr = ModBusClass.TargetAddr;
            ModBus.HostAddr = ModBusClass.HostAddr;
            ModBus.MsgFlag = (byte)MsgFlag.ReadScreenYLine;
            ModBus.MsgNum = 1;
            ModBus.DataLength = 2;
            ModBus.DataFlag = 0x01;
            ModBus.Data = Tools.Int16ToByte(ReadLine);
            int n=ModBusClass.ModBus_CreatBuf(ref ModBus, ref Buf);
            serialPort1.Write(Buf, 0, n);
            System.Threading.Thread.Sleep(50);
            ReadStructFromBuffer();
            ReadLineFromStruct();
            ReadLine++;
            if (ReadLine >= LCD_XSIZE) { ReadLine = 0; timer1.Enabled = false; button5.BackColor = button4.BackColor; }
            toolStripProgressBar1.Value = ReadLine;
            toolStripStatusLabel1.Text = (ReadLine * 100 / LCD_XSIZE).ToString()+"%";
        }
        private void WriteScreenLine()
        {
            byte[] Buf = new byte[1000];
            ModBus.TargetAddr = ModBusClass.TargetAddr;
            ModBus.HostAddr = ModBusClass.HostAddr;
            ModBus.MsgFlag = (byte)MsgFlag.WriteScreenYLine;
            ModBus.MsgNum = 1;
            ModBus.DataLength = LCD_YSIZE*2+2;
            ModBus.DataFlag = 0x01;
            ModBus.Data = ReadPictureLine();
            int n = ModBusClass.ModBus_CreatBuf(ref ModBus, ref Buf);
            serialPort1.Write(Buf, 0, n);
            System.Threading.Thread.Sleep(50);
            ReadLine++;
            if (ReadLine >= LCD_XSIZE) { ReadLine = 0; timer1.Enabled = false; button5.BackColor = button4.BackColor; }
            toolStripProgressBar1.Value = ReadLine;
            toolStripStatusLabel1.Text = (ReadLine * 100 / LCD_XSIZE).ToString() + "%";
        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            serialPort1.BaudRate = int.Parse(comboBox2.Text);
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (ScreenWriteTrue) WriteScreenLine();
            else Read_ScreenLine();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ScreenWriteTrue = false;
            timer1.Enabled = true;
            button5.BackColor = Color.Red;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ScreenWriteTrue = true;
            timer1.Enabled = true;
            button5.BackColor = Color.Red;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            button5.BackColor = button4.BackColor;
        }

        private void pictureBox1_DragDrop(object sender, DragEventArgs e)
        {
            string path = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            MessageBox.Show(path);
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.All;

            else e.Effect = DragDropEffects.None; 
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
          string  fileName = (e.Data.GetData(DataFormats.FileDrop, false) as String[])[0];
            try
            {
                this.pictureBox1.ImageLocation = fileName;
                img=(Bitmap) System.Drawing.Image.FromFile(fileName);  
            }
            catch (Exception) { MessageBox.Show("文件格式不对"); }
        }
    }
}
