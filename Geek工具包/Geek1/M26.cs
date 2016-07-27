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

namespace Geek1
{
    public partial class M26 : Form
    {
        public M26()
        {
            InitializeComponent();
        }

        private void M26_Load(object sender, EventArgs e)
        {
        }
        private string SendAT(string Msg, int Delay)
        {
            try
            {
                Value.Port1.Open();
            }
            catch { }
            byte[] RxBuf = new byte[100];
            richTextBox2.SelectionColor = Color.Blue;
            richTextBox2.AppendText(Msg);
            Value.Port1.WriteLine(Msg);
            Thread.Sleep(Delay);
            int Length = Value.Port1.Read(RxBuf, 0, Value.Port1.BytesToRead);
            richTextBox1.AppendText(Tools.HexToString(RxBuf, Length));
            richTextBox2.SelectionColor = Color.Green;
            richTextBox2.AppendText(Encoding.GetEncoding("GB2312").GetString(RxBuf));
            try
            {
                Value.Port1.Close();
            }
            catch { }
            Application.DoEvents();
            return Encoding.GetEncoding("GB2312").GetString(RxBuf);
        }
        private string SendAT(byte[] Msg, int Delay)
        {
            try
            {
                Value.Port1.Open();
            }
            catch { }
            byte[] RxBuf = new byte[100];
            richTextBox2.SelectionColor = Color.Blue;
            richTextBox2.AppendText(Tools.HexToString(Msg, Msg.Length));
            Value.Port1.Write(Msg, 0, Msg.Length);
            Thread.Sleep(Delay);
            int Length = Value.Port1.Read(RxBuf, 0, Value.Port1.BytesToRead);
            richTextBox1.AppendText(Tools.HexToString(RxBuf, Length));
            richTextBox2.SelectionColor = Color.Green;
            richTextBox2.AppendText(Encoding.GetEncoding("GB2312").GetString(RxBuf));
            try
            {
                Value.Port1.Close();
            }
            catch { }
            Application.DoEvents();
            return Encoding.GetEncoding("GB2312").GetString(RxBuf);
        }
        private void button3_Click(object sender, EventArgs e)
        {
            button3.Text = "正在初始化";
            string Msg = SendAT("AT\r", 100);
            Msg = SendAT("ATE0\r", 100);
            Msg = SendAT("AT+CSQ\r", 100);
            Msg = SendAT("AT+CREG?\r", 100);
            Msg = SendAT("AT+CGREG?\r", 100);
            Msg = SendAT("AT+QIFGCNT=0\r", 1000);
            Msg = SendAT("AT+QIDEACT\r", 1000);
            Msg = SendAT("AT+QIREGAPP\r", 1000);
            Msg = SendAT("AT+QIOPEN=\"TCP\",\"115.29.170.230\",1883", 1000);
            if (Msg.IndexOf("FAIL") > 0) { textBox1.Text = "连接失败,或许已经连接上"; return; }
            if (Msg.IndexOf("OK") > 0) { textBox1.Text = "连接成功"; return; }
            if (Msg.IndexOf("ERROR") > 0) { textBox1.Text = "无法连接"; return; }
            button3.Text = "初始化连接";
        }
        private byte[] CreateLogin()
        {
            byte[] Buf = new byte[100];
            byte[] Head = { 0x32, 88, 0x00, 0x01 };
            string Msg = "{\"sensorDatas\":[{\"value\":11.4},{\"value\":12.1},{\"lat\":1.1,\"lng\":1.0}]}";
            byte[] newArr = Head.Concat(Encoding.GetEncoding("GB2312").GetBytes("17K7GPTRZ9EZDPZP")).Concat(new byte[] { 0x00, 0x01 }).Concat(Encoding.ASCII.GetBytes(Msg)).ToArray();
            return newArr;
        }
        byte[] Buf;
        private void button2_Click(object sender, EventArgs e)
        {
            Buf = CreateLogin();
            Buf[1] = (byte)(Buf.Length - 2);
            SendAT("AT+QISEND=" + Buf.Length + "\r", 1000);

        }

        private void button4_Click(object sender, EventArgs e)
        {
            button2_Click(null, null);
            Thread.Sleep(1000);
            SendAT(Buf, 1000);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Buf = new byte[2];
            Buf[0] = 0xc0;
            Buf[1] = 0x00;
            SendAT("AT+QISEND=" + Buf.Length + "\r", 1000);
            Thread.Sleep(1000);
            SendAT(Buf, 1000);
        }
        private byte[] CreateConnect()
        {
            byte[] Buf = new byte[100];
            byte[] Head = { 0x10, 0x1c, 0x00, 0x04 };
            byte[] newArr = Head.Concat(Encoding.GetEncoding("GB2312").GetBytes("MQTT")).Concat(new byte[] { 0x04, 0x02, 0x00, 0x3c, 0x00, 0x10 }).Concat(Encoding.GetEncoding("GB2312").GetBytes("17K7GPTRZ9EZDPZP")).ToArray();
            return newArr;
        }
        private void button6_Click(object sender, EventArgs e)
        {
            byte[] Buf = CreateConnect();
            SendAT("AT+QISEND=" + Buf.Length + "\r", 1000);
            Thread.Sleep(1000);
            SendAT(Buf, 1000);
        }
    }
}
