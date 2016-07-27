using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
namespace TXTShow
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        StreamReader ReadTxt;
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                ReadTxt = new StreamReader(Config.Path, UnicodeEncoding.UTF8);

            }
            catch
            {
                if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    Config.Path = this.openFileDialog1.FileName;
                    ReadTxt = new StreamReader(Config.Path, UnicodeEncoding.UTF8);
                }
            }
            try
            {
                ReadTxt.Close();
                Config.Encoded = "UTF8";
                ExpEncoded();
            }
            catch { this.Close(); }
            textBox1.AllowDrop = true;
            textBox1.DragEnter += new DragEventHandler(textBox_DragEnter);
            textBox1.DragDrop += new DragEventHandler(textBox_DragDrop);
        }
          void textBox_DragDrop(object sender, DragEventArgs e)
        {
            string path = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            Config.Path = path;
            ExpEncoded();
        }
        void textBox_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Link;
            else 
                e.Effect = DragDropEffects.None;
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            /*StreamReader ReadTxt = new StreamReader(Config.Path, UnicodeEncoding.UTF8);
            textBox1.Text = ReadTxt.ReadToEnd();
            ReadTxt.Close();*/
        }

        private void textBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            contextMenuStrip1.Show(this.Left+e.X,this.Top+e.Y);
        }

        private void binHexToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Config.Encoded = "HEX";
            ExpEncoded();
        }

        private void uTF8ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Config.Encoded = "UTF8";
            ExpEncoded();
        }

        private void 保存ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.InitialDirectory = "c:";
            saveFileDialog1.Filter = "Txt文本(*.txt)|*.txt|所有格式(*.*)|*.*";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (saveFileDialog1.FileName != "")
                {
                    StreamWriter sw = new StreamWriter(saveFileDialog1.FileName);
                    sw.Write(textBox1.Text);
                }
            }
        }

        private void 打开ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Config.Path = this.openFileDialog1.FileName;
                ReadTxt = new StreamReader(Config.Path, UnicodeEncoding.UTF8);
                binHexToolStripMenuItem_Click(null,null);
            }
        }

        private void 字体ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FontDialog fontDialog = new FontDialog();
            if (fontDialog.ShowDialog() != DialogResult.Cancel)
            {
                textBox1.Font= fontDialog.Font;//将当前选定的文字改变字体  
            }
        }
        private void ExpEncoded()
        {
            if (Config.Encoded == "HEX")
            {
                FileStream fs = new FileStream(Config.Path, FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);
                int length = (int)fs.Length;
                byte[] Buffer = br.ReadBytes(length);
                string temp = "";
                for (int i = 0; i < length; i++)
                {
                    temp += ("0x" + Buffer[i].ToString("x2") + " ");
                    if (i % 32 == 31) textBox1.AppendText("\r\n");
                }
                textBox1.Text = temp;
                fs.Close();
                br.Close();
            }
            else if (Config.Encoded == "UTF8")
            {
                StreamReader ReadTxt = new StreamReader(Config.Path, UnicodeEncoding.UTF8);
                textBox1.Text = ReadTxt.ReadToEnd();
                ReadTxt.Close();
            }
        }
    }
    class Config
    {
        public static string Path;
        public static string Encoded="UTF8";
    }
}
