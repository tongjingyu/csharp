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
    public partial class Form1 : Form
    {
        
        public Form1()
        {
            InitializeComponent();
        }
        SationInfor TempInfor;
        Command SqlCom;
        private void Form1_Load(object sender, EventArgs e)
        {
            TempInfor = new SationInfor();
            SqlCom = new Command();
            SqlCom.List = new List[SationInfor.InforSize];
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GetInforToClass();
            PacketToRealSheet(ref SqlCom);
            string sCommand = FoundCommand(SqlCom);
            textBox9.Text = sCommand;
            int Error= CheakOnly();
            if (Error == 1000) return;
            if (Error > 0) { MessageBox.Show("数据库中已登记此仪表信息[" + Error.ToString() + "]条", "错误"); return; }
            try
            {
                SqlCommand objSqlCommand = new SqlCommand(sCommand, Sql.SqlConn);
                SqlDataReader Reader = objSqlCommand.ExecuteReader();
                while (Reader.Read())
                {
                    Console.WriteLine(Reader.GetValue(0));
                }
                Reader.Close();
                MessageBox.Show("成功添加仪表信息!");
            }
            catch (Exception E)
            {
                MessageBox.Show(E.Message,"请检查输入");
            }
        }
        private void GetInforToClass()
        {
            TempInfor.StationName = textBox1.Text;
            TempInfor.StationNumber = textBox8.Text;
            TempInfor.StationPasswd = textBox3.Text;
            TempInfor.StationState = textBox2.Text;
            TempInfor.StationPosition = textBox6.Text;
            TempInfor.StationCreateTime = textBox4.Text;
            TempInfor.StationType = comboBox1.Text;
            TempInfor.StationVersion = comboBox2.Text;
            TempInfor.StationOther = textBox5.Text;
            TempInfor.Note = textBox7.Text;
            TempInfor.StationLongitude = textBox10.Text;
            TempInfor.StationDimension = textBox11.Text;
        }
        private void GetClassToBox()
        {
            textBox1.Text=TempInfor.StationName ;
            textBox8.Text=TempInfor.StationNumber ;
            textBox3.Text=TempInfor.StationPasswd ;
            textBox2.Text=TempInfor.StationState;
            textBox6.Text=TempInfor.StationPosition;
            textBox4.Text=TempInfor.StationCreateTime;
            comboBox1.Text=TempInfor.StationType;
            comboBox2.Text=TempInfor.StationVersion;
            textBox5.Text=TempInfor.StationOther;
            textBox7.Text=TempInfor.Note;
            textBox10.Text = TempInfor.StationLongitude;
            textBox11.Text = TempInfor.StationDimension;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Text = "水位计";
            textBox8.Text = "2040602";
            textBox3.Text = "2040602";
            textBox2.Text ="0";
            textBox6.Text = "宜昌水文";
            textBox4.Text =DateTime.Now.ToString();
            comboBox1.Text = "水库水位";
            comboBox2.Text = "2.0";
            textBox5.Text = "无";
            textBox7.Text = "默认";
            textBox10.Text="0.0";
            textBox11.Text="0.0";
        }
        private static string FoundCommand(Command Com)
        {
            string temp = "";
            temp += "INSERT INTO ";
            temp += Com.Sheet + " (";
            for (int i = 0; i < SationInfor.InforSize; i++)
            {
                temp += Com.List[i].Name;
                if (i < (SationInfor.InforSize - 1)) temp += ",";
            }
            temp += ") " + "VALUES";
            temp += "(";
            for (int i = 0; i < SationInfor.InforSize; i++)
            {
                temp += Com.List[i].Data;
                if (i < (SationInfor.InforSize - 1)) temp += ",";
            }
            temp += ")";
            return temp;
        }
        private static string AddHead(string S)
        {
            string temp = "'";
            temp += S;
            temp += "'";
            return temp;
        }
        private  void PacketToRealSheet(ref Command Sheet)
        {
            int i;
            //(StationNumber,StationName,StationPosition,RecordDataTime,RecordWaterLevel,RecordRainFall,RecordTemperature,RecordVoltage,RecordOther,Note)";
            i = 0;
            Sheet.List[i].Name = StationInforSheet.StationName;
            Sheet.List[i++].Data =AddHead( TempInfor.StationName);
            Sheet.List[i].Name = StationInforSheet.StationNumber;
            Sheet.List[i++].Data = TempInfor.StationNumber;
            Sheet.List[i].Name = StationInforSheet.StationCreateTime;
            Sheet.List[i++].Data = AddHead(TempInfor.StationCreateTime);
            Sheet.List[i].Name =StationInforSheet.StationState;
            Sheet.List[i++].Data = TempInfor.StationState;
            Sheet.List[i].Name = StationInforSheet.Note;
            Sheet.List[i++].Data =AddHead(TempInfor.Note);
            Sheet.List[i].Name =StationInforSheet.StationType;
            Sheet.List[i++].Data =AddHead( TempInfor.StationType);
            Sheet.List[i].Name =StationInforSheet.StationVersion;
            Sheet.List[i++].Data = TempInfor.StationVersion;
            Sheet.List[i].Name = StationInforSheet.StationPasswd;
            Sheet.List[i++].Data = TempInfor.StationPasswd;
            Sheet.List[i].Name = StationInforSheet.StationOther;
            Sheet.List[i++].Data = AddHead(TempInfor.StationOther);
            Sheet.List[i].Name = StationInforSheet.StationPosition;
            Sheet.List[i++].Data = AddHead(TempInfor.StationPosition);

            Sheet.List[i].Name = StationInforSheet.RecordLongitude;
            Sheet.List[i++].Data = AddHead(TempInfor.StationLongitude);
            Sheet.List[i].Name = StationInforSheet.RecordDimension;
            Sheet.List[i++].Data = AddHead(TempInfor.StationDimension);
            Sheet.Sheet = "StationInfor";//空格暂时可以不加
        }
        private int CheakOnly()
        {
            int i = 0;
            string sCommand = "select * from StationInfor where StationNumber=" + TempInfor.StationNumber;
            try
            {
                SqlCommand objSqlCommand = new SqlCommand(sCommand, Sql.SqlConn);
                SqlDataReader Reader = objSqlCommand.ExecuteReader();
                while (Reader.Read())
                {
                    i++;
                }
                Reader.Close();
            }catch(Exception E)
                {
                    MessageBox.Show(E.Message,"请检查输入!");
                    return 1000;
                }
            return i;
        }
        private  string UpdateSheet(string Number,Command Com)
        {
            //  update StationInfor    set StationPassword=1  where StationNumber=2040602
            string temp = "";
            temp += "update ";
            temp +=Com.Sheet+" ";
            temp += " set ";
            for (int i = 0; i < SationInfor.InforSize; i++)
            {
                temp += " ";
                temp+=Com.List[i].Name+"=";
                temp += Com.List[i].Data;
                if (i < (SationInfor.InforSize - 1)) temp+=",";
            }
            temp += " where StationNumber=";
            temp += Number;
            return temp;

        }
        private void button2_Click(object sender, EventArgs e)
        {
           GetInforToClass();
           int i= CheakOnly();
           if(i==0){MessageBox.Show("数据库中无此表记录！");return;}
           if (i == 1000) return;
           PacketToRealSheet(ref SqlCom);
           string sCommand=UpdateSheet(TempInfor.StationNumber,SqlCom);
           textBox9.Text = sCommand;
           try
           {
               SqlCommand objSqlCommand = new SqlCommand(sCommand, Sql.SqlConn);
               SqlDataReader Reader = objSqlCommand.ExecuteReader();
               while (Reader.Read())
               {
                   Console.WriteLine(Reader.GetValue(0));
               }
               Reader.Close();
               MessageBox.Show("成功修改仪表信息!");
           }
           catch (Exception E)
           {
               MessageBox.Show(E.Message,"请检查输入");
           }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            int i = 0;
            GetInforToClass();
            string sCommand = "select * from StationInfor where StationNumber=" + TempInfor.StationNumber;
            try
            {
                SqlCommand objSqlCommand = new SqlCommand(sCommand, Sql.SqlConn);
                SqlDataReader Reader = objSqlCommand.ExecuteReader();
                while (Reader.Read())
                {
                    i++;
                    textBox8.Text = Reader.GetValue(1).ToString();
                    textBox3.Text = Reader.GetValue(2).ToString();
                    textBox1.Text = Reader.GetValue(3).ToString();
                    textBox2.Text = Reader.GetValue(4).ToString();
                    textBox6.Text = Reader.GetValue(5).ToString();
                    textBox11.Text = Reader.GetValue(6).ToString();
                    textBox10.Text = Reader.GetValue(7).ToString();
                    textBox4.Text = Reader.GetValue(8).ToString();
                    comboBox1.Text = Reader.GetValue(9).ToString();
                    comboBox2.Text = Reader.GetValue(10).ToString();
                    textBox5.Text = Reader.GetValue(11).ToString();
                    textBox7.Text= Reader.GetValue(12).ToString();

                }
                Reader.Close();
            }
            catch (Exception E)
            {
                MessageBox.Show(E.Message, "请检查输入!");
            }
            if (i == 0) MessageBox.Show("查无此项!");
        }
    }
}
