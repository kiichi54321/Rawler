using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rawler.Tool;

namespace RawlerMecab
{

    public class WordsCount : RawlerMultiBase
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<WordsCount>(parent);
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

                List<string> list = new List<string>();
                foreach (var item in SouceIterator.Texts.GetList().OrderByDescending(n => n.Length))
                {
                    target = GetCount(target,item,out count);
                    if (count >= MinCount)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            list.Add(item);
                        }
                        //var data = Codeplex.Data.DynamicJson.Serialize(new { SeachWord = item, Count = count });
                        //list.Add(data);
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


        public string GetCount( string target, string searchWord,out int result)
        {
            int positon = 0;
            int count = 0;
            while (true)
            {
                var i = target.IndexOf(searchWord, positon);
                if (i > 0)
                {
                    count++;
                    positon = i+1;
                }
                else
                {
                    break;
                }
            }
            result = count;
            return target.Replace(searchWord, " ");

        }


    }
}
