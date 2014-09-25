using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace RawlerLib.MyExtend
{
    public static class Collection
    {
        public static T2 FirstDefault<T,T2>(this IEnumerable<T> list,Func<T,T2> func ,T2 defaultValue)
        {
            if(list.Any())
            {
                return func( list.First());
            }
            else
            {
                return defaultValue;
            }
        }

        public static string JoinText(this IEnumerable<string> list, string separator)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in list)
            {
                sb.Append(item);
                sb.Append(separator);            
            }
            if (list.Any())
            {
                sb.Length = sb.Length - separator.Length;
            }
            return sb.ToString();
        }

        public static IEnumerable<string> Ngram(this IEnumerable<string> list, int n,string separeter)
        {           
             var d = list.ToArray();
             for (int i = 0; i < d.Length-n+1; i++)
             {
                 yield return d.Skip(i).Take(n).JoinText(separeter);
             }
        }

        public static List<T> Adds<T>(this List<T> list,IEnumerable<T> list1)
        {
            list.AddRange(list1);
            return list;
        }
    }


    //public static class Web
    //{
    //    public static string DownloadHtml(System.Net.WebClient wc,string url)
    //    {
    //        var data = wc.DownloadData(url);
    //        var text_utf8 = System.Text.Encoding.UTF8.GetString(data);

    //        var p1 = "<meta http-equiv=\"content-type\" content=\"text/html; charset=(.*)\">";
    //        var p2 = "<meta charset=\"(.*)\">";
    //        var encoding = System.Text.Encoding.UTF8;
    //        try
    //        {
    //            var head = text_utf8.Substring(0, 600);
    //            var m1 = Regex.Match(head, p1, RegexOptions.IgnoreCase);
    //            if (m1.Success)
    //            {
    //                encoding = System.Text.Encoding.GetEncoding(m1.Groups[1].Value);
    //                return encoding.GetString(data);
    //            }
    //            else
    //            {
    //                var m2 = Regex.Match(head, p2, RegexOptions.IgnoreCase);
    //                if (m2.Success)
    //                {
    //                    encoding = System.Text.Encoding.GetEncoding(m2.Groups[1].Value);
    //                    return encoding.GetString(data);
    //                }
    //            }
    //        }
    //        catch (Exception e)
    //        {
               
    //        }
    //        return text_utf8;
    //    }
    //}

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

        /// <summary>
        /// Nullの時、空文字
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string NullIsEmpty(this string text)
        {
            if (text == null) return string.Empty;
            return text;
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
