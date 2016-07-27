using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Data.SqlClient;
using System.Data.OleDb;
namespace RTUService
{
    class Sql
    {
        public static SqlConnection SqlConn;
        public static Thread RealCollectThread;

        public static void TryOpen()
        {
            try
            {
                SqlConn.Open();
            }
            catch { }
        }
        public static void TryClose()
        {
            try
            {
                SqlConn.Close();
            }
            catch { }
        }
        public static bool MainSql()
        {
            try
            {
                SqlConn = new SqlConnection(Read_User_Str());
                SqlConn.Open();
                if (SqlConn.State == ConnectionState.Open)
                {
                    SysFlag.SqlConnectOk = true;
                }
                else return false;
                RealCollectThread = new Thread(new ThreadStart(RealCollect));
                RealCollectThread.Start();
            }
            catch (Exception E)
            {
                CreateInfor.WriteLogs(E.Message+"导致【数据库连接失败】请手动修改当前目录下Configs配置文件");
                SysFlag.SqlConnectOk = false;
                return false;
            }
            return true;
        }
        public static void RealCollect()
        {
            RealTimePacket RPacket=new RealTimePacket();
            DensityPacket DPacket = new DensityPacket();
            while(SysFlag.ServiceStart)
            {
                if (Configs.RealTimePacketQueue.Count > 0)
                {
                    lock (Configs.RealTimePacketQueue)
                    {
                        RPacket = (RealTimePacket)Configs.RealTimePacketQueue.Dequeue();
                        SysFlag.RealTimePacketCount++;
                    }
                    InsertToRealTimeSheet(RPacket);
                    Oracle.InsertToRealTimeSheet(RPacket);
                    Oracle.UpdataToRealTimeSheet(RPacket);
                }
                if (Configs.DensityPacketQueue.Count > 0)
                {
                    lock (Configs.DensityPacketQueue)
                    {
                        DPacket = (DensityPacket)Configs.DensityPacketQueue.Dequeue();
                        SysFlag.DensityPacketCount++;
                    }
                    InsertToDensitySheet(DPacket);
                }
                else { System.Threading.Thread.Sleep(100); }
            }
            SqlConn.Close();
        }
        private static string Read_User_Str()
        {
            SqlConnectionStringBuilder SqlStr = new SqlConnectionStringBuilder();
            SqlStr.Password = Configs.SqlPassWord;//密码
            SqlStr.UserID = Configs.SqlName;// 用户名
            SqlStr.ConnectTimeout = 5000;//超时退出
            SqlStr.DataSource = Configs.SqlDataSource;//数据库主机地址
            SqlStr.IntegratedSecurity = false;
            SqlStr.InitialCatalog =Configs.SqlDataBase;//数据库名
            string SqlConnStr = SqlStr.ToString();
            return SqlConnStr;
        }
        private static string AddHead(string S)
        {
            string temp = "'";
            temp += S;
            temp += "'";
            return temp;
        }
        private static string ASearchBInSheet(string table, string Source, string Target)
        {
            string temp="select * from ";
            temp += table+" where ";
            temp += Source+"=";
            temp += Target;
            try
            {
                TryOpen();
                SqlCommand objSqlCommand = new SqlCommand(temp, Sql.SqlConn);
                SqlDataReader Reader = objSqlCommand.ExecuteReader();
                temp = "NULL";
                while (Reader.Read())
                {
                    temp = Reader.GetValue(0).ToString();
                    if (temp != null) break;
                }
                Reader.Close();
                TryClose();
            }
            catch (Exception E)
            {
                CreateInfor.WriteLogs(E.Message + "->导致【" + temp + "】执行失败!");
            }
            return temp;
        }
        private static bool InsertToRealTimeSheet(RealTimePacket Packet)
        {
            string sCommand = "";
            try
            {
                Packet.StationId = ASearchBInSheet(Configs.SqlStationInforSheet, "StationNumber", Packet.StationNumber);
                    sCommand = "INSERT INTO " + Configs.SqlRealTimeSheet + " VALUES("
                    + Packet.StationId + ","
                    + Packet.StationNumber + ","
                    + AddHead(Packet.RecordDataTime).ToString() + ","
                    + Packet.RecordWaterLevel + ","
                    + Packet.RecordRainFall + ","
                    + Packet.RecordTemperature + ","
                    + Packet.RecordDimension + ","
                    + Packet.RecordLongitude + ","
                    + Packet.RecordVoltage + ","
                    + AddHead(Packet.RecordOther).ToString() + ","
                    + AddHead(Packet.Note).ToString() + ")";
                TryOpen();
                SqlCommand objSqlCommand = new SqlCommand(sCommand, Sql.SqlConn);
                objSqlCommand.ExecuteNonQuery();
                TryClose();
            }
            catch (Exception E)
            {
                CreateInfor.WriteLogs(E.Message+"->导致【"+ sCommand+"】入库失败!");
                return false;
            }
            return true;
        }
        private static bool InsertToDensitySheet(DensityPacket Packet)
        {
            string sCommand = "";
            try
            {
                Packet.StationId = ASearchBInSheet(Configs.SqlStationInforSheet, "StationNumber", Packet.StationNumber);
                sCommand = "INSERT INTO " + Configs.SqlDensitySheet + " VALUES("
                    + Packet.StationId + ","
                    + Packet.StationNumber + ","
                    + AddHead(Packet.RecordDataTime).ToString() + ","
                    + Packet.RecordWaterLevel + ","
                    + Packet.RecordRainFall + ","
                    + Packet.RecordDimension + ","
                    + Packet.RecordLongitude + ","
                    + Packet.RecordTemperature + ","
                    + Packet.RecordVoltage + ","
                    + AddHead(Packet.RecordOther).ToString() + ","
                    + AddHead(Packet.Note).ToString() + ")";
                TryOpen();
                SqlCommand objSqlCommand = new SqlCommand(sCommand, Sql.SqlConn);
                objSqlCommand.ExecuteNonQuery();
                TryClose();
            }
            catch (Exception E)
            {
                CreateInfor.WriteLogs(E.Message+"->导致【"+sCommand+"】入库失败");
                Console.WriteLine(sCommand);
                return false;
            }
            return true;
        }
       
    }
}
