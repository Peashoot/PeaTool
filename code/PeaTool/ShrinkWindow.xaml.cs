using PeaTool.Converters;
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
    /// ShrinkWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ShrinkWindow : Window
    {
        public ShrinkWindow()
        {
            InitializeComponent();
            Loaded += delegate
            {
                MouseDown += (obj, e) =>
                {
                    if (e.LeftButton == MouseButtonState.Pressed)
                    {
                        DragMove();
                        RaiseEvent(new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, MouseButton.Left)
                        {
                            RoutedEvent = MouseLeftButtonUpEvent
                        });
                    }
                };
            };
            for (int i = 0; i < 50; i++)
            {
                RowDefinition newRow = new RowDefinition();
                ProgressGrid.RowDefinitions.Add(newRow);
                if (i % 2 == 0)
                {
                    Rectangle rect = new Rectangle();
                    rect.Margin = new Thickness(2, 0, 2, 0);
                    rect.SetValue(Grid.RowProperty, i);
                    rect.Fill = new SolidColorBrush(Color.FromRgb(0x20, 0xE6, 0x65));
                    Binding binding = new Binding()
                    {
                        Path = new PropertyPath("MemoryOccupy"),
                        Converter = new LevelToVisibleConverter(),
                        ConverterParameter = 100 - (i << 1),
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                        Mode = BindingMode.OneWay
                    };
                    rect.SetBinding(VisibilityProperty, binding);
                    ProgressGrid.Children.Add(rect);
                }
            }
            Init();
        }
        /// <summary>
        /// 数据绑定实体
        /// </summary>
        private SystemParameter SystemParameter;

        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            DataContext = SystemParameter = new SystemParameter();
            timer.Interval = 1;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }
        /// <summary>
        /// 定时检测内存占用，上下行流量
        /// </summary
        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            timer.Stop();
            SystemParameter.MemoryOccupy = (int)SystemMonitorHelper.GetMemoryOccupy();
            System.Threading.Thread.Sleep(1000);
            timer.Start();
        }
        /// <summary>
        /// 定时器
        /// </summary>
        System.Timers.Timer timer = new System.Timers.Timer();

        private void ChangeProgress(int history, int current)
        {
            history = 100 - history;
            current = 100 - current;
            int rowHis = history / 2;
            int rowCur = current / 2;
            if (Math.Abs(rowHis - rowCur) < 2) return;
            if (rowHis < rowCur)
            {
                for (int i = rowHis; i <= rowCur; i++)
                {
                    if (i % 2 == 0)
                    {
                        Dispatcher.BeginInvoke(new Action<int>((a) =>
                        ProgressGrid.Children[a >> 1].Visibility = Visibility.Hidden), i);
                    }
                }
            }
            else
            {
                for (int i = rowCur; i >= rowHis; i--)
                {
                    if (i % 2 == 0)
                    {
                        Dispatcher.BeginInvoke(new Action<int>((a) => ProgressGrid.Children[i >> 1].Visibility = Visibility.Visible), i);
                    }
                }
            }
        }
    }
}
