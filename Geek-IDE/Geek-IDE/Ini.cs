using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
namespace Geek_IDE
{
    class Ini
    {
        #region "声明变量"
        static public string strFilePath = Application.StartupPath + "\\Setup.ini";//获取INI文件路径
        /// <summary>
        /// 写入INI文件
        /// </summary>
        /// <param name="section">节点名称[如[TypeName]]</param>
        /// <param name="key">键</param>
        /// <param name="val">值</param>
        /// <param name="filepath">文件路径</param>
        /// <returns></returns>
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filepath);
        /// <summary>
        /// 读取INI文件
        /// </summary>
        /// <param name="section">节点名称</param>
        /// <param name="key">键</param>
        /// <param name="def">值</param>
        /// <param name="retval">stringbulider对象</param>
        /// <param name="size">字节大小</param>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retval, int size, string filePath);

        #endregion
        public static void Write(string Group,string Key,string Value)
        {
            WritePrivateProfileString(Group, Key, Value, strFilePath);
        }
        public static string Read(string Group,string Key)
        {
            StringBuilder temp = new StringBuilder(1024);
            GetPrivateProfileString(Group, Key, "", temp, 1024, strFilePath);
            return temp.ToString();
        }
        public static void Write(string Key, string Value)
        {

            Write("Other", Key, Value);

        }
        public static string Read(string Key)
        {
            return Read("Other", Key);
        }
    }
}
