using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace PeaTool.Converters
{
    /// <summary>
    /// 倍率数据转换类
    /// </summary>
    public class MultipleValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value == null ? 0 : (int)value * System.Convert.ToDouble(parameter);
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => value != null ? (int)value / System.Convert.ToDouble(parameter) : Binding.DoNothing;
    }
}
