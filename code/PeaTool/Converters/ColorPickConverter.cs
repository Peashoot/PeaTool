using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PeaTool.Converters
{
    /// <summary>
    /// 颜色选择器，数值为正或为0时选第一个，为负时选第二个
    /// </summary>
    public class ColorPickConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string[] colorPairs = parameter.ToString().Split(',');
            if (value == null) { return colorPairs[0]; }
            if (!double.TryParse(value.ToString(), out double dVal)) return colorPairs[0];
            return dVal < 0 ? colorPairs[1] : colorPairs[0];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
