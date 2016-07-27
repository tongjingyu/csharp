using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
namespace _433模块助手
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }
        byte[] Buf = new byte[17];
        string Msg="";
        private void Main_Load(object sender, EventArgs e)
        {
            button3_Click(null, null);
            button4_Click(null, null);
        }

        private void button3_Click(object sender, EventArgs e)
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
                button4.Enabled = true;
            }
            else button4.Enabled = false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (button4.Text == "打开串口")
                try
                {

                    serialPort1.PortName = comboBox1.Text;
                    serialPort1.BaudRate = int.Parse(comboBox2.Text);
                    serialPort1.Open();
                    Value.App_Run = true;
                    button4.Text = "关闭串口";
                    button4.ForeColor = Color.Red;
                }
                catch { MessageBox.Show("端口不存在或被占用", "警告"); }
            else
                try
                {
                    button4.Text = "打开串口";
                    Value.App_Run = false;
                    button4.ForeColor = button3.ForeColor;
                    serialPort1.Close();
                }
                catch (Exception E) { MessageBox.Show(E.Message); }
        }
        private byte[] GetBoxFill()
        {
            
            int i = 0;
            Buf[i++] = 0xfd;
            Buf[i++] = 0xd4;
            Buf[i++] = 0x0c;
            Buf[i++] = 0x08;
            Buf[i++] = 0x02;

            Buf[i++] = byte.Parse(comboBox3.Text);
            Buf[i++] = byte.Parse(comboBox4.Text);
            Buf[i++] = byte.Parse(comboBox5.Text);
            Buf[i++] = (byte)(int.Parse(comboBox6.Text) / 0xff);
            Buf[i++] = (byte)(int.Parse(comboBox6.Text) % 0xff);

            Buf[i++] = (byte)(int.Parse(comboBox8.Text) / 0xff);
            Buf[i++] = (byte)(int.Parse(comboBox8.Text) % 0xff);
            Buf[i++] = (byte)(int.Parse(comboBox7.Text) / 1200);
            Buf[i++] = byte.Parse(comboBox9.Text);

            Buf[i++] = 0x00;
            Buf[i++] = 0x16;
            Buf[i++] = 0x00;
            return Buf;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (timer1.Enabled == false)
            {
                timer1.Enabled = true;
                textBox1.Text = Tools.HexToString(GetBoxFill(), 17);
                button1.Text = "停止";
            }
            else
            {
                timer1.Enabled = false;
                button1.Text="写入配置";
            }
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] Temp=new byte[100];
           
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            serialPort1.Write(Buf, 0, 17);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox2.Text = serialPort1.ReadLine();
        }
    }
}
