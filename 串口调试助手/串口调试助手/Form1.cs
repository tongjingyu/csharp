using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
namespace 串口调试助手
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        string RxMsg="";
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
                    button2.ForeColor = button1.ForeColor;
                    serialPort1.Close();
                }
                catch (Exception E) { MessageBox.Show(E.Message); }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            button1_Click(null, null);
            button2_Click(null, null);
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                RxMsg += serialPort1.ReadLine();
            }
            catch { }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
           if(RxMsg!=null)richTextBox1.AppendText(RxMsg);
           RxMsg = null;
           toolStripStatusLabel1.Text = "R:" + richTextBox1.Text.Length;
        }
    }
}
