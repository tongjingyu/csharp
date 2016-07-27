using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace 模拟键盘
{
    enum ACFF
    {
        SCFF_GetSendKeyMsg=1,
    };
    class ZigBeeBus
    {
        public static bool ZigBee_CheckCrc(byte[] Buf)
        {
            try
            {
                int CrcValue, CrcSource, Length;
                Length = (int)Buf[3] + 6;
                CrcValue = Tools.GetCrc16(Buf, 2, (Length - 4));
                CrcSource = Buf[Length - 1];
                CrcSource <<= 8;
                CrcSource |= Buf[Length - 2];
                if (CrcSource == CrcValue) return true;
            }
            catch { }
            return false;
        }
        public static int ZigBee_AppendCrc(ref byte[] Buf, int Length)
        {
            int CrcValue;
            CrcValue = Tools.GetCrc16(Buf,2, Length - 2);
            Buf[Length++]=(byte)(CrcValue&0xff);
            Buf[Length++]=(byte)(CrcValue>>8);
            return Length;
        }
       public static int ZigBee_SendMsg(ref byte[] Buf, uint OnlyAddr, byte Cmd, byte[] Data, byte DataLegnth)
        {
            int i = 0,n;
            if (DataLegnth > 76) return 0;
            Buf[i++]=(byte)(OnlyAddr / 0xff);
            Buf[i++]=(byte)(OnlyAddr %0xff);
            Buf[i++] = Cmd;
            Buf[i++] = DataLegnth;
            for (n = 0; n < DataLegnth; n++) Buf[n + i] = Data[n];
            i += n;
            i = ZigBee_AppendCrc(ref Buf, i);
            return i;
        }
        public static byte[] ZigBee_WriteKeyMsg(uint OnlyAddr, byte[] Data,uint L)
        {
            byte[] Buf = new byte[1500];
            int Length = ZigBeeBus.ZigBee_SendMsg(ref Buf, OnlyAddr, (byte)ACFF.SCFF_GetSendKeyMsg, Data, (byte)L);
            ZigBeeBus.ZigBee_CheckCrc(Buf);
            byte[] Temp = new byte[Length];
            for (int i = 0; i < Length; i++) Temp[i] = Buf[i];
            return Buf;
        }
    }
}
