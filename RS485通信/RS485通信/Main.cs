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

namespace Geek
{
    public partial class RS485 : Form
    {
        StringBuilder SB = new StringBuilder();
        public RS485()
        {
            InitializeComponent();
        }


        private void button2_Click_1(object sender, EventArgs e)
        {
            COM设置 form = new COM设置(false);
            form.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (button1.Text == "Start")
                {
                    Value.COM_BaudRate = int.Parse(Ini.Read("COM_BaudRate"));
                    Value.DeviceAddr = int.Parse(Ini.Read("Device_Addr"));
                    Value.Port1 = new SerialPort(Ini.Read("COM_Name"), Value.COM_BaudRate);
                    Value.Port1.Open();
                    groupBox3.Enabled = true;
                    Value.Port1.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.Port1_DataReceived);
                    if (Value.Port1.IsOpen) button1.BackColor = Color.Green;
                    button1.Text = "Stop";
                }
                else
                {
                    button1.Text = "Start";
                    groupBox3.Enabled = false;
                    button1.BackColor = button2.BackColor;
                    Value.Port1.Close();
                }
            }
            catch (Exception E){ button1.BackColor = Color.Red; MessageBox.Show(E.Message);}
        }

        private void Port1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            
        }



        private void RS485_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void RS485_Load(object sender, EventArgs e)
        {
            groupBox3.Enabled = false;
            button1_Click(null, null);
            button10_Click(null, null);
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            AT指令 form = new AT指令();
            form.Show();
          
        }

        private void button6_Click(object sender, EventArgs e)
        {
            CC_Edit form = new CC_Edit();
            form.Show();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            M26 form = new M26();
            form.ShowDialog();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            手环测试 form = new 手环测试();
            form.ShowDialog();
        }
    }
}
