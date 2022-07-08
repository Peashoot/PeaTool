using PeaTool.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
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
    /// DigitalClock.xaml 的交互逻辑
    /// </summary>
    public partial class DigitalClock : Window
    {
        public DigitalClock()
        {
            InitializeComponent();
            DataContext = ViewModel = new DigitalClockViewModel();
            timer = new Timer(20);
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
            dragger = new WindowsDragger(this);
        }
        /// <summary>
        /// 定时刷新时间
        /// </summary>
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            DateTime now = DateTime.Now;
            ViewModel.QuantileSecond = now.Second % 10;
            ViewModel.DecileSecond = now.Second / 10;
            ViewModel.QuantileMinute = now.Minute % 10;
            ViewModel.DecileMinute = now.Minute / 10;
            ViewModel.QuantileHour = now.Hour % 10;
            ViewModel.DecileHour = now.Hour / 10;
        }
        /// <summary>
        /// 视图模型
        /// </summary>
        private DigitalClockViewModel ViewModel;
        /// <summary>
        /// 定时器
        /// </summary>
        private Timer timer = new Timer();
        /// <summary>
        /// 拖拽事件
        /// </summary>
        private WindowsDragger dragger;
    }

    public class DigitalClockViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 小时（十位）
        /// </summary>
        public int DecileHour { get; set; }
        /// <summary>
        /// 小时（个位）
        /// </summary>
        public int QuantileHour { get; set; }
        /// <summary>
        /// 分（个位）
        /// </summary>
        public int DecileMinute { get; set; }
        /// <summary>
        /// 分（个位）
        /// </summary>
        public int QuantileMinute { get; set; }
        /// <summary>
        /// 秒（十位）
        /// </summary>
        public int DecileSecond { get; set; }
        /// <summary>
        /// 秒（个位）
        /// </summary>
        public int QuantileSecond { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
