using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace BaseManage
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());
            Form2 frm1 = new Form2();
            frm1.ShowDialog();
            if (frm1.LoginIsOK == true)
            {
                Application.Run(new Form1());
            }
            else
            {
                Application.Exit();
            }
        }
    }
}
