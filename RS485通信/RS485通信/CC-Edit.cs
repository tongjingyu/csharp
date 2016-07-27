using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace Geek
{
    public partial class CC_Edit : Form
    {
        public CC_Edit()
        {
            InitializeComponent();
        }

        private void CC_Edit_Load(object sender, EventArgs e)
        {

        }

        private void richTextBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if(e.Button==MouseButtons.Right)
            {
                contextMenuStrip1.Show(MousePosition);
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            // richTextBox1.Select(richTextBox1.Select(richTextBox1.Find("while"), ("while").Length))
            richTextBox1.ForeColor = Color.Red;
    
        }

        private void tabControl1_MouseClick(object sender, MouseEventArgs e)
        {
            if(e.Button==MouseButtons.Right)
            {
              //  if(e.Location.X==3)
                TabPage tabPage3 = new System.Windows.Forms.TabPage();
                tabPage3.Location = new System.Drawing.Point(6, 33);
                tabPage3.Name = "tabPage1";
                tabPage3.Padding = new System.Windows.Forms.Padding(3);
                tabPage3.Size = new System.Drawing.Size(734, 401);
                tabPage3.TabIndex = 0;
                tabPage3.Text = "tabPage1";
                tabPage3.UseVisualStyleBackColor = true;
                tabControl1.Controls.Add(tabPage3);
            }
        }
    }
}
