using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;
using System.Windows.Forms;
using System.Data;
using System.Collections;
using System.Data.OleDb;
using System.Threading;
namespace Datamark
{
    class MySql
    {

        public static MySqlConnection Connect;
        public const string M_str_sqlcon = "server=45.116.144.5;user id=a0712211838;password=1c5bea2b;database=a0712211838;Charset=utf8"; //根据自己的设置
        public static void CreateSqlConnet()
        {
            try
            {
                Connect = new MySqlConnection(M_str_sqlcon);
                Connect.Open();
            }
            catch { }
        }
        public static bool TryOpen()
        {

            try
            {
                if(Connect.State.ToString()=="Open")return false;
                Connect.Open();
            }
            catch { CreateSqlConnet();}
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
        public static void ConnectSql()
        {
            //CreateSqlConnet();
        }
        public static string GetID()
        {
            TryOpen();
            string Com="SELECT FLOOR(RAND() * 999999999) AS random_num "+
                      "FROM tp_follow "+
                      "WHERE 'random_num' NOT IN(SELECT id FROM tp_follow) " +
                      "LIMIT 1 ";
            MySqlCommand mysqlcom = new MySqlCommand(Com, Connect);
            mysqlcom.ExecuteNonQuery();
            DataTable dt = new DataTable();
            MySqlDataReader sdr = mysqlcom.ExecuteReader();
            if (sdr.HasRows)
            {
                dt.Load(sdr);
                string Msg = dt.Rows[0][0].ToString();
                return (Msg);
            }
            MessageBox.Show("获取失败");
            return null;
        }
        public static string Instar(string ID,string Name,string Value)
        {
            TryOpen();
            string Com = "INSERT into tp_follow (id,NAME,VALUE) VALUES("+ ID + ",'"+Name+"','"+Value+"')";
            MySqlCommand mysqlcom = new MySqlCommand(Com, Connect);
            mysqlcom.ExecuteNonQuery();
            return null;
        }
    }


}