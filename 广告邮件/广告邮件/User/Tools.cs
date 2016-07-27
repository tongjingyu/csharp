using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using System.IO;
using System.Data;
using System.Windows.Forms;
using System.Drawing;

namespace 广告邮件
{
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
        public static float GetFloat_FromBytes(byte[] Bytes)
        {
            Union temp;
            temp.F = 0;
            temp.B0 = Bytes[0];
            temp.B1 = Bytes[1];
            temp.B2 = Bytes[2];
            temp.B3 = Bytes[3];
            return temp.F;
        }
        public static float ByteToFloat(byte[] Buffer, int Offset, int Mode)
        {
            Union temp;
            temp.F = 0;
            if (Mode == 0)
            {
                temp.B0 = Buffer[Offset];
                temp.B1 = Buffer[Offset + 1];
                temp.B2 = Buffer[Offset + 2];
                temp.B3 = Buffer[Offset + 3];
            }
            if (Mode == 1)
            {
                temp.B0 = Buffer[Offset+3];
                temp.B1 = Buffer[Offset + 2];
                temp.B2 = Buffer[Offset + 1];
                temp.B3 = Buffer[Offset + 0];
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
            float F = temp.F;
            return F;
        }
        public static string AddHead(object Data)
        {
            try
            {
                if (Data.GetType() == typeof(String)) return "'" + (string)Data + "'";
                else return Data.ToString();
            }
            catch { return ""; }
        }
        public static void SetColumnsHeader(DataGridView dataGridView1, string Serect, string Header)
        {
            try
            {
                dataGridView1.Columns[Serect].HeaderText = Header;
                dataGridView1.Columns[Serect].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            catch { }
        }
        public static void Grid_OutoSize(DataGridView DGV)
        {
            int i;
            try
            {
                DGV.AllowUserToResizeRows = false;
                DGV.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
                DGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
                DGV.RowHeadersVisible = false;
                DGV.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
                DGV.RowTemplate.Height = 23;
                DGV.TabIndex = 0;
                DGV.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
                for (i = 0; i < DGV.ColumnCount; i++)
                {
                    DGV.Columns[i].AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
                    DGV.Columns[i].SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
                }
                DGV.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
                DGV.Columns[0].ReadOnly = true;
                DGV.Columns[1].ReadOnly = true;
                DGV.Columns[4].ReadOnly = true;
                DGV.Columns[0].DefaultCellStyle.BackColor = Color.LightBlue;
                DGV.Columns[1].DefaultCellStyle.BackColor = Color.LightBlue;
                DGV.Columns[4].DefaultCellStyle.BackColor = Color.LightBlue;
                DGV.Columns[0].AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
                DGV.Columns[0].Width = 50;
                DGV.Columns[i - 1].AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            }
            catch { }
        }
        public static void ClearTable(DataTable DT)
        {
            if (DT != null) ;
            try
            {
                int Length = DT.Columns.Count;
                for (int i = 0; i < Length; i++)
                {
                    DT.Columns.RemoveAt(0);
                }
                DT.Clear();
            }
            catch { }
        }
        public static string  MailGetHost(string MailName)
        {
            StringBuilder SB = new StringBuilder(100);
            bool en = false;
            SB.Append("smtp");
            for (int i = 0; i < MailName.Length; i++)
            {
                if (en) SB.Append(MailName[i]);
                if (MailName[i] == '@')
                {
                    en = true;
                    SB.Append('.');
                }
                
            }

            return SB.ToString();
        }
        public static void SetHeader(DataGridView DGV)
        {
            Tools.SetColumnsHeader(DGV, "UserId", "用户ID");
            Tools.SetColumnsHeader(DGV, "Mail", "邮箱名");
            Tools.SetColumnsHeader(DGV, "Demain", "域名");
            Tools.SetColumnsHeader(DGV, "Note", "备注");
            Tools.SetColumnsHeader(DGV, "Cycle", "周期");
            Tools.SetColumnsHeader(DGV, "Id", "序号");
            Tools.SetColumnsHeader(DGV, "PassWord", "密码");
            Tools.SetColumnsHeader(DGV, "Error", "故障");
        }
        public static void SetDoubleBuf(Object datagrid, bool opened)
        {
            Type dgvType = datagrid.GetType();
            System.Reflection.PropertyInfo properInfo = dgvType.GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            properInfo.SetValue(datagrid, opened, null);
        }
        public static void FloatToBytes(float F, ref byte[] Bytes)
        {
            Union temp;
            temp.B0 = 0;
            temp.B1 = 0;
            temp.B2 = 0;
            temp.B3 = 0;
            temp.F = F;
            Bytes[0] = temp.B3;
            Bytes[1] = temp.B2;
            Bytes[2] = temp.B1;
            Bytes[3] = temp.B0;
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
        public static byte[] StringToHex(string s)
        {
            s = s.Replace(" ", "");
            if ((s.Length % 2) != 0)
            {
                s += "";
            }
            byte[] bytes = new byte[s.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                try
                {
                    bytes[i] = Convert.ToByte(s.Substring(i * 2, 2), 16);
                }
                catch (Exception E)
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
        public static byte[] DateTimeToBytes(DateTime DT)
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
    }
}
