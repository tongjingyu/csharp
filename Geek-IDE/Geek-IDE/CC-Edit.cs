using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScintillaNet;
using System.IO;
using System.Xml;
namespace Geek_IDE
{
    public partial class CC_Edit : Form
    {
        public Scintilla Myediter;
        public CC_Edit()
        {
            InitializeComponent();
            this.Myediter = new Scintilla();

            this.Myediter.Margins.Margin1.Width = 1;

            this.Myediter.Margins.Margin0.Type = MarginType.Number;

            this.Myediter.Margins.Margin0.Width = 0x23;

            this.Myediter.ConfigurationManager.Language = "cs";

            this.Myediter.Dock = DockStyle.Fill;

            this.Myediter.Scrolling.ScrollBars = ScrollBars.Both;

            this.Myediter.ConfigurationManager.IsBuiltInEnabled = true;

        }

        private void CC_Edit_Load(object sender, EventArgs e)
        {
            this.Myediter.Text = "fdsfsaf";
        }

        private void richTextBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(MousePosition);
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            // richTextBox1.Select(richTextBox1.Select(richTextBox1.Find("while"), ("while").Length))

        }

        private void tabControl1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                tabControl1.SelectedIndex = 2;
            }
        }
        private void tabControl1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                tabControl1.SelectedIndex = 2;
                contextMenuStrip1.Show(MousePosition);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Option form = new Option(false);
            form.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            New from = new New();
            from.ShowDialog();

        }

        private void looToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        private Scintilla MyediterNew()
        {
            Scintilla Tmpediter;
            Tmpediter = new Scintilla();

            Tmpediter.Margins.Margin1.Width = 1;

            Tmpediter.Margins.Margin0.Type = MarginType.Number;

            Tmpediter.Margins.Margin0.Width = 0x23;

            Tmpediter.ConfigurationManager.Language = "cs";

            Tmpediter.Dock = DockStyle.Fill;

            Tmpediter.Scrolling.ScrollBars = ScrollBars.Both;

            Tmpediter.ConfigurationManager.IsBuiltInEnabled = true;
            return Tmpediter;
        }
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TabPage tabPage = new TabPage();
            tabPage.Location = new System.Drawing.Point(4, 22);
            tabPage.Name = "tabPage1";
            tabPage.Padding = new System.Windows.Forms.Padding(3);
            tabPage.Size = new System.Drawing.Size(651, 248);
            tabPage.Tag = " ";
            tabPage.Text = "NewText1.cc*";
            tabPage.UseVisualStyleBackColor = true;
            this.tabControl1.Controls.Add(tabPage);
            Scintilla Tmpediter = MyediterNew();
            Tmpediter.Text = "";
            tabPage.Controls.Add(Tmpediter);//加入编辑代码的控件 这里取名“Myediter”。
            tabControl1.SelectTab(tabPage);
            richTextBox1.AppendText("New File Create\n");
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog OpenFile = new OpenFileDialog();
            DialogResult R=OpenFile.ShowDialog();
            if (DialogResult.Cancel == R) return;
            FileInfo fi = new FileInfo(OpenFile.FileName);
            TabPage tabPage = new TabPage();
            tabPage.Text = fi.Name;
            tabPage.Tag = OpenFile.FileName;
            for (int i = 0; i < tabControl1.TabCount; i++) if (tabControl1.TabPages[i].Tag.ToString() == OpenFile.FileName) return;
            this.tabControl1.Controls.Add(tabPage);
            Scintilla Tmpediter = MyediterNew();
            Tmpediter.Text = "";
            tabPage.Controls.Add(Tmpediter);//加入编辑代码的控件 这里取名“Myediter”。
            tabControl1.SelectTab(tabPage);
            richTextBox1.AppendText("New File Create\n");
            StreamReader ReadTxt;
            ReadTxt = new StreamReader(OpenFile.FileName, UnicodeEncoding.Default);
            Tmpediter.Text = ReadTxt.ReadToEnd();

        }

        private void newToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SaveFileDialog SFD = new SaveFileDialog();
            SFD.Filter = "(*.Projeek)|*.Projeek";               //过滤文件类型
            SFD.RestoreDirectory = true; //记忆上次浏览路径
            DialogResult R=SFD.ShowDialog();
            if (R == DialogResult.Cancel) return;
            FileInfo FI = new FileInfo(SFD.FileName);
            XmlDocument doc = new XmlDocument();
            XmlElement root = doc.CreateElement("Project");
            XmlElement list = doc.CreateElement("list");
            root.AppendChild(list);
            for (int i = 0; i < 4; i++)
            {
                XmlElement note = doc.CreateElement("text" + i + ".cc");
                list.AppendChild(note);
            }
            XmlElement set = doc.CreateElement("set");
            root.AppendChild(set);
            for (int i = 0; i < 4; i++)
            {
                XmlElement note = doc.CreateElement("text" + i + ".cc");
                set.AppendChild(note);
            }
            doc.AppendChild(root);
            string strFileName = SFD.FileName;
            doc.Save(strFileName);
            XmlNodeList nodes = doc.GetElementsByTagName("add");
            for (int i = 0; i < nodes.Count; i++)
            {
                XmlAttribute att = nodes[i].Attributes["key"];
                if (att.Value == "time")
                {
                    att = nodes[i].Attributes["value"];
                    att.Value = DateTime.Now.ToString(); ;
                    break;
                }
            }
            doc.Save(strFileName);
        }
    }
}
