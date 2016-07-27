using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SensorFace
{
    public partial class Help : Form
    {
        public Help()
        {
            InitializeComponent();
        }

        private void 帮助_Load(object sender, EventArgs e)
        {
            try
            {
                webBrowser1.ScriptErrorsSuppressed = true;
                webBrowser1.Url = new Uri(System.Environment.CurrentDirectory + "\\index.htm");
            }catch(Exception E){MessageBox.Show(E.Message);}
        }
    }
}
