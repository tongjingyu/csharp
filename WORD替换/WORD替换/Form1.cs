using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;
namespace WORD替换
{
    public partial class Form1 : Form
    {
        Thread t1;
        string Path;
        public Form1()
        {
            InitializeComponent();
        }
        bool Run = true;
        private void WordReplace(string filePath, string strOld, string strNew)
        {
            Microsoft.Office.Interop.Word.Application app = new Microsoft.Office.Interop.Word.Application();
            object nullobj = System.Reflection.Missing.Value;
            object file = filePath;
            Microsoft.Office.Interop.Word._Document doc = app.Documents.Open(
            ref file, ref nullobj, ref nullobj,
            ref nullobj, ref nullobj, ref nullobj,
            ref nullobj, ref nullobj, ref nullobj,
            ref nullobj, ref nullobj, ref nullobj,
            ref nullobj, ref nullobj, ref nullobj, ref nullobj) as Microsoft.Office.Interop.Word._Document;

            app.Selection.Find.ClearFormatting();
            app.Selection.Find.Replacement.ClearFormatting();
            app.Selection.Find.Text = strOld;
            app.Selection.Find.Replacement.Text = strNew;

            object objReplace = Microsoft.Office.Interop.Word.WdReplace.wdReplaceAll;
            app.Selection.Find.Execute(ref nullobj, ref nullobj, ref nullobj,
                                       ref nullobj, ref nullobj, ref nullobj,
                                       ref nullobj, ref nullobj, ref nullobj,
                                       ref nullobj, ref objReplace, ref nullobj,
                                       ref nullobj, ref nullobj, ref nullobj);
            doc.Save();
            doc.Close(ref nullobj, ref nullobj, ref nullobj);

            app.Quit(ref nullobj, ref nullobj, ref nullobj);
        }
        private void Move(string Name,string A, string B, int Size,string Text)
        {
            Random ran = new Random();
            int RandKey = ran.Next(1, int.Parse(Text));
            float Data = (float)RandKey / 10;
            WordReplace(Name, A, (Size + Data).ToString());
            WordReplace(Name, B, (Data).ToString());
        }
        private void WordPreas(string Name)
        {

            Move(Name, "aaaa1", "bbbb1", 20,textBox1.Text);
            Move(Name, "aaaa2", "bbbb2", 0, textBox1.Text);
            Move(Name, "aaaa3", "bbbb3", 20, textBox1.Text);
            Move(Name, "aaaa4", "bbbb4", 40, textBox1.Text);

        }
        private void Form1_Load(object sender, EventArgs e)
        {

            Path = Application.StartupPath+"\\";
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Run = false;
        }
        private void Thread1()
        {
            string DrsName = textBox4.Text;
            Run = true;
            int z =int.Parse(textBox2.Text);
            for (int i = 0; i < int.Parse(textBox3.Text); i++)
            {
                try
                {
                    File.Copy(Path+DrsName + ".doc", Path + DrsName + z.ToString("000") + ".doc");
                    WordReplace(Path + DrsName + z.ToString("000") + ".doc", "mmm", z.ToString("000"));
                    WordPreas(Path + DrsName + z.ToString("000") + ".doc");
                }
                catch (Exception E) { MessageBox.Show(E.Message); }
                z++;
                if (!Run) return;
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if(button1.Text=="启动")
            {
                button1.Text = "停止";
                t1 = new Thread(new ThreadStart(Thread1));
                t1.Start();
            }else
            {
                Run = false;
                t1.Abort();
                button1.Text = "启动";
            }

        }
    }
}
