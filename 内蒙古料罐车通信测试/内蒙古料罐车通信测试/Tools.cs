using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
namespace 内蒙古料罐车通信测试
{
    class Fiter
    {
        public double Pool = 0;
        public int PoolIndex = 0;
        public int PoolSize;
        public Fiter(int Size)
        {
            PoolSize = Size;
        }
        public double FlowPoolFilter(float Data)
        {
            double Old = 0;
            if (PoolIndex > 0) Old = ((Pool) / (PoolIndex));
            Pool += (double)Data;
            if ((PoolIndex) <= PoolSize) (PoolIndex)++;
            else (Pool) -= Old;
            return (Pool) / (PoolIndex);
        }
    }
    class Tools
    {
        [StructLayout(LayoutKind.Explicit)]
        public struct Union
        {
            [FieldOffset(0)]
            public UInt32 U32;
            [FieldOffset(0)]
            public Int32 I32;
            [FieldOffset(0)]
            public float F;
            [FieldOffset(0)]
            public byte B0;
            [FieldOffset(1)]
            public byte B1;
            [FieldOffset(2)]
            public byte B2;
            [FieldOffset(3)]
            public byte B3;
        }
        public static float ByteToFloat(byte[] Buffer, int Offset, int Mode)
        {
            Union temp;
            temp.F = 0;
            float F = 0;
            try
            {
                if (Mode == 0)
                {
                    temp.B0 = Buffer[Offset];
                    temp.B1 = Buffer[Offset + 1];
                    temp.B2 = Buffer[Offset + 2];
                    temp.B3 = Buffer[Offset + 3];
                }
                if (Mode == 1)
                {
                    temp.B3 = Buffer[Offset];
                    temp.B2 = Buffer[Offset + 1];
                    temp.B1 = Buffer[Offset + 2];
                    temp.B0 = Buffer[Offset + 3];
                }
                if (Mode == 2)
                {
                    temp.B3 = Buffer[Offset + 2];
                    temp.B2 = Buffer[Offset + 3];
                    temp.B1 = Buffer[Offset + 0];
                    temp.B0 = Buffer[Offset + 1];
                }
                if (Mode == 3)
                {
                    temp.B3 = Buffer[Offset + 1];
                    temp.B2 = Buffer[Offset + 0];
                    temp.B1 = Buffer[Offset + 3];
                    temp.B0 = Buffer[Offset + 2];
                }
                F = temp.F;
            }
            catch { return 0; }
            return F;
        }

