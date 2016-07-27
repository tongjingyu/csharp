using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace Geek_IDE
{
    enum ACFF
    {
        SCFF_GetSensorTest = 0,
        SCFF_GetSensorModel = 1,//获取传感器型号
        SCFF_GetSensorNumber = 2,//获取传感器编号
        SCFF_GetSensorName = 3,//获取传感器名称
        SCFF_GetSensorNote = 4,//获取传感器备注
        SCFF_GetBSD = 5,//获取板载信息
        SCFF_GetCPUModel = 6,//获取cpu型号
        SCFF_GetCPUID = 7,//获取cpuID
        SCFF_GetCANSpeed = 8,//获取CAN通信速率
        SCFF_GetSensorADC = 9,//获取角度模数值
        SCFF_GetSensorValue = 10,//获取角度值
        SCFF_SetSensorMax = 11,//校准大角度
        SCFF_SetSensorMin = 12,//校准小角度
        SCFF_InDeBug = 13,//进入调试模式
        SCFF_OutDeBug = 14,//退出调试模式
        SCFF_GetCANGroup = 15,//获取can通信组
        SCFF_GetCANStdId = 16,//获取can通信成员地址
        SCFF_SetCANGroup = 17,//设置can通信组
        SCFF_SetCANStdId = 18,//设置can通信成员地址
        SCFF_SetCTRBIT = 19,//设置继电器按位
        SCFF_ClrCTRBIT = 20,//清除继电器按位
        SCFF_SetCTRUINT32 = 21,//一次性设置继电器
        SCFF_GetSIGUINT32 = 22,//获取输入IO
        SCFF_ButtonClick = 23,//按钮事件
        SCFF_SetWriteAddr = 24,//写入写偏移地址
        SCFF_WriteBuffer = 25,//写入数组
        SCFF_SetChnenel = 26,//写入操作通道
        SCFF_CorrectWeight = 27,//校准重量
        SCFF_ClearWeight = 28,//清零重量
        SCFF_CorrectRange = 29,//校准幅度
        SCFF_ClearRange = 30,//清零幅度
        SCFF_ButtonOver = 31,//停止按按钮
        SCFF_GetAllSensorValue = 32,//获取所有传感器值	
        SCFF_GetStructOffSet = 33,//获取结构体偏移值
        SCFF_SetStructOffSet = 34,//设置结构体偏移值
        SCFF_RefurBishGet = 35,//下载数据
        SCFF_RefurBishSet = 36,//上传数据
        SCFF_SetSaveValue = 37,//保存参数
        SCFF_TankSIGUINT32 = 38,//罐子发送输入信号
        SCFF_TankSetCTRUINT32 = 39,//罐子继电器设置U32
        SCFF_RefurBishSet1 = 40,//上传数据
        SCFF_GoToBootLoader = 41,//进入下载模式
        SCFF_Write64Byte = 42,//写入64字节
        SCFF_Cheack1024Byte = 43,//校验1024字节
        SCFF_Write1024Byte = 44,//写入1024字节
        SCFF_Read64Byte = 45,//写入64字节
        SCFF_Read1024Byte = 46,//写入1024字节
        SCFF_EraseFlase = 47,//擦除flash
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
            CrcValue = Tools.GetCrc16(Buf, 2, Length - 2);
            Buf[Length++] = (byte)(CrcValue & 0xff);
            Buf[Length++] = (byte)(CrcValue >> 8);
            return Length;
        }
        public static int ZigBee_SendMsg(ref byte[] Buf, uint OnlyAddr, byte Cmd, byte[] Data, byte DataLegnth)
        {
            int i = 0, n;
            if (DataLegnth > 76) return 0;
            Buf[i++] = (byte)(OnlyAddr / 0xff);
            Buf[i++] = (byte)(OnlyAddr % 0xff);
            Buf[i++] = Cmd;
            Buf[i++] = DataLegnth;
            for (n = 0; n < DataLegnth; n++) Buf[n + i] = Data[n];
            i += n;
            i = ZigBee_AppendCrc(ref Buf, i);
            return i;
        }
        public static byte[] ZigBee_Write64Bytes(uint OnlyAddr, byte[] DataI, byte PageIndex)
        {
            byte[] Buf = new byte[1500];
            byte[] Data = new byte[65];
            Data[0] = PageIndex;
            for (int i = 0; i < 64; i++) Data[1 + i] = DataI[i];
            int Length = ZigBeeBus.ZigBee_SendMsg(ref Buf, OnlyAddr, (byte)ACFF.SCFF_Write64Byte, Data, (byte)Data.Length);
            ZigBeeBus.ZigBee_CheckCrc(Buf);
            return Buf;
        }
    }
}
