using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Data.SqlClient;


namespace BaseManage
{
    class Sql
    {
        public int a;
        public static SqlConnection SqlConn;
        public static String SqlSheetName = "StationNumber,StationPasswd,StationName,StationState,StationPosition,StationCreatTime,StationType,StationVersion,StationOther,Note";
    }
    class StationInforSheet
    {
        public const string StationNumber = "StationNumber";
        public const string StationPasswd = "StationPassword";
        public const string StationName = "StationName";
        public const string StationState = "StationState";
        public const string StationPosition = "StationPosition";
        public const string StationCreateTime = "StationCreateTime";
        public const string RecordDimension = "StationDimension";
        public const string RecordLongitude = "StationLongitude";
        public const string StationType = "StationType";
        public const string StationVersion = "StationVersion";
        public const string StationOther = "StationOther";
        public const string Note = "Note";
    }
   class SationInfor
    {
        public const int InforSize = 12;
        public string StationNumber;
        public string StationPasswd;
        public string StationName; 
        public string StationState;
        public string StationPosition;
        public string StationCreateTime;
        public string StationDimension;
        public string StationLongitude;
        public string StationType;
        public string StationVersion;
        public string StationOther;
        public string Note;
    };
   public struct List
   {
       public string Name;
       public string Data;
   };
   public struct Command
   {
       public string Com1;
       public string Com2;
       public string Sheet;
       public List[] List;
   }
}
