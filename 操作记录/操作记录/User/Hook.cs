using System;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;
namespace 透明时钟演示
{
    public class Hook
    {
        /// <summary> 
        public const int WM_KEYDOWN = 0x0100;//程序keydown事件
        public const int WH_KEYBOARD_LL = 13;//键盘钩子
        public const int WM_SYSKEYDOWN = 0x0104;//系统keydown事件
        /// <summary>
        /// 声明一个钩子句柄委托
        /// </summary>
        public delegate int HookHandlerDelegate(int nCode, IntPtr wparam, ref KBDLLHOOKSTRUCT lparam);
        /// <summary>
        /// 调用系统WIN32动态链接库中方法设置钩子
        /// </summary>
        /// <param name="idHook">钩子ID</param>
        /// <param name="lpfn">钩子执行函数指针</param>
        /// <param name="hmod">程序句柄</param>
        /// <param name="dwThreadID">默认参数为0</param>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SetWindowsHookExW(int idHook, HookHandlerDelegate lpfn, IntPtr hmod, uint dwThreadID);

        /// <summary>
        /// 委托回调函数
        /// </summary>
        /// <param name="nCode">是否触发键盘事件,-1为否,大于0为是</param>
        /// <param name="wparam">程序指针</param>
        /// <param name="lparam">返回键盘信息结构</param>
        private int HookCallback(int nCode, IntPtr wparam, ref KBDLLHOOKSTRUCT lparam)
        {
            if (nCode >= 0 && (wparam == (IntPtr)WM_KEYDOWN || wparam == (IntPtr)WM_SYSKEYDOWN))
            {
                //在这里你可以按照键盘的code码（askII码）进行你想要得操作,例如你要MessageBox.Show("ok~")
                //在这里的回调函数主要用于禁用键盘,返回1键盘就没有作任何响应
                if (lparam.vkCode == 91 || lparam.vkCode == 164 || lparam.vkCode == 9 || lparam.vkCode == 115 || (Keys)lparam.vkCode == Keys.RWin)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            return 0;
        }
    }
}