using System;
using System.Collections.Generic;
using System.Text;
using System.Data.OracleClient;
using System.Data.SqlClient;
using System.Data;
namespace RTUService
{
    class Oracle
    {
        public static OracleConnection OracleConn;
        public static string Read_User_Str()
        {
            //"Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=192.168.1.188)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=RWDB)));User Id=system;Password=123456";
            String connString = @"";
            connString += "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=";
            connString += Configs.OracleDataSource;
            connString += ")(PORT=";
            connString += Configs.OraclePoint.ToString();
            connString += "))(CONNECT_DATA=(SERVICE_NAME=";
            connString += Configs.OracleDataBase;
            connString += ")));User Id=";
            connString += Configs.OracleName;
            connString += ";Password=";
            connString += Configs.OraclePassword;
            return connString;
        }
        public static bool LinkOracle()
        {
            try
            {
                OracleConn = new OracleConnection(Read_User_Str());
                OracleConn.Open();
                if (OracleConn.State == ConnectionState.Open)
                {
                    return true;
                }
            }
            catch (Exception E)
            {
                 CreateInfor.WriteLogs(E.Message+"导致【数据库连接失败】请手动修改当前目录下Configs配置文件");
                SysFlag.SqlConnectOk = false;
                return false;
            }
            return true;
        }
        public static void TryOpen()
        {
            try
            {
                OracleConn.Open();
            }
            catch { }
        }
        public static void TryClose()
        {
            try
            {
                OracleConn.Close();
            }
            catch { }
        }
        public static bool UpdataToRealTimeSheet(RealTimePacket Packet)
        {
            string sCommand = "";
            try
            {
                int.Parse(Packet.RecordExplain);
            }
            catch { Packet.RecordExplain = "0"; }
            try
            {
                Packet.RecordDataTime = DateTime.Parse(Packet.RecordDataTime).AddSeconds(double.Parse(Packet.RecordExplain)).ToString();
                sCommand = "UPDATE " + SheetName.ST_GRW_R + " SET " +
                    " STCD=" + Tools.GetSqlString(Packet.StationNumber) + "," +
                    " TM=" + "to_date(" + Tools.GetSqlString(Packet.RecordDataTime) + ",'yyyy-mm-dd hh24:mi:ss')," +
                    " GWBD=" + Tools.GetSqlString(Packet.RecordWaterLevel) + "," +
                    " GWBDRMK=" + Tools.GetSqlString(Packet.RecordExplain) +
                    " WHERE STCD=" + Tools.GetSqlString(Packet.StationNumber)+
                    " AND TM=to_date(" + Tools.GetSqlString(Packet.RecordDataTime) + ",'yyyy-mm-dd hh24:mi:ss')";
                TryOpen();
                OracleCommand objSqlCommand = new OracleCommand(sCommand, OracleConn);
                objSqlCommand.ExecuteNonQuery();
                TryClose();
            }
            catch (Exception E)
            {
                CreateInfor.WriteLogs(E.Message + "->导致【" + sCommand + "】更新失败!");
                return false;
            }
            return true;
        }

        public static bool InsertToRealTimeSheet(RealTimePacket Packet)
        {
            string sCommand = "";
            try
            {
                int.Parse(Packet.RecordExplain);
            }
            catch { Packet.RecordExplain = "0"; }
            try
            {
                Packet.RecordDataTime=DateTime.Parse(Packet.RecordDataTime).AddSeconds(double.Parse(Packet.RecordExplain)).ToString();
                sCommand = "INSERT INTO " + SheetName.ST_GRW_R + "(STCD,TM,GWBD,GWBDRMK) VALUES("
                + Tools.GetSqlString(Packet.StationNumber)+ ","
                +  "to_date("+Tools.GetSqlString(Packet.RecordDataTime)+ ",'yyyy-mm-dd hh24:mi:ss'),"
                + Tools.GetSqlString(Packet.RecordWaterLevel) + ","
                + Tools.GetSqlString(Packet.RecordExplain) + ")";
                TryOpen();
                OracleCommand objSqlCommand = new OracleCommand(sCommand, OracleConn);
                objSqlCommand.ExecuteNonQuery();
                TryClose();
            }
            catch (Exception E)
            {
                CreateInfor.WriteLogs(E.Message + "->导致【" + sCommand + "】入库失败或数据被更新!");
                return false;
            }
            return true;
        }
    }
}
