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

        public static string JoinText(this IEnumerable<char> list,string separator)
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

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="source"></param>
        /// <param name="key"></param>
        /// <param name="defaultVal"></param>
        /// <returns></returns>
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

        public static Dictionary<T, List<T1>> AddList<T, T1>(this Dictionary<T, List<T1>> source, T key, T1 obj)
        {
            if (source == null)
            {
                return null;
            }
            if (source.ContainsKey(key))
            {
                source[key].Add(obj);
            }
            else
            {
                source.Add(key,new List<T1>(){obj});
            }
            return source;
        }

        /// <summary>
        /// 数え上げ用。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="key"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static Dictionary<T, int> AddCount<T>(this Dictionary<T, int> source, T key, int count)
        {
            if(source == null)
            {
                return null;
            }
            if( source.ContainsKey(key))
            {
                source[key] = source[key] + count;
            }
            else
            {
                source.Add(key, count);
            }
            return source;
        }
        /// <summary>
        /// 数え上げ用。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Dictionary<T, int> AddCount<T>(this Dictionary<T, int> source, T key)
        {
            return AddCount(source, key, 1);
        }

        public static Dictionary<T,int> Marge<T>(this IEnumerable<Dictionary<T,int>> list)
        {
            Dictionary<T, int> dic = new Dictionary<T, int>();
            foreach (var item in list.SelectMany(n=>n))
            {
                if(dic.ContainsKey(item.Key))
                {
                    dic[item.Key] = dic[item.Key] + item.Value;
                }
                else
                {
                    dic.Add(item.Key, item.Value);
                }
            }
            return dic;
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

    public static class Date
    {
        /// <summary>
        /// d1とd2の間の日付である。
        /// </summary>
        /// <param name="d"></param>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <returns></returns>
        public static bool Between(this DateTime d, DateTime d1,DateTime d2)
        {
            if(d>= d1 && d<= d2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// その月の何週目かを取得する。
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static int GetWeek(this DateTime d)
        {
            var dd = 7-( d.DayOfWeek - DayOfWeek.Sunday);
            return  ( d.Day + dd )/7+1;
        }
    }

    public static class Char
    {
        /// <summary>
        /// 指定した Unicode 文字が、ひらがなかどうかを示します。
        /// </summary>
        /// <param name="c">評価する Unicode 文字。</param>
        /// <returns>c がひらがなである場合は true。それ以外の場合は false。</returns>
        public static bool Isひらがな(this char c)
        {
            //「ぁ」～「より」までと、「ー」「ダブルハイフン」をひらがなとする
            return ('\u3041' <= c && c <= '\u309F')
                || c == '\u30FC' || c == '\u30A0';
        }

        public static bool Isひらがな(this string s)
        {
            if (s == null)
            {
                throw new ArgumentNullException("s");
            }

            foreach (char c in s)
            {
                if (!Isひらがな(c))
                {
                    //ひらがなでない文字が含まれているとき
                    return false;
                }
            }

            return true;
        }
        /// <summary>
        /// 指定した Unicode 文字が、カタカナかどうかを示します。
        /// </summary>
        /// <param name="c">評価する Unicode 文字。</param>
        /// <returns>c がカタカナである場合は true。それ以外の場合は false。</returns>
        public static bool Isカタカナ(this char c)
        {
            //「ダブルハイフン」から「コト」までと、カタカナフリガナ拡張と、
            //濁点と半濁点と、半角カタカナをカタカナとする
            //中点と長音記号も含む
            return ('\u30A0' <= c && c <= '\u30FF')
                || ('\u31F0' <= c && c <= '\u31FF')
                || ('\u3099' <= c && c <= '\u309C')
                || ('\uFF65' <= c && c <= '\uFF9F');
        }
        /// <summary>
        /// 指定した Unicode 文字が、漢字かどうかを示します。
        /// </summary>
        /// <param name="c">評価する Unicode 文字。</param>
        /// <returns>c が漢字である場合は true。それ以外の場合は false。</returns>
        public static bool IsKanji(this char c)
        {
            //CJK統合漢字、CJK互換漢字、CJK統合漢字拡張Aの範囲にあるか調べる
            return ('\u4E00' <= c && c <= '\u9FCF')
                || ('\uF900' <= c && c <= '\uFAFF')
                || ('\u3400' <= c && c <= '\u4DBF');
        }

        public static string Takeひらがな(this string s)
        {
            return s.TakeWhile(n => n.Isひらがな() == false).JoinText("");
        }

        public static string TakeKatakana(this string s)
        {
            return s.TakeWhile(n => n.Isカタカナ() == false).JoinText("");
        }

        public static string TakeAlphabets(this string s)
        {
            HashSet<char> alphas = new HashSet<char>("abcdefghijklmnopqrstuzwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToArray());
            return s.TakeWhile(n => alphas.Contains(n)).JoinText("");
        }
    }


    public static class Text
    {
        /// <summary>
        /// string.IsNullOrEmpty
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Textを改行区切り配列にする。
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static IEnumerable<string> ReadLines(this string text)
        {
            return ReadLines(text, true);
        }

        /// <summary>
        ///  Textを改行区切り配列にする。（空行スキップ機能付き）
        /// </summary>
        /// <param name="text"></param>
        /// <param name="skipEmpty"></param>
        /// <returns></returns>
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
