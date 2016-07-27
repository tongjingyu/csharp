using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;
using System.IO;
namespace 广告邮件
{
    class Excel
    {
        public static void SaveCSVFile(DataTable dt, string fileName)
        {
            FileStream fs = new FileStream(fileName, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.Default);
            string data = "";

            //写出列名称
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                data += dt.Columns[i].ColumnName.ToString();
                if (i < dt.Columns.Count - 1)
                {
                    data += ",";
                }
            }
            sw.WriteLine(data);

            //写出各行数据
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                data = "";
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    data += dt.Rows[i][j].ToString();
                    if (j < dt.Columns.Count - 1)
                    {
                        data += ",";
                    }
                }
                sw.WriteLine(data);
            }

            sw.Close();
            fs.Close();
        }
        public static void SaveCSV(DataTable dt)
        {
            SaveFileDialog Dlg = new SaveFileDialog();
            Dlg.DefaultExt = "csv";
            Dlg.Filter = "csv文件(*.csv)|*.csv";
            if (Dlg.ShowDialog() == DialogResult.Cancel) throw new Exception("操作已经取消"); 
            string fileName = Dlg.FileName;
            SaveCSVFile(dt, fileName);
            MessageBox.Show("CSV文件保存成功！");
        }
        public static DataTable OpenCSVFile(string fileName)
        {
            DataTable dt = new DataTable();
            FileStream fs = new FileStream(fileName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            StreamReader sr = new StreamReader(fs, System.Text.Encoding.Default);
            //记录每次读取的一行记录
            string strLine = "";
            //记录每行记录中的各字段内容
            string[] aryLine;
            //标示列数
            int columnCount = 0;
            //标示是否是读取的第一行
            bool IsFirst = true;

            //逐行读取CSV中的数据
            while ((strLine = sr.ReadLine()) != null)
            {
                aryLine = strLine.Split(',');
                if (IsFirst == true)
                {
                    IsFirst = false;
                    columnCount = aryLine.Length;
                    //创建列
                    for (int i = 0; i < columnCount; i++)
                    {
                        DataColumn dc = new DataColumn(aryLine[i]);
                        dt.Columns.Add(dc);
                    }
                }
                else
                {
                    DataRow dr = dt.NewRow();
                    for (int j = 0; j < columnCount; j++)
                    {
                        dr[j] = aryLine[j];
                    }
                    dt.Rows.Add(dr);
                }
            }

            sr.Close();
            fs.Close();
            return dt;
        }
        public static DataTable OpenCSV()
        {
            OpenFileDialog Dlg = new OpenFileDialog();
            Dlg.DefaultExt = "csv";
            Dlg.Filter = "csv文件(*.xml)|*.csv";
            if (Dlg.ShowDialog() == DialogResult.Cancel) throw new Exception("操作已经取消");
            string fileName = Dlg.FileName;
            return OpenCSVFile(fileName);
            
        }


    }
}
