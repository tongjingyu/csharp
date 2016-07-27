using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Reflection;


namespace KeyboardHookDll
{
        public class KeyboardHook
        {
            private const int WM_KEYDOWN = 0x100;
            private const int WM_KEYUP = 0x101;
            private const int WM_SYSKEYDOWN = 0x104;
            private const int WM_SYSKEYUP = 0x105;


            //全局事件
            public event KeyEventHandler OnKeyDownEvent;
            public event KeyEventHandler OnKeyUpEvent;
            public event KeyPressEventHandler OnKeyPressEvent;
            public static int hKeyboardHook = 0;//键盘钩子句柄

            //鼠标常量
            public const int WH_KEYBOARD_LL = 13;

            //键盘钩子事件类型
            HookProc KeyboardHookProcedure;
            [StructLayout(LayoutKind.Sequential)]
            public class KeyboardHookStruct
            {
                public int vkCode;//表示一个1到254间的虚拟键盘码
                public int scanCode;//表示硬件扫描码
                public int flags;
                public int time;
                public int dwExtraInfo;
            }

            //装置钩子函数
            [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
            public static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);

            //卸载钩子函数
            [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
            public static extern bool UnhookWindowsHookEx(int idHook);

            //下一个钩子函数
            [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
            public static extern int CallNextHookEx(int idHook, int nCode, Int32 wParam, IntPtr IParam);

            [DllImport("user32.dll")]
            public static extern int ToAscii(int uVirtKey, int uScanCode, byte[] lpbkeyCode, byte[] lpwTransKey, int fuState);

            [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
            public static extern int GetKeyboardState(byte[] pbKeyState);

            public delegate int HookProc(int nCode, Int32 wParam, IntPtr IParam);

            public KeyboardHook()
            {
                //Start();
            }

            //~KeyboardHook()
            //{
            //    Stop();
            //}
            //[STAThread]
            //public static void Main(string[] args)
            //{
            //    Program p = new Program();
            //    p.Start();
            //}

            public void Start()
            {
                //安装键盘钩子
                if (hKeyboardHook == 0)
                {
                    KeyboardHookProcedure = new HookProc(KeyboardHookProc);
                    hKeyboardHook = SetWindowsHookEx(WH_KEYBOARD_LL, KeyboardHookProcedure, Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().ManifestModule), 0);
                    if (hKeyboardHook == 0)
                    {
                        Stop();
                        throw new Exception("SetWindowsHookEx is failed.");
                    }
                }
            }
            public void Stop()
            {
                bool retKeyboard = true;
                if (hKeyboardHook != 0)
                {
                    retKeyboard = UnhookWindowsHookEx(hKeyboardHook);
                    hKeyboardHook = 0;
                }

                //如果卸下钩子失败 
                if (!(retKeyboard)) throw new Exception("UnhookWindowsHookEx   failed. ");

            }

            private int KeyboardHookProc(int nCode, Int32 wParam, IntPtr IParam)
            {
                if ((nCode >= 0) && (OnKeyDownEvent != null || OnKeyUpEvent != null || OnKeyPressEvent != null))
                {
                    KeyboardHookStruct MyKeyboardHookStruct = (KeyboardHookStruct)Marshal.PtrToStructure(IParam, typeof(KeyboardHookStruct));

                    //引发OnKeyDownEvent
                    if (OnKeyDownEvent != null && (wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN))
                    {
                        Keys keyData = (Keys)MyKeyboardHookStruct.vkCode;
                        KeyEventArgs e = new KeyEventArgs(keyData);
                        OnKeyDownEvent(this, e);
                    }
                    //引发OnKeyUpEvent
                    if (OnKeyUpEvent != null && (wParam == WM_KEYUP || wParam == WM_SYSKEYUP))
                    {
                        Keys keyData = (Keys)MyKeyboardHookStruct.vkCode;
                        //keyData.`
                        KeyEventArgs e = new KeyEventArgs(keyData);
                        OnKeyUpEvent(this, e);
                    }
                    //引发OnKeyPressEvent
                    if (OnKeyPressEvent != null && (wParam == WM_KEYDOWN))
                    {
                        byte[] keyState = new byte[256];
                        GetKeyboardState(keyState);
                        byte[] inBuffer = new byte[2];
                        if (ToAscii(MyKeyboardHookStruct.vkCode, MyKeyboardHookStruct.scanCode, keyState, inBuffer, MyKeyboardHookStruct.flags) == 1)
                        {
                            //string anjian=inBuffer.ToString();
                            //System.Console.WriteLine(anjian);
                            KeyPressEventArgs e = new KeyPressEventArgs((char)inBuffer[0]);
                            OnKeyPressEvent(this, e);
                        }
                    }
                }
                return CallNextHookEx(hKeyboardHook, nCode, wParam, IParam);
            }
        }
}
