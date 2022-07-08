using PeaTool.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PeaTool
{
    /// <summary>
    /// Clock.xaml 的交互逻辑
    /// </summary>
    public partial class Clock : Window
    {
        public Clock()
        {
            InitializeComponent();
            Init();
            int now_hours = DateTime.Now.Hour > 12 ? (DateTime.Now.Hour - 12) : DateTime.Now.Hour;
            int now_minutes = DateTime.Now.Minute;
            int now_seconds = DateTime.Now.Second;

            Storyboard seconds = (Storyboard)second.FindResource("sbseconds");
            seconds.Begin();
            seconds.Seek(new TimeSpan(0, 0, 0, now_seconds, 0));

            Storyboard minutes = (Storyboard)minute.FindResource("sbminutes");
            minutes.Begin();
            minutes.Seek(new TimeSpan(0, 0, now_minutes, now_seconds, 0));

            Storyboard hours = (Storyboard)hour.FindResource("sbhours");
            hours.Begin();
            hours.Seek(new TimeSpan(0, now_hours, now_minutes, now_seconds, 0));
            dragger = new WindowsDragger(this);
        }
        private WindowsDragger dragger;

        private void Init()
        {
            for (int i = 1; i <= 60; i++)
            {
                Line line = new Line();
                line.StrokeThickness = i % 5 == 0 ? 3 : 1;
                line.RenderTransform = new RotateTransform(i * 6);
                DialView.Children.Add(line);
            }
            for (int i = 1; i <= 12; i++)
            {
                Label label = new Label();
                label.Content = GetRomanNumber(i);
                label.RenderTransform = new RotateTransform(i * 30);
                NumberView.Children.Add(label);
            }
        }
        /// <summary>
        /// 十进制转罗马字符
        /// </summary>
        private string GetRomanNumber(int num)
        {
            string res = String.Empty;
            List<int> val = new List<int> { 1000, 900, 500, 400, 100, 90, 50, 40, 10, 9, 5, 4, 1 };
            List<string> str = new List<string> { "M", "CM", "D", "CD", "C", "XC", "L", "XL", "X", "IX", "V", "IV", "I" };
            for (int i = 0; i < val.Count; ++i)
            {
                while (num >= val[i])
                {
                    num -= val[i];
                    res += str[i];
                }
            }
            return res;
        }
    }
}