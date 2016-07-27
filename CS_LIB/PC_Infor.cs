using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace 奇奇邮件助手
{
    struct PC_InforStruct
    {
        public string IPInfor;
        public string PCName;
        public string MAC;
        public string DiskType;
        public string MemorySize;
        public string UserName;
        public string SysType;
        public string Record;
    };
    class PC_Infor
    {
        public static void Get(ref PC_InforStruct PI)
        {
            PI.MAC = Tools.GetMacAddress();
            PI.MemorySize = Tools.GetTotalPhysicalMemory();
            PI.PCName = Tools.GetComputerName();
            PI.UserName = Tools.GetUserName();
            PI.SysType = Tools.GetSystemType();
            PI.DiskType = Tools.GetDiskID();

        }

    }
}
