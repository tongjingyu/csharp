using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
namespace Server
{
    class Ini
    {
        private string Path;
        public Ini(string Path)
        {
            this.Path = Path+"\\Config\\";
            if (!Directory.Exists(this.Path))Directory.CreateDirectory(this.Path); 
        }
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filepath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retval, int size, string filePath);
        public void Write(string Key, string Value)
        {
            string strFilePath = Path + "SETUP.ini";//获取INI文件路径
            WritePrivateProfileString("参数", Key, Value, strFilePath);

        }
        public string Read(string Key)
        {
            StringBuilder temp = new StringBuilder(1024);
            string strFilePath = Path + "SETUP.ini";//获取INI文件路径
            GetPrivateProfileString("参数", Key, "", temp, 1024, strFilePath);
            return temp.ToString();
        }
    }
}