        public static bool CheakAscii(string Msg)
        {
            int A = 0, O = 0;
            for (int i = 0; i < Msg.Length; i++)
            {
                if (Msg[i] >= 0x20 & Msg[i] <= 0x7e) A++;
                else O++;
            }
            if (O < 2) return true;
            else return false;
        }
        public static string TryToString(string Msg)
        {
            if (CheakAscii(Msg)) return Msg;
            else return HexToString(System.Text.Encoding.ASCII.GetBytes(Msg), Msg.Length);
        }
        public static int ByteFromFloat(float Data, ref byte[] Buffer, int Offset, int Mode)
        {
            Union temp;
            temp.B0 = 0;
            temp.B1 = 0;
            temp.B2 = 0;
            temp.B3 = 0;
            temp.F = Data;
            if (Mode == 0)
            {
                Buffer[Offset] = temp.B0;
                Buffer[Offset + 1] = temp.B1;
                Buffer[Offset + 2] = temp.B2;
                Buffer[Offset + 3] = temp.B3;
            }
            if (Mode == 1)
            {
                Buffer[Offset] = temp.B3;
                Buffer[Offset + 1] = temp.B2;
                Buffer[Offset + 2] = temp.B1;
                temp.B0 = Buffer[Offset + 3];
            }
            if (Mode == 2)
            {
                Buffer[Offset + 2] = temp.B3;
                Buffer[Offset + 3] = temp.B2;
                Buffer[Offset + 0] = temp.B1;
                Buffer[Offset + 1] = temp.B0;
            }
            if (Mode == 3)
            {
                Buffer[Offset + 1] = temp.B3;
                Buffer[Offset + 0] = temp.B2;
                Buffer[Offset + 3] = temp.B1;
                Buffer[Offset + 2] = temp.B0;
            }
            return 4;
        }
        public static int ByteFromU32(UInt32 Data, ref byte[] Buffer, int Offset, int Mode)
        {
            Union temp;
            temp.B0 = 0;
            temp.B1 = 0;
            temp.B2 = 0;
            temp.B3 = 0;
            temp.U32 = Data;
            if (Mode == 0)
            {
                Buffer[Offset] = temp.B0;
                Buffer[Offset + 1] = temp.B1;
                Buffer[Offset + 2] = temp.B2;
                Buffer[Offset + 3] = temp.B3;
            }
            if (Mode == 1)
            {
                Buffer[Offset] = temp.B3;
                Buffer[Offset + 1] = temp.B2;
                Buffer[Offset + 2] = temp.B1;
                temp.B0 = Buffer[Offset + 3];
            }
            if (Mode == 2)
            {
                Buffer[Offset + 2] = temp.B3;
                Buffer[Offset + 3] = temp.B2;
                Buffer[Offset + 0] = temp.B1;
                Buffer[Offset + 1] = temp.B0;
            }
            if (Mode == 3)
            {
                Buffer[Offset + 1] = temp.B3;
                Buffer[Offset + 0] = temp.B2;
                Buffer[Offset + 3] = temp.B1;
                Buffer[Offset + 2] = temp.B0;
            }
            return 4;
        }
        public static byte Xor(byte[] Buf, int Length)
        {
            byte Temp = 0;
            for (int i = 0; i < Length; i++)
            {
                Temp ^= Buf[i];
            }
            return Temp;
        }
        public static String HexToString(byte[] str, int Length)
        {
            string String = "";
            for (int i = 0; i < Length; i++)
            {
                String += str[i].ToString("X2");
                String += " ";
            }
            return String;
        }
        public static byte[] StringToHex(string s)
        {
            s = s.Replace(" ", "");
            if (((s.Length - 1) % 2) != 0)
            {
                s += "0" + s;
            }
            byte[] bytes = new byte[s.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                try
                {
                    bytes[i] = Convert.ToByte(s.Substring(i * 2, 2), 16);
                }
                catch
                {

                }
            }
            return bytes;
        }
        public static byte BCD_ToTen(int bcd_data)
        {
            uint temp16, temp;
            uint data = (uint)bcd_data;
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
            byte[] Buf = new byte[8];
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
            Buf[0] = (byte)(Data >> 8);
            Buf[1] = (byte)(Data & 0xff);
            return Buf;
        }
        public static int ByteToInt16(byte[] Data, int Index, int Mode)
        {
            int Temp = 0;
            if (Mode == 0)
            {
                Temp = Data[Index];
                Temp <<= 8;
                Temp |= Data[Index + 1];
            }
            else if (Mode == 1)
            {
                Temp = Data[Index + 1];
                Temp <<= 8;
                Temp |= Data[Index];
            }
            return Temp;
        }
        public static Color GetColorFromTFT(int TFT)
        {
            int R, G, B;
            Color Temp = new Color();
            B = (TFT & 0x1F) << 3;
            B |= (B >> 5);
            G = ((TFT >> 5) & 0x3f) << 2;
            G |= (G >> 6);
            R = ((TFT >> 11) & 0x1F) << 3;
            R |= (R >> 5);
            Temp = Color.FromArgb(255, R, G, B);
            return Temp;
        }
        public static int GetTftFromColor(Color C)
        {
            int Temp;
            Temp = (C.R >> 3);
            Temp <<= 6;
            Temp |= (C.G >> 2);
            Temp <<= 5;
            Temp |= (C.B >> 3);
            return Temp;
        }
        public static string GetStringFromByte(byte[] Buffer)
        {
            string String;
            try
            {
                String = System.Text.Encoding.ASCII.GetString(Buffer);
            }
            catch { String = "NULL"; }
            return String;
        }
        public static void BufferCoppy(byte[] S, byte[] D, int offset, int Length)
        {
            int i = 0;
            while (Length-- > 0)
            {
                D[offset++] = S[i++];
            }
        }
        public static int GetCrc16(byte[] DataArray, int DataLenth)
        {
            int i, j, MyCRC = 0xFFFF;
            for (i = 0; i < DataLenth; i++)
            {
                MyCRC = (MyCRC & 0xFF00) | ((MyCRC & 0x00FF) ^ DataArray[i]);
                for (j = 1; j <= 8; j++)
                {
                    if ((MyCRC & 0x01) == 1) { MyCRC = (MyCRC >> 1); MyCRC ^= 0XA001; } else { MyCRC = (MyCRC >> 1); }
                }
            }
            return MyCRC;
        }
        public static int GetCrc16(byte[] DataArray, int Offset, int DataLenth)
        {
            int i, j, MyCRC = 0xFFFF;
            for (i = 0; i < DataLenth; i++)
            {
                MyCRC = (MyCRC & 0xFF00) | ((MyCRC & 0x00FF) ^ DataArray[i + Offset]);
                for (j = 1; j <= 8; j++)
                {
                    if ((MyCRC & 0x01) == 1) { MyCRC = (MyCRC >> 1); MyCRC ^= 0XA001; } else { MyCRC = (MyCRC >> 1); }
                }
            }
            return MyCRC;
        }
    }
}
