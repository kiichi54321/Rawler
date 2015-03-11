using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rawler.Tool
{
    public class Equal : RawlerBase
    {
        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }


        /// <summary>
        /// 空文字であるかを判定し、空文字なら、子をRunする。実質IF文。含まれないを指定するとき使う。
        /// </summary>
        public Equal()
            : base()
        {
            this.Result = result;
        }
        Dictionary<char, List<string>> dic = new Dictionary<char, List<string>>();
        private string equalCSV = string.Empty;
        private string[] equalArray = null;
        public string EqualCSV
        {
            get { return equalCSV; }
            set
            {
                equalCSV = value;
                equalArray = equalCSV.Split(',');
            }
        }

        public Iterator EqualDataTree { get; set; }

        private bool result = true;
        /// <summary>
        ///　空文字でTrue　含まれないものを判定するならFalse
        /// </summary>
        public bool Result
        {
            get { return result; }
            set { result = value; }
        }


        /// <summary>
        /// 含まれるか判定する。
        /// </summary>
        public bool Check
        {
            get
            {
                var t = GetText();
                if(dic.ContainsKey(t.First()))
                {
                    return  dic[t.First()].Any(n => n.Equals(t));
                }
                else
                {
                    return false;
                }
            }
        }
        /// <summary>
        /// このオブジェクトのテキスト
        /// </summary>
        public override string Text
        {
            get
            {
                return GetText();
            }

        }

        //public new void Run()
        //{
        //    Run(true);
        //}
        private bool createOnce = false;
        /// <summary>
        /// 実行
        /// </summary>
        /// <param name="runChildren"></param>
        public override void Run(bool runChildren)
        {
            if (createOnce == false)
            {
                if (EqualDataTree != null)
                {
                    EqualDataTree.SetParent(this);
                    EqualDataTree.Run();
                    dic.Clear();
                    foreach (var item in EqualDataTree.Texts)
                    {
                        if (item.Value.Length > 0)
                        {
                            if (dic.ContainsKey(item.Value.First()))
                            {
                                dic[item.Value.First()].Add(item.Value);
                            }
                            else
                            {
                                dic.Add(item.Value.First(), new List<string>() { item.Value });
                            }
                        }
                    }
                }
                if (equalArray != null && equalArray.Length > 0)
                {
                    foreach (var item in equalArray)
                    {
                        if (dic.ContainsKey(item.First()))
                        {
                            dic[item.First()].Add(item);
                        }
                        else
                        {
                            dic.Add(item.First(), new List<string>() { item });
                        }
                    }
                }
                createOnce = true;
            }

            if (Check == this.Result)
            {
                this.RunChildren(runChildren);
            }
        }

        /// <summary>
        /// クローンを作る。
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return this.Clone<Equal>(parent);
        }
    }
}
