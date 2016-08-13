using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
namespace Datamark
{
    public class PrintLab
    {

        [DllImport("CDFPSK.dll")]
        public static extern int OpenPort(uint px);
        [DllImport("CDFPSK.dll")]
        public static extern int PTK_SetPrintSpeed(uint px);
        [DllImport("CDFPSK.dll")]
        public static extern int PTK_SetDarkness(uint id);
        [DllImport("CDFPSK.dll")]
        public static extern int ClosePort();
        [DllImport("CDFPSK.dll")]
        public static extern int PTK_FormDel(string pid);
        [DllImport("CDFPSK.dll")]
        public static extern int PTK_FormDownload(string pid);
        [DllImport("CDFPSK.dll")]
        public static extern int PTK_FormEnd();
        [DllImport("CDFPSK.dll")]
        public static extern int PTK_ExecForm(string pid);
        [DllImport("CDFPSK.dll")]
        public static extern int PTK_Download();
        [DllImport("CDFPSK.dll")]
        public static extern int PTK_DownloadInitVar(string pstr);
        [DllImport("CDFPSK.dll")]
        public static extern int PTK_PrintLabel(uint number, uint cpnumber);
        [DllImport("CDFPSK.dll")]
        public static extern int PTK_DefineCounter(uint id, uint maxNum, short ptext, string pstr, string pMsg);
        [DllImport("CDFPSK.dll")]
        public static extern int PTK_DrawTextTrueTypeW
                                            (int x, int y, int FHeight,
                                            int FWidth, string FType,
                                            int Fspin, int FWeight,
                                            bool FItalic, bool FUnline,
                                            bool FStrikeOut,
                                            string id_name,
                                            string data);
        [DllImport("CDFPSK.dll")]
        public static extern int PTK_DrawBarcode(uint px,
                                        uint py,
                                        uint pdirec,
                                        string pCode,
                                        uint pHorizontal,
                                        uint pVertical,
                                        uint pbright,
                                        char ptext,
                                        string pstr);
        [DllImport("CDFPSK.dll")]
        public static extern int PTK_SetLabelHeight(uint lheight, uint gapH, int gapOffset, bool bFlag);
        [DllImport("CDFPSK.dll")]
        public static extern int PTK_SetLabelWidth(uint lwidth);
        [DllImport("CDFPSK.dll")]
        public static extern int PTK_ClearBuffer();
        [DllImport("CDFPSK.dll")]
        public static extern int PTK_DrawRectangle(uint px, uint py, uint thickness, uint pEx, uint pEy);
        [DllImport("CDFPSK.dll")]
        public static extern int PTK_DrawLineOr(uint px, uint py, uint pLength, uint pH);
        [DllImport("CDFPSK.dll")]
        public static extern int PTK_DrawBar2D_QR(uint x, uint y, uint w, uint v, uint o, uint r, uint m, uint g, uint s, string pstr);
        [DllImport("CDFPSK.dll")]
        public static extern int PTK_DrawBar2D_QREx(uint x, uint y, uint o, uint r, uint g, uint s, uint v, string id_name, string pstr);
        [DllImport("CDFPSK.dll")]
        public static extern int PTK_DrawBar2D_Pdf417(uint x, uint y, uint w, uint v, uint s, uint c, uint px, uint py, uint r, uint l, uint t, uint o, string pstr);
        [DllImport("CDFPSK.dll")]
        public static extern int PTK_PcxGraphicsDel(string pid);
        [DllImport("CDFPSK.dll")]
        public static extern int PTK_PcxGraphicsDownload(string pcxname, string pcxpath);
        [DllImport("CDFPSK.dll")]
        public static extern int PTK_DrawPcxGraphics(uint px, uint py, string gname);
        [DllImport("CDFPSK.dll")]
        public static extern int PTK_DrawText(uint px, uint py, uint pdirec, uint pFont, uint pHorizontal, uint pVertical, char ptext, string pstr);
        [DllImport("CDFPSK.dll")]
        public static extern int PTK_DrawTextEx(uint px, uint py, uint pdirec, uint pFont, uint pHorizontal, uint pVertical, char ptext, string pstr, bool varible);
        [DllImport("CDFPSK.dll")]
        public static extern int PTK_DrawBar2D_DATAMATRIX(uint x, uint y, uint w, uint h, uint o, uint m, string pstr);//DataMatrix二维条码
        //[DllImport("CDFPSK.dll")]
        //public static extern int PTK_DrawBar2D_Pdf417(uint x, uint y, uint w, uint v, uint s, uint c, uint px, uint py, uint r, uint l, uint t, uint o, string pstr);  //PDF417二维条码
        [DllImport("CDFPSK.dll")]
        public static extern int PTK_DrawBar2D_HANXIN(uint x, uint y, uint w, uint v, uint o, uint r, uint m, uint g, uint s, string pstr);//汉信码二维条码  
        [DllImport("CDFPSK.dll")]
        public static extern int PTK_BmpGraphicsDownload(string pcxname, string pcxpath, uint iDire);//汉信码二维条码  

    }
}
