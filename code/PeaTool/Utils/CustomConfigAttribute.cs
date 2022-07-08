using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using System.Reflection;

namespace PeaTool.Utils
{
    /// <summary>
    /// 全局配置标签
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public abstract class CustomConfigAttribute : Attribute
    {
        /// <summary>
        /// 默认值
        /// </summary>
        public object DefaultValue { get; set; }

        /// <summary>
        /// 配置名称
        /// </summary>
        public string ConfigName { get; set; }

        /// <summary>
        /// 获取配置的方法
        /// </summary>
        public abstract string GetConfig(string configName);

        /// <summary>
        /// 根据Config标签和变量名称获取配置变量
        /// </summary>
        public virtual bool GetConfigValue(Type type, string paramName, out object objectValue)
        {
            object defaultValue = null;
            try
            {
                defaultValue = type.IsValueType ? Activator.CreateInstance(type) : null;
                var configValue = GetConfig(string.IsNullOrWhiteSpace(ConfigName) ? paramName : ConfigName);
                if (string.IsNullOrEmpty(configValue))
                {
                    if (DefaultValue != null && type.IsAssignableFrom(DefaultValue.GetType()))
                    {
                        objectValue = DefaultValue;
                        return true;
                    }
                    objectValue = defaultValue;
                    return false;
                }
                objectValue = MyTypeDescriptor.GetConverter(type, this).ConvertFromString(configValue);
                return true;
            }
            catch
            {
                objectValue = defaultValue;
                return false;
            }
        }

        /// <summary>
        /// 设置配置
        /// </summary>
        public abstract void SetConfig(string configName, string configValue);

        /// <summary>
        /// 根据Config标签和变量名称设置配置
        /// </summary>
        public virtual void SetConfigValue(Type type, string paramName, object objectValue)
        {
            try
            {
                string strObjValue = string.Empty;
                if (objectValue != null)
                {
                    strObjValue = MyTypeDescriptor.GetConverter(type, this).ConvertToString(objectValue);
                }
                SetConfig(string.IsNullOrWhiteSpace(ConfigName) ? paramName : ConfigName, strObjValue);
            }
            catch
            {
            }
        }
    }

    /// <summary>
    /// 从本地ini文件获取配置
    /// </summary>
    public class IniConfigAttribute : CustomConfigAttribute
    {
        /// <summary>
        /// 配置文件名
        /// </summary>
        private string iniFileName = "application.ini";

        public string IniFileName { get { return iniFileName; } set { iniFileName = value; } }

        /// <summary>
        /// 配置节点名
        /// </summary>
        private string iniSection = "System";

        public string IniSection { get { return iniSection; } set { iniSection = value; } }

        /// <summary>
        /// 读取字符串的最大长度
        /// </summary>
        private int readSize = 255;

        public int ReadSize { get { return readSize; } set { readSize = value; } }

        /// <summary>
        /// 读取失败后返回的内容
        /// </summary>
        private string readFailResult = "读取失败";

        public string ReadFailureResult { get { return readFailResult; } set { readFailResult = value; } }

        [DllImport("kernel32")]
        public static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        /// <summary>
        /// 反射对象类型key
        /// </summary>
        private string reflectInfoConfigKey = "ReflectInto";
        public string ReflectInfoConfigKey { get { return reflectInfoConfigKey; } set { reflectInfoConfigKey = value; } }

