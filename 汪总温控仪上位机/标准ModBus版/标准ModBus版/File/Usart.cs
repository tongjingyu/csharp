using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.Windows.Forms;
using System.Threading;

namespace 标准ModBus版
{
    public class Usart
    {
        public static int Delay = 100;
        public static bool Busy = false;

        public static int SendDataOne(SerialPort CommPort, byte[] TxBuffer, int TxLength, ref byte[] RxBuffer, int RxLength)
        {
            while (true)
            {
                Thread.Sleep(1);
                if (Busy == false) { Busy = true; break; }
                if (!Value.App_Run) return 0;
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
                    if (!Value.App_Run) return 0;
                    Thread.Sleep(Delay);
                    if (!Value.App_Run) return 0;
                }
                if (ReadByteSCount > RxLength) { Busy = false; return ReadByteSCount; }
                if (ReadByteSCount < 1) { Busy = false; return ReadByteSCount; }
                CommPort.Read(RxBuffer, 0, ReadByteSCount);
            }
            catch (Exception E)
            {
                if (!Value.App_Run) return 0;
                MessageBox.Show(E.Message);
                Value.App_Run = false;
            }
            { Busy = false; return ReadByteSCount; }
        }
        public static int SendData(SerialPort CommPort, byte[] TxBuffer, int TxLength, ref byte[] RxBuffer, int RxLength)
        {
            int Length=0;
            MODBUS_TX_MSG MBTM = new MODBUS_TX_MSG();
            MBTM.Buf = new byte[100];
            for (int i = 0; i <2; i++)
            {
                Length=SendDataOne(CommPort, TxBuffer, TxLength,ref RxBuffer, RxLength);
              if(标准ModBus.Export_ModBus(ref MBTM, ref RxBuffer)!=-1)break;
               
            }
          //  MessageBox.Show("重试次数过多");
            return Length;
        }

    }

    
}
