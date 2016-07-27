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
//using System.Math;
namespace  RTUService

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
        public static string DAT_GetString(byte[] Buffer, int Offset,int Count)
        {
            byte[] Temp = new byte[Count];
            for (int i = 0; i < Count; i++)
            {
                Temp[i]=Buffer[Offset+i];
            }
            string TempString=Encoding.GetEncoding("gb2312").GetString(Temp, 0, Count);
            return TempString;
        }
        public static DateTime DAT_GetTime(byte[] Buffer,int Offset)
        {
            int Year, Month, Day, Hour, Min, Sec;
            Year = Buffer[Offset]+ Buffer[Offset + 1]* 256;
            Month=Buffer[Offset+2];
            Day = Buffer[Offset + 3];
            Hour = Buffer[Offset + 4];
            Min = Buffer[Offset + 5];
            Sec =0;
            if (Year > 2099) Year = 2001;
            if (Month > 12) Month = 1;
            if (Day > 31) Day = 1;
            if (Hour > 24) Hour = 1;
            if (Min > 59) Min = 1;
            DateTime DT = new DateTime(Year, Month, Day, Hour, Min, Sec);
            return DT;
        }
        public static DateTime DAT_GetSpaceTime(byte[] Buffer, int Offset,int TimeSpce,int Count)
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
        public static float ByteToFloat(byte[] Buffer,int Offset,int Mode)
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
                temp.B3 = Buffer[Offset+2];
                temp.B2 = Buffer[Offset +3];
                temp.B1 = Buffer[Offset + 0];
                temp.B0 = Buffer[Offset + 1];
            }
            if (Mode == 3)
            {
                temp.B3 = Buffer[Offset+1];
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
        public static string DAT_GetNum(byte[] Buffer, int Offset,int Mode,int Type)
        {
            int Nf = Type & 0x07;
            int Nfm = Type & 0x08;
            Type= Type >> 4;
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
                    TempBF = (uint)Math.Pow(10, ToUnsigned(Nfm,Nf));
                    TempF=TempI32;
                    if (Nfm==8)TempF /=TempBF;
                    else TempF *= TempBF;
                    Data = (TempF).ToString(); 
                    break;
                case 9://无符号长整形
                    UInt32 TempU32 =(UInt32)DAT_GetD32FromArray(Buffer, Offset, Mode);
                    TempBF = (uint)Math.Pow(10, ToUnsigned(Nfm, Nf));
                    TempF =(float)TempU32;
                    if (Nfm==8) TempF /= TempBF;
                    else TempF *= TempBF;
                    Data = (TempF).ToString();
                    break;
                default: break;
            }
            UInt32 Temp= (UInt32)DAT_GetD32FromArray(Buffer, Offset, Mode);
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
