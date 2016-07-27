using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Data;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Data.OleDb;
namespace 广告邮件
{
   
    class DataBase
    {
      
        public static string GetSendMailString()
        {
            string DataBaseCom = "select Id,UserID,Mail,PassWord,Error,Note from send_mail where UserId=" + Value.UserId.ToString();
            return DataBaseCom;
        }
        public static string GetReceiveMailString()
        {
            string DataBaseCom = "select Id,UserID,Mail,Cycle,Error,Note from receive_mail where UserId=" + Value.UserId.ToString();
            return DataBaseCom;
        }
        public static string InsertNewUserid()
        {
            string DataBaseCom="insert into mail_users (Userid) values ("+Value.UserId.ToString()+")";
            return DataBaseCom;
        }
        public static string GetMaxIdString()
        {
            string DataBaseCom = "select max(id) from mail_users";
            return DataBaseCom;
        }
        public static void UpdateAndAdd()
        {

        }

    }
}
