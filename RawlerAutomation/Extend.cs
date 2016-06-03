using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using RawlerLib.MyExtend;
using CsvHelper;
using System.IO;

namespace RawlerExpressLib.Automation.Extend
{
    public class HTMLAnalyzeResult
    {
        public string HTML { get; set; }
        public int Length { get; set; }
        public string StructHTML { get; set; }
        public HtmlNode Node { get; set; }
        public string HtmlTag { get; set; }
        public bool CanUse { get; set; }
    }

    public static class HTMLExtend
    {
        /// <summary>
        /// "span", 抜いた
        /// </summary>
//        public static HashSet<string> TargetAnalyzeTag = new HashSet<string>() { "div", "dd", "dl", "dt", "table", "tr", "td", "th", "ul", "ol", "li", "h1", "h2", "h3", "h4", "h5", "h6" };
        public static HashSet<string> TargetAnalyzeTag = new HashSet<string>() { "div", "dd", "dl", "dt",  "ul", "ol", "li", "h1", "h2", "h3", "h4", "h5", "h6" };
        public static HashSet<string> TargetAnalyzeAttribute = new HashSet<string>() { "id", "class" };

        public static HTMLAnalyzeResult GetHTMLStruct(this HtmlNode node)
        {
            if (node.InnerText.Length > 0)
            {
                HTMLAnalyzeResult result = new HTMLAnalyzeResult() { HTML = node.InnerHtml, Length = node.InnerText.Length, Node = node, CanUse = true };
                if (TargetAnalyzeTag.Contains(node.Name))
                {
                    StringBuilder sb = new StringBuilder();
                    result.HtmlTag = node.Name + " " + node.Attributes.Where(n => TargetAnalyzeAttribute.Contains(n.Name)).Select(n => n.Name + "=" + n.Value).JoinText(" ");
                    sb.Append("<" + result.HtmlTag + ">");
                    if (node.InnerHtml.Length > 0)
                    {
                        sb.Append(node.ChildNodes.Where(n => TargetAnalyzeTag.Contains(n.Name)).Select(n => n.GetHTMLStruct().StructHTML).JoinText(string.Empty));
                    }
                    sb.Append("</" + node.Name + ">");
                    result.StructHTML = sb.ToString();
                }
                else
                {
                    result.StructHTML = string.Empty;
                }
                return result;
            }
            return new HTMLAnalyzeResult() { StructHTML = string.Empty,CanUse = true,Node = node,HTML = string.Empty,Length = 0  };

        }

        public static HTMLAnalyzeResult GetHTMLStruct(this HtmlNode node, Dictionary<HtmlNode, HTMLAnalyzeResult> cache)
        {
            if(cache.ContainsKey(node))
            {
                return cache[node];
            }

            if (node.InnerText.Length > 0)
            {
                HTMLAnalyzeResult result = new HTMLAnalyzeResult() { HTML = node.InnerHtml, Length = node.InnerText.Length, Node = node, CanUse = true };
                if (TargetAnalyzeTag.Contains(node.Name))
                {
                    StringBuilder sb = new StringBuilder();
                    result.HtmlTag = node.Name + " " + node.Attributes.Where(n => TargetAnalyzeAttribute.Contains(n.Name)).Select(n => n.Name + "=" + n.Value).JoinText(" ");
                    sb.Append("<" + result.HtmlTag + ">");
                    if (node.InnerHtml.Length > 0)
                    {
                        sb.Append(node.ChildNodes.Where(n => TargetAnalyzeTag.Contains(n.Name)).Select(n => n.GetHTMLStruct(cache).StructHTML).JoinText(string.Empty));
                    }
                    sb.Append("</" + node.Name + ">");
                    result.StructHTML = sb.ToString();
                }
                else
                {
                    result.StructHTML = string.Empty;
                }
                cache.Add(node, result);
                return result;
            }
            var r = new HTMLAnalyzeResult() { StructHTML = string.Empty, CanUse = true, Node = node, HTML = string.Empty, Length = 0 };
            cache.Add(node, r);
            return r;

        }


        public static String GetHTMLWithoutValue(this HtmlNode node)
        {
            StringBuilder sb = new StringBuilder();
            if (node.Name != "#text")
            {
                var tag = node.Name + " " + node.Attributes.Where(n => TargetAnalyzeAttribute.Contains(n.Name)).Select(n => n.Name + "=" + n.Value).JoinText(" ");
                sb.Append("<" + tag + ">");
                foreach (var item in node.ChildNodes)
                {
                    sb.Append(item.GetHTMLWithoutValue());
                }
                sb.Append("</" + node.Name + ">");
            }
            return sb.ToString();
        }

