using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.IO.Ports;
namespace CAN分析
{
    public class Usart
    {
        private System.IO.Ports.SerialPort SP;
        public static bool Busy = false;
        public static int Delay = 40;
        public Usart(System.IO.Ports.SerialPort SP)
        {
            this.SP = SP;
        }
        public void WriteBuffer(byte[] Buf, int Length)
        {
            SP.DiscardInBuffer();
            SP.Write(Buf, 0, Length);
            Thread.Sleep(40);
        }
        public byte[] ReadBuffer()
        {
            byte[] Buffer_Get = null;
            Thread.Sleep(1000);
            int N = SP.BytesToRead;
            Buffer_Get = new byte[N];
            SP.Read(Buffer_Get, 0, N);
            return Buffer_Get;
        }
        public static int SendDataOne(SerialPort CommPort, byte[] TxBuffer, int TxLength, ref byte[] RxBuffer, int RxLength)
        {
            while (true)
            {
                Thread.Sleep(1);
                if (Busy == false) { Busy = true; break; }
            }
            int ReadByteSCount = 0, Old_Count = 0;
            try
            {
                CommPort.DiscardInBuffer();
                CommPort.Write(TxBuffer, 0, TxLength);                               //写命令
                ReadByteSCount = 0;
                for (int i = 0; i < 30; i++)
                {
                    ReadByteSCount = CommPort.BytesToRead;
                    if (ReadByteSCount != Old_Count) i = 0;
                    else i += 7;
                    // if (ReadByteSCount > 9) i += 15;
                    Old_Count = ReadByteSCount;
                    Thread.Sleep(Delay);
                }
                MessageBox.Show(ReadByteSCount.ToString());
                if (ReadByteSCount > RxLength) { Busy = false; return ReadByteSCount; }
                if (ReadByteSCount < 1) { Busy = false; return ReadByteSCount; }
                CommPort.Read(RxBuffer, 0, ReadByteSCount);

            }
            catch (Exception E)
            {
                MessageBox.Show(E.Message);
            }
            { Busy = false; return ReadByteSCount; }
        }
        public int SendDataOne(byte[] TxBuffer, int TxLength, ref byte[] RxBuffer, int RxLength)
        {
            while (true)
            {
                if (Busy == false) { Busy = true; break; }
                Thread.Sleep(1);
            }
            int ReadByteSCount = 0, Old_Count = 0;
            try
            {
                SP.DiscardInBuffer();
                SP.Write(TxBuffer, 0, TxLength);                               //写命令
                ReadByteSCount = 0;
                for (int i = 0; i < 30; i++)
                {
                    ReadByteSCount = SP.BytesToRead;
                    //if (ReadByteSCount != Old_Count) i = 0;
                    //else i += 2;
                    //if (ReadByteSCount > 9) i += 15;
                    //Old_Count = ReadByteSCount;
                    Thread.Sleep(Delay);
                }
                if (ReadByteSCount > RxLength) { Busy = false; return ReadByteSCount; }
                if (ReadByteSCount < 1) { Busy = false; return ReadByteSCount; }
                SP.Read(RxBuffer, 0, ReadByteSCount);
            }
            catch (Exception E)
            {
                MessageBox.Show(E.Message);
            }
            { Busy = false; return ReadByteSCount; }
        }
        public static int SendData(SerialPort CommPort, byte[] TxBuffer, int TxLength, ref byte[] RxBuffer, int RxLength)
        {
            int Length = 0;
            for (int n = 0; n < RxLength; n++) RxBuffer[n] = 0;
            if (!CommPort.IsOpen) return 0;
            for (int i = 0; i < 10; i++)
            {
                Length = SendDataOne(CommPort, TxBuffer, TxLength, ref RxBuffer, RxLength);
                if (ZigBeeBus.ZigBee_CheckCrc(RxBuffer)) return Length;
            }
            MessageBox.Show("重试次数过多");
            return Length;
        }
        public int SendData(byte[] TxBuffer, int TxLength, ref byte[] RxBuffer, int RxLength)
        {
            int Length = 0;
            for (int n = 0; n < RxLength; n++) RxBuffer[n] = 0;
            if (!SP.IsOpen) return 0;
            for (int i = 0; i < 10; i++)
            {

                Length = SendDataOne(TxBuffer, TxLength, ref RxBuffer, RxLength);
                if (ZigBeeBus.ZigBee_CheckCrc(RxBuffer)) return Length;
            }
            MessageBox.Show("重试次数过多");
            return Length;
        }
    }
}
