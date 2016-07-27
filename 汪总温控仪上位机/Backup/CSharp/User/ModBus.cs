using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



namespace CSharp
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

    };
    enum MsgFlag
    {
        CheakDevice=0x01,
        TimeLock = 0x02,
        GetBusId =0x20,
        SetBusId=0x10,
        GetRecordCount=0x30,
        GetOneRecord=0x40,//获取某一次记录时间
        GetThisRecord= 0x50,//获取某一次记录时刻
        GetAllRecordRime=0x60,
        ClearHistory=0x70,
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
        public const int TargetAddr = 0x60;
        public const int StartFlag = 0x7e;
        public const int EndFlag = 0x7f;
        public const int BroadAddr = 0xff;
      
        public static void ModBus_CreatBuf(ref MB ModBus, ref byte[] Buf)
        {
            int i = 0;
            if (ModBus.Data == null)
            {
                ModBus.Data = new byte[ModBus.DataLength];
            }
            Buf[i++] = ModBus.Start;
            Buf[i++] = ModBus.TargetAddr;
            Buf[i++] = ModBus.HostAddr;
            Buf[i++] = ModBus.MsgFlag;
            Buf[i++] = ModBus.MsgNum;
            Buf[i++] = (byte)(ModBus.MsgLength >> 8);
            Buf[i++] = (byte)(ModBus.MsgLength & 0xff);
            if (ModBus.MsgLength >=3)
            {
                Buf[i++] = (byte)(ModBus.DataFlag);
                Buf[i++] = (byte)(ModBus.DataLength >> 8);
                Buf[i++] = (byte)(ModBus.DataLength & 0xff);
                for (int n = 0; n < ModBus.DataLength; n++) Buf[i++] = ModBus.Data[n];
            }
            ModBus.XorValue = Tools.Xor(Buf, i);
            Buf[i++] = ModBus.XorValue;
            Buf[i++] = EndFlag;
        }
        public static void ModBus_CreatStruct(ref MB ModBus, byte[] Buf)
        {
            int i = 0;
            ModBus.Start = Buf[i++];
            ModBus.TargetAddr = Buf[i++];
            ModBus.HostAddr = Buf[i++];
            ModBus.MsgFlag = Buf[i++];
            ModBus.MsgNum = Buf[i++];
            ModBus.MsgLength = Buf[i++];
            ModBus.MsgLength <<= 8;
            ModBus.MsgLength |= Buf[i++];
            if (ModBus.MsgLength >=3)
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

    }
}
