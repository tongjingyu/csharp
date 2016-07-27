using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
namespace 透明时钟演示
{
    struct PC_Infor
    {
        public string OutIp;
        public string InIp;
        public string Province;
        public string City;
        public string Provider;
        public string PCName;
        public string MAC;
        public string DiskType;
        public string MemorySize;
        public string UserName;
        public string SysType;
        public string Record;
    };
    class DataBase
    {
        public static string CreateAddCommand(PC_Infor PI)
        {
            try
            {
                string Com = "insert into PC_Infor (Record,UserName,InIP,SysType,Memory,DateTime,OutIp,Province,PCName,City,Provider,Mac) VALUES(" +
                Tools.AddHead(PI.Record) + "," +
                Tools.AddHead(PI.UserName) + "," +
                Tools.AddHead(PI.InIp) + "," +
                Tools.AddHead(PI.SysType) + "," +
                Tools.AddHead(PI.MemorySize) + "," +
                Tools.AddHead(DateTime.Now.ToString()) + "," +
                Tools.AddHead(PI.OutIp) + "," +
                Tools.AddHead(PI.Province) + "," +
                Tools.AddHead(PI.PCName) + "," +
                Tools.AddHead(PI.City) + "," +
                Tools.AddHead(PI.Provider) + "," +
                Tools.AddHead(PI.MAC) + ")";
                return Com;
            }
            catch (Exception E) { MessageBox.Show(E.Message); }
            return "";
        }
        public static void AddRecord(string FileName)
        {
                PC_Infor PI = new PC_Infor();
                string Com;
                GetIpInfor.GetInfor(ref PI);
                Tools.GetInfor(ref PI);
                Tools.JoinRecord(FileName, ref PI);
                Com = CreateAddCommand(PI);
                MessageBox.Show(Com);
                MySql.SqlCommand(Com);
        }
        
    }
}
