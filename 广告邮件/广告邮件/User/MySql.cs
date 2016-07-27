using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;
using System.Windows.Forms;
namespace 广告邮件
{
    class MySql
    {

        public static MySqlConnection Connect;
        public static void CreateSqlConnet()
        {
            
            string M_str_sqlcon = "server=67.229.126.66;user id=tongjinlv;password=TONGJINLV;database=tongjinlv;Charset=utf8"; //根据自己的设置
            Connect = new MySqlConnection(M_str_sqlcon);
        }
        public static bool TryOpen()
        {

            try
            {
                Connect.Open();
                Value.LinkOk = true;
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
        public static void SqlCommand(string M_str_sqlstr)
        {
            CreateSqlConnet();
            TryOpen();
            MessageBox.Show(M_str_sqlstr);
            MySqlCommand Conmand = new MySqlCommand(M_str_sqlstr, Connect);
            Conmand.ExecuteNonQuery();
            Conmand.Dispose();
            TryClose();
            Conmand.Dispose();
        }
    }
}
