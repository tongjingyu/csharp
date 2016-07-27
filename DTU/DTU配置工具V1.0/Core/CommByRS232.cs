using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.Windows.Forms;


namespace DTU配置工具V1._0
{
    public class CommByRS232
    {

        public static SerialPort CommPort = new SerialPort();
        public static ModBusProc.Modbus Modbus232 = new ModBusProc.Modbus();

        public struct CmdStruct
        {
            public int CmdKey;
            public int CmdType;
            public int CmdParam;
            public int CmdDataH;
            public int CmdDataL;
            public int CmdCheck;
        };



        #region 串口初始化
        public static bool InitCommPort(string CommName, int BaudRate, int LocalAddr, bool LocalMode)
        {
            ModBusProc.Modbus_Create(ref Modbus232, LocalAddr, LocalMode);
            try
            {
                if (CommPort.IsOpen) CommPort.Close();
                CommPort.PortName = CommName;
                CommPort.BaudRate = BaudRate;
                CommPort.DataBits = 8;
                CommPort.Parity = Parity.None;
                CommPort.StopBits = StopBits.One;
                CommPort.Open();
            }
            catch (Exception E)
            {
                MessageBox.Show("错误：" + E.Message, "串口打开错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return CommPort.IsOpen;
        }
        #endregion

        #region 发送命令
        public static bool SendCommand(int DevAddr, int CmdType, int CmdParam, int CmdData)
        {
            byte[] CmdDatas = new byte[12];
            CmdDatas[0] = 0x55;
            CmdDatas[1] = 0xAA;
            CmdDatas[2] = (byte)(CmdType >> 8);
            CmdDatas[3] = (byte)(CmdType & 0xFF);
            CmdDatas[4] = (byte)(CmdParam >> 8);
            CmdDatas[5] = (byte)(CmdParam & 0xFF);
            CmdDatas[6] = (byte)(CmdData >> 24);
            CmdDatas[7] = (byte)(CmdData >> 16);
            CmdDatas[8] = (byte)(CmdData >> 8);
            CmdDatas[9] = (byte)(CmdData & 0xFF);
            CmdDatas[10] = 0xAA;
            CmdDatas[11] = 0x55;
            bool R = WritePacket(DevAddr, 3000, 6, CmdDatas, 12);
            return R;

        }
        #endregion


        #region 发送信息
        public static bool WritePacket(int DevAddr, int StartReg, int RegCount, byte[] Datas, int DataLength)
        {
            if (CommByRS232.CommPort.IsOpen == false)
            {
                MessageBox.Show("端口未打开,请先打开端口!");
                return false;
            }

            ModBusProc.CreateTxHeader(ref Modbus232, DevAddr, ModBusProc.CmdCode_Write, StartReg, RegCount, Datas, DataLength);
            byte[] TxBuffer = new byte[320];
            int TxLength = ModBusProc.CreateTxMessage(ref Modbus232, ref TxBuffer);
            return SendData(TxBuffer, TxLength, 8);
        }
        #endregion

        #region 接收信息
        public static bool ReadPacket(int DevAddr, int StartReg, int RegCount, ref byte[] Datas, ref int DataLength)
        {
            if (CommByRS232.CommPort.IsOpen == false)
            {
                MessageBox.Show("端口未打开,请先打开端口!");
                return false;
            }

            bool R;
            ModBusProc.CreateTxHeader(ref Modbus232, DevAddr, ModBusProc.CmdCode_Read, StartReg, RegCount, Datas, 0);
            byte[] TxBuffer = new byte[320];
            int TxLength = ModBusProc.CreateTxMessage(ref Modbus232, ref TxBuffer);
            R = SendData(TxBuffer, TxLength, 5 + RegCount * 2);
            if (R == true)
            {
                Array.Copy(Modbus232.RxMsg.DataBuffer, Datas, Modbus232.RxMsg.DataLenth);
                DataLength = Modbus232.RxMsg.DataLenth;
            }
            return R;
        }
        #endregion


        #region 发送信息
        public static bool SendData(byte[] TxBuffer, int TxLength, int RxLength)
        {
            int ReadByteSCount = 0, LeftByteSCount;
            int RetryTimes;
            byte[] ReadDatas = new byte[320];
            for (RetryTimes = 0; RetryTimes < 3; RetryTimes++)
            {
                try
                {
                    CommPort.DiscardOutBuffer();
                    CommPort.DiscardInBuffer();
                    CommPort.ReadTimeout = 400;                                          //200ms超时
                    CommPort.Write(TxBuffer, 0, TxLength);                               //写命令
                    if (TxBuffer[0] == 0xFE) return true;                                //对于子广播命令,发送命令后直接返回
                    ReadByteSCount = 0;
                    LeftByteSCount = RxLength;
                    if (TxBuffer[0] != 0xFE)
                    {
                        while (ReadByteSCount < RxLength)
                        {                                                                    //开始读取数据
                            ReadByteSCount += CommPort.Read(ReadDatas, ReadByteSCount, LeftByteSCount);
                            LeftByteSCount = RxLength - ReadByteSCount;
                        }
                        if (ReadByteSCount >= RxLength) RetryTimes = 1000;
                    }
                    else break;
                }
                catch (TimeoutException)                                                 //读取超时
                {

                }
            }
            if (ReadByteSCount >= 5)
            {
                int Result = ModBusProc.ExpendRxMessage(ref Modbus232, ReadDatas, ReadByteSCount);
                if (Result == ModBusProc.MsgSuccess)
                {
                    return true;
                }
            }
            if (TxBuffer[0] == 0xFE) return true;                       //广播地址总是返回正确
            return false;
        }
        #endregion

    }

}