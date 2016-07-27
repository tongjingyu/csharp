using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using System.IO;
namespace Server
{
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
        #region 获取能作为服务器本机的IP地址和端口号
        public static IPEndPoint Get_IpPoint(int Point)
        {
            IPAddress IPAddr = IPAddress.Parse("127.0.0.1");
            IPAddress[] arrIPAddresses = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (IPAddress ip in arrIPAddresses)
            {
                if (ip.AddressFamily.Equals(AddressFamily.InterNetwork))
                {
                    IPAddr = ip;
                }
            }
            IPEndPoint Ipep = new IPEndPoint(IPAddr, Point);
            return Ipep;
        }
        #endregion
        #region 获取一组布尔型数据中为假的首个偏移地址
        public static int SearchEmpty(bool[] Tab)
        {
            for (int i = 0; i < Tab.Length; i++)
            {
                if (!Tab[i]) { return i; }
            }
            return Tab.Length + 1;
        }
        #endregion
        public static int SearchFull(bool[] Tab)
        {
            for (int i = 0; i < Tab.Length; i++)
            {
                if (Tab[i]) { return i; }
            }
            return Tab.Length + 1;
        }
        public static byte[] ByteFromU32(UInt32 Data, uint Mode)
        {
            Union temp;
            temp.B0 = 0;
            temp.B1 = 0;
            temp.B2 = 0;
            temp.B3 = 0;
            temp.U32 = Data;
            byte[] Buffer = new byte[4];
            int Offset = 0;
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
            return Buffer;
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
        public static byte[] GetCrc16byte(byte[] DataArray, int Offset, int DataLenth)
        {
            int crc=GetCrc16(DataArray, Offset, DataLenth);
            byte[] Crc = new byte[2];
            Crc[0] = (byte)(crc & 0xff);
            Crc[1] = (byte)(crc >> 8);
            return Crc;
        }
        public static string GetSqlString(Object Data)
        {
            try
            {
                if (Data.GetType() == typeof(String)) return "'" + (string)Data + "'";
                else return Data.ToString();
            }
            catch { return ""; }
        }
        public static void SetThreadOnFlag(ref bool[] Tab, int Index)
        {
            Tab[Index] = true;
        }
        public static void ClearThreadOnFlag(ref bool[] Tab, int Index)
        {
            Tab[Index] = false;
        }
        #region 获取空闲线程数
        public static int GetEmptpSize(bool[] Tab)
        {
            int Size = 0;
            for (int i = 0; i < Tab.Length; i++)
            {
                if (!Tab[i]) Size++;
            }
            return Size;
        }
        #endregion
        public static string DAT_GetString(byte[] Buffer, int Offset, int Count)
        {
            byte[] Temp = new byte[Count];
            for (int i = 0; i < Count; i++)
            {
                Temp[i] = Buffer[Offset + i];
            }
            string TempString = Encoding.GetEncoding("gb2312").GetString(Temp, 0, Count);
            return TempString;
        }
        public static DateTime DAT_GetTime(byte[] Buffer, int Offset)
        {
            int Year, Month, Day, Hour, Min, Sec;
            Year = Buffer[Offset] + Buffer[Offset + 1] * 256;
            Month = Buffer[Offset + 2];
            Day = Buffer[Offset + 3];
            Hour = Buffer[Offset + 4];
            Min = Buffer[Offset + 5];
            Sec = 0;
            if (Year > 2099) Year = 2001;
            if (Month > 12) Month = 1;
            if (Day > 31) Day = 1;
            if (Hour > 24) Hour = 1;
            if (Min > 59) Min = 1;
            DateTime DT = new DateTime(Year, Month, Day, Hour, Min, Sec);
            return DT;
        }
        public static byte[] DateTimeToBytes(DateTime dt)
        {
            byte[] bytes = new byte[7];
            if (dt != null)
            {
                bytes[0] = Convert.ToByte(dt.Year.ToString().Substring(2, 2), 10);
                bytes[1] = Convert.ToByte(dt.Month.ToString(), 10);
                bytes[2] = Convert.ToByte(dt.Day.ToString(), 10);
                bytes[3] = Convert.ToByte(dt.Hour.ToString(), 10);
                bytes[4] = Convert.ToByte(dt.Minute.ToString(), 10);
                bytes[5] = Convert.ToByte(dt.Second.ToString(), 10);
                bytes[6] = Convert.ToByte(((int)dt.DayOfWeek).ToString(), 10);
            }
            return bytes;
        }
        public static DateTime DAT_GetSpaceTime(byte[] Buffer, int Offset, int TimeSpce, int Count)
        {
            int Year, Month, Day, Hour, Min, Sec;
            Year = Buffer[Offset] + Buffer[Offset + 1] * 256;
            Month = Buffer[Offset + 2];
            Day = Buffer[Offset + 3];
            Hour = Buffer[Offset + 4];
            Min = Buffer[Offset + 5];
            Sec = 0;
            Min += (TimeSpce * Count);
            Hour += Min / 60;
            Min = Min % 60;
            if (Year > 2099) Year = 2001;
            if (Month > 12) Month = 1;
            if (Day > 31) Day = 1;
            if (Hour > 24) Hour = 1;
            if (Min > 59) Min = 1;
            DateTime DT = new DateTime(Year, Month, Day, Hour, Min, Sec);
            return DT;
        }
        public static string ConvertTo(Byte[] AData)
        {
            string String = "";
            for (int i = 0; i < AData.Length; i++)
            {
                String += AData[i].ToString("X2");
            }
            return String;
        } 
        public static void HaseSetJoin(string Key,HashSet<HashKey> hs,int i,string msg)
        {
            foreach (HashKey de in hs)
            {
                if (de.Key == Key) de.Str[i]= msg;
            }
        }
        public static string GetGBK(string Msg,int i)
        {
            byte[] B=new byte[2];
            B[0]=(byte)Convert.ToByte(Msg.Substring(i + 1, 2), 16);
            B[1] = (byte)Convert.ToByte(Msg.Substring(i + 4, 2), 16);
            string msg = Encoding.GetEncoding("GBK").GetString(B);
            return msg;
        }
        public static string GetPostValue(string msg,string Key)
        {
            bool RecordEn = false;
            StringBuilder sb=new StringBuilder();
            int i=msg.IndexOf(Key);
            if (i < 0) return "";
            while(msg[i]!='&')
            {
                switch(msg[i])
                {
                    case '=': RecordEn = true; i++; break;
                    case (char)0x00: goto Over;
                    case (char)0x20: goto Over;
                    case '%': if (RecordEn) sb.Append(GetGBK(msg,i)); i += 6; break;
                    case '&': goto Over;
                    default: if (RecordEn) sb.Append(msg[i]); i++; break;
                }
            }
        Over:
            Value.WriteLog.WriteLine("这里最牛:" + sb.ToString(), LogType.LT_Warning); 
            return sb.ToString();
        }
        public static void ReChange(string Msg)
        {
            try
            {
                if (Msg.IndexOf("ButtonSubmit2") > -1)
                {
                    HashKey hk = Tools.HaseGetKey(Html.GetHttpValue(Msg), Value.hs);
                    if (hk != null)
                    {

                        for (int i = 0; i < hk.Str.Length; i++)
                        {
                            string[] array = hk.Str[i].Split(':');
                            if (array.Length > 1) hk.Str[i] = array[0] + ":" + GetPostValue(Msg, "TextBox" + i + "=");
                        }
                        hk.Update = 1;
                    }
                }
                if (Msg.IndexOf("ButtonSubmit1") > -1)
                {
                    HashKey hk = Tools.HaseGetKey(Html.GetHttpValue(Msg), Value.hs);
                    if (hk != null)
                    {
                        hk.Update = 2;
                    }
                }
            }
            catch(Exception E) { Value.WriteLog.WriteLine("ReChange"+E.Message, LogType.LT_Warning); }
        }
        public static HashKey HaseGetKey(string Key, HashSet<HashKey> hs)
        {
            foreach (HashKey de in hs)
            {
                if (de.Key == Key) return de;
            }
            return null;
        }
        public static void RemoveHK(string Key)
        {
            try
            {
                foreach (HashKey de in Value.hs)
                {
                    if (de.OnlyTime == Key)
                    {
                        Value.hs.Remove(de);
                        return;
                    }
                }
            }catch{ }
        }
        public static string[] GetHaseKey(string Key,HashSet<HashKey> hs)
        {
            foreach (HashKey de in hs)
            {
                if (de.Key == Key)return de.Str;
            }
            return new string[0];
        }
        public static int DAT_GetD32FromArray(byte[] Datas, int Offset, int Mode)
        {
            int T = 0;
            if (Mode == 0)
            {    //小端格式
                T = Datas[Offset + 3]; T <<= 8;
                T |= Datas[Offset + 2]; T <<= 8;
                T |= Datas[Offset + 1]; T <<= 8;
                T |= Datas[Offset];
            }
            else if (Mode == 1)
            {   //大端格式
                T = Datas[Offset]; T <<= 8;
                T |= Datas[Offset + 1]; T <<= 8;
                T |= Datas[Offset + 2]; T <<= 8;
                T |= Datas[Offset + 3];
            }
            else if (Mode == 2)
            {
                T = Datas[Offset + 1]; T <<= 8;
                T |= Datas[Offset]; T <<= 8;
                T |= Datas[Offset + 3]; T <<= 8;
                T |= Datas[Offset + 2];
            }
            else if (Mode == 3)
            {
                T = Datas[Offset + 2]; T <<= 8;
                T |= Datas[Offset + 3]; T <<= 8;
                T |= Datas[Offset]; T <<= 8;
                T |= Datas[Offset + 1];
            }
            return T;
        }
        public static float ByteToFloat(byte[] Buffer, int Offset, int Mode)
        {
            Union temp;
            temp.F = 0;
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
            float F = temp.F;
            return F;
        }
        public static int ToUnsigned(int Nfm, int Nf)
        {
            int Temp = 0;
            if (Nfm > 0)
            {
                Temp = 0x07 & (~Nf) + 1;
            }
            else Temp = Nf;
            return Temp;
        }
        public static string DAT_GetNum(byte[] Buffer, int Offset, int Mode, int Type)
        {
            int Nf = Type & 0x07;
            int Nfm = Type & 0x08;
            Type = Type >> 4;
            string Data = "NULL";
            uint TempBF = 0;
            float TempF = 0;
            // Console.WriteLine("{0},{1},{2}",Nf, Nfm, Type);
            switch (Type)
            {
                case 0://浮点型
                    Data = (ByteToFloat(Buffer, Offset, Mode)).ToString();
                    break;
                case 1://有符号长整形
                    Int32 TempI32 = DAT_GetD32FromArray(Buffer, Offset, Mode);
                    TempBF = (uint)Math.Pow(10, ToUnsigned(Nfm, Nf));
                    TempF = TempI32;
                    if (Nfm == 8) TempF /= TempBF;
                    else TempF *= TempBF;
                    Data = (TempF).ToString();
                    break;
                case 9://无符号长整形
                    UInt32 TempU32 = (UInt32)DAT_GetD32FromArray(Buffer, Offset, Mode);
                    TempBF = (uint)Math.Pow(10, ToUnsigned(Nfm, Nf));
                    TempF = (float)TempU32;
                    if (Nfm == 8) TempF /= TempBF;
                    else TempF *= TempBF;
                    Data = (TempF).ToString();
                    break;
                default: break;
            }
            UInt32 Temp = (UInt32)DAT_GetD32FromArray(Buffer, Offset, Mode);
            if (Temp == 0x80000000) Data = "NULL";
            return Data;
        }
        public static bool IfNotFileCreat(string Path)
        {
            if (!Directory.Exists(Path))
            {
                try
                {
                    Directory.CreateDirectory(Path);
                    return true;
                }
                catch { return false; }
            }
            else return true;
        }
    }
}
