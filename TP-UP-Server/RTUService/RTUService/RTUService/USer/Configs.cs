using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Data.SqlClient;
namespace RTUService
{
    class Configs
    {
        public static int AgreementAAASize = 9;
        public static string AgreementAAAHead = "$AAA";
        public static string AgreementAAAEnd = "$END";
        public static string AgreementBAAHead = "$BAA";
        public static int AgreementAAAMinLength = 20;
        public const int DataByteSize = 1600;//接收缓冲区大小
        public static int ServicePoint = 5010;//服务器使用端口
        public static int UserFacePoint = 2014;//用户访问接口
        public static int UserTimeOut = 60000;//用户响应时间一分钟
        public static Queue<RealTimePacket> RealTimePacketQueue = new Queue<RealTimePacket>();
        public static Queue<DensityPacket> DensityPacketQueue = new Queue<DensityPacket>(); 
        public static Socket UserSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        public static Socket ListenSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        public static int ReviceTimeOut=120000;//接收超时时间10s
        public static int AgainFailCount = 100;//失败重接收次数
        public static string SqlPassWord = "123456";
        public static string UserRootPassWord = "123456";
        public static string UserRootName = "admin";
        public static string SqlName = "TestUser";
        public static string OracleDataSource = @"222.42.174.120";
        public static string OracleName = "system";
        public static string OraclePassword = "123456";
        public static int OraclePoint = 1521;
        public static string OracleDataBase = "RWDB";
        public static string SqlDataSource = @"192.168.1.188\SQLEXPRESS";
        public static string SqlDataBase = "YLN";
        public static string SqlDensitySheet = "DensityRecord";
        public static string SqlRealTimeSheet = "RealTimeRecord";
        public static string SqlStationInforSheet = "StationInfor";
        public static int SqlConnectTimeOut = 5000;
        public static int RealSheetSize = 12;
        public static int DensitySheetSize = 12;
        public static string Path = "F:/RTU/DXS/Service/";
    }
    class SysFlag
    {
        public static bool SqlConnectOk = false;//连接数据库成功标志
        public static bool ListenOk = false;
        public static int RealTimePacketCount = 0;
        public static int DensityPacketCount = 0;
        public static int ClientCount = 0;//设备在线台数
        public static int UserCount = 0;//用户在线台数
        public static int QueueCount = 0;
        public static bool ServiceStart = true;
    }
    class SheetName
    {
        public const string YLN_DEV_INFOR = "YLN_DEV_INFOR";
        public const string ST_GRW_R="ST_GRW_R";
    }
   
}
