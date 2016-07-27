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

namespace 模拟键盘
{
    public partial class KeyBoard : Form
    {
        public KeyBoard()
        {
            InitializeComponent();
        }
        Keylogger KeyLog;
        bool ComError = false;
        private void Form1_Load(object sender, EventArgs e)
        {
            button3_Click(null, null);
            button4_Click(null, null);
            button1_Click(null, null);
            comboBox2.Text = Ini.Read("RecordBautRate");
        }

        private void button1_Click(object sender, EventArgs e)
        {


            KeyLog = new Keylogger("KeyRecord.txt");
            KeyLog.startLoging();

            if (button1.Text == "启动")
            {
                button1.Text = "关闭";
                timer1.Enabled = true;
                if (!serialPort1.IsOpen) return;
                else Value.App_Run = true;
                Thread ServiceThread = new Thread(new ThreadStart(SendKey));
                ServiceThread.Start();
            }
            else
            {
                button1.Text = "启动";
                Value.App_Run = false;
                timer1.Enabled = false;
            }
        }
    

        private void timer1_Tick(object sender, EventArgs e)
        {
            label3.Text ="键代码:"+ Value.KeyMsg.Key_Value.ToString();
            label2.Text= "键名:"+ Enum.GetName(typeof(Keys), Value.KeyMsg.Key_Value);
        }
        private void Com_Open()
        {
            button1_Click(null, null);
            button4_Click(null, null);
            button4_Click(null, null);
            button1_Click(null, null);
            ComError = false;
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
                    button4.ForeColor = button1.ForeColor;
                    serialPort1.Close();
                }
                catch (Exception E) { }
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
        private void SendKey()
        {
            byte[] Data=new byte[2]; 
            while (Value.App_Run)
            {
                if(Value.KeyValue > 0)
                {
                    Data[0] = (byte)Value.KeyValue;
                    Value.KeyMsg.Key_Value= Value.KeyValue;
                    Value.KeyValue = 0;
                    byte[] Buf = ZigBeeBus.ZigBee_WriteKeyMsg(0x00, Data, 2);
                    try {
                        serialPort1.Write(Buf, 0, Buf.Length);
                        serialPort1.Write(Data, 0, Data.Length);
                    } catch { ComError=true; } 

                }
                Thread.Sleep(6);
            }
        }

        private void KeyBoard_FormClosing(object sender, FormClosingEventArgs e)
        {
            Value.App_Run = false;
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if(ComError)Com_Open();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Ini.Write("RecordCOM", comboBox1.Text);
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            Ini.Write("RecordBautRate", comboBox2.Text);
        }
    }
}
