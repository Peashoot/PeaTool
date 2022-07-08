using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace PeaTool.Utils
{
    /// <summary>
    /// 单例模式
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    public class SingletonMode<T> where T : class
    {
        private static T _instance;

        public static T Instance
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get { return _instance = _instance ?? (T)Activator.CreateInstance(typeof(T)); }
        }
    }
    /// <summary>
    /// 单例操作类
    /// </summary>
    public class SingletonMode
    {
        /// <summary>
        /// 根据Type获取单例对象
        /// </summary>
        /// <param name="generic">具体对象的类型</param>
        /// <returns>单例对象</returns>
        public static object GetInstance(Type generic)
        {
            return typeof(ServiceHost<>).MakeGenericType(generic).GetProperty("Singleton").GetValue(null, null);
        }
    }

    /// <summary>
    /// 服务托管操作类
    /// </summary>
    public class ServiceHost
    {
        public ServiceHost()
        {
            singletonCacheDic = new ConcurrentDictionary<Type, object>();
        }
        /// <summary>
        /// 单例对象缓存字典
        /// </summary>
        private readonly ConcurrentDictionary<Type, object> singletonCacheDic;
        /// <summary>
        /// 获取托管服务单例
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="constructionParameters">构造函数参数</param>
        public T Singleton<T>(params object[] constructionParameters)
        {
            return (T)Singleton(typeof(T), constructionParameters);
        }
        /// <summary>
        /// 获取托管服务单例
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="constructionParameters">构造函数参数</param>
        public object Singleton(Type type, params object[] constructionParameters)
        {
            return singletonCacheDic.GetOrAdd(type, (t) => Activator.CreateInstance(t, constructionParameters));
        }
        /// <summary>
        /// 获取托管服务新实例
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="constructionParameters">构造函数参数</param>
        public T Transient<T>(params object[] constructionParameters)
        {
            return (T)Transient(typeof(T), constructionParameters);
        }
        /// <summary>
        /// 获取托管服务新实例
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="constructionParameters">构造函数参数</param>
        public object Transient(Type type, params object[] constructionParameters)
        {
            return Activator.CreateInstance(type, constructionParameters);
        }
    }
    /// <summary>
    /// 泛型托管服务操作类
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    public class ServiceHost<T>
    {
        /// <summary>
        /// 获取无参类型单例
        /// </summary>
        public static T Singleton { get { return SingletonMode<ServiceHost>.Instance.Singleton<T>(); } }
    }
}
