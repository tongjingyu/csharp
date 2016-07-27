using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.Windows.Forms;
using System.Threading;

namespace 模拟键盘
{
    public class Usart
    {
        public static int Delay = 10;
        public static bool Busy = false;
        public static byte[] ThreadTxBuffer=new byte[100];
        public static byte[] ThreadRxBuffer = new byte[100];
        public static int ThreadTxBufferSize;
        public static int ThreadRxBufferSize;
        public static MB ThreadRxModBusMsg;
        public static MB ThreadTxModBusMsg;
        public static int SendDataOne(SerialPort CommPort, byte[] TxBuffer, int TxLength, ref byte[] RxBuffer, int RxLength)
        {
            while (true)
            {
                Thread.Sleep(1);
                if (Busy == false) { Busy = true; break; }
            }
            int ReadByteSCount = 0,Old_Count=0;
            try
            {
                CommPort.DiscardInBuffer();
                CommPort.Write(TxBuffer, 0, TxLength);                               //写命令
                if (TxBuffer[1] == ModBusClass.BroadAddr) { Busy = false; return 0; }                               //对于子广播命令,发送命令后直接返回
                ReadByteSCount = 0;
                for (int i = 0; i < 30; i++)
                {
                    ReadByteSCount = CommPort.BytesToRead;
                    if (ReadByteSCount != Old_Count) i = 0;
                    else i +=7;
                    if (ReadByteSCount > 9) i+= 15;
                    Old_Count = ReadByteSCount;
                    Thread.Sleep(Delay);
                }
                if (ReadByteSCount > RxLength) { Busy = false; return ReadByteSCount; }
                if (ReadByteSCount < 1) { Busy = false; return ReadByteSCount; }
                CommPort.Read(RxBuffer, 0, ReadByteSCount);
            }
            catch (Exception E)
            {
                MessageBox.Show(E.Message);
                Value.App_Run = false;
            }
            { Busy = false; return ReadByteSCount; }
        }
        public static int SendData(SerialPort CommPort, byte[] TxBuffer, int TxLength, ref byte[] RxBuffer, int RxLength)
        {
            int Length=0;
            MB TxModBus=new MB();
            MB RxModBus = new MB();
            ModBusClass.ModBusCoppyCreate(ref ModBusClass.DefMoBus, ref TxModBus);
            ModBusClass.ModBusCoppyCreate(ref ModBusClass.DefMoBus, ref RxModBus);
            for (int i = 0; i <10; i++)
            {
                Length=SendDataOne(CommPort, TxBuffer, TxLength,ref RxBuffer, RxLength);
                ModBusClass.ModBus_Expend(RxBuffer, Length, ref RxModBus);
                if (RxModBus.ErrorFlag == ModBusClass.ModBus_Ok) return Length;
            }
            MessageBox.Show("重试次数过多");
            return Length;
        }

    }

    
}
