using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace RTUService
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Tools.IfNotFileCreat(Configs.Path);
            CreateInfor.WriteLogs("================================================Start=== ====================================================");
            if (XML.LoadConfig()) CreateInfor.WriteLogs("读取配置成功!");
            else { CreateInfor.WriteLogs("读取配置失败，已重置配置,服务结束!"); this.Stop(); }
            if (Sql.MainSql()) CreateInfor.WriteLogs("SqlSerice数据库连接成功!");
            else { CreateInfor.WriteLogs("SqlSerice数据库连接失败,服务结束!"); this.Stop(); }
          //  if (UserFaceThread.CreateUser()) CreateInfor.WriteLogs("用户访问通道建立!");
           // else { CreateInfor.WriteLogs("用户访问通道创建失败,服务结束!"); this.Stop(); }
            if (Oracle.LinkOracle()) CreateInfor.WriteLogs("Oracle数据库连接成功!");
            else { CreateInfor.WriteLogs("Oracle数据库连接失败,服务结束!"); this.Stop(); }
            if (ClientThread.CreateServiceThread()) CreateInfor.WriteLogs("侦听通道建立!");
            else { CreateInfor.WriteLogs("侦听通道通道创建失败,服务结束!"); this.Stop(); }
            CreateInfor.WriteLogs("服务完全进入运行状态");
        }

        protected override void OnStop()
        {

            CreateInfor.WriteLogs("================================================Stop========================================================\r\n");
            SysFlag.ServiceStart = false;
            System.GC.Collect();
        }
    }
}
