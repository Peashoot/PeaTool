using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PeaTool.Utils
{
    public class IniFileHelper
    {
        #region API函数声明

        [DllImport("kernel32")]
        private static extern int WritePrivateProfileString(string section, string key,
            string val, string filePath);
        //需要调用GetPrivateProfileString的重载
        [DllImport("kernel32", EntryPoint = "GetPrivateProfileString")]
        private static extern int GetPrivateProfileString(string section, string key,
            string def, StringBuilder retVal, int size, string filePath);

        [DllImport("kernel32", EntryPoint = "GetPrivateProfileString")]
        private static extern uint GetPrivateProfileStringA(string section, string key,
            string def, Byte[] retVal, int size, string filePath);

        #endregion

        /// <summary>
        /// 获取所有section
        /// </summary>
        public static List<string> ReadSections(string iniFilename)
        {
            List<string> result = new List<string>();
            byte[] buf = new byte[1 << 16];
            uint len = GetPrivateProfileStringA(null, null, null, buf, buf.Length, iniFilename);
            int j = 0;
            for (int i = 0; i < len; i++)
            {
                if (buf[i] == 0)
                {
                    result.Add(Encoding.Default.GetString(buf, j, i - j));
                    j = i + 1;
                }
            }
            return result;
        }
        /// <summary>
        /// 获取section下所有的key
        /// </summary>
        public static List<string> ReadKeys(string iniFilename, string SectionName)
        {
            List<string> result = new List<string>();
            byte[] buf = new byte[1 << 16];
            uint len = GetPrivateProfileStringA(SectionName, null, null, buf, buf.Length, iniFilename);
            int j = 0;
            for (int i = 0; i < len; i++)
            {
                if (buf[i] == 0)
                {
                    result.Add(Encoding.Default.GetString(buf, j, i - j));
                    j = i + 1;
                }
            }
            return result;
        }
        /// <summary>
        /// 读ini配置文件
        /// </summary>
        public static string ReadIniData(string iniFilePath, string Section, string Key, int readSize = 256, string NoText = "读取失败")
        {
            if (!Path.IsPathRooted(iniFilePath))
            {
                iniFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, iniFilePath);
            }
            if (File.Exists(iniFilePath))
            {
                StringBuilder temp = new StringBuilder(readSize);
                GetPrivateProfileString(Section, Key, NoText, temp, readSize, iniFilePath);
                return temp.ToString();
            }
            else
            {
                return string.Empty;
            }
        }
        /// <summary>
        /// 写ini配置文件
        /// </summary>
        public static bool WriteIniData(string Section, string Key, string Value, string iniFilePath)
        {
            return File.Exists(iniFilePath) && WritePrivateProfileString(Section, Key, Value, iniFilePath) == 0;
        }
    }
}