        /// <summary>
        /// 从本地ini文件获取配置
        /// </summary>
        private string InnerGetConfig(string section, string configName)
        {
            try
            {
                var iniFilePath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, IniFileName);
                var configStringValue = IniFileHelper.ReadIniData(iniFilePath, section, configName, ReadSize, ReadFailureResult);
                return configStringValue != ReadFailureResult && configStringValue != "" ? configStringValue : "";
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 从本地ini文件获取配置
        /// </summary>
        public override string GetConfig(string configName)
        {
            return InnerGetConfig(IniSection, configName);
        }
        /// <summary>
        /// 获取配置实体
        /// </summary>
        public override bool GetConfigValue(Type type, string paramName, out object objectValue)
        {
            return InnerGetConfigValue(type, IniSection, paramName, out objectValue);
        }
        /// <summary>
        /// 内部获取配置项值
        /// </summary>
        private bool InnerGetConfigValue(Type type, string section, string paramName, out object objectValue)
        {
            object defaultValue = null;
            try
            {
                defaultValue = type.IsValueType ? Activator.CreateInstance(type) : null;
                var configValue = InnerGetConfig(section, string.IsNullOrWhiteSpace(ConfigName) ? paramName : ConfigName);
                if (string.IsNullOrEmpty(configValue))
                {
                    if (DefaultValue != null && DefaultValue.GetType().Equals(type))
                    {
                        objectValue = DefaultValue;
                        return true;
                    }
                    objectValue = defaultValue;
                    return false;
                }
                if (!IsSystemType(type) && configValue.StartsWith("[") && configValue.EndsWith("]") && !configValue.Contains(","))
                {
                    objectValue = CreateObjectFromSection(type, configValue);
                }
                else
                {
                    objectValue = MyTypeDescriptor.GetConverter(type, this).ConvertFromString(configValue);
                }
                return true;
            }
            catch
            {
                objectValue = defaultValue;
                return false;
            }
        }

        /// <summary>
        /// 从section中创建对象
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="sectionName">section</param>
        /// <returns></returns>
        public object CreateObjectFromSection(Type type, string sectionName)
        {
            sectionName = sectionName.Substring(1, sectionName.Length - 2);
            var iniFilePath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, IniFileName);
            var sectionKeys = IniFileHelper.ReadKeys(iniFilePath, sectionName);
            var parentType = type;
            if (sectionKeys != null && sectionKeys.Contains(ReflectInfoConfigKey))
            {
                type = GetType(IniFileHelper.ReadIniData(iniFilePath, sectionName, ReflectInfoConfigKey, ReadSize, ReadFailureResult));
            }
            if (!type.IsAssignableFrom(parentType))
            {
                throw new Exception("Unmatched type");
            }
            var instance = Activator.CreateInstance(type);
            if (sectionKeys == null || sectionKeys.Count == 0) { return instance; }
            var searchFlags = BindingFlags.Public | BindingFlags.Instance;
            foreach (var key in sectionKeys)
            {
                if (key == ReflectInfoConfigKey) { continue; }
                object objVal;
                var field = type.GetField(key, searchFlags);
                if (field != null)
                {
                    InnerGetConfigValue(field.FieldType, sectionName, field.Name, out objVal);
                    field.SetValue(instance, objVal);
                    break;
                }
                var property = type.GetProperty(key, searchFlags);
                if (property != null)
                {
                    InnerGetConfigValue(property.PropertyType, sectionName, property.Name, out objVal);
                    property.SetValue(instance, objVal, null);
                }
            }
            return instance;
        }
        /// <summary>
        /// 从当前应用程序域中根据名称获取类型
        /// </summary>
        private static Type GetType(string typeName)
        {
            Type type = null;
            Assembly[] assemblyArray = AppDomain.CurrentDomain.GetAssemblies();
            int assemblyArrayLength = assemblyArray.Length;
            for (int i = 0; i < assemblyArrayLength; ++i)
            {
                type = assemblyArray[i].GetType(typeName);
                if (type != null)
                {
                    return type;
                }
            }

            if (type == null)
            {
                throw new ArgumentException("No type matched");
            }
            return type;
        }
        /// <summary>
        /// 是否是系统类型
        /// </summary>
        private static bool IsSystemType(Type typeOfObj)
        {
            return typeOfObj.IsPrimitive || typeOfObj == typeof(string) || typeOfObj.IsArray;
        }
        /// <summary>
        /// 设置配置
        /// </summary>
        public override void SetConfig(string configName, string configValue)
        {
            try
            {
                var iniFilePath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, IniFileName);
                IniFileHelper.WriteIniData(IniSection, configName, configValue, iniFilePath);
            }
            catch
            {
            }
        }
    }

    /// <summary>
    /// 从app.config获取配置
    /// </summary>
    public class AppConfigAttribute : CustomConfigAttribute
    {
        /// <summary>
        /// 优先获取该可执行程序的配置文件
        /// </summary>
        public static string PrioritizedExecutableProgram { get; set; }
        /// <summary>
        /// xml解析对象
        /// </summary>
        private Lazy<XmlDocument> lazyXmlDoc = new Lazy<XmlDocument>(GetConfigFilePath);
        /// <summary>
        /// xml配置文件路径
        /// </summary>
        private static string xmlPath;
        /// <summary>
        /// 从app.config获取配置
        /// </summary>
        public override string GetConfig(string configName)
        {
            try
            {
                return lazyXmlDoc.Value.SelectSingleNode("//appSettings//add[@key='" + configName + "']")?.Attributes["value"]?.Value;
            }
            catch
            {
                return string.Empty;
            }
        }
        /// <summary>
        /// 获取配置文件路径
        /// </summary>
        private static XmlDocument GetConfigFilePath()
        {
            string str_path = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName + ".config";
            if (!string.IsNullOrEmpty(PrioritizedExecutableProgram))
            {
                string tempPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PrioritizedExecutableProgram + ".exe.config");
                if (File.Exists(tempPath))
                {
                    str_path = tempPath;
                }
            }
            var xDoc = new XmlDocument();
            //获取可执行文件的路径和名称
            xDoc.Load(str_path);
            xmlPath = str_path;
            return xDoc;
        }
        /// <summary>
        /// 设置配置
        /// </summary>
        public override void SetConfig(string configName, string configValue)
        {
            try
            {
                lazyXmlDoc.Value.SelectSingleNode("//appSettings//add[@key='" + configName + "']").Attributes["value"].Value = configValue;
                lazyXmlDoc.Value.Save(xmlPath);
            }
            catch
            {
            }
        }
    }
    /// <summary>
    /// 从json配置文件获取
    /// </summary>
    public class JsonConfigAttribute : CustomConfigAttribute
    {
        /// <summary>
        /// 全文标记
        /// </summary>
        public const string EntireFile = "entire";
        /// <summary>
        /// json文件名
        /// </summary>
        private string jsonFileName = "application.json";
        public string JsonFileName { get { return jsonFileName; } set { jsonFileName = value; } }
        /// <summary>
        /// 已读取过的json对象
        /// </summary>
        private static Dictionary<string, Newtonsoft.Json.Linq.JObject> ReadJsonObjectDic = new Dictionary<string, Newtonsoft.Json.Linq.JObject>();

        public override string GetConfig(string configName)
        {
            try
            {
                string jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, jsonFileName);
                Newtonsoft.Json.Linq.JObject jobj = null;
                if (ReadJsonObjectDic.ContainsKey(jsonFilePath))
                {
                    jobj = ReadJsonObjectDic[jsonFilePath];
                }
                if (File.Exists(jsonFilePath))
                {
                    jobj = Newtonsoft.Json.Linq.JObject.Parse(File.ReadAllText(jsonFilePath));
                }
                if (configName == EntireFile)
                {
                    return Newtonsoft.Json.JsonConvert.SerializeObject(jobj);
                }
                if (jobj != null && jobj.TryGetValue(configName, out Newtonsoft.Json.Linq.JToken jtoken))
                {
                    return jtoken.ToString();
                }
            }
            catch (Exception)
            {
            }
            return string.Empty;
        }

        public override void SetConfig(string configName, string configValue)
        {
            if (configName != EntireFile) return;
            try
            {
                string jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, jsonFileName);
                if (File.Exists(jsonFilePath))
                {
                    File.Delete(jsonFilePath);
                }
                using (FileStream fileStream = File.OpenWrite(jsonFilePath))
                {
                    byte[] configBytes = Encoding.UTF8.GetBytes(configValue);
                    fileStream.Write(configBytes, 0, configBytes.Length);
                    fileStream.Flush();
                }
            }
            catch (Exception)
            {
            }
        }
    }

    /// <summary>
    /// 列表装换类
    /// </summary>
    /// <typeparam name="TList">列表类型</typeparam>
    public class ListConverter<TList> : TypeConverter
    {
        public ListConverter(params Attribute[] attributes)
            : this(',', attributes)
        {
        }
        public ListConverter(char delimiter, params Attribute[] attributes)
        {
            Delimiter = delimiter;
            innerAttributes = attributes;
        }
        /// <summary>
        /// 分隔符
        /// </summary>
        public char Delimiter;
        /// <summary>
        /// 内部标签
        /// </summary>
        private Attribute[] innerAttributes;

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var valueString = value is string ? (string)value : value.ToString();
            var pieces = valueString.Split(new char[] { Delimiter }, StringSplitOptions.RemoveEmptyEntries);
            var result = Activator.CreateInstance(typeof(TList)) as IList;
            for (int i = 0; i < pieces.Length; i++)
            {
                var iniAttribute = innerAttributes != null ? innerAttributes.FirstOrDefault(item => item is IniConfigAttribute) : null;
                if (iniAttribute != null && pieces[i].StartsWith("[") && pieces[i].EndsWith("]") && !pieces[i].Contains(","))
                {
                    result.Add((iniAttribute as IniConfigAttribute).CreateObjectFromSection(typeof(TList).GetGenericArguments()[0], pieces[i]));
                }
                else
                {
                    result.Add(MyTypeDescriptor.GetConverter(typeof(TList).GetGenericArguments()[0], innerAttributes).ConvertFromString(pieces[i]));
                }
            }
            return result;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (typeof(string) == destinationType)
            {
                var strbud = new StringBuilder();
                var list = value as IList;
                for (var i = 0; i < list.Count; i++)
                {
                    strbud.Append(MyTypeDescriptor.GetConverter(typeof(TList).GetGenericArguments()[0], innerAttributes).ConvertToString(list[i])).Append(Delimiter);
                }
                return strbud.ToString().TrimEnd(Delimiter);
            }
            else
            {
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }
    }

    /// <summary>
    /// 一维数组装换类
    /// </summary>
    /// <typeparam name="TElement">元素类型</typeparam>
    public class SingleDimensionalArrayConverter<TElement> : ListConverter<List<TElement>>
    {
        public SingleDimensionalArrayConverter(params Attribute[] attributes)
            : this(',', attributes)
        {
        }
        public SingleDimensionalArrayConverter(char delimiter, params Attribute[] attributes)
            : base(delimiter, attributes)
        {

        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var result = base.ConvertFrom(context, culture, value) as IList;
            var array = Array.CreateInstance(typeof(TElement), result.Count);
            for (var i = 0; i < result.Count; i++)
            {
                array.SetValue(result[i], i);
            }
            return array;
        }
    }

    public class AttributeStringConverter<TObject> : TypeConverter
    {
        public AttributeStringConverter(ConvertableAttribute convertAttribute)
        {
            this.convertAttribute = convertAttribute;
        }
        /// <summary>
        /// 可以强制转换的标签
        /// </summary>
        private ConvertableAttribute convertAttribute;

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return value is string ? convertAttribute.ConvertFromString((string)value, typeof(TObject)) : base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            return destinationType == typeof(string) ? convertAttribute.ConvertToString(value) : base.ConvertTo(context, culture, value, destinationType);
        }
    }

    /// <summary>
    /// 自定义类型解释器
    /// </summary>
    public class MyTypeDescriptor
    {
        public static TypeConverter GetConverter(Type type, params Attribute[] attributes)
        {
            var convertableAttribute = default(ConvertableAttribute);
            if (attributes != null)
            {
                convertableAttribute = (ConvertableAttribute)attributes.FirstOrDefault(i => typeof(ConvertableAttribute).IsAssignableFrom(i.GetType()));
            }
            convertableAttribute = convertableAttribute ?? (ConvertableAttribute)Attribute.GetCustomAttribute(type, typeof(ConvertableAttribute));
            if (convertableAttribute != null)
            {
                return Activator.CreateInstance(typeof(AttributeStringConverter<>).MakeGenericType(type), convertableAttribute) as TypeConverter;
            }
            if (typeof(IDictionary).IsAssignableFrom(type))
            {
                return Activator.CreateInstance(typeof(JsonConvertableAttribute).MakeGenericType(type), attributes) as TypeConverter;
            }
            if (type.IsArray && type.HasElementType)
            {
                var elementType = type.GetElementType();
                return Activator.CreateInstance(typeof(SingleDimensionalArrayConverter<>).MakeGenericType(elementType), attributes) as TypeConverter;
            }
            if (typeof(IList).IsAssignableFrom(type))
            {
                return Activator.CreateInstance(typeof(ListConverter<>).MakeGenericType(type), attributes) as TypeConverter;
            }
            if (typeof(DateTime).IsAssignableFrom(type))
            {
                return new LongFormatDatetimeConverter();
            }
            return TypeDescriptor.GetConverter(type);
        }
    }
    public class LongFormatDatetimeConverter : TypeConverter
    {
        /// <summary>
        /// 日期格式
        /// </summary>
        private string format = "yyyy-MM-dd HH:mm:ss";

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return value is string ? DateTime.ParseExact((string)value, format, System.Globalization.CultureInfo.CurrentCulture) : base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            return destinationType == typeof(string) ? ((DateTime)value).ToString(format) : base.ConvertTo(context, culture, value, destinationType);
        }
    }

    /// <summary>
    /// 可进行json反序列化的标签
    /// </summary>
    public class JsonConvertableAttribute : ConvertableAttribute
    {
        /// <summary>
        /// 缩进格式
        /// </summary>
        public Newtonsoft.Json.Formatting indented = Newtonsoft.Json.Formatting.None;
        public Newtonsoft.Json.Formatting Indented { get { return indented; } set { indented = value; } }

        public override string ConvertToString(object obj)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj, Indented);
        }

        public override object ConvertFromString(string str, Type objType)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject(str, objType);
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public abstract class ConvertableAttribute : Attribute
    {
        /// <summary>
        /// object序列化成string
        /// </summary>
        public abstract string ConvertToString(object obj);
        /// <summary>
        /// string反序列化成object
        /// </summary>
        public abstract object ConvertFromString(string str, Type objType);
    }
}