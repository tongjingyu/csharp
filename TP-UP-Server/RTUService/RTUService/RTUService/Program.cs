using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTUService
{
    class Program
    {
        static void Main(string[] args)
        {
            Tools.IfNotFileCreat(Configs.Path);
            CreateInfor.WriteLogs("================================================Start=======================================================");
            if (XML.LoadConfig()) CreateInfor.WriteLogs("读取配置成功!");
            else { CreateInfor.WriteLogs("读取配置失败，已重置配置,服务结束!"); return; }
            if (Sql.MainSql()) CreateInfor.WriteLogs("SqlService数据库连接成功!");
            else { CreateInfor.WriteLogs("SqlService数据库连接失败,服务结束!"); return; }
            if (Oracle.LinkOracle()) CreateInfor.WriteLogs("Oracle数据库连接成功!");
            else { CreateInfor.WriteLogs("Oracle数据库连接失败,服务结束!"); return; }
          //  if (UserFaceThread.CreateUser()) CreateInfor.WriteLogs("用户访问通道建立!");
           // else { CreateInfor.WriteLogs("用户访问通道创建失败,服务结束!"); return; }
            if (ClientThread.CreateServiceThread()) CreateInfor.WriteLogs("侦听通道建立!");
            else { CreateInfor.WriteLogs("侦听通道通道创建失败,服务结束!"); return; }
            CreateInfor.WriteLogs("服务完全进入运行状态");
        }
    }
}
