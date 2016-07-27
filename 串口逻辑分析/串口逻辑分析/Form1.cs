using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
namespace 串口逻辑分析
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        byte[] receivebuf=new byte[100];
        byte[] oldreceivebuf = new byte[100];
        int receiveCount;
        Bitmap img;
        Graphics G;
        private void Form1_Load(object sender, EventArgs e)
        {
            serialPort1.BaudRate = 115200;
            serialPort1.PortName = "COM16";
            serialPort1.Open();
            serialPort1.DataReceived += serialPort1_DataReceived;
        }
        private void WriteChart(Bitmap image,byte[] Bytes,bool Clear)
        {
            int x=0,y=0;
            bool H = true;
            for(int i=0;i<Bytes.Length;i++)
            {
                for (int n = 0; n < Bytes[i]; n++)
                {
                    if (x/2 >= (pictureBox1.Width - 1)) return;
                    if (Clear)
                    {  
                        if (H) image.SetPixel(x++/2, pictureBox1.Height - 3, Color.Black);
                        else image.SetPixel(x++ / 2, pictureBox1.Height - pictureBox1.Height/2, Color.Black);
                    }
                    else 
                    { 
                   if(H)image.SetPixel( x++/2,pictureBox1.Height-3, Color.Red);
                   else image.SetPixel(x++ / 2, pictureBox1.Height - pictureBox1.Height/2, Color.Red);
                    }
                }
                if (Clear) for (int n = 3; n < pictureBox1.Height/2; n++) image.SetPixel(x / 2, pictureBox1.Height - n, Color.Black);
                else for (int n = 3; n < pictureBox1.Height/2; n++) image.SetPixel(x / 2, pictureBox1.Height - n, Color.Red);
                H = !H;
            }
        }
        void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
                Thread.Sleep(20);
                int bytes = serialPort1.BytesToRead;
                receiveCount = bytes;
                oldreceivebuf = receivebuf;
                receivebuf = new byte[bytes];
                serialPort1.Read(receivebuf, 0, bytes);
                textBox1.Invoke(new MethodInvoker(delegate
                {
                    textBox1.Text = Tools.HexToString(receivebuf, receiveCount);
                }));
                pictureBox1.Invoke(new MethodInvoker(delegate
                {
                    img = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                    G = Graphics.FromImage(img);
                    WriteChart(img, oldreceivebuf, true);
                    WriteChart(img, receivebuf, false);
                    pictureBox1.Image = img;
                }));
        }
    }
}
