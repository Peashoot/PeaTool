using PeaTool.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PeaTool
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Init();
        }
        /// <summary>
        /// 窗口拖动
        /// </summary>
        WindowsDragger dragger;
        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            DataContext = SystemParameter = new SystemParameter();
            this.adapters = new ArrayList();
            PerformanceCounterCategory category = new PerformanceCounterCategory("Network Interface");

            foreach (string name in category.GetInstanceNames())
            {
                // This one exists on every computer.  
                if (name == "MS TCP Loopback interface" || name.Contains("isatap") || name.Contains("Interface"))
                    continue;
                // Create an instance of NetworkAdapter class, and create performance counters for it.  
                NetworkAdapter adapter = new NetworkAdapter(name);
                adapter.dlCounter = new PerformanceCounter("Network Interface", "Bytes Received/sec", name);
                adapter.ulCounter = new PerformanceCounter("Network Interface", "Bytes Sent/sec", name);
                adapters.Add(adapter); // Add it to ArrayList adapter  
                break;
            }

            dragger = new WindowsDragger(this);
            workAdapter = (NetworkAdapter)adapters[0];
            workAdapter.init();
            timer.Interval = 1;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }
        /// <summary>
        /// 数据绑定实体
        /// </summary>
        private SystemParameter SystemParameter;
        private ArrayList adapters;
        private NetworkAdapter workAdapter;
        /// <summary>
        /// 历史内存占用等级
        /// </summary>
        private int lastIndex;
        /// <summary>
        /// 进度条颜色
        /// </summary>
        public static string[] ProgressBarColorPair = new string[]
        {
            "#2BFC71",
            "#FFA11E",
            "#FF7D71"
        };
        /// <summary>
        /// 内框线颜色
        /// </summary>
        public static string[] InnerBorderColorPair = new string[]
        {
            "#36BB60",
            "#E8901E",
            "#FF6E64"
        };
        /// <summary>
        /// 渐变背景颜色
        /// </summary>
        public static string[] GradientBackgroundColorPair = new string[]
        {
            "#CEEBD0",
            "#F1E6D0",
            "#FFEBE8"
        };
        /// <summary>
        /// 定时检测内存占用，上下行流量
        /// </summary
        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            timer.Stop();
            SystemParameter.MemoryOccupy = (int)SystemMonitorHelper.GetMemoryOccupy();
            int index = SystemParameter.MemoryOccupy < 80 ? 0 : SystemParameter.MemoryOccupy < 96 ? 1 : 2;
            if (lastIndex != index)
            {
                SystemParameter.ProgressBarColor = ProgressBarColorPair[index];
                SystemParameter.InnerBorderColor = InnerBorderColorPair[index];
                SystemParameter.GradientBackgroundColor = GradientBackgroundColorPair[index];
                lastIndex = index;
            }
            workAdapter.refresh();
            double uplinkSpeed = Math.Round(workAdapter.UploadSpeedKbps, 1);
            SystemParameter.UplinkSpeed = uplinkSpeed > 1024 ?
                (uplinkSpeed / 1024).ToString("0.#").PadLeft(4, ' ').Substring(0, 4).TrimEnd('.') + "M/s" :
                uplinkSpeed.ToString("0.#").PadLeft(4, ' ').Substring(0, 4).TrimEnd('.') + "K/s";
            double downlinkSpeed = Math.Round(workAdapter.DownloadSpeedKbps, 2);
            SystemParameter.DownlinkSpeed = downlinkSpeed > 1024 ?
                (downlinkSpeed / 1024).ToString("0.#").PadLeft(4, ' ').Substring(0, 4).TrimEnd('.') + "M/s" :
                downlinkSpeed.ToString("0.#").PadLeft(4, ' ').Substring(0, 4).TrimEnd('.') + "K/s";
            SystemParameter.ShowUplink = SystemParameter.ShowDownlink;
            SystemParameter.ShowDownlink = !SystemParameter.ShowDownlink;
            System.Threading.Thread.Sleep(1000);
            timer.Start();
        }
        /// <summary>
        /// 定时器
        /// </summary>
        System.Timers.Timer timer = new System.Timers.Timer();
    }
}
