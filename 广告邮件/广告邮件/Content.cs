using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Security.Permissions; //需加的
using System.IO;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Data.OleDb;
 namespace 广告邮件
 {
     [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]//需加的
     [System.Runtime.InteropServices.ComVisibleAttribute(true)]//需加的 
     public partial class Content : Form
     {
         public Content()
         {
             InitializeComponent();
         }
 
         private void Content_Load(object sender, EventArgs e)
         {
             textBox1.Text = Value.Title;
             webBrowser1.Navigate(Application.StartupPath + "/Msg.htm");
             textBox2.Text = Value.Msg = File.ReadText(Application.StartupPath + "/Msg.htm");
             textBox1.Text=File.ReadText(Application.StartupPath + "/Title.htm");
         }

         private void button1_Click(object sender, EventArgs e)
         {
             if (button1.Text == "预览")
             {
                 button1.Text ="编辑";
                 webBrowser1.Navigate(Application.StartupPath + "/Msg.htm");
                 webBrowser1.Visible = true;
                 textBox2.Visible = false;
             }
             else
             {
                 button1.Text = "预览";
                 Value.Msg = textBox2.Text;
                 webBrowser1.Visible = false;
                 textBox2.Visible = true;
             }
         }

         private void button2_Click(object sender, EventArgs e)
         {
             FileInfo Finfo = new FileInfo(Application.StartupPath + "/Msg.htm");
              if (Finfo.Exists)
              {
                  Finfo.Delete();
              }
              File.WriteText(Application.StartupPath + "/Msg.htm", textBox2.Text);
              File.WriteText(Application.StartupPath + "/Title.htm", textBox1.Text);
              Value.Title=textBox1.Text;
              Value.Msg = textBox2.Text;
         }

         private void button3_Click(object sender, EventArgs e)
         {
              MySql.CreateSqlConnet();
              if (MySql.TryOpen())
              {
                  string Com="update mail_users set Msg='"+@Value.Msg+"',Title='"+@Value.Title+"' where Id="+Value.UserId;
                  MySqlCommand Conmand = new MySqlCommand(Com, MySql.Connect);
                  try
                  {
                      Conmand.ExecuteNonQuery();
                  }
                  catch (Exception E)
                  {
                      MessageBox.Show(E.Message);
                  }
              }
         }

         private void button4_Click(object sender, EventArgs e)
         {
             MySqlDataAdapter MysqlAdapter;
             DataTable DT = new DataTable();
             MysqlAdapter = new MySqlDataAdapter("select msg,title from mail_users where Id="+Value.UserId, MySql.Connect);
             MySqlCommandBuilder msb = new MySqlCommandBuilder(MysqlAdapter);
             MysqlAdapter.Fill(DT);
             textBox1.Text = DT.Rows[0][1].ToString();
             textBox2.Text = DT.Rows[0][0].ToString();
         }
 
     }
 }