using System;
using System.IO.Ports;
using System.Net.NetworkInformation;

namespace DTU配置工具V1._0
{
    /// <summary>
    /// ModBus 的摘要说明。
    /// </summary>
    public class ModBusProc
    {

        /*--------------------------------------------------------------------------------------------------
         Func: MODBUS协议定义
         Time: 2010-8-10
         Ver.: V1.0
         Note: 												   	   
        --------------------------------------------------------------------------------------------------*/
        public const int CmdCode_Read = 0x03;								    //Modbus读变量命令码
        public const int CmdCode_Write = 0x10;									//Modbus写命令命令码

        public const int MsgSuccess = 0x00;
        public const int CmdCodeError = 0x01;
        public const int DataLenthError = 0x02;
        public const int CrcCheckError = 0x03;
        public const int RegRangeError = 0x04;
        public const int InvalidOperation = 0x05;


        public const bool HostMode = true;
        public const bool SlaveMode = false;

        /*--------------------------------------------------------------------------------------------------
         Func: MODBUS报文结构							   	   
        --------------------------------------------------------------------------------------------------*/
        public struct ModbusMsg
        {
            public int DevAddr;
            public int CmdCode;
            public int RegStart;
            public int RegCount;
            public int DataLenth;
            public byte[] DataBuffer;
        };

        public struct Modbus
        {
            public int LocalAddr;
            public bool LocalMode;
            public ModbusMsg RxMsg;
            public ModbusMsg TxMsg;
        };


        public ModBusProc()
        {

        }



        /*--------------------------------------------------------------------------------------------------
         Func: 初始化MODBUS发送报文结构
         Time: 2010-6-29
         Ver.: V1.0
         Note: 												   	   
        --------------------------------------------------------------------------------------------------*/
        public static void CreateTxHeader(ref Modbus CurModbus, int DevAddr, int CmdCode, int RegStart, int RegCount, byte[] DataBuffer, int DataLenth)
        {
            CurModbus.TxMsg.DevAddr = DevAddr;
            CurModbus.TxMsg.CmdCode = CmdCode;
            CurModbus.TxMsg.RegStart = RegStart;
            CurModbus.TxMsg.RegCount = RegCount;
            for (int i = 0; i < DataLenth; i++) CurModbus.TxMsg.DataBuffer[i] = DataBuffer[i];
            CurModbus.TxMsg.DataLenth = DataLenth;
        }


        /*--------------------------------------------------------------------------------------------------
         Func: 生成ModBus数据报文
         Time: 2010-6-28
         Ver.: V1.0
         Note: 
               本方法对于主机: 生成查询报文
               本方法对于从机: 若TxMsg->CmdCode为读命令,则生成返回数据报文
                               若TxMsg->CmdCode为写命令,则生成应答报文
                               若收到非法不可执行命令,则生成错误码状态报文
       
               本方法根据Msg->DataLenth指定生成是否含Datas域
               Infor: DevAddr[1]+CmdCode[1]+StartReg[2]+RegCount[2]+CRC16[2]
               Input: *Msg/报文结构	 *Buffer/报文帧数据缓冲区
        --------------------------------------------------------------------------------------------------*/
        public static int CreateTxMessage(ref Modbus CurModbus, ref byte[] TxBuffer)
        {
            int L = 0;
            TxBuffer[0] = (byte)CurModbus.TxMsg.DevAddr;												//写入设备地址字节
            TxBuffer[1] = (byte)CurModbus.TxMsg.CmdCode;												//写入命令码字节
            if (CurModbus.LocalMode == HostMode)
            {
                TxBuffer[2] = (byte)(CurModbus.TxMsg.RegStart >> 8);
                TxBuffer[3] = (byte)(CurModbus.TxMsg.RegStart);           							//写入起始地址
                TxBuffer[4] = (byte)(CurModbus.TxMsg.RegCount >> 8);
                TxBuffer[5] = (byte)(CurModbus.TxMsg.RegCount);           							//写入寄存器个数
                if (CurModbus.TxMsg.CmdCode == 0x03)
                {
                    L = 6;
                }
                else
                {
                    TxBuffer[6] = (byte)(CurModbus.TxMsg.DataLenth);                                    //写入数据长度
                    L = 7;
                }
                for (int i = 0; i < CurModbus.TxMsg.DataLenth; i++) TxBuffer[L + i] = CurModbus.TxMsg.DataBuffer[i];			//写入数据域
                L += CurModbus.TxMsg.DataLenth;

            }
            uint T = CheckTools.GetCrc16(TxBuffer, L);          								//求本帧数据校验码
            TxBuffer[L] = (byte)(T);
            TxBuffer[L + 1] = (byte)(T >> 8);   										//写入CRC16到帧尾部
            return L + 2;								                				//设定发送帧长度
        }


