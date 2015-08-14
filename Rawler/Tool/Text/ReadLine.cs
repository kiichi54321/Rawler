using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rawler.Tool
{
    /// <summary>
    /// テキストを改行送りに変換する。複数出力
    /// </summary>
    public class ReadLines:RawlerMultiBase
    {
        /// <summary>
        /// 読み込むテキスト。
        /// </summary>
        public string Lines { get; set; }

        public override void Run(bool runChildren)
        {
            string t;
            //Linesがあるとき。
            if (string.IsNullOrEmpty(Lines) == false)
            {
                var  t1 = Lines.Split(' ');
                if(trim)
                {
                    t1 = t1.Select(n => n.Trim()).ToArray();
                }
                if(skipEmpty)
                {
                    t1 = t1.Where(n => n.Length > 0).ToArray();
                }
                base.RunChildrenForArray(runChildren, t1);
            }
            //ないとき
            else
            {
                t = GetText();
                if (t.Length > 0)
                {
                    System.IO.StringReader sr = new System.IO.StringReader(t);
                    List<string> list = new List<string>();
                    while (sr.Peek() > -1)
                    {
                        string line = sr.ReadLine();
                        if (trim) line = line.Trim();
                        if (skipEmpty)
                        {
                            if (line.Length > 0)
                            {
                                list.Add(line);
                            }
                        }
                        else
                        {
                            list.Add(line);
                        }
                    }
                    sr.Close();
                    base.RunChildrenForArray(runChildren, list);
                }
            }
           
        }

        bool trim = false;
        bool skipEmpty = true;

        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<ReadLines>(parent);
        }

        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }

        /// <summary>
        /// 一行ごとにTrimをする
        /// </summary>
        public bool Trim
        {
            get
            {
                return trim;
            }

            set
            {
                trim = value;
            }
        }

        /// <summary>
        /// 空白行をスキップする
        /// </summary>
        public bool SkipEmpty
        {
            get
            {
                return skipEmpty;
            }

            set
            {
                skipEmpty = value;
            }
        }


        #endregion
    }
}
