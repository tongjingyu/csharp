using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NS_DataSave;
namespace 奇奇邮件助手
{
    public partial class QQ邮箱通信录转换 : Form
    {
        public QQ邮箱通信录转换()
        {
            InitializeComponent();
        }

        private void QQ邮箱通信录转换_Load(object sender, EventArgs e)
        {
            Tools.Grid_OutoSize(dataGridView1);
            Tools.SetDoubleBuf(dataGridView1, true);
            //Tools.Grid_OutoSize(dataGridView2);
            //Tools.SetDoubleBuf(dataGridView2, true);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog Dlg = new OpenFileDialog();
            Dlg.DefaultExt = "CSV";
            Dlg.Filter = "CSV文件(*.CSV)|*.CSV";
            if (Dlg.ShowDialog() == DialogResult.Cancel) return;
            DataTable DT = new DataTable();
            DT = Excel.OpenCSVFile(Dlg.FileName);
            dataGridView2.DataSource = DT;
           // Tools.Grid_OutoSize(dataGridView2);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            while(dataGridView1.RowCount<dataGridView2.RowCount)
            {
                dataGridView1.Rows.Add();
            }
            try
            {
                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    dataGridView1[0, i].Value = dataGridView2[dataGridView2.SelectedCells[1].ColumnIndex, i].Value;
                    Application.DoEvents();
                }
                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    dataGridView1[1, i].Value = dataGridView2[dataGridView2.SelectedCells[0].ColumnIndex, i].Value;
                    Application.DoEvents();
                }
            }
            catch (Exception E)
            {
                MessageBox.Show(E.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SaveFileDialog Dlg = new SaveFileDialog();
            Dlg.InitialDirectory = Value.PathWork;
            Dlg.DefaultExt = "CSV";
            Dlg.Filter = "CSV文件(*.CSV)|*.CSV";
            if (Dlg.ShowDialog() == DialogResult.Cancel) return;
            DataSave.SetDataToFile(dataGridView1, Dlg.FileName);
            Ini.Write("MailPath", Dlg.FileName);
            this.Text = Dlg.FileName + Value.SoftName;
        }
    }
}
