using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace PeaTool.Converters
{
    /// <summary>
    /// 差值数据转换类
    /// </summary>
    public class AddUpValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value == null ? 0 : (double)value + System.Convert.ToDouble(parameter);
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => value != null ? (double)value - System.Convert.ToDouble(parameter) : Binding.DoNothing;
    }
}
