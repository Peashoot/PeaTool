using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PeaTool.Components
{
    /// <summary>
    /// DigitalNumber.xaml 的交互逻辑
    /// </summary>
    public partial class DigitalNumber : UserControl
    {
        public DigitalNumber()
        {
            InitializeComponent();
            Init();
        }

        #region DigitalValue
        /// <summary>
        /// 显示值
        /// </summary>
        public int DigitalValue
        {
            get => (int)GetValue(DigitalValueProperty);
            set => SetValue(DigitalValueProperty, value);
        }

        // Using a DependencyProperty as the backing store for DigitalValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DigitalValueProperty =
            DependencyProperty.RegisterAttached("DigitalValue", typeof(int), typeof(DigitalNumber), new PropertyMetadata(8));

        #endregion
        #region BorderColor
        /// <summary>
        /// 边界颜色
        /// </summary>
        public Brush BorderColor
        {
            get => (Brush)GetValue(BorderColorProperty);
            set => SetValue(BorderColorProperty, value);
        }

        // Using a DependencyProperty as the backing store for BorderColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BorderColorProperty =
            DependencyProperty.RegisterAttached("BorderColor", typeof(Brush), typeof(DigitalNumber), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(0, 0, 0))));

        #endregion
        #region BoundaryThickness
        /// <summary>
        /// 边界宽度
        /// </summary>
        public int BoundaryThickness
        {
            get => (int)GetValue(BoundaryThicknessProperty);
            set => SetValue(BoundaryThicknessProperty, value);
        }

        // Using a DependencyProperty as the backing store for BoundaryThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BoundaryThicknessProperty =
            DependencyProperty.RegisterAttached("BoundaryThickness", typeof(int), typeof(DigitalNumber), new PropertyMetadata(0));

        #endregion
        #region BrightColor
        /// <summary>
        /// 点亮状态颜色
        /// </summary>
        public Brush BrightColor
        {
            get => (Brush)GetValue(BrightColorProperty);
            set => SetValue(BrightColorProperty, value);
        }

        // Using a DependencyProperty as the backing store for BrightColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BrightColorProperty =
            DependencyProperty.RegisterAttached("BrightColor", typeof(Brush), typeof(DigitalNumber), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF))));

        #endregion
        #region DarkColor
        /// <summary>
        /// 非点亮状态颜色
        /// </summary>
        public Brush DarkColor
        {
            get => (Brush)GetValue(DarkColorProperty);
            set => SetValue(DarkColorProperty, value);
        }

        // Using a DependencyProperty as the backing store for DarkColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DarkColorProperty =
            DependencyProperty.RegisterAttached("DarkColor", typeof(Brush), typeof(DigitalNumber), new PropertyMetadata(null));

        #endregion

        private void Init()
        {
            CreateBinding(TopLine, 0, 2, 3, 5, 6, 7, 8, 9);
            CreateBinding(LeftTop, 0, 4, 5, 6, 8, 9);
            CreateBinding(MiddleLine, 2, 3, 4, 5, 6, 8, 9);
            CreateBinding(RightTop, 0, 1, 2, 3, 4, 7, 8, 9);
            CreateBinding(LeftBottom, 0, 2, 6, 8);
            CreateBinding(BottomLine, 0, 2, 3, 5, 6, 8, 9);
            CreateBinding(RightBottom, 0, 1, 3, 4, 5, 6, 7, 8, 9);
        }
        /// <summary>
        /// 创建绑定
        /// </summary>
        private void CreateBinding(Path line, params int[] args)
        {
            BindingOperations.SetBinding(line, Shape.FillProperty, new Binding()
            {
                Path = new PropertyPath("DigitalValue"),
                Source = this,
                Converter = new DigitalNumberDisplayColorConverter(this),
                ConverterParameter = args
            });
        }
    }

    public class DigitalNumberDisplayColorConverter : IValueConverter
    {
        public DigitalNumberDisplayColorConverter(DigitalNumber element)
        {
            this.element = element;
        }
        /// <summary>
        /// 数字控件
        /// </summary>
        private DigitalNumber element;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var lightUpArray = parameter as int[];
            var intValue = int.Parse(value.ToString());
            return lightUpArray.Contains(intValue) ? element.BrightColor : element.DarkColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ThicknessDoubleValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? 0 : double.TryParse(value.ToString(), out double result) ? result : 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
