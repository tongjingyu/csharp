using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



namespace 串口显示屏
{


    public struct MB
    {
        public byte Start;
        public byte HostAddr;
        public byte TargetAddr;
        public byte MsgFlag;
        public byte MsgNum;
        public int MsgLength;
        public byte DataFlag;
        public int DataLength;
        public byte[] Data;
        public byte XorValue;
        public byte End;
        public byte ErrorFlag;

    };
    enum MsgFlag
    {
        ReadScreenXline=0x01,
        ReadScreenYLine=0x02,
        WriteScreenXLine=0x03,
        WriteScreenYLine=0x04
    };
    enum Status
    {
        Succeed=0,
        Fail=1,
        TryAgain=2
    }
    class ModBusClass
    {

        public const int HostAddr = 0x01;
        public const int TargetAddr = 0x94;
        public const int StartFlag = 0x7e;
        public const int EndFlag = 0x7f;
        public const int BroadAddr = 0xff;
       
        public static int ModBus_CreatBuf(ref MB ModBus, ref byte[] Buf)
        {
            int i = 0;
            if (ModBus.DataLength>0) ModBus.MsgLength = ModBus.DataLength + 3;
            else ModBus.MsgLength = 0;

            Buf[i++] = ModBus.Start;
            Buf[i++] = ModBus.TargetAddr;
            Buf[i++] = ModBus.HostAddr;
            Buf[i++] = ModBus.MsgFlag;
            Buf[i++] = ModBus.MsgNum;
            Buf[i++] = (byte)(ModBus.MsgLength >> 8);
            Buf[i++] = (byte)(ModBus.MsgLength & 0xff);
            if (ModBus.MsgLength>0)
            {
                Buf[i++] = (byte)(ModBus.DataFlag);
                Buf[i++] = (byte)(ModBus.DataLength >> 8);
                Buf[i++] = (byte)(ModBus.DataLength & 0xff);
                for (int n = 0; n < ModBus.DataLength; n++) Buf[i++] = ModBus.Data[n];
            }
            ModBus.XorValue = Tools.Xor(Buf, i);
            Buf[i++] = ModBus.XorValue;
            Buf[i++] = EndFlag;
            return i;
        }
        public static void ModBus_CreatStruct(ref MB ModBus, byte[] Buf)
        {
            int i = 0;
            ModBus.ErrorFlag = 0;
            ModBus.Start = Buf[i++];
            ModBus.TargetAddr = Buf[i++];
            ModBus.HostAddr = Buf[i++];
            ModBus.MsgFlag = Buf[i++];
            ModBus.MsgNum = Buf[i++];
            ModBus.MsgLength = Buf[i++];
            ModBus.MsgLength <<= 8;
            ModBus.MsgLength |= Buf[i++];
            if (ModBus.MsgLength > 0)
            {
                ModBus.DataFlag = Buf[i++];
                ModBus.DataLength = Buf[i++];
                ModBus.DataLength <<= 8;
                ModBus.DataLength |= Buf[i++];
                ModBus.Data = new byte[ModBus.DataLength];
                try
                {
                    for (int n = 0; n < ModBus.DataLength; n++) ModBus.Data[n] = Buf[i++];
                }
                catch { }
            }
            
            ModBus.XorValue = Buf[i++];
            ModBus.End = Buf[i++];
        }
        public static bool CheakXor(MB ModBuf)
        {
            byte[] Buf = new byte[100];
            ModBus_CreatBuf(ref ModBuf,ref Buf);
            byte XorValue = Tools.Xor(Buf, ModBuf.MsgLength + 7);
            if (XorValue == ModBuf.XorValue) return true;
            else return false;
        }
        public static void ModBus_Create(ref MB ModBus, byte HostAddr, byte Target, byte StartFlag, byte EndFlag)
        {
            ModBus.HostAddr = HostAddr;
            ModBus.TargetAddr = TargetAddr;
            ModBus.Start = StartFlag;
            ModBus.End = EndFlag;
        }

    }
}
