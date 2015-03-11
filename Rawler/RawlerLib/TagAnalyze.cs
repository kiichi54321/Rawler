using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RawlerLib.MarkupLanguage
{
    public class TagAnalyze
    {
        private enum TagType
        {
            Start, End
        }


        /// <summary>
        /// change がTrueの時、タグを大文字、小文字に変形して取得する。
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="tag"></param>
        /// <param name="change"></param>
        /// <returns></returns>
        public static IEnumerable<TagClass> GetTag(string txt, string tag,bool change)
        {
            if (change)
            {
                List<TagClass> list = new List<TagClass>();
                list.AddRange(GetTag(txt,tag.ToUpper()));
                list.AddRange(GetTag(txt,tag.ToLower()));
                list.AddRange(GetTag(txt, tag));
                return list;
            }
            else
            {
                return GetTag(txt, tag);
            }
        }


        /// <summary>
        /// txtのから、tagで挟まれた要素を入れ子関係も含めて取ってくる。
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static IEnumerable<TagClass> GetTag(string txt, string tag)
        {
            List<RawlerLib.Collections.Pair<int, TagType>> list = new List<RawlerLib.Collections.Pair<int, TagType>>();
            //            string startTag = "<" + tag + " " ;
            string endTag = "</" + tag + ">";
            var startList = StringIndexList(txt, new string[] { "<" + tag + " ", "<" + tag + ">" });
            var endList = StringIndexList(txt, endTag);
            Dictionary<int, TagClass> dic = new Dictionary<int, TagClass>();
            var t = txt.ToArray();
            foreach (var item in startList.OrderBy(n=>n))
            {
                list.Add(new RawlerLib.Collections.Pair<int, TagType>(item, TagType.Start));
                dic.Add(item, new TagClass(tag, item, txt,t));
            }
            foreach (var item in endList)
            {
                list.Add(new RawlerLib.Collections.Pair<int, TagType>(item, TagType.End));
            }
            list.Sort();

            while (list.Count > 0)
            {
                List<RawlerLib.Collections.Pair<int, TagType>> tmpList = new List<RawlerLib.Collections.Pair<int, TagType>>();
                for (int i = 0; i < list.Count - 1; i++)
                {
                    if (list[i].Value == TagType.Start && list[i + 1].Value == TagType.End)
                    {
                        var tagClass = dic[list[i].Key];
                        tagClass.End = list[i + 1].Key;
                        tmpList.Add(list[i]);
                        tmpList.Add(list[i + 1]);

                        if (i > 0 && list[i - 1].Value == TagType.Start)
                        {
                            var tagParent = dic[list[i - 1].Key];
                            tagParent.Children.Add(tagClass);
                            tagClass.Parent = tagParent;
                        }
                    }
                }

                if (tmpList.Count == 0)
                {
                    break;
                }
                foreach (var item in tmpList)
                {
                    list.Remove(item);
                }

                int s = 0;
                int e = 0;
                foreach (var item in list)
                {
                    if (item.Value == TagType.Start) s++;
                    else if (item.Value == TagType.End) e++;
                }
                if (s == 0 || e == 0)
                {
                    if (e == 0)
                    {
                        //開始タグばかりの時
                        for (int i = 0; i < list.Count - 1; i++)
                        {
                            dic[list[i].Key].End = dic[list[i + 1].Key].Start - 1;
                        }
                    }
                    break;
                }
            }

            return dic.Values.OrderBy(n=>n.Start);

            //List<TagClass> result = new List<TagClass>();
            //foreach (var item in dic.Values)
            //{
            //    if (item.Parent == null)
            //    {
            //        result.Add(item);
            //    }
            //}
            //return result;
        }

        public static IEnumerable<TagClass>　GetTopTag(string txt,string tag)
        {
            List<TagClass> result = new List<TagClass>();
            foreach (var item in GetTag(txt,tag))
            {
                if (item.Parent == null)
                {
                    result.Add(item);
                }
            }
            return result;
        }

        //public static ICollection<TagClass> GetAllTag(string txt, string tag)
        //{
        //    var top = GetTag(txt, tag);
        //    List<TagClass> list = new List<TagClass>();
        //    Queue<TagClass> queue = new Queue<TagClass>(top);
        //    TagClass current;
        //    while (queue.Count>0)
        //    {
        //        current = queue.Dequeue();
        //        list.Add(current);
        //        foreach (var item in current.Children)
        //        {
        //            queue.Enqueue(item);

        //        }

        //    }
        //    return list;
        //}
        /// <summary>
        /// 文章に対して単語を探して見つかった場所のリストです。
        /// </summary>
        /// <param name="text"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        public static List<int> StringIndexList(string text, string word)
        {
            List<int> list = new List<int>();
            int idx = 0;
            int s = 0;
            int e = 0;

            while (idx > -1)
            {
                s = text.IndexOf(word, idx);
                if (s > -1)
                {
                    list.Add(s);
                }
                else
                {
                    break;
                }
                idx = s + 1;
            }

            return list;
        }

        /// <summary>
        /// 文章に対して単語を探して見つかった場所のリストです。
        /// </summary>
        /// <param name="text"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        public static List<int> StringIndexList(string text, string[] words)
        {
            List<int> list = new List<int>();

            foreach (var item in words)
            {
                list.AddRange(StringIndexList(text, item));
            }

            return list;
        }
    }
}
