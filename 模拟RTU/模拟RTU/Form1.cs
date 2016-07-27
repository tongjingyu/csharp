using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data;
using System.Data.OleDb;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Management;
namespace 模拟RTU
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private bool SendOk = false;
        private bool SendEn = false;
        private int SendCount = 0;
        private int SendSec = 0;
        private int TimeSpace = 0;
        private int SendCountRailTimeSucceed = 0;
        private int SendCountDensitySucceed = 0;
        private int SendCountRailTimeFail = 0;
        private int SendCountDensityFail = 0;
        private int RealTimeCycle = 0;
        private int DensityCycle = 0;
        static TcpClient Client_New;
        PerformanceCounter Pc;
        float CpuLoad;
        int DataType = 0x00;
        float[] CpuLoadList = new float[60];
        int CpuLoadIndex = 0;
        DateTime DT1 = new DateTime();
        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "开启模拟")
            {
                SendOk = true;
                button1.Text = "关闭模拟"; 
                button1.BackColor = Color.Red;
            }
            else
            {
                SendOk = false;
                button1.BackColor = Color.White;
                button1.Text = "开启模拟";
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (SendOk)
            {
                textBox20.Text = SendCount++.ToString();
                if (SendCount % RealTimeCycle == 0)
                {
                    if (SendRealTimeRecord()) SendCountRailTimeSucceed++;
                    else SendCountRailTimeFail++;
                }
                if (SendCount % DensityCycle == 0)
                {
                    if (SendDensityRecord()) SendCountDensitySucceed++;
                    else SendCountDensityFail++;
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Pc = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            DT1 = DateTime.Now;
        }
        private  bool SendRead(byte[] TxBuffer,int Length)
        {
            byte[] ReadBuffer = new byte[1000];
            try
            {
                IPEndPoint IpPoint = new IPEndPoint(IPAddress.Parse(textBox2.Text), int.Parse(textBox3.Text));
                Client_New = new TcpClient();
                Client_New.SendTimeout = 10000;
                Client_New.Connect(IpPoint);
                SysFlag.ServiceLink = true;
            }
            catch (Exception E) { MessageBox.Show(E.Message); }
            try
            {
                NetworkStream SendStream = Client_New.GetStream();
                Client_New.SendTimeout = 10000;
                Client_New.ReceiveTimeout=10000;
                SendStream.Write(TxBuffer, 0,Length);
                int RxLength;
                RxLength = SendStream.Read(ReadBuffer, 0, 1000);
                SendStream.Close();
            }
            catch (Exception E)
            { MessageBox.Show(E.Message);}
            string Msg = Encoding.GetEncoding("gb2312").GetString(ReadBuffer, 0, ReadBuffer.Length);
            if (Msg.Contains("OK")) return true;
            else return false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SendEn = true;
            if (SendRealTimeRecord()) SendCountRailTimeSucceed++;
            else SendCountRailTimeFail++;
            SendEn = false;
        }
        private bool SendRealTimeRecord()
        {
            byte[] SendBuffer = new byte[1000];
            string Msg = "$AAA;";
            Msg += textBox1.Text+";";
            Msg += textBox14.Text + ";";
            Msg += textBox15.Text + ";";
            Msg += textBox16.Text + ";";
            Msg += DT1.ToString()+";";
            Msg += ReadRandom(1,50) + ",";
            Msg += ReadRandom(11,14) + ",";
            Msg += ReadRandom(25,32) + ",";
            Msg += ReadRandom(90,120) + ",";
            Msg += ReadRandom(30,50) + ";";
            Msg+="水位(m),电池电压(V),设备温度(℃),经度(°),纬度(°);$END";
            textBox19.Text = Msg;
            SendBuffer = Encoding.GetEncoding("gb2312").GetBytes(Msg);
            if (SendEn)
            {
                if (SendRead(SendBuffer, SendBuffer.Length)) return true;
                else return false;
            }
            else return false;
        }
        private bool SendDensityRecord()
        {
            byte[] SendBuf = new byte[1000];
            int i = 0, LengthOffset = 0;
            i = Tools.StrInArray(ref SendBuf, "$BAA", i, 0);
            i = Tools.D32InArray(ref SendBuf, int.Parse(textBox1.Text), i, 0);
            i = Tools.D32InArray(ref SendBuf, int.Parse(textBox14.Text), i, 0);
            Tools.StrInArray(ref SendBuf, textBox15.Text, i, 0); i += 16; LengthOffset = i; i += 2;//说明宽度和包长位置
            i = Tools.D8InArray(ref SendBuf, int.Parse(textBox5.Text), i, 0);
            i = Tools.D8InArray(ref SendBuf, DataType, i, 0);/*此前长度固定*/
            i = Tools.DateTimeInArray(ref SendBuf, DT1.AddHours(-1), i, 0);
            for (int N = 0; N < (60 / TimeSpace); N++) i = Tools.FInArray(ref SendBuf, CpuLoadList[N], i, 0);
            i = Tools.DateTimeInArray(ref SendBuf, DT1.AddHours(-1), i, 0);
            for (int N = 0; N < (60 / TimeSpace); N++) i = Tools.FInArray(ref SendBuf, ReadRandomF(80, 80 + N), i, 0);//ReadRandomF(10, 10+N)
            i = Tools.StrInArray(ref SendBuf, "$END", i, 0);
            Tools.D16InArray(ref SendBuf, i, LengthOffset, 0);//插入长度
            DeBugDensity(SendBuf, i);
            if (SendEn)
            {
                if (SendRead(SendBuf, i)) return true;
                else return false;
            }
            else return false;
        }
        private void SetRichColor(ref int Stat, int Length,int CI)
        {
            Color[] C = new Color[] { Color.Green, Color.Red, Color.Black, Color.DarkSlateGray, Color.Red, Color.Black, Color.Green, Color.Red, Color.Black, Color.Green };
            richTextBox1.Select(Stat * 5, Length * 5);
            richTextBox1.SelectionColor =C[CI];
            Stat += Length;
        }
        private void DeBugDensity(byte[] Buffer,int Length)
        {

            string temp = "";
            
            int z = 0;
            for (int i = 0; i < Length; i++)
            {
                temp += ("" + Buffer[i].ToString("X2") + " ");
            }
            richTextBox1.Text = temp;
            richTextBox1.Select(0, 1);
            SetRichColor(ref z, 4,2);
            SetRichColor(ref z, 4, 3);
            SetRichColor(ref z, 4, 0);
            SetRichColor(ref z, 16, 1);
            SetRichColor(ref z, 2, 2);
            SetRichColor(ref z, 1, 3);
            SetRichColor(ref z, 1, 0);
            SetRichColor(ref z, 6, 2);
            for (int i = 0; i < (60 / TimeSpace); i++) SetRichColor(ref z, 4, i%4);
            SetRichColor(ref z, 6, 2);
            for (int i = 0; i < (60 / TimeSpace); i++) SetRichColor(ref z, 4, i%4);

            
        }
        private void timer2_Tick(object sender, EventArgs e)
        {
            if (SendOk) SendSec++;
            CpuLoad = Pc.NextValue();
            CpuLoadList[CpuLoadIndex++] = CpuLoad;
            if (CpuLoadIndex > 58) CpuLoadIndex = 0;
            textBox17.Text = CpuLoad.ToString("0.00");
            textBox18.Text = ReadRandom(1, 100);
            textBox6.Text = SendCountRailTimeSucceed.ToString();
            textBox7.Text = SendCountRailTimeFail.ToString();
            textBox8.Text = SendCountDensitySucceed.ToString();
            textBox11.Text = SendCountDensityFail.ToString();
            textBox12.Text = (SendCountDensitySucceed + SendCountRailTimeSucceed).ToString();
            textBox9.Text = (SendCountRailTimeFail + SendCountDensityFail).ToString();
            RealTimeCycle = int.Parse(textBox10.Text);
            DensityCycle = int.Parse(textBox5.Text);
            TimeSpace =int.Parse(textBox5.Text);
            textBox21.Text = SendSec.ToString();
            DT1 = DT1.AddMinutes(1);
            textBox22.Text = DT1.ToString("yyy-MM-dd HH-mm-ss");
        }
        private string ReadCpuload()
        {
            string Temp = Pc.NextValue().ToString("0.00");
            return Temp;
        }
        private string ReadRandom(int Min,int Max)
        {
            Min *= 100;
            Max *= 100;
            int n = new Random().Next(Max) + Min;
            double Re = (double)n / 100;
            return Re.ToString("0.00");
        }
        private float ReadRandomF(int Min, int Max)
        {
           return float.Parse(ReadRandom(Min, Max));
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox19.Text = "";
            richTextBox1.Text = "";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (button4.Text == "启用发送")
            {
                button4.BackColor = Color.Blue;
                button4.Text = "停止发送";
                SendEn = true;
            }
            else
            {
                button4.BackColor = Color.White;
                button4.Text = "启用发送";
                SendEn = false;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            SendEn = true;
            if (SendDensityRecord()) SendCountDensitySucceed++;
            else SendCountDensityFail++;
            SendEn = false;
        }

      

    }
}
