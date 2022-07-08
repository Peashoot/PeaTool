using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace PeaTool.Utils
{
    /// <summary>
    /// 鼠标全局钩子
    /// </summary>
    public class MouseHook
    {
        private const int WM_MOUSEMOVE = 0x200;
        private const int WM_LBUTTONDOWN = 0x201;
        private const int WM_RBUTTONDOWN = 0x204;
        private const int WM_MBUTTONDOWN = 0x207;
        private const int WM_LBUTTONUP = 0x202;
        private const int WM_RBUTTONUP = 0x205;
        private const int WM_MBUTTONUP = 0x208;
        private const int WM_LBUTTONDBLCLK = 0x203;
        private const int WM_RBUTTONDBLCLK = 0x206;
        private const int WM_MBUTTONDBLCLK = 0x209;

        /// <summary>
        /// 点
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public class POINT
        {
            public int x;
            public int y;
        }

        /// <summary>
        /// 钩子结构体
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public class MouseHookStruct
        {
            public POINT pt;
            public int hWnd;
            public int wHitTestCode;
            public int dwExtraInfo;
        }

        public const int WH_MOUSE_LL = 14; // mouse hook constant

        // 装置钩子的函数
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);

        // 卸下钩子的函数
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool UnhookWindowsHookEx(int idHook);

        // 下一个钩挂的函数
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int CallNextHookEx(int idHook, int nCode, Int32 wParam, IntPtr lParam);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);
        // 全局的鼠标事件
        public event MouseEventHandler OnMouseActivity;

        // 钩子回调函数
        public delegate int HookProc(int nCode, Int32 wParam, IntPtr lParam);

        // 声明鼠标钩子事件类型
        private HookProc _mouseHookProcedure;
        private static int _hMouseHook = 0; // 鼠标钩子句柄

        /// <summary>
        /// 构造函数
        /// </summary>
        public MouseHook()
        {

        }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~MouseHook()
        {
            Stop();
        }

        /// <summary>
        /// 启动全局钩子
        /// </summary>
        public void Start()
        {
            // 安装鼠标钩子
            if (_hMouseHook == 0)
            {
                // 生成一个HookProc的实例.
                _mouseHookProcedure = new HookProc(MouseHookProc);
                ProcessModule cModule = Process.GetCurrentProcess().MainModule;

                var mh = GetModuleHandle(cModule.ModuleName);
                _hMouseHook = SetWindowsHookEx(WH_MOUSE_LL, _mouseHookProcedure, mh, 0);

                //如果装置失败停止钩子
                if (_hMouseHook == 0)
                {
                    Stop();
                    throw new Exception("SetWindowsHookEx failed.");
                }
            }
        }

        /// <summary>
        /// 停止全局钩子
        /// </summary>
        public void Stop()
        {
            bool retMouse = true;

            if (_hMouseHook != 0)
            {
                retMouse = UnhookWindowsHookEx(_hMouseHook);
                _hMouseHook = 0;
            }

            // 如果卸下钩子失败
            // if (!(retMouse))
            //  throw new Exception("UnhookWindowsHookEx failed.");
        }
        /// <summary>
        /// 拖动锁
        /// </summary>
        public bool DragLock = false;

        public int RelativeTop = 0;
        public int RelativeLeft = 0;
        public int WindowsWidth = 0;
        public int WindowsHeight = 0;
        int isUp = 0;
        /// <summary>
        /// 鼠标钩子回调函数
        /// </summary>
        private int MouseHookProc(int nCode, Int32 wParam, IntPtr lParam)
        {
            try
            {
                // 如果正常运行并且用户要监听鼠标的消息
                if ((nCode >= 0))//&& (OnMouseActivity != null))
                {
                    MouseButtons button = MouseButtons.None;
                    int clickCount = 0;
                    switch (wParam)
                    {
                        case WM_LBUTTONDOWN:
                            button = MouseButtons.Left;
                            clickCount = 1;
                            isUp = 1;
                            break;
                        case WM_LBUTTONUP:
                            button = MouseButtons.Left;
                            clickCount = 1;
                            isUp = 2;
                            DragLock = false;
                            break;
                        case WM_LBUTTONDBLCLK:
                            button = MouseButtons.Left;
                            clickCount = 2;
                            break;
                        case WM_RBUTTONDOWN:
                            button = MouseButtons.Right;
                            clickCount = 1;
                            isUp = 1;
                            break;
                        case WM_RBUTTONUP:
                            button = MouseButtons.Right;
                            clickCount = 1;
                            isUp = 2;
                            break;
                        case WM_RBUTTONDBLCLK:
                            button = MouseButtons.Right;
                            clickCount = 2;
                            break;
                        default:
                            if (isUp == 2) isUp = 0;
                            break;
                    }

                    // 从回调函数中得到鼠标的信息
                    MouseHookStruct MyMouseHookStruct =
                        (MouseHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseHookStruct));
                    var x = MyMouseHookStruct.pt.x;
                    var y = MyMouseHookStruct.pt.y;
                    MouseEventArgs e = new MouseEventArgs(button, clickCount, x, y, isUp);

                    // 如果想要限制鼠标在屏幕中的移动区域可以在此处设置
                    // 后期需要考虑实际的x、y的容差
                    if (DragLock && !Screen.PrimaryScreen.Bounds.Contains(Rectangle.FromLTRB(e.X - RelativeLeft, e.Y - RelativeTop, WindowsWidth, WindowsHeight)))
                    {
                        // 启动下一次钩子
                        return 1;
                    }
                    //OnMouseActivity(this, e);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            // 启动下一次钩子
            return CallNextHookEx(_hMouseHook, nCode, wParam, lParam);
        }
        /// <summary>
        /// 鼠标左键按下
        /// </summary>
        public static void LeftMousePressed(System.Windows.Window windows, System.Windows.Input.MouseEventArgs args)
        {
            MouseHook hook = SingletonMode<MouseHook>.Instance;
            System.Windows.Point relaPos = args.GetPosition(windows);
            hook.RelativeLeft = (int)relaPos.X;
            hook.RelativeTop = (int)relaPos.Y;
            hook.WindowsWidth = (int)windows.Width;
            hook.WindowsHeight = (int)windows.Height;
            hook.DragLock = true;
        }
    }
}
