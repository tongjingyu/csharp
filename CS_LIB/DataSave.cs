using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
namespace NS_DataSave
{
    class DataSave
    {
        public static bool GetDataFromFile(string Path,DataGridView DGV)
        {
           try
            {
                FileStream fs = new FileStream(Path, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                StreamReader sr = new StreamReader(fs, System.Text.Encoding.Default);
                string[] aryLine;
                string strLine;
                int i = 0;
                for (int n = 0; n < DGV.RowCount; n++) DGV.Rows.Clear();
                while ((strLine = sr.ReadLine()) != null)
                {
                    aryLine = strLine.Split(',');
                    bool Ok=true;
                    if(aryLine.Length>DGV.ColumnCount)Ok=false;
                    for (int n = 0; n < aryLine.Length; n++) if (aryLine[n] == null) Ok = false;
                    if (Ok)
                    {
                        if(i>=(DGV.RowCount-1))DGV.Rows.Add();
                        for (int j = 0; j < aryLine.Length; j++)
                        {
                            DGV[j, i].Value = aryLine[j];
                            Application.DoEvents();
                        }
                        i++;
                    }
                }
                sr.Close();
                fs.Close();
            }
            catch { return false; }
            return true;
        }
        public static bool GetDataFromTextbox(string textbox, DataGridView DGV)
        {
            try
            {
                string[] Mm = textbox.Split('\n');
                string[] aryLine;
                string strLine;
                int i = 0;
                for (int n = 0; n < DGV.RowCount; n++) DGV.Rows.Clear();
                for(int m=0;m<Mm.Length;m++)
                {
                    strLine = Mm[m];
                    aryLine = strLine.Split(',');
                    bool Ok = true;
                    if (aryLine.Length > DGV.ColumnCount) Ok = false;
                    for (int n = 0; n < aryLine.Length; n++) if (aryLine[n] == null) Ok = false;
                    if (Ok)
                    {
                        if (i >= (DGV.RowCount - 1)) DGV.Rows.Add();
                        for (int j = 0; j < aryLine.Length; j++)
                        {
                            DGV[j, i].Value = aryLine[j];
                            Application.DoEvents();
                        }
                        i++;
                    }
                }
            }
            catch { return false; }
            return true;
        }
        public static bool IfSave(DataGridView DGV, string Path)
        {
            FileStream fs = new FileStream(Path, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            StreamReader sr = new StreamReader(fs, System.Text.Encoding.Default);
            int i=0;
            while (sr.ReadLine()!=null)
            {
                i++;
            }

            if (MessageBox.Show("新增记录"+(DGV.RowCount-i-1).ToString()+"条?", "确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) 
            {
                return true;
            } 
            else
            {
                return false;
            }
        }
        public static bool SetDataToFile(DataGridView DGV, string Path)
        {
           try
            {
                int Error = 0;
                StringBuilder SB = new StringBuilder();
                for (int y = 0; y < DGV.RowCount-1; y++)
                {
                    bool Ok = true;

                    for (int x = 0; x < DGV.ColumnCount; x++)
                    {
                        if (DGV[x, y].Value==null) Ok = false;
                    }
                    if (!Ok) Error++;
                    string Str = "";
                    if (Ok)
                    {
                        for (int x = 0; x < DGV.ColumnCount; x++)
                        {
                            Str += DGV[x, y].Value.ToString();
                            if (x < (DGV.ColumnCount - 1)) Str += ",";
                            
                        }
                        SB.AppendLine(Str);
                    }
                }
               if(Error>0)
               {
                   if (MessageBox.Show("错误记录【" + (Error).ToString() + "】条被丢弃? \n结果未被保存请修正!", "确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) goto SAVE;
                   else goto NOTSAVE;
               }else goto SAVE;
               
         SAVE:    {
                FileStream fs = new FileStream(Path, System.IO.FileMode.Create, System.IO.FileAccess.ReadWrite);
                StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.Default);
                sw.Write("");
                sw.Write(SB.ToString());
                sw.Close();
                fs.Close();
               }
        NOTSAVE:;
            }
            catch { return false; }
            return true;
        }
    }
}
