using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;

namespace IcoPng批量转换
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private bool Stop = false;
        private int Succeed = 0,Fail=0;
        private string SaveType = "Png";
        Stream stream;
        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog FB = new FolderBrowserDialog();
            if (FB.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = FB.SelectedPath;
            }
            textBox2.Text = textBox1.Text + "\\" + SaveType + "Out" + "\\";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog FB = new FolderBrowserDialog();
            if (FB.ShowDialog() == DialogResult.OK)
            {
                textBox2.Text = FB.SelectedPath;
            }
        }
        private void SavePicture(string S_Name)
        {
            string[] arry = S_Name.Split('.');
            Bitmap Bmp = new Bitmap((Bitmap)Image.FromFile(textBox1.Text + "\\" + S_Name), int.Parse(textBox3.Text), int.Parse(textBox4.Text));
            string SaveName = textBox2.Text +"\\";
            IfNotFileCreat(SaveName);
            SaveName+=(arry[0]+"."+SaveType);
            switch(SaveType)
            {
                case "Png": Bmp.Save(SaveName, ImageFormat.Png); break;
                case "Ico": Bmp.Save(SaveName, ImageFormat.Icon);
                    System.Drawing.Icon icon = System.Drawing.Icon.FromHandle(Bmp.GetHicon());
                    System.IO.FileStream fileStream = new System.IO.FileStream(SaveName, System.IO.FileMode.Create);
                    icon.Save(fileStream);
                    Bmp.Dispose();
                    fileStream.Close();
                    fileStream.Dispose();
                    icon.Dispose();
                    break;
                case "Emf": Bmp.Save(SaveName, ImageFormat.Emf); break;
                case "Jpg": Bmp.Save(SaveName, ImageFormat.Jpeg); break;
                case "Wmf": Bmp.Save(SaveName, ImageFormat.Wmf); break;
                case "Gif": Bmp.Save(SaveName, ImageFormat.Gif); break;
                case "Exif": Bmp.Save(SaveName, ImageFormat.Exif); break;
                case "Bmp": Bmp.Save(SaveName, ImageFormat.Bmp); break;
                default:break;
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            
            textBox1_TextChanged(null, null);
            int Length=richTextBox1.Lines.Length;
            progressBar1.Maximum = Length;
            Stop = false;
            Succeed = 0;
            Fail = 0;
            for(int i=0;i<Length-1;i++)
            {
                button4.Text = "停止转换";
                System.Threading.Thread.Sleep(30);
                if (Stop) break;
                try
                {
                    panel1.BackgroundImage = (Bitmap)Image.FromFile(textBox1.Text + "\\" + richTextBox1.Lines[i]);
                    int start = richTextBox1.Text.IndexOf(richTextBox1.Lines[i], 0, StringComparison.OrdinalIgnoreCase);
                    int length = (richTextBox1.Lines[i]).Length;
                    richTextBox1.SelectionStart = start;
                    richTextBox1.SelectionLength = length;
                    SavePicture(richTextBox1.Lines[i]);
                    Succeed++;
                }
                catch (Exception E)
                {
                    richTextBox1.SelectionColor = Color.Red;
                    Fail++;
                }
                progressBar1.Value = i;
                button3.Text = i.ToString() + "/" + Length.ToString();
                 Application.DoEvents();
                 label5.Text = "成功:" + Succeed.ToString();
                 label6.Text = "失败:" + Fail.ToString();
            }
            if (!Stop)
            {
                button3.Text = "转换完成";
                progressBar1.Value = 0;
            }
                button4.Text = "刷新文件";
            Stop = false;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            button3.Text = "开始转换";
            try
            {
                richTextBox1.Text = "";
                DirectoryInfo dir = new DirectoryInfo(textBox1.Text);
                FileInfo[] files = dir.GetFiles(comboBox2.Text);
                for (int i = 0; i < files.Length; i++)
                {
                    richTextBox1.AppendText(files[i].Name + "\r\n");
                }
            }
            catch { MessageBox.Show("没有选择目录"); }
            if (richTextBox1.Text == "") richTextBox1.Text = "没找到指定格式文件";
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox1_TextChanged(null, null);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            comboBox1_SelectedIndexChanged(null, null);
            if(button4.Text=="刷新文件")textBox1_TextChanged(null, null);
            if (button4.Text == "停止转换") Stop = true;
        }
        private bool IfNotFileCreat(string Path)
        {
            if (!Directory.Exists(Path))
            {
                try
                {
                    Directory.CreateDirectory(Path);
                    return true;
                }
                catch { return false; }
            }
            else return true;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
             string []arry=comboBox1.Text.Split('.');
             SaveType = arry[1];
             textBox2.Text = textBox1.Text + "\\" + SaveType + "Out" + "\\";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1_SelectedIndexChanged(null,null);
        }
    }
}
