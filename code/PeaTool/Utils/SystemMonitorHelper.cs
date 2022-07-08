using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;

namespace PeaTool.Utils
{
    public class SystemMonitorHelper
    {
        /// <summary>
        /// 获取内存占用
        /// </summary>
        public static double GetMemoryOccupy()
        {
            long phav = PerformanceInfo.GetPhysicalAvailableMemoryInMiB();
            long tot = PerformanceInfo.GetTotalMemoryInMiB();
            decimal percentFree = phav / (decimal)tot * 100;
            decimal percentOccupied = 100 - percentFree;
            return (int)percentOccupied;
        }

        [DllImport("user32.dll")]
        public static extern bool EnumWindows(EnumWindowsProc proc, IntPtr lParam);
        public delegate bool EnumWindowsProc(IntPtr hwnd, IntPtr lParam);
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string className, string winName);
        [DllImport("user32.dll")]
        public static extern IntPtr SetParent(IntPtr hwnd, IntPtr parentHwnd);
        /// <summary>
        /// 将窗体嵌入到桌面
        /// </summary>
        /// <param name="window"></param>
        public static void SetOnDesktop(Window window)
        {
            IntPtr windowHandle = new WindowInteropHelper(window).Handle;
            // 遍历顶级窗口
            EnumWindows((hwnd, lParam) =>
            {
                IntPtr nWinHandle = FindWindowEx(hwnd, IntPtr.Zero, "SHELLDLL_DefView", null);
                if (nWinHandle != IntPtr.Zero)
                {
                    SetParent(windowHandle, nWinHandle);
                    return false;
                }
                return true;
            }, IntPtr.Zero);
            HideAltTab(windowHandle);
        }
        /// <summary>
        /// 桌面是否启动完毕
        /// </summary>
        public static bool ExistDesktop(out bool existWindow)
        {
            bool ewResult = false, result = false;
            // 遍历顶级窗口
            EnumWindows((hwnd, lParam) =>
            {
                IntPtr nWinHandle = FindWindowEx(hwnd, IntPtr.Zero, "SHELLDLL_DefView", null);
                if (nWinHandle != IntPtr.Zero)
                {
                    result = true;
                    IntPtr tWinHandle = FindWindowEx(nWinHandle, IntPtr.Zero, null, SingletonMode<ApplicationSetting>.Instance.WindowTitle);
                    ewResult = tWinHandle != IntPtr.Zero;
                    return false;
                }
                return true;
            }, IntPtr.Zero);
            existWindow = ewResult;
            return result;
        }

        #region 在windows Alt+Tab 任务视图中隐藏 定义为工具软件


        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);

        public static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
        {
            int error = 0;
            IntPtr result = IntPtr.Zero;
            // Win32 SetWindowLong doesn't clear error on success
            SetLastError(0);

            if (IntPtr.Size == 4)
            {
                // use SetWindowLong
                Int32 tempResult = IntSetWindowLong(hWnd, nIndex, IntPtrToInt32(dwNewLong));
                error = Marshal.GetLastWin32Error();
                result = new IntPtr(tempResult);
            }
            else
            {
                // use SetWindowLongPtr
                result = IntSetWindowLongPtr(hWnd, nIndex, dwNewLong);
                error = Marshal.GetLastWin32Error();
            }

            if ((result == IntPtr.Zero) && (error != 0))
            {
                throw new System.ComponentModel.Win32Exception(error);
            }

            return result;
        }

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", SetLastError = true)]
        private static extern IntPtr IntSetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong", SetLastError = true)]
        private static extern Int32 IntSetWindowLong(IntPtr hWnd, int nIndex, Int32 dwNewLong);

        private static int IntPtrToInt32(IntPtr intPtr)
        {
            return unchecked((int)intPtr.ToInt64());
        }
        [DllImport("kernel32.dll", EntryPoint = "SetLastError")]
        public static extern void SetLastError(int dwErrorCode);


        #endregion
        private static void HideAltTab(IntPtr windowHandle)
        {
            var exStyle = GetWindowLong(windowHandle, -20);
            SetWindowLong(windowHandle, (-20), new IntPtr(0x80));
        }
    }

    public static class PerformanceInfo
    {
        [DllImport("psapi.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetPerformanceInfo([Out] out PerformanceInformation PerformanceInformation, [In] int Size);

        [StructLayout(LayoutKind.Sequential)]
        public struct PerformanceInformation
        {
            public int Size;
            public IntPtr CommitTotal;
            public IntPtr CommitLimit;
            public IntPtr CommitPeak;
            public IntPtr PhysicalTotal;
            public IntPtr PhysicalAvailable;
            public IntPtr SystemCache;
            public IntPtr KernelTotal;
            public IntPtr KernelPaged;
            public IntPtr KernelNonPaged;
            public IntPtr PageSize;
            public int HandlesCount;
            public int ProcessCount;
            public int ThreadCount;
        }

        public static Int64 GetPhysicalAvailableMemoryInMiB()
        {
            PerformanceInformation pi = new PerformanceInformation();
            if (GetPerformanceInfo(out pi, Marshal.SizeOf(pi)))
            {
                return Convert.ToInt64((pi.PhysicalAvailable.ToInt64() * pi.PageSize.ToInt64() / 1048576));
            }
            else
            {
                return -1;
            }

        }

        public static Int64 GetTotalMemoryInMiB()
        {
            PerformanceInformation pi = new PerformanceInformation();
            if (GetPerformanceInfo(out pi, Marshal.SizeOf(pi)))
            {
                return Convert.ToInt64((pi.PhysicalTotal.ToInt64() * pi.PageSize.ToInt64() / 1048576));
            }
            else
            {
                return -1;
            }

        }
    }

    public class SystemParameter : INotifyPropertyChanged
    {
        /// <summary>
        /// 内存占用
        /// </summary>
        public int MemoryOccupy { get; set; } = 20;
        /// <summary>
        /// 上行网速
        /// </summary>
        public string UplinkSpeed { get; set; } = "0K/s";
        /// <summary>
        /// 下行网速
        /// </summary>
        public string DownlinkSpeed { get; set; } = "0K/s";
        /// <summary>
        /// 是否显示下行网速
        /// </summary>
        public bool ShowDownlink { get; set; } = false;
        /// <summary>
        /// 显示上行网速
        /// </summary>
        public bool ShowUplink { get; set; } = true;
        /// <summary>
        /// 进度条颜色
        /// </summary>
        public string ProgressBarColor { get; set; } = MainWindow.ProgressBarColorPair[0];
        /// <summary>
        /// 内框线颜色
        /// </summary>
        public string InnerBorderColor { get; set; } = MainWindow.InnerBorderColorPair[0];
        /// <summary>
        /// 渐变背景颜色
        /// </summary>
        public string GradientBackgroundColor { get; set; } = MainWindow.GradientBackgroundColorPair[0];
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
