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

namespace 内蒙古料罐车通信测试
{
    public partial class Form1 : Form
    {


        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            byte[] Buf = new byte[1500];
            byte[] Tata = new byte[8];
            Tools.ByteFromFloat(float.Parse(textBox1.Text), ref Tata, 0,0);
            Tools.ByteFromFloat(float.Parse(textBox2.Text), ref Tata, 4, 0);
            int Length = ZigBeeBus.ZigBee_SendMsg(ref Buf, 0x10, 0x30, Tata, (byte)Tata.Length);
            ZigBeeBus.ZigBee_CheckCrc(Buf);
            textBox3.Text = Tools.HexToString(Buf, Length);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            byte[] Buf = new byte[1500];
            byte[] Tata = new byte[1];
            Tata[0] = byte.Parse(textBox6.Text);
            int Length = ZigBeeBus.ZigBee_SendMsg(ref Buf, 0x11, 0x31, Tata, (byte)Tata.Length);
            ZigBeeBus.ZigBee_CheckCrc(Buf);
            textBox4.Text = Tools.HexToString(Buf, Length);
        }
        private byte[] CreateBag(byte dId, byte mainCmd, byte subCmd, byte[] src, uint len)
        {
            //buf 为存储打包的目标地址 
            byte[] buf = new byte[len + 11];
            UInt16 crc = 0;
            uint dstlen;
            buf[0] = (byte)'#';
            buf[1] = (byte)'#';
            dstlen = len + 11;//计算总长度 
            buf[2] = (byte)(dstlen & 0xff);//长度低位字节在前 
            buf[3] = (byte)(dstlen >> 8);//长度高位字节在后 
            buf[4] = dId;   //外设类型
            buf[5] = mainCmd;// 主命令字 
            buf[6] = subCmd;// 从命令字 
            for (int i = 0; i < len; i++) buf[7 + i] = src[i];
            for (int i = 0; i < (dstlen - 4); i++) crc ^= buf[i];
            buf[dstlen - 4] = (byte)(crc & 0xff);//长度低位字节在前 
            buf[dstlen - 3] = (byte)(crc >> 8);//长度高位字节在后 
            buf[dstlen - 2] = 0x0d;//结束符 
            buf[dstlen - 1] = 0x0a;//结束符 
            return buf;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            byte[] Data = new byte[8];
            byte[] Buf= CreateBag(0x0b, 0x40, 0x02, Data, 8);
            textBox4.Text = Tools.HexToString(Buf, Buf.Length);
        }
    }
}
