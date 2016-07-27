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
namespace 模拟RTU
{
    class Configs
    {
        public const string ServiceName = "RTUService";
        public static int UserFacePoint = 2014;//用户访问接口
        public static int ServicePoint = 2013;
        public static string ServiceAddr = "192.168.1.188";
        public static int UserTimeOut = 60000;//用户响应时间一分钟
        public static SqlConnection SqlConn;
        public static TcpClient Client_New;
        public static int ReviceTimeOut = 30000;//接收超时时间10s
        public static string SqlPassWord = "123456";
        public static string UserRootPassWord = "123456";
        public static string SqlName = "TestUser";
        public static string SqlDataSource = @"192.168.1.188\SQLEXPRESS";
        public static string SqlDataBase = "YLN";
        public static string SqlDensitySheet = "DensityRecord";
        public static string SqlRealTimeSheet = "RealTimeRecord";
        public static string SqlStationInforSheet = "StationInfor";
        public static int SqlConnectTimeOut = 5000;
        public static int RealSheetSize = 12;
        public static int DensitySheetSize = 12;
        public static string Path = "D:/客户端服务程序/";
        public static string[] Status = new string[] { "停用", "运行", "维护" };
        public const int GridViewReSize = 50;
        public const int ChartViewSize = 500;
        public static int GridViewSize = GridViewReSize;
        public static string ShowMode = "RealTimeRecord";
        public const string ShowModeRealTime = "RealTimeRecord";
        public const string ShowModeDesity = "DensityRecord";
        public const string ShowModeChart = "Chart";
        public static int SqlCountAll = 0;
        public const string RainFallString = "RecordRainFall";
        public const string WaterLevelString = "RecordWaterLevel";
        public const string RecordTemperature = "RecordTemperature";
        public const string RecordVoltage = "RecordVoltage";
        public static string SerectChart = WaterLevelString;
        public static string ChertName = "Series";
        public const int ShowStyleDataSheet = 0;
        public const int ShowStylePicture = 1;
        public static int ShowStyle = ShowStyleDataSheet;
        public static int StationCount = 0;
        public const int UpdataTypeALL = 0;
        public const int UpdataTypeLostShow = 1;
        public const int UpdataTypeChart = 2;
        public static int UpdataType = UpdataTypeALL;
        public static bool CloseEn = true;
        public static int PageCount = 0;
        public static int PageIndex = 0;
        public static bool ServiceHost = false;
    }
    class SysFlag
    {
        public static bool ServiceLink = false;
        public static string SqlConnectOk = "断开";//连接数据库成功标志
        public static string ServiceState = "未启动";
        public static string ListenOk = "未侦听";
        public static string ClientCount = "0";//设备在线台数
        public static string UserCount = "0";//用户在线台数
        public static string QueueCount = "0";
        public static string RealTimePacketCount = "0";
        public static string DensityPacketCount = "0";
        public static bool ClientEnabled = true;
        public static bool SqlOk = false;
    }
}