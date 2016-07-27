using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace CSHARP_POSTEK_PRINT
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            PrintLab.OpenPort(255);//�򿪴�ӡ���˿�
            PrintLab.PTK_ClearBuffer();           //��ջ�����
            PrintLab.PTK_SetPrintSpeed(1);        //���ô�ӡ�ٶ�
            PrintLab.PTK_SetDarkness(60);         //���ô�ӡ�ڶ�
            PrintLab.PTK_SetLabelHeight(160, 19,0,false); //���ñ�ǩ�ĸ߶ȺͶ�λ��϶\����\���׵ĸ߶�
            PrintLab.PTK_SetLabelWidth(520);      //���ñ�ǩ�Ŀ��

            for (int i = 1; i <= 1; i++)
            {

           //     Zgke.MyImage.ImageFile.ImagePcx _Pcx = new Zgke.MyImage.ImageFile.ImagePcx();
           //     _Pcx.PcxImage = new Bitmap("logo.bmp");
           //     _Pcx.Save("1.pcx");
           //     // ������
           ////     PrintLab.PTK_DrawRectangle(58, 15, 3, 558, 312);

           //     // ��ӡPCXͼƬ ��ʽһ
           //     PrintLab.PTK_PcxGraphicsDel("PCX");
           //     PrintLab.PTK_PcxGraphicsDownload("PCX", "1.pcx");
           //     PrintLab.PTK_DrawPcxGraphics(10, 5, "PCX");
           //     PrintLab.PTK_DrawPcxGraphics(260, 5, "PCX");
               
                // ��ӡPCXͼƬ ��ʽ��
                // PTK_PrintPCX(80,30,pchar('logo.pcx'));

                // ��ӡһ������;
               // PrintLab.PTK_DrawBarcode(300, 23, 0, "1", 2, 2, 50, 'B', "123456789");              

                // �����ָ���
                //PrintLab.PTK_DrawLineOr(58, 100, 500, 3);               

                //// ��ӡһ��TrueTypeFont����;
                PrintLab.PTK_DrawTextTrueTypeW(20, 129, 26, 0, "����", 1, 400, false, false, false, "����", "1234567890abcdef");                

                //// ��ӡһ���ı�����(���������������);
                //PrintLab.PTK_DrawText(80, 168, 0, 3, 1, 1, 'N', "Internal Soft Font");
                
                //// ��ӡPDF417��
                //PrintLab.PTK_DrawBar2D_Pdf417(80, 210, 400, 300, 0, 0, 3, 7, 10, 2, 0, 0, "123456789");//PDF417��
                
                //// ��ӡQR��
                PrintLab.PTK_DrawBar2D_QR(70, 4, 300, 300, 0, 5, 2, 0, 0, "http://www.trtos.com/1234567890abcdef");
                PrintLab.PTK_DrawBar2D_QR(306, 4, 300, 300, 0, 5, 2, 0, 0, "Postek Electronics Co., Ltd.");
                

                // ��ӡһ��TrueTypeFont������ת;
             //   PrintLab.PTK_DrawTextTrueTypeW(520, 102, 22, 0, "Arial", 2, 400, false, false, false, "A2", "www.postek.com.cn");
           //     PrintLab.PTK_DrawTextTrueTypeW(80, 260, 19, 0, "Arial", 1, 700, false, false, false, "A3", "Use different ID_NAME for different Truetype font objects");
                

                // �����ӡ��ִ�д�ӡ����
                PrintLab.PTK_PrintLabel(1, 1);
              
            }
            PrintLab.ClosePort();//�رմ�ӡ���˿�
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
        }

        private void Form1_Load(object sender, EventArgs e)
        {
           
        }
    }
}
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
    public static extern int PTK_SetLabelHeight(uint lheight, uint gapH, int gapOffset,bool bFlag);
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
    public static extern int PTK_DrawBar2D_DATAMATRIX(uint x, uint y, uint w, uint h, uint o, uint m, string pstr);//DataMatrix��ά����
    //[DllImport("CDFPSK.dll")]
    //public static extern int PTK_DrawBar2D_Pdf417(uint x, uint y, uint w, uint v, uint s, uint c, uint px, uint py, uint r, uint l, uint t, uint o, string pstr);  //PDF417��ά����
    [DllImport("CDFPSK.dll")]
    public static extern int PTK_DrawBar2D_HANXIN(uint x, uint y, uint w, uint v, uint o, uint r, uint m, uint g, uint s, string pstr);//�������ά����  
    [DllImport("CDFPSK.dll")]
    public static extern int PTK_BmpGraphicsDownload(string pcxname, string pcxpath, uint iDire);//�������ά����  

}