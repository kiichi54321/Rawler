using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

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

    public static class StringRegexExtentions
    {
        public static string Match(this string target, string pattern)
        {
            var rx = new Regex(pattern);
            var mc = rx.Matches(target);
            return (mc.Count == 0) ? null : mc[0].Value;
        }

        public static string Match(this string target, string pattern,int group)
        {
            var rx = new Regex(pattern);
            var mc = rx.Matches(target);
            return (mc.Count == 0) ? null : mc[0].Groups[group].Value;
        }

        public static string Match(this string target, string pattern, int group,RegexOptions regexOption)
        {
            var rx = new Regex(pattern,regexOption);
            var mc = rx.Matches(target);
            return (mc.Count == 0) ? null : mc[0].Groups[group].Value;
        }


        public static IEnumerable<Match> Matches(this string target,string pattern)
        {
            var rx = new Regex(pattern);
            var mc = rx.Matches(target);
            return mc.Cast<Match>();
        }

        public static bool IsMatch(this string target, string pattern)
        {
            var rx = new Regex(pattern);
            return rx.IsMatch(target);
        }
    }

    public static class Text
    {

        public static bool IsNullOrEmpty(this string text)
        {
            return string.IsNullOrEmpty(text);
        }

        public static IEnumerable<string> ReadLines(this string text)
        {
            return ReadLines(text, true);
        }

        public static IEnumerable<string> ReadLines(this string text,bool skipEmpty)
        {
            using (System.IO.StringReader sr = new System.IO.StringReader(text))
            {
                while (sr.Peek() > -1)
                {
                    string line = sr.ReadLine();
                    if(skipEmpty == false)
                    {
                        yield return line;
                    }
                    else if (skipEmpty && line.Length > 0)
                    {
                        yield return line;
                    }
                }
            }
        }
    }
}
