using PeaTool.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PeaTool
{
    /// <summary>
    /// RunToRichWindow.xaml 的交互逻辑
    /// </summary>
    public partial class RunToRichWindow : Window
    {
        public RunToRichWindow()
        {
            InitializeComponent();
            Loaded += delegate { SystemMonitorHelper.SetOnDesktop(this); };
            Init();
        }
        /// <summary>
        /// 窗口拖动
        /// </summary>
        private WindowsDragger dragger;
        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            Left = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width - Width;
            Top = 0;
            DataContext = ManagementData = new FinancialManagementData();
            ManagementData.Setting = SingletonMode<ApplicationSetting>.Instance;
            dragger = new WindowsDragger(this, Header, false);
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }
        /// <summary>
        /// 定时器
        /// </summary>
        private System.Timers.Timer timer = new System.Timers.Timer(200);
        /// <summary>
        /// 托管数据
        /// </summary>
        private FinancialManagementData ManagementData;
        /// <summary>
        /// 定时查询数据
        /// </summary>
        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            timer.Stop();
            DataQueryer.RefreshData(ManagementData, Dispatcher).Wait();
            System.Threading.Thread.Sleep(ManagementData.Setting.RefreshDataInterval * 1000);
            timer.Start();
        }
        /// <summary>
        /// 点击更多时切换下一批指数信息
        /// </summary>
        private void ShowNextStock_Click(object sender, MouseButtonEventArgs e)
        {
            // 不是左键时不处理
            if (e.ChangedButton != MouseButton.Left) return;
            // 没有数据时不处理
            if (ManagementData.AllStockDataList.Count <= 3) return;
            // 没有数据被选中时不处理
            if (ManagementData.SelectedStockDataList.Count < 3) return;
            Dispatcher.Invoke(() =>
            {
                for (int i = 0; i < ManagementData.SelectedStockDataList.Count; i++)
                {
                    ManagementData.SelectedStockStartIndex++;
                    ManagementData.SelectedStockStartIndex %= ManagementData.AllStockDataList.Count;
                    ManagementData.SelectedStockDataList[i] = ManagementData.AllStockDataList[ManagementData.SelectedStockStartIndex];
                }
            });
        }
        /// <summary>
        /// 收起和展开
        /// </summary>
        private void ShrinkAndUnfold_Click(object sender, MouseButtonEventArgs e)
        {
            if (ViewBody.Visibility == Visibility.Visible)
            {
                ViewBody.Visibility = Visibility.Hidden;
                ImgShrink.Source = new BitmapImage(new Uri(@"/Images/unfold.png", UriKind.RelativeOrAbsolute));
            }
            else if (ViewBody.Visibility == Visibility.Hidden)
            {
                ViewBody.Visibility = Visibility.Visible;
                ImgShrink.Source = new BitmapImage(new Uri(@"/Images/shrink.png", UriKind.RelativeOrAbsolute));
            }
        }
        /// <summary>
        /// 锁定和解锁
        /// </summary>
        private void LockAndUnlock_Click(object sender, MouseButtonEventArgs e)
        {
            if (!dragger.Enabled)
            {
                ImgLock.Source = new BitmapImage(new Uri(@"/Images/unlock.png", UriKind.RelativeOrAbsolute));
                dragger.Enabled = true;
            }
            else
            {
                ImgLock.Source = new BitmapImage(new Uri(@"/Images/lock.png", UriKind.RelativeOrAbsolute));
                dragger.Enabled = false;
            }
        }
    }
}
