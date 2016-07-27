using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Server
{
    enum ACFF
    {
        SCFF_GetSensorTest = 0,
    };
    class ZBUS
    {
        public static bool ZBUS_CheckCrc(byte[] Buf)
        {
            try
            {
                int CrcValue, CrcSource, Length;
                Length = (int)Buf[3];
                Length<<=8;
                Length+=(int)Buf[4] + 7;
                CrcValue = Tools.GetCrc16(Buf, 2, (Length - 4));
                CrcSource = Buf[Length - 1];
                CrcSource <<= 8;
                CrcSource |= Buf[Length - 2];
                if (CrcSource == CrcValue) return true;
            }
            catch (Exception E) { Console.WriteLine(E.Message); }
            return false;
        }
        public static int ZBUS_AppendCrc(ref byte[] Buf, int Length)
        {
            int CrcValue;
            CrcValue = Tools.GetCrc16(Buf, 2, Length - 2);
            Buf[Length++] = (byte)(CrcValue & 0xff);
            Buf[Length++] = (byte)(CrcValue >> 8);
            return Length;
        }
        public static int ZBUS_SendMsg(ref byte[] Buf, int OnlyAddr, byte Cmd, byte[] Data, int DataLegnth)
        {
            int i = 0, n;
            Buf[i++] = (byte)(OnlyAddr / 0xff);
            Buf[i++] = (byte)(OnlyAddr % 0xff);
            Buf[i++] = Cmd;
            Buf[i++] = (byte)(DataLegnth /0xff);
            Buf[i++] = (byte)(DataLegnth %0xff);
            for (n = 0; n < DataLegnth; n++) Buf[n + i] = Data[n];
            i += DataLegnth;
            i = ZBUS_AppendCrc(ref Buf, i);
            return i;
        }
    }
}
