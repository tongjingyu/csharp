using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace 广告邮件
{
    class File
    {
        public static string ReadText(string File)
        {
            StreamReader sr = new StreamReader(File, Encoding.UTF8);//.GetEncoding("GB2312"));
            string  line = sr.ReadToEnd();
            sr.Close();
            return line;
        }
        public static void WriteText(string File,string Content)
        {
            StreamWriter sw = new StreamWriter(File, false, Encoding.UTF8);//GetEncoding("GB2312"));
            sw.Write(Content);
            sw.Close();
        }
    }
}
