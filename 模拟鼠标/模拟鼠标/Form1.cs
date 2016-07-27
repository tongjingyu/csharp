using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;
namespace 模拟鼠标
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
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

        private void Form1_Load(object sender, EventArgs e)
        {
            button1_Click(null, null);
            button2_Click(null, null);
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int x, y;
                byte[] Buf = new byte[6];
                x = e.X * (4096 / 320); y = e.Y * (4096 / 240);
                Buf[0] = (byte)(y >> 8);
                Buf[1] = (byte)(y & 0xff);
                Buf[2] = (byte)(x >> 8);
                Buf[3] = (byte)(x & 0xff);
                Buf[4] = 0xff;
                Buf[5] = 0xff;
                label1.Text = x.ToString();
                label2.Text = y.ToString();
                serialPort1.Write(Buf, 0, Buf.Length);
            }
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            int x, y;
            byte[] Buf = new byte[6];
            x = e.X * (4096 / 320); y = e.Y * (4096 / 240);
            Buf[0] = (byte)(y >> 8);
            Buf[1] = (byte)(y & 0xff);
            Buf[2] = (byte)(x >> 8);
            Buf[3] = (byte)(x & 0xff);
            Buf[4] = 0xff;
            Buf[5] = 0xff;
            label1.Text = x.ToString();
            label2.Text = y.ToString();
            serialPort1.Write(Buf, 0, Buf.Length);
        }
    }
}
