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
namespace 杭州升降机模拟设备
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private  TCPIP Client_New;
        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void button1_Click(object sender, EventArgs e)
        {
            Client_New = new TCPIP(new IPEndPoint(IPAddress.Parse(textBox1.Text), int.Parse(textBox2.Text)));
            Client_New.Connect();
        }

   
    
        private byte[] ReviceHadnle(byte[] ReByte)
        {
            byte[] Buf = new byte[2048];
            int z = 0;
            Buf[z++] = ReByte[0];
            Buf[z++] = ReByte[1];
            for (int n = 2; n < (ReByte.Length - 2);)
            {
                if (ReByte[n] == 0x99)
                {
                    n++;
                    switch (ReByte[n])
                    {
                        case 0xa5: { Buf[z++] = (byte)~ReByte[n];  } break;
                        case 0x66: { Buf[z++] = (byte)~ReByte[n];  } break;
                        case 0x95: { Buf[z++] = (byte)~ReByte[n];  } break;
                        default: return null;
                    }
                }
                else Buf[z++] = ReByte[n];
                n++;

            }
            Buf[z++] = ReByte[ReByte.Length - 2];
            Buf[z++] = ReByte[ReByte.Length - 1];
            byte[] ReByte1 = new byte[z];
            for (int n = 0; n < z; n++) ReByte1[n] = Buf[n];
            return ReByte1;
        }
        UInt16 Num;
        private byte[] CreateMsg( byte Cmd, byte[] Data)
        {
            byte[] Buf = new byte[2048];
            byte[] TBuf = new byte[2048];
            int  i = 0,z=2;
            byte Temp = 0;
            Buf[i++] = 0x5a;
            Buf[i++] = 0x55;
            Buf[i++] = (byte)(Data.Length + 6);
            Buf[i++] = (byte)(Num >> 8);
            Buf[i++] = (byte)(Num & 0xff);
            Buf[i++] = 0X0D;
            Buf[i++] = Cmd;
            for (int n = 0; n < Data.Length; n++) Buf[i++] = Data[n];//复制数据到缓冲区
            for (int n = 2; n < i; n++)
            {
                switch (Buf[n])
                {
                    case 0xa5: { TBuf[z++] = 0x99; TBuf[z++] = (byte)~Buf[n]; } break;
                    case 0x66: { TBuf[z++] = 0x99; TBuf[z++] = (byte)~Buf[n]; } break;
                    case 0x95: { TBuf[z++] = 0x99; TBuf[z++] = (byte)~Buf[n]; } break;
                    default: TBuf[z++] = Buf[n]; break;
                }
            }
           i = 2;
            for (int n = 2; n < (z); n++) Buf[i++] = TBuf[n];
            for (int n = 0; n < (i ); n++) Temp += Buf[n];//计算校验和
            Buf[i++] = Temp;
            Buf[i++] = 0x6a;
            Buf[i++] = 0x69;
            byte[] ReByte = new byte[i];
            for (int n = 0; n < i; n++) ReByte[n] = Buf[n];
            return ReByte;
           
        }
        private void WriteFrom()
        {
            byte[] z = Tools.StringToHex(textBox5.Text);
            byte[] SBuf = CreateMsg(Convert.ToByte(textBox9.Text, 16), z);
            try { Client_New.Write(SBuf, SBuf.Length); }
            catch { button1_Click(null, null); Client_New.Write(SBuf, SBuf.Length); }
            textBox10.Text = null;
            Application.DoEvents();
            textBox6.Text = Tools.HexToString(SBuf, SBuf.Length);
            SBuf = ReviceHadnle(Client_New.Read());
            textBox10.Text = Tools.HexToString(SBuf, SBuf.Length);
        }
        private byte[] JoinByte(byte[] Old,byte[] New,UInt16 Length)
        {
            int n = 0;
            byte[] Buf= new byte[Old.Length + Length];
            for (int i = 0; i < Old.Length; i++) Buf[n++] = Old[i];
            for (int i = 0; i < New.Length; i++) Buf[n++] = New[i];
            return Buf;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            byte[] Buf1 = new byte[1];
            Buf1[0] = 0x01;
            byte[] Buf2 = JoinByte(Buf1,  System.Text.Encoding.Default.GetBytes(textBox12.Text), 12);
            textBox5.Text = Tools.HexToString(Buf2, Buf2.Length);
            byte[] z = Tools.StringToHex(textBox5.Text);
            byte[] SBuf = CreateMsg(Convert.ToByte(textBox9.Text, 16), z);
            textBox6.Text = Tools.HexToString(SBuf, SBuf.Length);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            byte[] z = Tools.StringToHex(textBox6.Text);
           // byte[] zz= SendHandle(z);
           // textBox11.Text = Tools.HexToString(zz, zz.Length);
            WriteFrom();
            Num++;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            byte[] Buf1 = new byte[1];
            Buf1[0] = 0x01;
            textBox9.Text = "06";
            byte[] Buf2 = JoinByte(Buf1, System.Text.Encoding.Default.GetBytes(textBox12.Text), 12);
            Buf1[0] = 0x01;
            byte[] Buf3 = JoinByte(Buf2, Buf1, 1);
            byte[] Buf4= JoinByte(Buf3, System.Text.Encoding.Default.GetBytes(textBox4.Text), 18);
            byte[] Buf5 = JoinByte(Buf4, System.Text.Encoding.GetEncoding("GBK").GetBytes(textBox3.Text), 10);
            byte[] Buf6 = JoinByte(Buf5, System.Text.Encoding.GetEncoding("GBK").GetBytes(textBox7.Text), 48);
            textBox5.Text = Tools.HexToString(Buf6, Buf6.Length);
            byte[] z = Tools.StringToHex(textBox5.Text);
            byte[] SBuf = CreateMsg(Convert.ToByte(textBox9.Text, 16), z);
            textBox6.Text = Tools.HexToString(SBuf, SBuf.Length);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            textBox10.Text = textBox10.Text.Replace(" ", "");
        }
    }
}
