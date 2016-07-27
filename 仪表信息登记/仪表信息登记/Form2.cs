using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Data.SqlClient;

namespace BaseManage
{
   
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private bool loginIsOK;

        public bool LoginIsOK
        {
            get { return loginIsOK; }
            set { loginIsOK = value; }
        }
        private void button10_Click(object sender, EventArgs e)
        {
            Form1 form = new Form1();
            SqlConnectionStringBuilder SqlStr = new SqlConnectionStringBuilder();
                SqlStr.Password = textBox8.Text;//密码
                SqlStr.UserID = textBox7.Text;// 用户名
                SqlStr.ConnectTimeout = 5000;//超时退出
                SqlStr.DataSource = Configs.SqlDataSource;//数据库主机地址
                SqlStr.IntegratedSecurity = false;
                SqlStr.InitialCatalog = Configs.SqlDataBase;//数据库名
                string SqlConnStr = SqlStr.ToString();
                Sql.SqlConn = new SqlConnection(SqlConnStr);
                try
                {
                    Sql.SqlConn.Open();
                    if (Sql.SqlConn.State == ConnectionState.Open)
                    {
                        loginIsOK = true;
                        this.Close();
                    }
                }
                catch (Exception E)
                {
                    MessageBox.Show(E.Message);
                }
        }
        class Configs
        {
            public const string SqlPassWord = "123456";
            public const string SqlName = "TestUser";
            public const string SqlDataSource = @"192.168.1.188\SQLEXPRESS";
            public const string SqlDataBase = "YLN";
            public const string SqlDensitySheet = "DensityRecord";
            public const string SqlRealTimeSheet = "StationInfor";
            public const int SqlConnectTimeOut = 5000;
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }
    }
}
