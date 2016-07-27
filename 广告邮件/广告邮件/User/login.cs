using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Data.OleDb;
using System.IO;
using System.Windows.Forms;
namespace 广告邮件
{
    class login
    {
        public static void LoginThread()
        {
            MySqlDataAdapter MysqlAdapter;
            string Id =RegEdit.GetReg("userid");
            if (Id == "")
            {
                MySql.CreateSqlConnet();
                if (MySql.TryOpen())
                {
                    Value.LinkOk = true;
                    MySqlCommand Conmand = new MySqlCommand(DataBase.InsertNewUserid(), MySql.Connect);
                    Conmand.ExecuteNonQuery();
                    DataTable DT = new DataTable();
                    MysqlAdapter = new MySqlDataAdapter(DataBase.GetMaxIdString(), MySql.Connect);
                    MySqlCommandBuilder msb = new MySqlCommandBuilder(MysqlAdapter);
                    MysqlAdapter.Fill(DT);
                    RegEdit.SetReg("userid", DT.Rows[0][0].ToString());
                    Value.UserId=int.Parse(DT.Rows[0][0].ToString());
                    MessageBox.Show("自动分配到ID:"+Value.UserId.ToString(),"提示!" );
                }
            }
            else
            {
                Id =RegEdit.GetReg("userid");
                Value.UserId=int.Parse(Id);
            }
        }
    }
}
