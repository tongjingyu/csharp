using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace TXTShow
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            try{
                Config.Path = args[0];
            }
            catch { Config.Path = ""; }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
