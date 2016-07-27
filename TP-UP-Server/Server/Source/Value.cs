using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    enum LogType
    {
        LT_Error = 1,
        LT_Warning = 2,
        LT_Infor = 3,
        LT_All=7
    }
    enum SoftType
    {
       ST_Console = 1,
       ST_Server = 2
    }
    enum UpGradeCmd
    {
        UGC_SetBinFile = 0x01,
        UGC_SetTextFile = 0x02,
        UGC_GetBinFileInfor = 0x03,
        UGC_GetTextFileInfor = 0x04,
        UGC_ReadBinFile = 0x05,
        UGC_ReadTextFile = 0x06,
        UCG_GetDateTime = 0x07,
        UCG_CreateBinCFG = 0x08,
        UCG_SendBinCFG = 0x09,
        UCG_SetBinCFGFile = 0x0a,
        UCG_SetMenuList = 0x0b,
        UCG_GetMenuList=0x0c,
        UCG_CheckNewMenu = 0x0d,
    }
    class TagetValue
    {
        public static LogType SystemLogType = LogType.LT_All;
        public static SoftType SystemType = SoftType.ST_Console;
        public static bool Run = true;
    }
    class HashKey
    {
        public string Key;
        public string[] Str;
        public byte Update;
        public string OnlyTime;
    }
    class Value
    {
        public static string Path;
        public static Configs CFG;
        public static Log WriteLog;
        public static FileIO fileIO;
        public static int ServerPort;
        public static int DevOnlineCount = 0;
        public static List<string> list = new List<string>();
        public static HashSet<HashKey> hs = new HashSet<HashKey>();
    }
}
