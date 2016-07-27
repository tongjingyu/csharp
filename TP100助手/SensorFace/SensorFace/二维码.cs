using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using ThoughtWorks.QRCode.Codec;
namespace SensorFace
{
    public partial class 二维码 : Form
    {
       string DataCode;
        string Num;
        public 二维码(string DC)
        {
            Num = DC;
            InitializeComponent();
        }

        private void 二维码_Load(object sender, EventArgs e)
        {
            if(Num.Length<15)
            {
                MessageBox.Show("未找到识别码");
                return;
            }
            DataCode = GetCodeNumber(this.Num);
            Bitmap bs = Create_ImgCode(DataCode, 8);
            SaveImg(currentPath, bs);
            pictureBox1.BackgroundImage = bs;
            this.Refresh();
        }
        private string GetCodeNumber(string Number)
        {
            string R = "http://tp500.toprie.cn/wechat/35_" + Number + ".htm";
            return R;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            uint x = 50;
            PrintLab.OpenPort(255);//打开打印机端口
            PrintLab.PTK_ClearBuffer();           //清空缓冲区
            PrintLab.PTK_SetPrintSpeed(1);        //设置打印速度
            PrintLab.PTK_SetDarkness(60);         //设置打印黑度
            PrintLab.PTK_SetLabelHeight(160, 19, 0, false); //设置标签的高度和定位间隙\黑线\穿孔的高度
            PrintLab.PTK_SetLabelWidth(520);      //设置标签的宽度
            for (int i = 1; i <= 1; i++)
            {
                for (int z = 0; z < 4; z++)
                {
                    PrintLab.PTK_DrawTextTrueTypeW(400 + (int)x, 24 + 24 * z, 26, 0, "宋体", 1, 400, false, false, false, z.ToString(), Num.Substring(z * 4, 4));
                }
                for (int z = 0; z < 4; z++)
                {
                    PrintLab.PTK_DrawTextTrueTypeW(145 + (int)x, 24 + 24 * z, 26, 0, "宋体", 1, 400, false, false, false, z.ToString(), Num.Substring(z * 4, 4));
                }
                PrintLab.PTK_DrawBar2D_QR(x, 8, 300, 300, 0, 4, 2, 0, 0, DataCode);
                PrintLab.PTK_DrawBar2D_QR(252 + x, 8, 300, 300, 0, 4, 2, 0, 0, DataCode);
                PrintLab.PTK_PrintLabel(1, 1);
            }
            PrintLab.ClosePort();//关闭打印机端口
            MessageBox.Show(DataCode, "打印内容");
        }

        //程序路径
        readonly string currentPath = Application.StartupPath + @"\BarCode_Images";

        /// <summary>
        /// 保存图片
        /// </summary>
        /// <param name="strPath">保存路径</param>
        /// <param name="img">图片</param>
        public void SaveImg(string strPath, Bitmap img)
        {
            //保存图片到目录
            if (Directory.Exists(strPath))
            {
                //文件名称
                string guid = Guid.NewGuid().ToString().Replace("-", "") + ".png";
                img.Save(strPath + "/" + guid, System.Drawing.Imaging.ImageFormat.Png);
            }
            else
            {
                //当前目录不存在，则创建
                Directory.CreateDirectory(strPath);
            }
        }
        /// <summary>
        /// 生成二维码图片
        /// </summary>
        /// <param name="codeNumber">要生成二维码的字符串</param>     
        /// <param name="size">大小尺寸</param>
        /// <returns>二维码图片</returns>
        public Bitmap Create_ImgCode(string codeNumber, int size)
        {
            //创建二维码生成类
            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            //设置编码模式
            qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            //设置编码测量度
            qrCodeEncoder.QRCodeScale = size;
            //设置编码版本
            qrCodeEncoder.QRCodeVersion = 0;
            //设置编码错误纠正
            qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
            //生成二维码图片
            System.Drawing.Bitmap image = qrCodeEncoder.Encode(codeNumber);
            return image;
        }
 
        public void DeleteDir(string aimPath)
        {
            try
            {
                //目录是否存在
                if (Directory.Exists(aimPath))
                {
                    // 检查目标目录是否以目录分割字符结束如果不是则添加之
                    if (aimPath[aimPath.Length - 1] != Path.DirectorySeparatorChar)
                        aimPath += Path.DirectorySeparatorChar;
                    // 得到源目录的文件列表，该里面是包含文件以及目录路径的一个数组
                    // 如果你指向Delete目标文件下面的文件而不包含目录请使用下面的方法
                    string[] fileList = Directory.GetFiles(aimPath);
                    //string[] fileList = Directory.GetFileSystemEntries(aimPath);
                    // 遍历所有的文件和目录
                    foreach (string file in fileList)
                    {
                        // 先当作目录处理如果存在这个目录就递归Delete该目录下面的文件
                        if (Directory.Exists(file))
                        {
                            DeleteDir(aimPath + Path.GetFileName(file));
                        }
                        // 否则直接Delete文件
                        else
                        {
                            File.Delete(aimPath + Path.GetFileName(file));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
