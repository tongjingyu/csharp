using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Data.SqlClient;
using System.Data.OleDb;
namespace 模拟RTU
{
    class Sql
    {
         public static bool SqlConnect()
        {
            try
            {
                Configs.SqlConn = new SqlConnection(Read_User_Str());
                Configs.SqlConn.Open();
                if (Configs.SqlConn.State == ConnectionState.Open)
                {
                    return true;
                }
                else return false;
            }
            catch { return false; }
        }
         private static string Read_User_Str()
         {
             SqlConnectionStringBuilder SqlStr = new SqlConnectionStringBuilder();
             SqlStr.Password = Configs.SqlPassWord;//密码
             SqlStr.UserID = Configs.SqlName;// 用户名
             SqlStr.ConnectTimeout = 5000;//超时退出
             SqlStr.DataSource = Configs.SqlDataSource;//数据库主机地址
             SqlStr.IntegratedSecurity = false;
             SqlStr.InitialCatalog = Configs.SqlDataBase;//数据库名
             string SqlConnStr = SqlStr.ToString();
             return SqlConnStr;
         }
    }
}
