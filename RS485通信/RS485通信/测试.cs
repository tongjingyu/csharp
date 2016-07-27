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
namespace Geek
{
    public partial class 手环测试 : Form
    {
        public 手环测试()
        {
            InitializeComponent();
        }
        
        private void 手环测试_Load(object sender, EventArgs e)
        {
            Value.Run = true;
            Value.Port1.DataReceived+=Port1_DataReceived;
           
        }
        private void WriteY()
        {
            SendAT(Value.BLE初始化命令);
            Thread.Sleep(500);
            SendAT(Value.BLE获取参数1);
            Thread.Sleep(500);
            SendAT(Value.BLE获取参数2);
            Thread.Sleep(500);
            SendAT(Value.BLE获取参数3);
            Thread.Sleep(500);
            SendAT(Value.BLE获取参数4);
            Thread.Sleep(500);
          
        }
        void Port1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            Thread.Sleep(40); 
            int n = Value.Port1.BytesToRead;
            byte[] buf = new byte[n];
            Value.Port1.Read(buf, 0, n);
            Value.ReviceData.Add(buf);
            timer1.Enabled = true;
        }
        private void SendAT(byte[] Msg)
        {
            byte[] RxBuf = new byte[100];
            BeginInvoke(new MethodInvoker(delegate()
            {
                richTextBox2.SelectionColor = GetRandomColor();
                richTextBox2.AppendText(Tools.HexToString(Msg, Msg.Length) + "\r\n");
            })); 
            Value.Port1.Write(Msg, 0, Msg.Length);

        }

        private void button6_Click(object sender, EventArgs e)
        {
            SendAT(new byte[] { 0x01, 0x04, 0xfe, 0x03, 0x03, 0x01, 0x00 });
            while(comboBox1.Items.Count > 0) comboBox1.Items.RemoveAt(0);
        }
        public System.Drawing.Color GetRandomColor()
        {
            Random RandomNum_First = new Random((int)DateTime.Now.Ticks);
            System.Threading.Thread.Sleep(RandomNum_First.Next(50));
            Random RandomNum_Sencond = new Random((int)DateTime.Now.Ticks);
            int int_Red = RandomNum_First.Next(256);
            int int_Green = RandomNum_Sencond.Next(256);
            int int_Blue = (int_Red + int_Green > 400) ? 0 : 400 - int_Red - int_Green;
            int_Blue = (int_Blue > 255) ? 255 : int_Blue;

            return System.Drawing.Color.FromArgb(int_Red, int_Green, int_Blue);
        }
        private void 手环测试_FormClosed(object sender, FormClosedEventArgs e)
        {
            Value.Run = false;
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            while (Value.ReviceData.Count > 0)
            {
                byte[] item = Value.ReviceData[0];
                richTextBox1.SelectionColor = GetRandomColor();
                if (Tools.Comment(item, Value.BLE搜索到设备))
                {
                    richTextBox1.AppendText("搜索到设备:\r\n");
                    comboBox1.Text = Tools.HexToString(item, 8, 6);
                    comboBox1.Items.Add(comboBox1.Text);
                }
                if (Tools.Comment(item, Value.BLE搜索响应)) richTextBox1.AppendText("响应:\r\n");
                if (Tools.Comment(item, Value.BLE初始化完毕)) richTextBox1.AppendText("初始化完毕:\r\n");
                if (Tools.Comment(item, Value.BLE初始化成功)) richTextBox1.AppendText("BLE初始化成功:\r\n");
                if (Tools.Comment(item, Value.BLE扫描完成)) richTextBox1.AppendText("BLE扫描完成:\r\n");
                if (Tools.Comment(item, Value.BLE操作忙)) richTextBox1.AppendText("BLE操作忙:\r\n");
                if (Tools.Comment(item, Value.BLE已经执行该操作)) richTextBox1.AppendText("BLE已经执行该操作:\r\n");
                if (Tools.Comment(item, Value.BLE配对响应)) richTextBox1.AppendText("BLE配对响应:\r\n");
                if (Tools.Comment(item, Value.BLE配对成功)) richTextBox1.AppendText("BLE配对成功:\r\n");
                if (Tools.Comment(item, Value.BLE断开连接)) richTextBox1.AppendText("BLE断开连接:\r\n");
                if (Tools.Comment(item, Value.BLE拒绝访问)) richTextBox1.AppendText("BLE拒绝访问:\r\n");
                if (Tools.Comment(item, Value.BLE读取成功)) richTextBox1.AppendText("BLE读取成功:\r\n");
                richTextBox1.AppendText(Tools.HexToString(item, item.Length) + "\r\n");
                Value.ReviceData.RemoveAt(0);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SendAT(new byte[] { 0x01, 0x09, 0xFE, 0x09, 0x00, 0x00, 0x00 }.Concat(Tools.StringToHex1(comboBox1.Text)).ToArray());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = textBox1.Text.Replace(" ", ",0x");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(WriteY);
            t.Start();   
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button4_Click(object sender, EventArgs e)
        {
           // SendAT(new byte[] { 0x01, 0x09, 0xFE, 0x09, 0x00, 0x00, 0x00 }.Concat(Tools.StringToHex1(comboBox1.Text)).ToArray());
            SendAT(Value.BLE获取内容UUID);
        }
    }
}
