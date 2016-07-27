using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace RTUService
{
    struct RealTimePacket
    {
        public string StationId;
        public string StationOrder;//指令
        public string StationNumber;//设备编号
        public string StationName;//设备名称
        public string StationPosition;//站点名称
        public string RecordDataTime;//记录时间
        public string RecordWaterLevel;//记录水位
        public string RecordRainFall;
        public string RecordTemperature;
        public string RecordDimension;
        public string RecordLongitude;
        public string RecordVoltage;
        public string RecordExplain;
        public string RecordOther;
        public string Note;
    }
    struct DensityPacket
    {
        public string StationId;
        public string StationOrder;
        public string StationNumber;
        public string StationPosition;
        public string RecordDataTime;
        public string RecordWaterLevel;
        public string RecordRainFall;
        public string RecordTemperature;
        public string RecordDimension;
        public string RecordLongitude;
        public string RecordVoltage;
        public string RecordOther;
        public string Note;
    }

    class DataCollection
    {
        public static int CreateReturnInfor(ref byte[] TxBuffer,string Msg)
        {
            string Temp = "#00(" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + Msg+")\n";
            TxBuffer=Encoding.GetEncoding("gb2312").GetBytes(Temp);
            return TxBuffer.Length;
        }
        public static void ClearRealTimePacket(ref RealTimePacket Packet)
        {
            Packet.RecordOther = "NULL";
            Packet.RecordRainFall = "NULL";
            Packet.RecordTemperature = "NULL";
            Packet.RecordVoltage = "NULL";
            Packet.RecordWaterLevel = "NULL";
            Packet.RecordDimension = "NULL";
            Packet.RecordLongitude = "NULL";
            Packet.StationPosition = "NULl";
            Packet.Note = "NULL";
        }
        public static string DeleteUnVisibleChar(string sourceString)
        {
            System.Text.StringBuilder sBuilder = new System.Text.StringBuilder(131);
            for (int i = 0; i < sourceString.Length; i++)
            {
                int Unicode = sourceString[i];
                if (Unicode >= 16)
                {
                    sBuilder.Append(sourceString[i].ToString());
                }
            }
            return sBuilder.ToString();
        }
        public static bool IfTrueWithAAA(byte[] RxBuffer, int Length)
        {
            string Msg = Encoding.GetEncoding("gb2312").GetString(RxBuffer, 0, Length);
            Msg = DeleteUnVisibleChar(Msg);
            if(!Msg.EndsWith(Configs.AgreementAAAEnd))return false;
            string[] Arry = Msg.Split(';');
            if (Arry.Length != Configs.AgreementAAASize) return false;
            return true;
        }
        public static string SelectDataFromArry(string[] Info, string[] Data,string Key)
        {
            for(int i=0;i<Data.Length;i++)
            {
                if (Info[i].IndexOf(Key) != -1) return Data[i];
            }
            return "NULL";
        }
        public static bool ExpendWithAAA(byte[] RxBuffer, int Length)
        {
            if(!IfTrueWithAAA(RxBuffer,Length))return false;
            RealTimePacket TempPacket = new RealTimePacket();
            string Msg = Encoding.GetEncoding("gb2312").GetString(RxBuffer, 0, Length);
            string[] Arry = Msg.Split(';');
            int i = 1;
            ClearRealTimePacket(ref TempPacket);
            TempPacket.StationNumber = Arry[i++];
            TempPacket.StationOrder = Arry[i++];
            TempPacket.StationPosition = Arry[i++];
            TempPacket.StationName = Arry[i++];
            TempPacket.RecordDataTime = Arry[i++].ToString();
            string[] DataArry = Arry[i++].Split(',');
            string[] InfoArry = Arry[i++].Split(',');
            TempPacket.RecordRainFall = SelectDataFromArry(InfoArry, DataArry, "雨量");
            TempPacket.RecordWaterLevel = SelectDataFromArry(InfoArry, DataArry, "水位");
            TempPacket.RecordDimension = SelectDataFromArry(InfoArry, DataArry, "经度");
            TempPacket.RecordLongitude = SelectDataFromArry(InfoArry, DataArry, "纬度");
            TempPacket.RecordTemperature = SelectDataFromArry(InfoArry, DataArry, "温度");
            TempPacket.RecordVoltage = SelectDataFromArry(InfoArry, DataArry, "电压");
            TempPacket.RecordExplain = SelectDataFromArry(InfoArry, DataArry, "水势");
            lock (Configs.RealTimePacketQueue)
            {
                Configs.RealTimePacketQueue.Enqueue(TempPacket);
            }
            return true;
        }
        public static void ClearDensityPacket(ref DensityPacket Packet)
        {
            Packet.RecordOther = "NULL";
            Packet.RecordRainFall = "NULL";
            Packet.RecordTemperature = "NULL";
            Packet.RecordVoltage = "NULL";
            Packet.RecordWaterLevel = "NULL";
            Packet.RecordDimension = "NULL";
            Packet.RecordLongitude = "NULL";
            Packet.StationPosition = "NULl";
            Packet.Note = "NULL";
        }
        public static bool IfTrueWithBAA(byte[] RxBuffer, int Length)
        {
            if (CheckTools.DAT_GetD16FromArray(RxBuffer, 28, 0) != Length) return false;
            return true;
        }
        public static bool ExpendWithBAA(byte[] RxBuffer, int Length)
        {
            if (!IfTrueWithBAA(RxBuffer,Length)) return false;
            DensityPacket TempPacket = new DensityPacket();
            int PacketLength;
            int TimeSpace;
            int DataType;
            int BigBagSize;
            int i = 4;
            int TimeOffset;
            ClearDensityPacket(ref TempPacket);
            TempPacket.StationNumber = CheckTools.DAT_GetD32FromArray(RxBuffer, i, 0).ToString(); i += 4;
            TempPacket.StationOrder = CheckTools.DAT_GetD32FromArray(RxBuffer, i, 0).ToString(); i += 4;
            TempPacket.Note = Tools.DAT_GetString(RxBuffer, i, 16).Trim(); i += 16;
            PacketLength = CheckTools.DAT_GetD16FromArray(RxBuffer, i, 0); i += 2;
            TimeSpace = RxBuffer[i]; i += 1;
            DataType= RxBuffer[i]; i += 1;
            TimeOffset=i;
            TempPacket.RecordDataTime = Tools.DAT_GetTime(RxBuffer, i).ToString(); i += 6;
            int N = 60 / TimeSpace;
            int SmallBagSize = N * 4 + 6;
            BigBagSize = (PacketLength - 36) / (SmallBagSize);
            if (BigBagSize != 2)
            return false;//总共只有水位和雨量两项
            for (int n = 0; n < N; n++)
            {
                TempPacket.RecordDataTime = Tools.DAT_GetSpaceTime(RxBuffer, TimeOffset, TimeSpace, n).ToString();
                TempPacket.RecordWaterLevel = Tools.DAT_GetNum(RxBuffer, i + n*4, 0, DataType);
                TempPacket.RecordRainFall = Tools.DAT_GetNum(RxBuffer, i + SmallBagSize + n*4, 0, DataType); 
                lock (Configs.DensityPacketQueue)
                {
                    Configs.DensityPacketQueue.Enqueue(TempPacket);
                }
            }
            if (SmallBagSize> 0) return true;
            return false;
        }
        public static bool ExpendRecord(byte[] RxBuffer, int Length)
        {
            if (Length == 0) return false;
            string TempString = Encoding.GetEncoding("gb2312").GetString(RxBuffer, 0, 4);
            if (TempString.StartsWith(Configs.AgreementAAAHead))
            {
                return ExpendWithAAA(RxBuffer, Length);
            }
            else if (TempString.StartsWith( Configs.AgreementBAAHead))
            {
                return ExpendWithBAA(RxBuffer, Length);
            }
            else
            {
                return false;
            }
        }


    }
}
