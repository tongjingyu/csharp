using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Data;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Data.OleDb;
namespace 奇奇邮件助手
{
   
    class DataBase
    {
      
        public static string GetSendMailString()
        {
            string DataBaseCom = "select Id,Mail,Error,Note from send_mail where UserId=" + Value.UserId.ToString();
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
        public static string GetJoinFromMail(string EMail, string Password)
        {
            string DataBaseCom = "INSERT INTO dd_frommail(EMail,Password)VALUES ('" + EMail + "', '" + Password + "');";
            return DataBaseCom;
        }
        public static string GetJoinRegUser(string LoginEmail, string UserName, string Password)
        {
            string DataBaseCom = "INSERT INTO dd_users(LoginEmail,UserName,Password,CreateDate)VALUES ('" + LoginEmail + "', '" + UserName + "','" + Password + "','"+DateTime.Now.ToString()+"');";
            return DataBaseCom;
        }
        public static string GetRecordCount()
        {
            string DataBaseCom = "select count(*) as number from dd_frommail where Id>0";
            return DataBaseCom;
        }
        public static string GetRecordCount(string Str1)
        {
            string DataBaseCom = "select count(*) as number from " + Str1;
            return DataBaseCom;
        }
        public static void UpdateAndAdd()
        {

        }
        public static string GetLoginInfor(string Name, string Password)
        {
            string DataBaseCom = "SELECT UserName,CreateDate,BeUser FROM dd_users WHERE LoginEmail='" + Name + "' AND Password='" + Password + "'";
            return DataBaseCom;
        }
        public static string GetJoinLeave(string LeaveMsg, string LeaveName, string LeaveContact)
        {
            string DataBaseCom = "INSERT INTO dd_leave(LeaveMsg,LeaveName,LeaveContact,CreateDate)VALUES ('" + LeaveMsg + "', '" + LeaveName + "','" + LeaveContact + "','" + DateTime.Now.ToString() + "');";
            return DataBaseCom;
        }
        public static string GetObjectValue(string Name)
        {
            string DataBaseCom ="SELECT CreateDate,object FROM dd_object WHERE Name='" + Name + "'";
            return DataBaseCom;
        }
        public static string GetFromMailList()
        {
            string DataBaseCom = "SELECT Email,Password FROM dd_frommail";
            return DataBaseCom;
        }
        public static string GetJoinLoginReg(PC_InforStruct PI)
        {
            try
            {
                string Com = "insert into dd_loginlist (Record,IPInfor,UserName,SysType,Memory,DateTime,PCName,Mac) VALUES(" +
                Tools.AddHead(PI.Record) + "," +
                Tools.AddHead(PI.IPInfor) + "," +
                Tools.AddHead(PI.UserName) + "," +
                Tools.AddHead(PI.SysType) + "," +
                Tools.AddHead(PI.MemorySize) + "," +
                Tools.AddHead(DateTime.Now.ToString()) + "," +
                Tools.AddHead(PI.PCName) + "," +
                Tools.AddHead(PI.MAC) + ")";
                return Com;
            }
            catch (Exception E) { MessageBox.Show(E.Message); }
            return "";
        }
    }
}
