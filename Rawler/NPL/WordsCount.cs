using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rawler.Tool;
using RawlerLib.MyExtend;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Rawler.NPL
{
    [Serializable()]
    public class WordListInSource
    {
        Dictionary<string, List<string>> dic = new Dictionary<string, List<string>>();
      
        public WordListInSource()
        { }

        public WordListInSource(IEnumerable<string> wordList)
        {
            dic = wordList.GroupBy(n => n.Take(2).JoinText("")).ToDictionary(n => n.Key, n => n.OrderByDescending(m =>m.Length).ToList());
        }

        IEnumerable<Tuple<string,string>> NGram(string text, int n)
        {
            for (int i = 0; i < text.Length - n; i++)
            {
                yield return new Tuple<string,string>(text.Substring(i, n),text.Substring(i));
            }
           
        }

        public void Save(string fileName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                BinaryFormatter bf = new BinaryFormatter();
                //シリアル化して書き込む
                bf.Serialize(fs, this);
            }
        }

        public static WordListInSource Load(string fileName)
        {
            FileStream fs = new FileStream(fileName,    FileMode.Open,    FileAccess.Read);
            BinaryFormatter f = new BinaryFormatter();
            //読み込んで逆シリアル化する
            object obj = f.Deserialize(fs);
            fs.Close();            
            return obj as WordListInSource;
        }

        public IEnumerable<string> GetWords(string text)
        {
            string t = text;
            while(t.Length>1)
            {
                bool flag = true;
                var w = t.Substring(0, 2);
                var a = t.TakeAlphabets();
                if(a.Length>1 && dic.ContainsKey(a.Substring(0,2)))
                {
                    foreach (var item in dic[a.Substring(0,2)])
                    {
                        if(item == a)
                        {
                            t = t.Substring(item.Length);
                            flag = false;
                            yield return item;
                            break;
                        }
                    }
                }
                else if(dic.ContainsKey(w))
                {                    
                    foreach (var item in dic[w])
                    {
                        if(t.Length >= item.Length && t.Substring(0,item.Length)==item)
                        {
                            t = t.Substring(item.Length);
                            flag = false;
                            yield return item;
                            break;
                        }
                    }
                }
                else if(dic.ContainsKey(w.First().ToString()))
                {
                    yield return w.First().ToString();
                }
                if (flag)
                {
                    t = t.Substring(1);
                }
                if (t.Length < 2)
                {
                    break;
                }
            }

        }
    }

    /// <summary>
    /// SouceIterator にある文字列のみを取得し、子に流します（複数）
    /// </summary>
    public class GetWordsInSouce : RawlerMultiBase
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<GetWordsInSouce>(parent);
        }

        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }
        #endregion

        public Iterator SouceIterator { get; set; }
        public int MinCount { get; set; }

        bool createOnce = true;
        bool createOnceFlag = false;
        public bool CreateOnce
        {
            get { return createOnce; }
            set { createOnce = value; }
        }
        Dictionary<string, List<string>> dic = new Dictionary<string, List<string>>();
        int keyWordLength = 2;

        /// <summary>
        /// このクラスでの実行すること。
        /// </summary>
        /// <param name="runChildren"></param>
        public override void Run(bool runChildren)
        {
           
            if (SouceIterator != null)
            {
                SouceIterator.SetParent(this);
                SouceIterator.Run();

                string target = GetText();
                int count = 0;
                if (createOnceFlag == false || createOnce == false)
                {
                    //辞書の作成
                    dic = new Dictionary<string, List<string>>();
                    var wordList = SouceIterator.Texts.GetList().OrderByDescending(n => n.Length).ToArray();
                    foreach (var item in wordList)
                    {
                        List<string> tmpList = new List<string>();
                        if (dic.TryGetValue(item.Substring(0,keyWordLength), out tmpList) == false)
                        {
                            tmpList = new List<string>();
                            dic.Add(item.Substring(0, keyWordLength), tmpList);
                        }
                        tmpList.Add(item);
                    }
                }
                List<string> list = new List<string>();
                foreach (var item in NGram(GetText(), keyWordLength).Distinct())
                {
                    List<string> tmpList = new List<string>();
                    if (dic.TryGetValue(item, out tmpList))
                    {
                        foreach (var item2 in tmpList)
                        {
                            target = GetCount(target, item2, out count);
                            if (count >= MinCount)
                            {
                                for (int i = 0; i < count; i++)
                                {
                                    list.Add(item2);
                                }
                            }
                        }
                    }
                }

                RunChildrenForArray(runChildren, list);
            }
            else
            {
                ReportManage.ErrReport(this, "SouceIteratorがありません");
            }

        }

        /// <summary>
        /// 子が参照するテキスト。
        /// </summary>
        public override string Text
        {
            get
            {
                return base.Text;
            }
        }


        public string GetCount(string target, string searchWord, out int result)
        {
            int positon = 0;
            int count = 0;
            while (true)
            {
                var i = target.IndexOf(searchWord, positon);
                if (i > 0)
                {
                    count++;
                    positon = i + 1;
                }
                else
                {
                    break;
                }
            }
            result = count;
            return target.Replace(searchWord, " ");

        }

        public IEnumerable<string> NGram(string text, int n)
        {
            List<string> list = new List<string>();
            for (int i = 0; i < text.Length-n; i++)
            {
                list.Add(text.Substring(i, n));
            }
            return list;
        }


    }
}
