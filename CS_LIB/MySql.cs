using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;
using System.Windows.Forms;
using System.Data;
using System.Collections;
using System.Data.OleDb;
using System.Threading;
namespace 奇奇邮件助手
{
    class MySql
    {

        public static MySqlConnection Connect;
        public const string M_str_sqlcon = "server=67.229.126.66;user id=tongjinlv;password=TONGJINLV;database=tongjinlv;Charset=utf8"; //根据自己的设置
        public static void CreateSqlConnet()
        {
            Connect = new MySqlConnection(M_str_sqlcon);
        }
        public static bool TryOpen()
        {

            try
            {
                Connect.Open();
                
            }
            catch { return false; }
            return true;
 
        }
        public static bool TryClose()
        {
            try
            {
                Connect.Close();
            }
            catch { return false; }
            return true;
        }
        public static void LoginThread()
        {
            Value.LinkStatus = Value.LinkIng;
            CreateSqlConnet();
            TryOpen();
            int i = 0;
            Thread.Sleep(10);
            while (Value.App_Run)
            {
                if (Connect.State==ConnectionState.Open) Value.LinkStatus = Value.LinkOk;
                else {
                    Value.LinkStatus = Value.LinkError;
                    MessageBox.Show(Connect.State.ToString());
                    CreateSqlConnet();
                    TryOpen();
                }
                i = 10;
                while (i-- > 0) { Thread.Sleep(10); if (!Value.App_Run)return; }
            }
        }
        public static void SqlCommand(string M_str_sqlstr)
        {
            if (Value.App_Run)
            {
                MySqlConnection Con = new MySqlConnection(M_str_sqlcon);
                Con.Open();
                MySqlCommand Conmand = new MySqlCommand(M_str_sqlstr, Con);
                Conmand.ExecuteNonQuery();
                Con.Close();
            }
        }
        public static DataTable GetDataBase(string M_str_sqlstr)
        {
            if (Value.App_Run)
            {
                MySqlConnection Con = new MySqlConnection(M_str_sqlcon);
                Con.Open();
                MySqlDataAdapter MysqlAdapter;
                MysqlAdapter = new MySqlDataAdapter(M_str_sqlstr, Con);
                DataTable db = new DataTable();
                MysqlAdapter.Fill(db);
                Con.Close();
                return db;
            }
            return null;
        }

    }
}
