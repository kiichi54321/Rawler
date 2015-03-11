using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;

namespace Rawler.Tool
{
    /// <summary>
    /// 複数のテキストを扱うRawlerクラス
    /// </summary>
    [ContentProperty("Children")]
    [Serializable]
    public class Iterator : RawlerMultiBase
    {
        ///// <summary>
        ///// このオブジェクトのテキスト
        ///// </summary>
        //public override string Text
        //{
        //    get
        //    {
        //        return texts.First().Value;
        //    }

        //}
        TextVauleList texts = new TextVauleList();
        /// <summary>
        /// このオブジェクトのテキスト（複数）
        /// </summary>
        public TextVauleList Texts
        {
            get
            {
                return texts;
            }
            set
            {
                texts = value;
            }
        }
        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }
        /// <summary>
        /// テキストをセットする。
        /// </summary>
        /// <param name="list"></param>
        public void SetTexts(List<string> list)
        {
            texts.SetList(list);
        }

        public void AddText(string text)
        {
            texts.Add(text);
        }

        public void TextsClear()
        {
            texts.Clear();
        }

        bool createDataOnce = true;
        bool createDataOnceFlag = false;

        public bool CreateDataOnce
        {
            get { return createDataOnce; }
            set { createDataOnce = value; }
        }
        

        public RawlerBase SourceTree { get; set; }

        /// <summary>
        /// 実行する
        /// </summary>
        /// <param name="runChildren"></param>
        public override void Run(bool runChildren)
        {
            if (SourceTree != null)
            {
                if ((createDataOnceFlag == false) || createDataOnce == false)
                {
                    SourceTree.SetParent();
                    SourceTree.SetParent(this);
                    SourceTree.Run();
                    createDataOnceFlag = true;
                }
            }

            RunChildrenForArray(runChildren, texts.GetList());
        }

        /// <summary>
        /// クローンを作る
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<Iterator>(parent);
        }



    }

    public class IteratorSourceAddText : RawlerBase
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<IteratorSourceAddText>(parent);
        }

        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }
        #endregion

        /// <summary>
        /// このクラスでの実行すること。
        /// </summary>
        /// <param name="runChildren"></param>
        public override void Run(bool runChildren)
        {
            var t = GetText();
            var iterator = this.GetAncestorRawler().OfType<Iterator>();
            if (iterator.Any())
            {
                iterator.First().AddText(t);
            }
            else
            {
                ReportManage.ErrReport(this, "上流にIteratorが見つかりません");
            }
        }


        /// <summary>
        /// 子が参照するテキスト。
        /// </summary>
        public override string Text
        {
            get
            {
                return GetText();
            }
        }
    }

    public class IteratorSourceClear : RawlerBase
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<IteratorSourceClear>(parent);
        }

        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }
        #endregion

        /// <summary>
        /// このクラスでの実行すること。
        /// </summary>
        /// <param name="runChildren"></param>
        public override void Run(bool runChildren)
        {
            var t = GetText();
            var iterator = this.GetAncestorRawler().OfType<Iterator>();
            if (iterator.Any())
            {
                iterator.First().TextsClear();
            }
            else
            {
                ReportManage.ErrReport(this, "上流にIteratorが見つかりません");
            }
        }


        /// <summary>
        /// 子が参照するテキスト。
        /// </summary>
        public override string Text
        {
            get
            {
                return GetText();
            }
        }
    }

    public class GetCurrentIterator : RawlerBase
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<GetCurrentIterator>(parent);
        }

        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }
        #endregion

        /// <summary>
        /// このクラスでの実行すること。
        /// </summary>
        /// <param name="runChildren"></param>
        public override void Run(bool runChildren)
        {
           
            var iterator = this.GetAncestorRawler().OfType<Iterator>();
            if (iterator.Any())
            {
                SetText( iterator.First().Text);
            }
            else
            {
                ReportManage.ErrReport(this, "上流にIteratorが見つかりません");
            }
            RunChildren(runChildren);
        }


        /// <summary>
        /// 子が参照するテキスト。
        /// </summary>
        public override string Text
        {
            get
            {
                return this.text;
            }
        }
    }

}
