using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 料罐车激活工具
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private int GetCrc16(byte[] DataArray, int DataLenth)
        {
            int i, j, MyCRC = 0xFFFF;
            for (i = 0; i < DataLenth; i++)
            {
                MyCRC = (MyCRC & 0xFF00) | ((MyCRC & 0x00FF) ^ DataArray[i]);
                for (j = 1; j <= 8; j++)
                {
                    if ((MyCRC & 0x01) == 1) { MyCRC = (MyCRC >> 1); MyCRC ^= 0XA001; } else { MyCRC = (MyCRC >> 1); }
                }
            }
            return MyCRC;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            byte[] Temp;
            Temp = new ASCIIEncoding().GetBytes(textBox1.Text);
            label3.Text = (GetCrc16(Temp, Temp.Length)%9999).ToString();
            label5.Text = ((GetCrc16(Temp, Temp.Length-1))%9999).ToString();

        }
    }
}
