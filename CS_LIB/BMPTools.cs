using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections;
using System.ComponentModel;
using System.Data;
namespace NS_BMP_Tools
{
    class BMP_Tools
    {
        public Bitmap SBMP;
        private Size ResPic1UnitSize = new Size(125, 125);
        private Point LeftTopPoint= new Point(0, 0);
        public  Bitmap Cut(int x, int y, int width, int height)
        {
            Bitmap bmp = new Bitmap(width, height);
            bmp.SetResolution(SBMP.HorizontalResolution, SBMP.VerticalResolution);
            Graphics g = Graphics.FromImage(bmp);
             g.Clear(Color.White);
             g.InterpolationMode = InterpolationMode.HighQualityBicubic;
             g.SmoothingMode = SmoothingMode.HighQuality;
             g.DrawImage(SBMP, new Rectangle(0, 0, width, height), new Rectangle(x, y, width, height), GraphicsUnit.Pixel);
             return bmp;
        }
        public Bitmap Cut(Rectangle Rec)
        {
            return Cut(Rec.X, Rec.Y, Rec.Width, Rec.Height);
        }
        public void GetImage(string FilePath)
        {
            SBMP= (Bitmap)Image.FromFile(FilePath);
        }
        public void GetImage(Bitmap img)
        {
            SBMP = img;
        }
        public  Bitmap CutButtonImage(int Index_X, int Index_Y)
        {
            Index_X *= ResPic1UnitSize.Width;
            Index_X+=LeftTopPoint.X;
            Index_Y *= ResPic1UnitSize.Height;
            Index_Y+=LeftTopPoint.Y;
           // Rectangle rec = new Rectangle(Index_X, Index_Y, ResPic1UnitSize.Width+Index_X, ResPic1UnitSize.Height+Index_Y);
           // return SBMP.Clone(rec, SBMP.PixelFormat);
            return Cut(Index_X, Index_Y, ResPic1UnitSize.Width , ResPic1UnitSize.Height );
        }
        public Rectangle GetSelectRectangle(int X, int Y)
        {
            int X0=0, Y0=0, X1=0, Y1=0;
            Color B_Color = Color.FromArgb(0xff, 0x00, 0x00, 0x00);
            for (int i = X; i>0; i--)
            {
                Color TempC = SBMP.GetPixel(i, Y);
               // MessageBox.Show(TempC.ToString());
                if (TempC == B_Color) { X0 = i; break; }
            }
            for (int i = X; i > 0; i--)
            {
                Color TempC = SBMP.GetPixel(X, i);
                if (TempC == B_Color) { Y0 = i; break; }
            }
            for (int i = X; i < (SBMP.Width - X); i++)
            {
                Color TempC = SBMP.GetPixel(i, Y);
                if (TempC == B_Color) { X1 = i; break; }
            }
            for (int i = Y; i < (SBMP.Height - Y); i++)
            {
                Color TempC = SBMP.GetPixel(X, i);
                if (TempC == B_Color) { Y1 = i; break; }
            }
          Rectangle Rec= new Rectangle(X0, Y0, X1-X0,Y1-Y0 );
          if (Rec.Height <= 0 || Rec.Height <= 0) MessageBox.Show("无效区域");
          return Rec;
        }
    }
}
