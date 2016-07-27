using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace 标准ModBus版
{
    //ModBus报文消息
    public struct MODBUS_RX_MSG
    {//内存考虑，只做从机模式，该结构体为主机到从机的报文
        public int Addr; //标准modbus里只有从机地址
        public byte FuncCode;//功能码
        public int Offset;//寄存器偏移位置
        public int Length;//寄存器数量
        public int CheckValue;
    };
    //modbus报文信息
    public struct MODBUS_TX_MSG
    {//内存考虑，只做从机模式 该结构体
        public byte Addr;//本地地址
        public byte FuncCode;//功能码
        public byte Length; //数据长度
        public byte[] Buf;
        public int CheckValue;
    };
    class 标准ModBus
    {
        public static int Export_ModBus(ref MODBUS_TX_MSG MBTM, ref byte[] Buf)
        {
            int i = 0;
            int CheckValue;
            MBTM.Addr=Buf[i++];
            MBTM.FuncCode=Buf[i++];
            MBTM.Length = Buf[i++];
            if (MBTM.Length > 100) return 0;
            for (int n = 0; n < (MBTM.Length); n++) MBTM.Buf[n] = Buf[n + i]; i += MBTM.Length;
            CheckValue = Tools.GetCrc16(Buf, i);
            MBTM.CheckValue=Buf[i+1];
            MBTM.CheckValue <<= 8;
            MBTM.CheckValue |= Buf[i];
            if (CheckValue == MBTM.CheckValue) return MBTM.Length;
            return -1;
        }
        public static int CreateMsg_ModBus(MODBUS_RX_MSG MBRM,ref byte[] Buf)
        {
            int i=0;
            Buf[i++]=(byte)MBRM.Addr;
            Buf[i++] = (byte)(MBRM.FuncCode);
            Buf[i++] = (byte)(MBRM.Offset >> 8);
            Buf[i++] = (byte)(MBRM.Offset & 0xff);
            
            Buf[i++] = (byte)(MBRM.Length >>8);
            Buf[i++] = (byte)(MBRM.Length & 0xff);
            MBRM.CheckValue=Tools.GetCrc16(Buf, i);
            Buf[i + 1] = (byte)(MBRM.CheckValue>>8);
            Buf[i] = (byte)(MBRM.CheckValue & 0xff);
            return (i + 2);
        }
    }
}
