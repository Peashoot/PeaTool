using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Threading;
using PeaTool.Utils;

namespace PeaTool
{
    /// <summary>
    /// DeamonWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DeamonWindow : Window
    {
        public DeamonWindow()
        {
            InitializeComponent();
            Loaded += delegate
            {
                Visibility = Visibility.Hidden;
            };
        }
        /// <summary>
        /// 守护定时器
        /// </summary>
        System.Timers.Timer DeamonTimer = new System.Timers.Timer(1000);
        /// <summary>
        /// 初始化时启动定时器
        /// </summary>
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            DeamonTimer.Elapsed += DeamonTimer_Elapsed;
            DeamonTimer.Start();
        }
        /// <summary>
        /// 定时检测窗体是否关闭
        /// </summary>
        private void DeamonTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            DeamonTimer.Stop();
            if (SystemMonitorHelper.ExistDesktop(out bool existWindow) && !existWindow)
            {
                Dispatcher.Invoke(() => new RunToRichWindow().Show());
            }
            Thread.Sleep(9 * 1000);
            DeamonTimer.Start();
        }
    }
}
