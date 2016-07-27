using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
namespace 串口显示屏
{
    class Tools
    {

        public static byte Xor(byte[] Buf,int Length)
        {
            byte Temp=0;
            for(int i=0;i<Length;i++)
            {
              Temp^=Buf[i];
            }
            return Temp;
        }
        public static String HexToString(byte[] str,int Length)
        {
            string String = "";
            for (int i = 0; i < Length; i++)
            {
                String += str[i].ToString("X2");
                if (str[i] == ModBusClass.EndFlag) return String;
                String += " ";
            }
            return String;
        }
        public static byte[] StringToHex(string s)
        {
            s = s.Replace(" ", "");
            if ((s.Length % 2) != 0)
            {
                s += "";
            }
            byte[] bytes = new byte[s.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                try
                {
                    bytes[i] = Convert.ToByte(s.Substring(i * 2, 2), 16);
                }
                catch (Exception E)
                {
                    
                }
            }
            return bytes;
        }
        public static byte BCD_ToTen(int bcd_data)
        {
            uint temp16, temp;
            uint data=(uint)bcd_data;
            temp16 = data >> 4;
            temp = 0x0f & data;
            data = (temp16 * 10) + temp;
            return (byte)data;
        }
        public static byte Ten_ToBCD(int hex_data)
        {
            uint temp10, temp;
            uint data = (uint)hex_data;
            temp10 = data % 100 / 10;
            temp = data % 10;
            data = temp10 << 4 | temp;
            return (byte)data;
        }
        public static byte[] DateTimeToBytes(DateTime DT)
        {
            byte[] Buf=new byte[8];
            Buf[0] = Ten_ToBCD(DT.Year / 100);
            Buf[1] = Ten_ToBCD(DT.Year % 100);
            Buf[2] = Ten_ToBCD(DT.Month);
            Buf[3] = Ten_ToBCD(DT.Day);
            Buf[4] = Ten_ToBCD(DT.Hour);
            Buf[5] = Ten_ToBCD(DT.Minute);
            Buf[6] = Ten_ToBCD(DT.Second);
            Buf[7] = Ten_ToBCD(Convert.ToInt32(DT.DayOfWeek));
            return Buf;  
        }
        public static byte[] Int16ToByte(int Data)
        {
            byte[] Buf = new byte[2];
            Buf[0] =(byte) (Data >> 8);
            Buf[1] = (byte)(Data & 0xff);
            return Buf;
        }
        public static int ByteToInt16(byte[] Data,int Index)
        {
            int Temp=0;
            Temp = Data[Index];
            Temp <<= 8;
            Temp |= Data[Index+1];
            return Temp;
        }
        public static Color GetColorFromTFT(int TFT)
        {
            int R, G, B;
            Color Temp = new Color();
            B = (TFT& 0x1F)<<3;
            B |= (B >> 5);
            G =((TFT>>5)&0x3f)<<2;
            G |= (G >> 6);
            R =((TFT >> 11) &0x1F)<<3;
            R |= (R >> 5);
            Temp = Color.FromArgb(255, R, G , B);
            return Temp;
        }
        public static int GetTftFromColor(Color C)
        {
            int Temp;
            Temp = (C.R >>3);
            Temp <<= 6;
            Temp |=(C.G >>2);
            Temp <<= 5;
            Temp |= (C.B >>3);
            return Temp;
        }
    }
}
