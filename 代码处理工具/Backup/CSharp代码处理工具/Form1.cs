using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CSharp代码处理工具
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string Msg = "";
            StringBuilder SB;
            
            for (int i = 0; true; i++)
            {
                try
                {
                    bool En = false;
                    string Temp = textBox1.Lines[i];
                    SB = new StringBuilder(200);
                    for (int n = 0; n < Temp.Length; n++)
                    {
                        if (En) SB.Append(Temp[n]);
                        if (Temp[n] == ' ') En = true;
                    }
                    Msg += SB.ToString();
                    Msg += "\r\n";
                }
                catch (Exception E) { break; }
            }
            textBox1.Text = Msg;
            textBox1.SelectAll();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Clipboard.Clear();
            Clipboard.SetData(DataFormats.Text, textBox1.Text);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Text = Clipboard.GetData(DataFormats.Text).ToString(); 
        }
        string Clearzhushi(string Temp)
        {
            StringBuilder SB;
            bool En = true;
            SB = new StringBuilder(200);
            for (int n = 0; n < Temp.Length; n++)
            {
                if (Temp[n] == '/') En = false;
                if (En) SB.Append(Temp[n]);
            }
            return SB.ToString();
        }
        private void button4_Click(object sender, EventArgs e)
        {
            string Msg = "";
            StringBuilder SB;
            int Index = 0;
            bool En=true;
            bool En1=true;
            int Index1=0;
            bool En2 = true;
            for (int i = 0; true; i++)
            {
                try
                {
                     En = true;
                    string Temp = textBox1.Lines[i];

                    if (Temp.IndexOf('{') != -1) { Index++; En = false; }
                    if (Temp.IndexOf('}') != -1) {Index--; En = false; }
                    if (Index == 0)if(En)if(Temp.Length>2) 
                    {
                        En2 = true;
                        try
                        {
                            Index1 = Temp.IndexOf('/');
                            if (Temp[Index1 + 1] == '*') { En1 = false; En2 = false; }
                            
                        }
                        catch { }
                        try
                        {
                            Index1 = Temp.IndexOf('/');
                            if (Index1 > 1) if (Temp[Index1 - 1] == '*') { En1 = true; En2 = false; }
                            
                        }
                        catch { }
                        if (En1) if (En2) { Msg += Clearzhushi(Temp); Msg += ";\r\n"; }
                        
                    }
                    
                }
                catch (Exception E) { break; }
            }
            textBox1.Text = Msg;
            textBox1.SelectAll();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            textBox1.Text = textBox1.Text.Replace(textBox2.Text, textBox3.Text);
        }
    }
}
