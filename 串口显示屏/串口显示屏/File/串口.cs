using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.Windows.Forms;

namespace 串口显示屏
{
    public class Usart
    {

        public static bool SendData(SerialPort CommPort,byte[] TxBuffer, int TxLength,ref byte[] RxBuffer,int RxLength)
        {
            int ReadByteSCount = 0, LeftByteSCount;
            int RetryTimes;
            byte[] ReadDatas = new byte[300];
            for (RetryTimes = 0; RetryTimes < 3; RetryTimes++)
            {
                try
                {
                    CommPort.DiscardOutBuffer();
                    CommPort.DiscardInBuffer();
                    CommPort.ReadTimeout = 200;                                  //200ms超时
                    CommPort.Write(TxBuffer, 0, TxLength);                               //写命令
                    if (TxBuffer[1] == ModBusClass.BroadAddr) return true;                                //对于子广播命令,发送命令后直接返回
                    ReadByteSCount = 0;
                    LeftByteSCount = RxLength;
                    if (TxBuffer[0] != ModBusClass.BroadAddr)
                    {
                        while (ReadByteSCount < RxLength)
                        {                                                                    //开始读取数据
                            ReadByteSCount += CommPort.Read(RxBuffer, ReadByteSCount, LeftByteSCount);
                            LeftByteSCount = RxLength - ReadByteSCount;
                        }
                        if (ReadByteSCount >= RxLength) return false;
                    }
                    else break;
                }
                catch (TimeoutException)                                                 //读取超时
                {

                }
            }
            if (TxBuffer[1] == 0xFF) return true;                       //广播地址总是返回正确
            if (ReadByteSCount > 8) return true;
            else return false;
        }
        public static bool ReadDatas(SerialPort CommPort, byte[] Buf)
        {
            CommPort.ReadBufferSize = 100000;
            
            return true;
        }

    }

    
}