        /*--------------------------------------------------------------------------------------------------
         Func: 截取请求报文中的数据段
         Time: 2010-6-28
         Ver.: V1.0
         Note: 本方法根据RegCount和Lenth自动判断是否有合适的Datas域
               Infor:	DevAddr[1]+CmdCode[1]+StartReg[2]+RegCount[2]+Datas[RegCount*2]+CRC16[2]
               Input:	*Data/原如数据报文  Lenth/报文总长度  *Msg/返回的报文信息
               return:  0/报文信息提取成功	11/报文长度不足	12/报文数据校验错误	13/数据报分长度错误 0xFF/地址不匹配
        --------------------------------------------------------------------------------------------------*/
        public static int ExpendRxMessage(ref Modbus CurModbus, byte[] RxBuffer, int RxLenth)
        {
            int T, P;
            if (CurModbus.LocalMode != HostMode)
            {
                if ((RxBuffer[0]) != CurModbus.LocalAddr) return 0xFF;
            }
            if (RxLenth < 5) return DataLenthError;			        					//判断报文长度是否正常
            T = RxBuffer[RxLenth - 1]; T <<= 8;
            T |= (int)RxBuffer[RxLenth - 2];
            P = (int)CheckTools.GetCrc16(RxBuffer, RxLenth - 2);						//获取报文CRC16检验码
            if (T != P) return CrcCheckError;	        								//检验CRC16码是否正确
            CurModbus.RxMsg.DevAddr = RxBuffer[0];												//取设备地址
            CurModbus.RxMsg.CmdCode = RxBuffer[1];												//取命令码
            if (CurModbus.LocalMode == HostMode)
            {
                if (CurModbus.RxMsg.CmdCode == CmdCode_Read)
                {
                    CurModbus.RxMsg.DataLenth = RxBuffer[2];
                    for (int i = 0; i < CurModbus.TxMsg.RegCount * 2; i++) CurModbus.RxMsg.DataBuffer[i] = RxBuffer[i + 3];
                }
                else if ((CurModbus.RxMsg.CmdCode & 0x80) == 0x80)
                {
                    CurModbus.RxMsg.DataLenth = 1;
                    CurModbus.RxMsg.DataBuffer[0] = RxBuffer[2];
                }
                else if (CurModbus.RxMsg.CmdCode == CmdCode_Write)
                {
                    CurModbus.RxMsg.DataLenth = RxBuffer[2];
                }
            }
            return MsgSuccess;
        }


        /*--------------------------------------------------------------------------------------------------
         Func: MODBUS协议创建初始化
         Time: 2010-6-29
         Ver.: V1.0
         Note:
        --------------------------------------------------------------------------------------------------*/
        public static void Modbus_Create(ref Modbus CurModbus, int CurAddr, bool CurLocalMode)
        {
            CurModbus.LocalAddr = CurAddr;
            CurModbus.LocalMode = CurLocalMode;
            CurModbus.TxMsg = new ModbusMsg();
            CurModbus.TxMsg.CmdCode = 0;
            CurModbus.TxMsg.DataLenth = 0;
            CurModbus.TxMsg.DevAddr = 0;
            CurModbus.TxMsg.RegStart = 0;
            CurModbus.TxMsg.RegCount = 0;
            CurModbus.TxMsg.DataBuffer = new byte[300];
            CurModbus.RxMsg = new ModbusMsg();
            CurModbus.RxMsg.CmdCode = 0;
            CurModbus.RxMsg.DataLenth = 0;
            CurModbus.RxMsg.DevAddr = 0;
            CurModbus.RxMsg.RegStart = 0;
            CurModbus.RxMsg.RegCount = 0;
            CurModbus.RxMsg.DataBuffer = new byte[300];
        }




    }
}

