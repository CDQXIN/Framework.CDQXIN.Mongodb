using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace Framework.CDQXIN.MongodbEx1
{
    public class MemoryCacheHelper
    {

        private static ObjectCache obcache = MemoryCache.Default;


        #region 获取内存缓存值

        /// <summary>
        /// 获取内存缓存值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static object getCacheValue(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return null;
            }
            return obcache.Get(key);
        }

        #endregion 获取内存缓存值

        #region 增加文件依赖缓存项
        /// <summary>
        /// 增加文件依赖缓存项
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="filePath"></param>
        public static void InsertFileDependency(string key, object value, string filePath)
        {
            if (obcache[key] == null)
            {
                CacheItemPolicy policy = new CacheItemPolicy();
                policy.ChangeMonitors.Add(new HostFileChangeMonitor(new List<string> { filePath }));
                obcache.Set(key, value, policy);
            }
        }
        #endregion

        #region 从缓存项中移除指定项
        /// <summary>
        /// 从缓存项中移除指定项
        /// </summary>
        /// <param name="key"></param>
        public static void RemoveCache(string key)
        {
            if (obcache[key] != null)
            {
                obcache.Remove(key);
            }
        }
        #endregion

    }
}
