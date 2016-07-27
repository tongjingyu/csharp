using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Collections;
using System.Configuration;
using System.Globalization;
using System.ServiceProcess;
using System.IO;

namespace 安装卸载服务
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        ServiceController Service;
        private void Form1_Load(object sender, EventArgs e)
        {

            Config.Path = Ini.Read("PATH");
            Config.Name = Ini.Read("Name");
            Config.DisName = Ini.Read("DisName");
            textBox1.Text = Config.Path;
            textBox2.Text = Config.Name;
            textBox3.Text = Config.DisName;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.InitialDirectory = Directory.GetCurrentDirectory();
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = fileDialog.FileName.ToString();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                MessageBox.Show("请先选中安装文件!");
                return;
            }
             Service = new ServiceController(textBox2.Text);
             try
             {
                 string State = Service.Status.ToString();
                 MessageBox.Show("已经安装此服务!");
                 return;
             }
             catch{}
             if (ServiceInstaller.InstallService(textBox1.Text, textBox2.Text, textBox3.Text))
                 MessageBox.Show("成功安装服务‘" + textBox3.Text + "’到服务组！", "成功!");
             Application.Restart();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Service = new ServiceController(textBox2.Text);
            try
            {
                if (Service.Status.ToString() == "StartPending")
                {
                    MessageBox.Show("服务启动过程中无法卸载", "错误");
                    return;
                }
            }
            catch { }
            if (ServiceInstaller.UnInstallService(textBox2.Text))
            {
                MessageBox.Show("卸载完成！");
                try
                {
                    Service.Stop();
                }
                catch { }
            }
            else
            {
                MessageBox.Show("拒绝访问!");
                return;
            }
            Application.Restart();

        }

        private void button5_Click(object sender, EventArgs e)
        {
            Service = new ServiceController(textBox2.Text);
            try
            {
                Service.Stop();
                Service.Dispose();
            }
            catch (Exception E)
            {
                Service.Dispose();
                MessageBox.Show(E.Message);
            }
        }
        private void CheakServiceState()
        {
            progressBar1.Value = 0;
            Service = new ServiceController(textBox2.Text);
            try
            {
                string State = Service.Status.ToString();
            }
            catch (Exception E)
            {
                MessageBox.Show(E.Message);
            }
            progressBar1.Value = 100;
        }


        private void button3_Click(object sender, EventArgs e)
        {
            Service = new ServiceController(textBox2.Text);
            try
            {
                Service.Start();
            }
            catch (Exception E)
            {
                MessageBox.Show(E.Message);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
          
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Config.Path = textBox1.Text;
            Ini.Write("Path", Config.Path);
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            Config.Name = textBox2.Text;
            Ini.Write("Name", Config.Name);
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            Config.DisName = textBox3.Text;
            Ini.Write("DisName", Config.DisName);

        }

        private void timer1_Tick(object sender, EventArgs e)
        {

          try
          {
              Service = new ServiceController(textBox2.Text);
              string State = Service.Status.ToString();
              label5.Text = State;
          }
          catch { label5.Text = "未安装"; };
        }

    }
}
