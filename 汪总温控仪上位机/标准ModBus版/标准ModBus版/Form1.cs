using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO.Ports;
namespace 标准ModBus版
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        string RxMsg = "";
        Thread RS485ReadThread;    
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
                    Value.App_Run = true;
                    button2.Text = "关闭串口";
                    button2.ForeColor = Color.Red;
                }
                catch { MessageBox.Show("端口不存在或被占用", "警告"); }
            else
                try
                {
                    button2.Text = "打开串口";
                    Value.App_Run = false;
                    button2.ForeColor = button1.ForeColor;
                    serialPort1.Close();
                }
                catch (Exception E) { MessageBox.Show(E.Message); }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //button1_Click(null, null);
            button2_Click(null, null);
            Value.RSValue.RS485Addr =int.Parse(comboBox3.Text);
            Value.RSValue.Model = "XXFD";
        }
        private MODBUS_TX_MSG GetMsg(int Offset,int Count)
        {
            MODBUS_RX_MSG MBRM = new MODBUS_RX_MSG();
            MODBUS_TX_MSG MBTM = new MODBUS_TX_MSG();
            MBTM.Buf = new byte[100];
            byte[] RxBuffer = new byte[200];
            byte[] TxBuffer = new byte[200];
            MBRM.Addr = byte.Parse(comboBox3.Text);
            MBRM.FuncCode = 0x03;
            MBRM.Length = Count;
            MBRM.Offset = (byte)Offset;
            int Length = 标准ModBus.CreateMsg_ModBus(MBRM, ref TxBuffer);
            serialPort1.ReadTimeout = 500;
            
            Length = Usart.SendData(serialPort1, TxBuffer, Length, ref RxBuffer, 200);
            标准ModBus.Export_ModBus(ref MBTM, ref RxBuffer);
            return MBTM;
        }
        private MODBUS_TX_MSG ThreadGetMsg(int Offset, int Count)
        {
            MODBUS_RX_MSG MBRM = new MODBUS_RX_MSG();
            MODBUS_TX_MSG MBTM = new MODBUS_TX_MSG();
            MBTM.Buf = new byte[100];
            byte[] RxBuffer = new byte[200];
            byte[] TxBuffer = new byte[200];
            MBRM.Addr = Value.RSValue.RS485Addr;
            MBRM.FuncCode = 0x03;
            MBRM.Length = Count;
            MBRM.Offset = (byte)Offset;
            int Length = 标准ModBus.CreateMsg_ModBus(MBRM, ref TxBuffer);
            Length = Usart.SendData(serialPort1, TxBuffer, Length, ref RxBuffer, 200);
            标准ModBus.Export_ModBus(ref MBTM, ref RxBuffer);
            return MBTM;
        }

        private void RS485Read()
        {
            MODBUS_TX_MSG MBTM;
            while (Value.App_Run)
            {
                if (!Value.App_Run) return;
                MBTM = ThreadGetMsg(13, 4);
                Value.RSValue.Model = System.Text.Encoding.Default.GetString(MBTM.Buf, 0, 4);
                MBTM = ThreadGetMsg(5, 4);
                Value.RSValue.Oil_Temperature = Tools.ByteToFloat(MBTM.Buf, 0, 1); ;
                Value.RSValue.Room_Temperature = Tools.ByteToFloat(MBTM.Buf, 4, 1); ;
                Value.RSValue.Dampness_1 = Tools.ByteToFloat(MBTM.Buf, 8, 1); ;
                Value.RSValue.Dampness_2 = Tools.ByteToFloat(MBTM.Buf, 12, 1); ;
                if (!Value.App_Run) return;
                MBTM = ThreadGetMsg(11, 4);
                Value.RSValue.F_H_Value = Tools.ByteToFloat(MBTM.Buf, 0, 1); ;
                Value.RSValue.T_C_Value = Tools.ByteToFloat(MBTM.Buf, 4, 1); ;
                Value.RSValue.T_T_Value = Tools.ByteToFloat(MBTM.Buf, 8, 1); ;
                Value.RSValue.SH1_Value = Tools.ByteToFloat(MBTM.Buf, 12, 1); ;
                Value.RSValue.SH2_Value = Tools.ByteToFloat(MBTM.Buf, 16, 1); ;
                MBTM = ThreadGetMsg(7, 4);
                if (!Value.App_Run) return;
               Value.RSValue.Ctr = MBTM.Buf[0];
               MBTM = ThreadGetMsg(12, 4);
               if (!Value.App_Run) return;
               Value.RSValue.TOP_Value = Tools.ByteToFloat(MBTM.Buf, 0, 1); ;
              
               
               Thread.Sleep(100);
            }
        }
        private void button6_Click(object sender, EventArgs e)
        {
            RS485ReadThread = new Thread(RS485Read);
            if (button6.Text =="连续读")
            {
                RS485ReadThread.Start();
                Value.App_Run = true;
                button6.Text = "停止";
        
            }
            else
            {
                button6.Text = "连续读";
                Value.App_Run = false;
                RS485ReadThread.Abort();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Value.App_Run = false;
            timer1.Enabled = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            textBox3.Text = Value.RSValue.Oil_Temperature.ToString("0.00");
            textBox6.Text = Value.RSValue.Room_Temperature.ToString("0.00");
            textBox7.Text = Value.RSValue.Dampness_1.ToString("0.00");
            textBox8.Text = Value.RSValue.Dampness_2.ToString("0.00");
            textBox12.Text = Value.RSValue.F_H_Value.ToString("0.00");
            textBox11.Text = Value.RSValue.T_C_Value.ToString("0.00");
            textBox10.Text = Value.RSValue.T_C_Value.ToString("0.00");
            textBox2.Text = Value.RSValue.SH1_Value.ToString("0.00");
            textBox9.Text = Value.RSValue.SH2_Value.ToString("0.00");
            textBox1.Text = Value.RSValue.TOP_Value.ToString("0.00");
             label1.Text =Value.RSValue.Model+"系列温控仪";
            int Temp = Value.RSValue.Ctr;
            if ((Temp & (1 << 0)) > 0)
            {
                checkBox1.Checked = true; pictureBox6.BackgroundImage = Properties.Resources.Led;
            }
            else
            {
                checkBox1.Checked = false;
                pictureBox6.BackgroundImage = Properties.Resources.Led_Off;
            }
            if ((Temp & (1 << 1)) > 0)
            {
                checkBox2.Checked = true;
                pictureBox7.BackgroundImage = Properties.Resources.Led;
            }
            else
            {
                checkBox2.Checked = false;
                pictureBox7.BackgroundImage = Properties.Resources.Led_Off;
            }
            if ((Temp & (1 << 2)) > 0)
            {
                pictureBox8.BackgroundImage = Properties.Resources.Led;
                checkBox3.Checked = true;
            }
            else
            {
                checkBox3.Checked = false;
                pictureBox8.BackgroundImage = Properties.Resources.Led_Off;
            }
            if ((Temp & (1 << 3)) > 0)
            {
                pictureBox9.BackgroundImage = Properties.Resources.Led;
                checkBox4.Checked = true;
            }
            else
            {
                checkBox4.Checked = false;
                pictureBox9.BackgroundImage = Properties.Resources.Led_Off;
            }
            if ((Temp & (1 << 4)) > 0)
            {
                pictureBox10.BackgroundImage = Properties.Resources.Led;
                checkBox5.Checked = true;
            }
            else
            {
                checkBox5.Checked = false;
                pictureBox10.BackgroundImage = Properties.Resources.Led_Off;
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void comboBox3_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Value.RSValue.RS485Addr = int.Parse(comboBox3.Text);
            }
            catch { }
        }

    }
}
