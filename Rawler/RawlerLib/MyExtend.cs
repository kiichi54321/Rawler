using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RawlerLib.MyExtend
{
    public static class Collection
    {
        public static string JoinText(this IEnumerable<string> list, string separator)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in list)
            {
                sb.Append(item);
                sb.Append(separator);
            }
            return sb.ToString();
        }
    }

    public static class DictionaryExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="source"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key)
        {
            if (source == null || source.Count == 0)
                return default(TValue);

            TValue result;
            if (source.TryGetValue(key, out result))
                return result;
            return default(TValue);
        }
        ///// 任意のデフォルト値指定可能版
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="source"></param>
        /// <param name="key"></param>
        /// <param name="defaultVal"></param>
        /// <returns></returns>
        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, TValue defaultVal = default(TValue))
        {
            if (source == null || source.Count == 0)
                return defaultVal;

            TValue result;
            if (source.TryGetValue(key, out result))
                return result;
            return defaultVal;
        }

        public static TValue GetValueOrAdd<TKey,TValue>(this IDictionary<TKey, TValue> source, TKey key, TValue defaultVal = default(TValue))
        {
            if (source == null )
                return defaultVal;

            TValue result;
            if (source.TryGetValue(key, out result))
                return result;
            else
            {
                source.Add(key, defaultVal);
                return source[key];
            }
        }
    }

    public static class Text
    {

        public static bool IsNullOrEmpty(this string text)
        {
            return string.IsNullOrEmpty(text);
        }

    }
}
