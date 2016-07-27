using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;
using System.Windows.Forms;
using System.Data;
using System.Collections;
using System.Data.OleDb;
using System.Threading;
namespace SensorFace
{
    class MySql
    {

        public static MySqlConnection Connect;
        public const string M_str_sqlcon = "server=67.229.126.66;user id=tongjinlv;password=TONGJINLV;database=tongjinlv;Charset=utf8"; //根据自己的设置
        public static void CreateSqlConnet()
        {
            try
            {
                Connect = new MySqlConnection(M_str_sqlcon);
            }
            catch { }
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
            try
            {
                CreateSqlConnet();
                TryOpen();
            }
            catch { return; }
           
            int i = 0;
            Thread.Sleep(10);
            try
            {
                while (Value.App_Run)
                {
                    if (Connect.State.ToString() == "Open")
                    {
                        if (Value.queue.Count > 0)
                        {
                            MySqlCommand mysqlcom = new MySqlCommand(DataBase.GetIntoCfg(Value.queue.Dequeue()), Connect);
                            mysqlcom.ExecuteNonQuery();
                        }
                    }
                    else TryOpen();
                    i = 10;
                    while (i-- > 0) { Thread.Sleep(100); if (!Value.App_Run)return; }
                }
            }
            catch { }
        
        }
    }


}