        /// <summary>
        /// ClassNameを得る。からの場合は上流を遡る。
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static string GetClassName(this HtmlNode node)
        {
            if (node.Attributes.Where(n => n.Name == "class").Any())
            {
                return node.Attributes.Where(n => n.Name == "class").First().Value+"_"+node.Name ;
            }
            else
            {
                if (node.ParentNode != null)
                {
                    return GetClassName(node.ParentNode);
                }
                return string.Empty;
            }
        }

        //public static string JoinText(this IEnumerable<string> list, string separator)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    foreach (var item in list)
        //    {
        //        sb.Append(item);
        //        sb.Append(separator);
        //    }
        //    return sb.ToString();
        //}

        public static void Run<T>(this IEnumerable<T> list, Action<T> action)
        {
            foreach (var item in list)
            {
                action(item);
            }
        }
        public static IEnumerable<TSource> GetTakeTopValue<TSource, TKey>(this IEnumerable<TSource> list, Func<TSource, TKey> func)
        {
            if (list.Any())
            {
                var list2 = list.OrderByDescending(n => func(n));
                var value = func(list2.First());
                return list2.TakeWhile(n => func(n).Equals(value));
            }
            else
            {
                return new List<TSource>();
            }
        }

        /// <summary>
        /// Rawler.Tool.Dataを評価値の高い物順にソートする。
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static IOrderedEnumerable<Rawler.Tool.Data> DataSort(this IEnumerable<Rawler.Tool.Data> list)
        {
            return list.OrderByDescending(n=>n.AvgColumns*Math.Log(n.TotalRows));
        }

        public static IEnumerable<HTMLAnalyzeResult> GetAnalyzeResults(this HtmlNode node, Dictionary<HtmlNode, HTMLAnalyzeResult> cache)
        {
            yield return node.GetHTMLStruct(cache);
            foreach (var item in node.ChildNodes)
            {
                foreach (var item2 in GetAnalyzeResults(item,cache))
                {
                    yield return item2;
                }
            }
        }

        public static IEnumerable<HTMLAnalyzeResult> GetAnalyzeResults(this HtmlNode node)
        {
            Dictionary<HtmlNode, HTMLAnalyzeResult> cache = new Dictionary<HtmlNode, HTMLAnalyzeResult>();
            yield return node.GetHTMLStruct(cache);
            foreach (var item in node.ChildNodes)
            {
                foreach (var item2 in GetAnalyzeResults(item,cache))
                {
                    yield return item2;
                }
            }
        }

        public static void Reflesh<T>(this System.Collections.ObjectModel.ObservableCollection<T> observable, IEnumerable<T> list)
        {
            observable.Clear();
            foreach (var item in list)
            {
                observable.Add(item);
            }
        }

        public static string ToCsv(this Rawler.Tool.Data data)
        {
            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream))
            using (var streamReader = new StreamReader(memoryStream))
            using (var csvWriter = new CsvWriter(streamWriter))
            {
                StringBuilder strBuilder = new StringBuilder();

                HashSet<string> hash = new HashSet<string>();
                foreach (var item2 in data.GetDataRows())
                {
                    foreach (var item3 in item2.DataDic.Keys)
                    {
                        hash.Add(item3);
                    }
                }
                foreach (var key in hash)
                {
                    csvWriter.WriteField(key);
                }
                csvWriter.NextRecord();
                streamWriter.Flush();
                memoryStream.Position = 0;
                strBuilder.Append(streamReader.ReadToEnd());
                memoryStream.SetLength(0);  // Clear

                foreach (var row in data.GetDataRows())
                {
                    foreach (var key in hash)
                    {
                        string val = "";
                        if (row.DataDic.ContainsKey(key))
                        {
                            StringBuilder str = new StringBuilder();
                            bool flag = true;
                            foreach (var item5 in row.DataDic[key])
                            {
                                if (item5 != null)
                                {
                                    str.Append(item5.Replace("\n", "").Replace("\r", "").Replace("\t", "") + ",");
                                }
                                else
                                {
                                    flag = false;
                                }
                            }
                            if (flag)
                            {
                                str.Length = str.Length - 1;
                            }
                            val = str.ToString();
                        }
                        csvWriter.WriteField(val);
                    }
                    csvWriter.NextRecord();
                    streamWriter.Flush();
                    memoryStream.Position = 0;
                    strBuilder.Append(streamReader.ReadToEnd());
                    memoryStream.SetLength(0);  // Clear
                }

                return strBuilder.ToString();
            }
        }
    }
}
