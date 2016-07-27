using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
namespace 奇奇邮件助手
{
    class FileMan
    {
        public static string PathFile = Application.StartupPath + "/";
        public static string ReadText(string File)
        {
            try
            {
                StreamReader sr = new StreamReader(PathFile + File, Encoding.UTF8);//.GetEncoding("GB2312"));
                string line = sr.ReadToEnd();
                sr.Close();
                return line;
            }
            catch { return ""; }
        }
        public static void WriteText(string File,string Content)
        {
            StreamWriter sw = new StreamWriter(PathFile+File, false, Encoding.UTF8);//GetEncoding("GB2312"));
            sw.Write(Content);
            sw.Close();
        }
        public static void WritePathFileText(string File, string Content)
        {
            StreamWriter sw = new StreamWriter(File, false, Encoding.UTF8);//GetEncoding("GB2312"));
            sw.Write(Content);
            sw.Close();
        }
    }
}
