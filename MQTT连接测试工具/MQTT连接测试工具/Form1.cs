using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
namespace MQTT连接测试工具
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            
            InitializeComponent();
        }
        TcpClient tcpClient;
        TcpClient tcpClient1;
        NetworkStream ns;
        NetworkStream ns1;
        private void Form1_Load(object sender, EventArgs e)
        {
            //tcpClient = new TcpClient();
            //tcpClient1 = new TcpClient();
            //tcpClient.Connect(IPAddress.Parse(textBox4.Text), Int32.Parse(textBox3.Text));
            //tcpClient1.Connect(IPAddress.Parse(textBox1.Text), Int32.Parse(textBox2.Text));
            //ns = tcpClient.GetStream();
            //ns1 = tcpClient1.GetStream();
           
        }
        private byte[] CreateLogin()
        {
            byte[] Buf = new byte[100];
            byte[] Head={0x32,88,0x00,0x01};
            string Msg="{\"sensorDatas\":[{\"value\":11.4},{\"value\":12.1},{\"lat\":1.1,\"lng\":1.0}]}";
            byte[] newArr = Head.Concat(Encoding.GetEncoding("GB2312").GetBytes("H718L9X19767N1G2")).Concat(new byte[]{0x00,0x01}).Concat(Encoding.ASCII.GetBytes(Msg)).ToArray();
            return newArr;
        }
        private void button1_Click(object sender, EventArgs e)
        {

            ns.Write(CreateLogin(), 0, CreateLogin().Length);
            ns1.Write(CreateLogin(), 0, CreateLogin().Length);
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {

                if (button4.Text == "启动")
                {
                    label1.Text = "开始发送";
                    timer1.Interval = 1000;
                    tcpClient1 = new TcpClient();
                    tcpClient1.Connect(IPAddress.Parse(textBox8.Text), Int32.Parse(textBox7.Text));
                    ns1 = tcpClient1.GetStream();
                    byte[] Buf = System.Text.Encoding.Default.GetBytes(textBox9.Text);
                    ns1.Write(Buf, 0, Buf.Length);
                    button4.Text = "停止";
                    timer1.Enabled = true;
                }
                else
                {
                    button4.Text = "启动";
                    timer1.Enabled = false;
                    tcpClient1.Close();
                }
        }
        int Count = 0;
        int FCount = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                timer1.Interval = int.Parse(textBox11.Text) * 1000;
                label1.Text = "发送计数[" + ++Count + "]";
                Random ran = new Random();
                float RandKey = float.Parse(ran.Next(1, 5).ToString());
                RandKey /= 10;
                RandKey += float.Parse(textBox12.Text);
                textBox10.Text = "#TP300,+0" + RandKey + ",0.00";
                byte[] Buf = System.Text.Encoding.Default.GetBytes(textBox10.Text + "\r\n");
                ns1.Write(Buf, 0, Buf.Length);
            }
            catch
            {
                try
                {
                    tcpClient1 = new TcpClient();
                    tcpClient1.Connect(IPAddress.Parse(textBox8.Text), Int32.Parse(textBox7.Text));
                    ns1 = tcpClient1.GetStream();
                    byte[] Buf = System.Text.Encoding.Default.GetBytes(textBox9.Text);
                    ns1.Write(Buf, 0, Buf.Length);
                    label6.Text ="重新连接次数"+ FCount++.ToString();
                }catch{ }
            }
        }
    }
}
