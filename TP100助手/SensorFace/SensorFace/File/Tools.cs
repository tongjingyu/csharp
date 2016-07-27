using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.IO;
namespace SensorFace
{
    class Fiter
    {
        public double Pool=0;
        public int PoolIndex=0;
        public int PoolSize;
        public Fiter(int Size)
        {
            PoolSize=Size;
        }
        public double FlowPoolFilter(float Data)
        {
            double Old=0;
            if (PoolIndex > 0) Old = ((Pool) / (PoolIndex));
            Pool+=(double) Data;
            if ((PoolIndex) <= PoolSize) (PoolIndex)++;
            else (Pool) -= Old;
            return (Pool) / (PoolIndex);
        }
    }
    class DrawWatch
    {
        public Bitmap BMP_H, BMP_M, BMP_D, BMP_high, BMP_B;
        public Fiter F;
        public void Init()
        {
            BMP_M = new Bitmap(Properties.Resources.system_s);
            BMP_H = new Bitmap(Properties.Resources.system_m);
            BMP_D = new Bitmap(Properties.Resources.system_dot);
            BMP_high = new Bitmap(Properties.Resources.system_highlights);
            BMP_B = new Bitmap(Properties.Resources.system);
            F = new Fiter(10);
        }
        public Bitmap Create(double A,PictureBox pictureBox5)
        { 
        Bitmap BGI = new Bitmap(BMP_B.Width, BMP_B.Height);
        Graphics g = Graphics.FromImage(BGI);
            if (A > 1000) return BGI;
            if (A <-1000) return BGI;
            g.DrawImage(BMP_B, 0, 0, BMP_B.Width, BMP_B.Height);
        g.TranslateTransform((float)BMP_B.Width / 2.0f, (float)BMP_B.Height / 2.0f);//移动中心位置

        int z = (int)A;
        float PY = 90;
        g.RotateTransform((float)((A - z) * 360) - PY);
        g.DrawImage(BMP_M, -BMP_M.Width / 2, -BMP_M.Height / 2, BMP_M.Width, BMP_M.Height);
        g.RotateTransform(PY - (float)(A - z) * 360);

        g.RotateTransform((float)A - PY);
        g.DrawImage(BMP_H, -BMP_H.Width / 2, -BMP_H.Height / 2, BMP_H.Width, BMP_H.Height);
        g.RotateTransform(PY - (float)A);
        g.DrawImage(BMP_D, -BMP_D.Width / 2, -BMP_D.Height / 2, BMP_D.Width, BMP_D.Height);
        g.TranslateTransform(-(float)pictureBox5.Width / 2.0f, -(float)pictureBox5.Height / 2.0f);//恢复原点

        g.DrawImage(BMP_high, 0, 0, pictureBox5.Width, pictureBox5.Height);
            return BGI;
        }
    }
    class Tools
    {
        [StructLayout(LayoutKind.Explicit)]
        public struct Union
        {
            [FieldOffset(0)]
            public UInt32 U32;
            [FieldOffset(0)]
            public Int32 I32;
            [FieldOffset(0)]
            public float F;
            [FieldOffset(0)]
            public byte B0;
            [FieldOffset(1)]
            public byte B1;
            [FieldOffset(2)]
            public byte B2;
            [FieldOffset(3)]
            public byte B3;
        }
        public static void CreateCvsFromDGV(DataGridView dgv)
        {
            string Text="";
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "csv files(*.csv)|*.csv|txt files(*.txt)|*.txt|All files(*.*)|*.*";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                FileStream fs1 = new FileStream(sfd.FileName, FileMode.Create, FileAccess.Write);//创建写入文件 
                StreamWriter sw = new StreamWriter(fs1, Encoding.Default);
                for(int i=0;i<(dgv.Rows.Count-1);i++)
                {
                    sw.WriteLine(dgv.Rows[i].Cells[0].Value.ToString());
                    Text += dgv.Rows[i].Cells[0].Value.ToString();
                }
                Value.queue.Enqueue(sfd.FileName+":"+Text);
                sw.Close();
                fs1.Close();
            }
        }
        public static void ResetCom()
        {
            try
            {
                Value.serialPort1.Close();
            }
            catch { }
            try
            {
                Value.serialPort1.BaudRate = 115200;
                Value.serialPort1.PortName = Ini.Read("COM");
                Value.serialPort1.Open();
            }
            catch (Exception E) { MessageBox.Show(E.Message); }
        }
        public static byte[] DateTimeToBytes(DateTime dt)
        {
            byte[] bytes = new byte[7];
            if (dt != null)
            {
                bytes[0] = Convert.ToByte(dt.Year.ToString().Substring(2, 2), 10);
                bytes[1] = Convert.ToByte(dt.Month.ToString(), 10);
                bytes[2] = Convert.ToByte(dt.Day.ToString(), 10);
                bytes[3] = Convert.ToByte(dt.Hour.ToString(), 10);
                bytes[4] = Convert.ToByte(dt.Minute.ToString(), 10);
                bytes[5] = Convert.ToByte(dt.Second.ToString(), 10);
                bytes[6] = Convert.ToByte(((int)dt.DayOfWeek).ToString(), 10);
            }
            return bytes;
        }
        public static void DGVInit(DataGridView dataGridView1)
        {
            DataGridViewTextBoxColumn DGTBC = new DataGridViewTextBoxColumn();
            dataGridView1.Columns.Add(DGTBC);
            dataGridView1.Columns[0].HeaderText = "点击这里刷新(F5)、右键快捷菜单";
            DataGridViewCellStyle style = dataGridView1.ColumnHeadersDefaultCellStyle;
            style.BackColor = Color.Navy;
            style.ForeColor = Color.White;
            style.Font = new Font("楷体",15, FontStyle.Bold);
            DataGridViewCellStyle style1 = dataGridView1.DefaultCellStyle;
            style1.Font = new Font("宋体", 15, FontStyle.Regular);
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.Columns[0].FillWeight = 100;
            dataGridView1.EditMode = DataGridViewEditMode.EditProgrammatically;
            dataGridView1.AutoSizeRowsMode =DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
            dataGridView1.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Raised;
            dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dataGridView1.GridColor = SystemColors.ActiveBorder;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.BackgroundColor = Color.White;
            dataGridView1.Dock = DockStyle.Fill;
        }
        public static void CreateCvsToDGV(DataGridView dgv)
        {

            OpenFileDialog sfd = new OpenFileDialog();
            sfd.Filter = "csv files(*.csv)|*.csv|txt files(*.txt)|*.txt|All files(*.*)|*.*";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                string[] fileContents = File.ReadAllLines(sfd.FileName, Encoding.Default);
                while (dgv.RowCount > 1) dgv.Rows.RemoveAt(0);
                int i=0;
                foreach (string item in fileContents)
                {
                    dgv.Rows.Add();
                    dgv.Rows[i++].Cells[0].Value = item;
                }
            }
        }
        public static string GetOnlyID(DataGridView DGV)
        {
            for(int i=0;i<DGV.Rows.Count;i++)
            {
                if(DGV.Rows[i].Cells[0].Value.ToString().IndexOf("登录包")>0)
                {
                    string[] Array = DGV.Rows[i].Cells[0].Value.ToString().Split(':');
                    return Array[1];
                }
            }
            return null;
        }
        public static void CreateCvsToDGV(DataGridView dgv,string path)
        {

                string[] fileContents = File.ReadAllLines(path, Encoding.Default);
                while (dgv.RowCount > 1) dgv.Rows.RemoveAt(0);
                int i = 0;
                foreach (string item in fileContents)
                {
                    dgv.Rows.Add();
                    dgv.Rows[i++].Cells[0].Value = item;
                }
        }
        public static float ByteToFloat(byte[] Buffer, int Offset, int Mode)
        {
            Union temp;
            temp.F = 0;
            float F=0;
            try
            {
                if (Mode == 0)
                {
                    temp.B0 = Buffer[Offset];
                    temp.B1 = Buffer[Offset + 1];
                    temp.B2 = Buffer[Offset + 2];
                    temp.B3 = Buffer[Offset + 3];
                }
                if (Mode == 1)
                {
                    temp.B3 = Buffer[Offset];
                    temp.B2 = Buffer[Offset + 1];
                    temp.B1 = Buffer[Offset + 2];
                    temp.B0 = Buffer[Offset + 3];
                }
                if (Mode == 2)
                {
                    temp.B3 = Buffer[Offset + 2];
                    temp.B2 = Buffer[Offset + 3];
                    temp.B1 = Buffer[Offset + 0];
                    temp.B0 = Buffer[Offset + 1];
                }
                if (Mode == 3)
                {
                    temp.B3 = Buffer[Offset + 1];
                    temp.B2 = Buffer[Offset + 0];
                    temp.B1 = Buffer[Offset + 3];
                    temp.B0 = Buffer[Offset + 2];
                }
                F = temp.F;
            }
            catch { return 0; }
            return F;
        }

        public static bool CheakAscii(string Msg)
        {
            int A=0,O=0;
            for (int i = 0; i < Msg.Length; i++)
            {
                if (Msg[i] >= 0x20 & Msg[i] <= 0x7e) A++;
                else O++;
            }
            if (O < 2) return true;
            else return false;
        }
        public static string TryToString(string Msg)
        {
            if(CheakAscii(Msg))return Msg;
            else return HexToString(System.Text.Encoding.ASCII.GetBytes(Msg),Msg.Length);
        }
        public static int ByteFromFloat(float Data, ref byte[] Buffer, int Offset, int Mode)
        {
            Union temp;
            temp.B0 = 0;
            temp.B1 = 0;
            temp.B2 = 0;
            temp.B3 = 0;
            temp.F = Data;
            if (Mode == 0)
            {
                Buffer[Offset]=temp.B0;
                Buffer[Offset + 1]=temp.B1;
                Buffer[Offset + 2]=temp.B2 ;
                 Buffer[Offset + 3]= temp.B3;
            }
            if (Mode == 1)
            {
                Buffer[Offset]=temp.B3;
                Buffer[Offset + 1]=temp.B2;
                Buffer[Offset + 2]=temp.B1;
                temp.B0 = Buffer[Offset + 3];
            }
            if (Mode == 2)
            {
                  Buffer[Offset + 2]=temp.B3;
                  Buffer[Offset + 3]=temp.B2;
                  Buffer[Offset + 0]=temp.B1;
                  Buffer[Offset + 1]=temp.B0;
            }
            if (Mode == 3)
            {
               Buffer[Offset + 1]=temp.B3;
               Buffer[Offset + 0]=temp.B2;
               Buffer[Offset + 3]=temp.B1;
               Buffer[Offset + 2]=temp.B0;
            }
            return 4;
        }
        public static int ByteFromU32(UInt32 Data, ref byte[] Buffer, int Offset, int Mode)
        {
            Union temp;
            temp.B0 = 0;
            temp.B1 = 0;
            temp.B2 = 0;
            temp.B3 = 0;
            temp.U32 = Data;
            if (Mode == 0)
            {
                Buffer[Offset] = temp.B0;
                Buffer[Offset + 1] = temp.B1;
                Buffer[Offset + 2] = temp.B2;
                Buffer[Offset + 3] = temp.B3;
            }
            if (Mode == 1)
            {
                Buffer[Offset] = temp.B3;
                Buffer[Offset + 1] = temp.B2;
                Buffer[Offset + 2] = temp.B1;
                temp.B0 = Buffer[Offset + 3];
            }
            if (Mode == 2)
            {
                Buffer[Offset + 2] = temp.B3;
                Buffer[Offset + 3] = temp.B2;
                Buffer[Offset + 0] = temp.B1;
                Buffer[Offset + 1] = temp.B0;
            }
            if (Mode == 3)
            {
                Buffer[Offset + 1] = temp.B3;
                Buffer[Offset + 0] = temp.B2;
                Buffer[Offset + 3] = temp.B1;
                Buffer[Offset + 2] = temp.B0;
            }
            return 4;
        }
        public static byte Xor(byte[] Buf,int Length)
        {
            byte Temp=0;
            for(int i=0;i<Length;i++)
            {
              Temp^=Buf[i];
            }
            return Temp;
        }
        public static String HexToString(byte[] str,int Length)
        {
            string String = "";
            for (int i = 0; i < Length; i++)
            {
                String += str[i].ToString("X2");
                if (str[i] == ModBusClass.EndFlag)if(str[i+1]==0)return String;
                String += " ";
            }
            return String;
        }
        public static byte[] StringToHex(string s)
        {
            s = s.Replace(" ", "");
            if (((s.Length-1) % 2) != 0)
            {
                s += "0"+s;
            }
            byte[] bytes = new byte[s.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                try
                {
                    bytes[i] = Convert.ToByte(s.Substring(i * 2, 2), 16);
                }
                catch 
                {
                    
                }
            }
            return bytes;
        }
        public static byte BCD_ToTen(int bcd_data)
        {
            uint temp16, temp;
            uint data=(uint)bcd_data;
            temp16 = data >> 4;
            temp = 0x0f & data;
            data = (temp16 * 10) + temp;
            return (byte)data;
        }
        public static byte Ten_ToBCD(int hex_data)
        {
            uint temp10, temp;
            uint data = (uint)hex_data;
            temp10 = data % 100 / 10;
            temp = data % 10;
            data = temp10 << 4 | temp;
            return (byte)data;
        }
        public static byte[] Da1teTimeToBytes(DateTime DT)
        {
            byte[] Buf=new byte[8];
            Buf[0] = Ten_ToBCD(DT.Year / 100);
            Buf[1] = Ten_ToBCD(DT.Year % 100);
            Buf[2] = Ten_ToBCD(DT.Month);
            Buf[3] = Ten_ToBCD(DT.Day);
            Buf[4] = Ten_ToBCD(DT.Hour);
            Buf[5] = Ten_ToBCD(DT.Minute);
            Buf[6] = Ten_ToBCD(DT.Second);
            Buf[7] = Ten_ToBCD(Convert.ToInt32(DT.DayOfWeek));
            return Buf;  
        }
        public static byte[] Int16ToByte(int Data)
        {
            byte[] Buf = new byte[2];
            Buf[0] =(byte) (Data >> 8);
            Buf[1] = (byte)(Data & 0xff);
            return Buf;
        }
        public static int ByteToInt16(byte[] Data,int Index,int Mode)
        {
            int Temp=0;
            if (Mode == 0)
            {
                Temp = Data[Index];
                Temp <<= 8;
                Temp |= Data[Index + 1];
            }
            else if (Mode == 1)
            {
                Temp = Data[Index+1];
                Temp <<= 8;
                Temp |= Data[Index];
            }
            return Temp;
        }
        public static Color GetColorFromTFT(int TFT)
        {
            int R, G, B;
            Color Temp = new Color();
            B = (TFT& 0x1F)<<3;
            B |= (B >> 5);
            G =((TFT>>5)&0x3f)<<2;
            G |= (G >> 6);
            R =((TFT >> 11) &0x1F)<<3;
            R |= (R >> 5);
            Temp = Color.FromArgb(255, R, G , B);
            return Temp;
        }
        public static int GetTftFromColor(Color C)
        {
            int Temp;
            Temp = (C.R >>3);
            Temp <<= 6;
            Temp |=(C.G >>2);
            Temp <<= 5;
            Temp |= (C.B >>3);
            return Temp;
        }
        public static string GetStringFromByte(byte[] Buffer)
        {
            string String;
               try{
                    String=System.Text.Encoding.GetEncoding("GB2312").GetString(Buffer);
               }
               catch { String = "NULL"; }
            return String;
        }
        public static void BufferCoppy(byte[] S, byte[] D,int offset, int Length)
        {
            int i=0;
            while (Length-->0)
            {
                D[offset++] = S[i++];
            }
        }
        public static int GetCrc16(byte[] DataArray, int DataLenth)
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
    }
}
