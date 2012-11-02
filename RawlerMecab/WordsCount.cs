using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rawler.Tool;

namespace RawlerMecab
{

    public class GetWords : RawlerMultiBase
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<GetWords>(parent);
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
