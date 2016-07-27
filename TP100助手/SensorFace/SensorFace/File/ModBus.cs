using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace SensorFace
{

    
    public struct MB
    {
        public byte Start;
        public byte MasterAddr;
        public byte SlaveAddr;
        public byte MsgFlag;
        public byte MsgNum;
        public int MsgLength;
        public byte DataFlag;
        public int DataLength;
        public byte[] Data;
        public byte XorValue;
        public int CrcValue;
        public int CheakMode;
        public byte End;
        public byte ErrorFlag;
        public byte WorkMode;

    };
    enum MsgFlag
    {
        ReadScreenXline=0x01,
        ReadScreenYLine=0x02,
        WriteScreenXLine=0x03,
        WriteScreenYLine=0x04
    };
     enum ACFF
    {
        SCFF_GetSensorTest = 0,
        SCFF_GetSensorModel = 1,//获取传感器型号
        SCFF_GetSensorNumber = 2,//获取传感器编号
        SCFF_GetSensorName = 3,//获取传感器名称
        SCFF_GetSensorNote = 4,//获取传感器备注
        SCFF_GetCPUModel = 5,//获取cpu型号
        SCFF_GetHandVersion = 6,//硬件版本
        SCFF_GetSoftVersion = 7,//软件版本
        SCFF_GetSystemInfor = 8,//系统信息
        SCFF_GetLibVersion = 9,//获取固件库版本
        SCFF_GetProgTime = 10,//获取烧录时间
        SCFF_GetFlashSize = 11,//获取flash大小
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
        SCFF_GotoApplication = 42,//运行应用
        SCFF_Cheack1024Byte = 43,//校验1024字节
        SCFF_Write1024Byte = 44,//写入1024字节
        SCFF_Read64Byte = 45,//写入64字节
        SCFF_Read1024Byte = 46,//写入1024字节
        SCFF_EraseFlase = 47,//擦除flash
        SCFF_SendCenterRoom = 48,
        SCFF_SendSkyBus = 49,
        SCFF_SetReadValue = 50,//获取恢复配置
        SCFF_SetFullWeight = 51,//设置额定重量
        SCFF_SetReg = 52,//写寄存器
        SCFF_SetAlarmList = 53,//报警参数
        SCFF_GetAlarmList = 54,//报警参数
        SCFF_SetIpPortList = 55,//设置IP端口
        SCFF_GetIpPortList = 56,//设置IP端口
        SCFF_SetCallList = 57,//设置电话清单
        SCFF_GetCallList = 58,//获取电话清单
        SCFF_GetSensorADC = 59,//获取角度模数值
        SCFF_GetSensorValue = 60,//获取角度值
        SCFF_SetSensorMax = 61,//校准大角度
        SCFF_SetSensorMin = 62,//校准小角度
        SCFF_InDeBug = 63,//进入调试模式
        SCFF_OutDeBug = 64,//退出调试模式
        SCFF_GetCANGroup = 65,//获取can通信组
        SCFF_GetCANStdId = 66,//获取can通信成员地址
        SCFF_SetCANGroup = 67,//设置can通信组
        SCFF_SetCANStdId = 68,//设置can通信成员地址
        SCFF_SetCTRBIT = 69,//设置继电器按位
        SCFF_ClrCTRBIT = 70,//清除继电器按位
        SCFF_GetLoginHead = 71,//获取登陆头
        SCFF_SetLoginHead = 72,//设置登陆头
        SCFF_GetHeartBeat = 73,//获取心跳包
        SCFF_SetHeartBeat = 74,//设置心跳包
        SCFF_COMMenuList = 75,//串口菜单
        SCFF_SetDeBugMode = 76,//设置调试模式
        SCFF_SetDateTime = 77,//设置时间
        SCFF_PrintHistory = 78,//输出历史记录
        SCFF_CleanHistory = 79,//输出历史记录
    };
    enum Status
    {
        Succeed=0,
        Fail=1,
        TryAgain=2
    }
    enum MasterSlaveMode
    {
	    WorkMode_Master=0,//主机模式
	    WorkMode_Slave=1,//从机模式
    }
    class ModBusClass
    {
        public const int CheakMode_Crc=1;
        public const int CheakMode_Xor = 2;
        public const int HostAddr = 0x01;
        public const int TargetAddr = 0x05;
        public const int StartFlag = 0x7e;
        public const int EndFlag = 0x7f;
        public const int BroadAddr = 0xff;
        public const int ModBus_Ok=0;
        public const int ModBus_SizeError=1;//数据长度错误
        public const int ModBus_CheakError=2;//校验不通过
        public const int ModBus_MsgError=3;//未知命令
        public const int ModBus_DataOver=4; //访问内容不存在
        public const int ModBus_FlagError=5;//起始结束标志错误
        public const int ModBus_AddrError = 6;
        public static MB DefMoBus=new MB();
        public static void ModBus_Create(ref MB ModBusMsg, int HostAddr, int TargetAddr, MasterSlaveMode WorkMode, int CheakMode)
        {
            ModBusMsg.Start = StartFlag;
            ModBusMsg.End = EndFlag;
            ModBusMsg.ErrorFlag = ModBus_FlagError;
            ModBusMsg.SlaveAddr =(byte)TargetAddr;
            ModBusMsg.MasterAddr = (byte)HostAddr;
            ModBusMsg.CheakMode = CheakMode;
            ModBusMsg.WorkMode =(byte)WorkMode;
        }
        public static int ModBus_CreateMsg(ref byte[] Buf, ref MB ModBusMsg, int MsgFlag, int MsgNum, int DataFlag, byte[] Data, int DataLength)
        {
            int i = 0;
            int Cheak;
            ModBusMsg.MsgFlag =(byte)MsgFlag;
            ModBusMsg.MsgNum =(byte)MsgNum;
            ModBusMsg.DataFlag =(byte)DataFlag;
            ModBusMsg.DataLength = DataLength;
            if (DataLength>0) ModBusMsg.MsgLength = DataLength + 3;
            else ModBusMsg.MsgLength = 0;
            Buf[i++] =(byte)ModBusMsg.Start;
            if (ModBusMsg.WorkMode ==(byte)MasterSlaveMode.WorkMode_Master)//当协议运行为主机模式
            {
                Buf[i++] = ModBusMsg.SlaveAddr;
                Buf[i++] = ModBusMsg.MasterAddr;
            }
            else if (ModBusMsg.WorkMode ==(byte) MasterSlaveMode.WorkMode_Slave)//当协议运行在从机模式
            {
                Buf[i++] = ModBusMsg.MasterAddr;
                Buf[i++] = ModBusMsg.SlaveAddr;
            }
            Buf[i++] = (byte)ModBusMsg.MsgFlag;
            Buf[i++] = (byte)ModBusMsg.MsgNum;
            Buf[i++] = (byte)(ModBusMsg.MsgLength >> 8);
            Buf[i++] = (byte)(ModBusMsg.MsgLength & 0xff);
            if (DataLength>0)//当消息包含数据
            {
                Buf[i++] =(byte) (ModBusMsg.DataFlag);
                Buf[i++] = (byte)(ModBusMsg.DataLength >> 8);
                Buf[i++] = (byte)(ModBusMsg.DataLength & 0xff);
               Tools.BufferCoppy(Data, Buf,i, ModBusMsg.DataLength); i += ModBusMsg.DataLength;
            }
            if (ModBusMsg.CheakMode == CheakMode_Crc)
            {
                Cheak = Tools.GetCrc16(Buf, i);
                Buf[i++] =(byte)(Cheak >> 8);
                Buf[i++] =(byte)(Cheak & 0xff);
            }
            else if (ModBusMsg.CheakMode == CheakMode_Xor)
            {
                Buf[i++] = Tools.Xor(Buf, i-1);
            }
            Buf[i++] = ModBusMsg.End;
            return i;
        }
        public static int ModBus_Expend(byte[] Buf, int Length, ref MB ModBusMsg)
        {
            int i = 0;
            int DataOffSet = 0;
            int Cheak;
            try
            {
                ModBusMsg.ErrorFlag = (byte)ModBus_SizeError;
                if (ModBusMsg.Start != Buf[i++]) return 0;
                if (ModBusMsg.WorkMode == (byte)MasterSlaveMode.WorkMode_Master)//当协议运行为主机模式
                {
                    if (ModBusMsg.MasterAddr != Buf[i++]) return 0;
                    i++;
                }
                else if (ModBusMsg.WorkMode == (byte)MasterSlaveMode.WorkMode_Slave)//当协议运行在从机模式
                {
                    byte TempAddr = Buf[i++];
                    if ((TempAddr != ModBusMsg.SlaveAddr) & (TempAddr != BroadAddr)) return 0;
                    if (Buf[i++] != ModBusMsg.MasterAddr) return 0;
                }
                ModBusMsg.MsgFlag = Buf[i++];
                ModBusMsg.MsgNum = Buf[i++];
                ModBusMsg.MsgLength = Buf[i++];
                ModBusMsg.MsgLength <<= 8;
                ModBusMsg.MsgLength |= Buf[i++];
                if (ModBusMsg.MsgLength > 0)
                {
                    ModBusMsg.DataFlag = Buf[i++];
                    ModBusMsg.DataLength = Buf[i++];
                    ModBusMsg.DataLength <<= 8;
                    ModBusMsg.DataLength |= Buf[i++];
                    DataOffSet = i;
                    i += ModBusMsg.DataLength;
                    ModBusMsg.Data = new byte[ModBusMsg.DataLength];
                    for (int n = 0; n < ModBusMsg.DataLength; n++)
                    {
                        if (DataOffSet >= (Buf.Length - 1)) return 0;
                        ModBusMsg.Data[n] = Buf[DataOffSet++];
                    }
                    if (ModBusMsg.MsgLength != (ModBusMsg.DataLength + 3)) return 0;
                }
                if (ModBusMsg.CheakMode == CheakMode_Xor)
                {
                    Cheak = Tools.Xor(Buf, i);
                    if (Cheak != Buf[i]) return 0;
                    i++;//xor校验一位
                    if (ModBusMsg.End != Buf[i++]) return 0;
                    // if (i != (Length - 2)) ModBusMsg.ErrorFlag = ModBus_SizeError;//有可能尾部还有其他直接忽略
                }
                else if (ModBusMsg.CheakMode == CheakMode_Crc)
                {
                    Cheak = Tools.GetCrc16(Buf, i);
                    //MessageBox.Show(Cheak.ToString("X")); MessageBox.Show(Tools.ByteToInt16(Buf, i, 0).ToString("X"));
                    if (Cheak != Tools.ByteToInt16(Buf, i, 0)) return 0;
                    i += 2;//crc校验2位
                    if (ModBusMsg.End != Buf[i++]) return 0;
                    // if (i != (Length)) return 0;//有可能尾部还有其他直接忽略
                }
                if (ModBusMsg.End != Buf[ModBusMsg.MsgLength + 9]) return 0;
                if (i != (ModBusMsg.MsgLength + 10)) return 0;
                ModBusMsg.ErrorFlag = ModBus_Ok;
                return DataOffSet;
            }catch{ return 0; }
        }
        public static void ModBus_Clear(ref MB ModBus)
        {
            ModBus.Data = null;
            ModBus.DataFlag = 0;
            ModBus.DataLength = 0;
            ModBus.End = 0;
            ModBus.ErrorFlag = 0;
            ModBus.MasterAddr = 0;
            ModBus.MsgFlag = 0;
            ModBus.MsgLength = 0;
            ModBus.MsgNum = 0;
            ModBus.Start = 0;
            ModBus.SlaveAddr = 0;
            ModBus.XorValue = 0;
            ModBus.CrcValue = 0;
            ModBus.CheakMode = 0;
        }
        public static void ModBusCoppyCreate(ref MB S, ref MB D)
        {
            D.Start = S.Start;
            D.End = S.End;
            D.ErrorFlag = S.ErrorFlag;
            D.MasterAddr = S.MasterAddr;
            D.SlaveAddr = S.SlaveAddr;
            D.WorkMode = S.WorkMode;
            D.CheakMode = S.CheakMode;
        }



    }
}
