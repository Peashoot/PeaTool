
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PeaTool.Utils
{
    public class ApplicationSetting
    {
        /// <summary>
        /// 指数代码列表（必须超过3个，否则显示有问题）
        /// </summary>
        [IniConfig]
        public List<string> StockCodes { get; set; } = new List<string>() { "000001", "399001", "399006", "000300", "399004", "000016" };
        /// <summary>
        /// 基金代码列表（最好有3个）
        /// </summary>
        [IniConfig]
        public List<string> FundCodes { get; set; } = new List<string>() { "005939", "161725", "161724", "000067", "000689" };
        /// <summary>
        /// 数据刷新频率（单位：秒）
        /// </summary>
        [IniConfig]
        public int RefreshDataInterval { get; set; } = 60;
        /// <summary>
        /// 窗体透明度(0-1)
        /// </summary>
        [IniConfig]
        public double WindowTransparency { get; set; } = 0.65;
        /// <summary>
        /// 程序标题
        /// </summary>
        [IniConfig]
        public string WindowTitle { get; set; } = "小小脑袋！大大梦想！";

        public ApplicationSetting()
        {
            try
            {
                var distType = GetType();
                var staticProperties = distType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
                foreach (var property in staticProperties)
                {
                    var attribute = Attribute.GetCustomAttribute(property, typeof(CustomConfigAttribute)) as CustomConfigAttribute;
                    if (attribute == null) { continue; }
                    object varValue;
                    if (attribute.GetConfigValue(property.PropertyType, property.Name, out varValue))
                    {
                        property.SetValue(this, varValue, null);
                    }
                }
                var staticFields = distType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                foreach (var field in staticFields)
                {
                    var attribute = Attribute.GetCustomAttribute(field, typeof(CustomConfigAttribute)) as CustomConfigAttribute;
                    if (attribute == null) { continue; }
                    object varValue;
                    if (attribute.GetConfigValue(field.FieldType, field.Name, out varValue))
                    {
                        field.SetValue(this, varValue);
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}
