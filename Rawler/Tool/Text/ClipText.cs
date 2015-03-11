using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;

namespace Rawler.Tool
{
    

    /// <summary>
    /// 指定した範囲でテキストを切り取る
    /// </summary>
    [Serializable]
    [ContentProperty("Children")]
    public class ClipText : RawlerMultiBase
    {
        /// <summary>
        /// 指定した範囲でテキストを切り取る
        /// </summary>
        public ClipText()
            : base()
        {
            init();
        }

        /// <summary>
        /// 指定した範囲でテキストを切り取る
        /// </summary>
        /// <param name="StartClip">開始文字</param>
        /// <param name="EndClip">終了文字</param>
        public ClipText(string StartClip, string EndClip)
            : base()
        {
            this.StartClip = StartClip;
            this.EndClip = EndClip;
            init();

        }

        ///// <summary>
        ///// 指定した範囲でテキストを切り取る
        ///// </summary>
        ///// <param name="StartClip">開始文字</param>
        ///// <param name="EndClip">終了文字</param>
        ///// <param name="isMulti">複数？</param>
        //public ClipText(string StartClip, string EndClip,bool isMulti)
        //    : base()
        //{
        //    this.StartClip = StartClip;
        //    this.EndClip = EndClip;
        //    this.IsMulti = isMulti;
        //    init();
        //}

        private void init()
        {
  
        }
        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }


        /// <summary>
        /// 開始文字列
        /// </summary>
        public string StartClip { get; set; }
        /// <summary>
        /// 終了文字列
        /// </summary>
        public string EndClip { get; set; }
        private bool emptyReport = false;
        /// <summary>
        /// 空文字列の時、報告するか？
        /// </summary>
        public bool EmptyReport
        {
            get { return emptyReport; }
            set { emptyReport = value; }
        }
        #region interface

        //public new void Run()
        //{
        //    Run(true);
        //}

        /// <summary>
        /// 実行
        /// </summary>
        /// <param name="runChildren"></param>
        public override void Run(bool runChildren)
        {
            if (this.Parent != null && this.Parent.Text !=null)
            {
              //  if (IsMulti)
                {
                    if (this.UseInner)
                    {
                        this.texts = RawlerLib.Web.GetTagContentList(GetText(), StartClip, EndClip);
                    }
                    else
                    {
                        this.texts = RawlerLib.Web.GetTagContentList(GetText(), StartClip, EndClip, true);
                    }
                    if (emptyReport && this.texts.Count() == 0)
                    {
                        ReportManage.ErrReport(this, "対象が見つかりませんでした");
                    }

                    RunChildrenForArray(runChildren, this.texts);
                }
                //else
                //{
                //    if (this.UseInner)
                //    {
                //        this.text = RawlerLib.Web.GetTagContent(GetText(), StartClip, EndClip);
                //    }
                //    else
                //    {
                //        this.text = RawlerLib.Web.GetTagContent(GetText(), StartClip, EndClip, true);
                //    }
                //    if (emptyReport && this.text.Length == 0)
                //    {
                //        ReportManage.ErrReport(this, "対象が見つかりませんでした");
                //    }
                //    this.RunChildren(runChildren);

                //}
            }
        }


        //string interText = string.Empty;
        //public string InnerText
        //{
        //    get
        //    {
        //        if (Parent != null)
        //        {
        //            interText = MyLib.Web.GetTagContent(Parent.Text, StartClip, EndClip);
        //        }
        //        return interText;
        //    }
        //    set
        //    {
        //        interText = value;
        //    }
        //}

        //string outerText = string.Empty;
        //public string OuterText
        //{
        //    get
        //    {
        //        if (Parent != null)
        //        {
        //            outerText = MyLib.Web.GetTagContent(Parent.Text, StartClip, EndClip,true);
        //        }
        //        return outerText;
        //    }
        //    set
        //    {
        //        outerText = value;
        //    }
        //}



        //bool isMulti = false;
        ///// <summary>
        ///// 複数かどうか。
        ///// </summary>
        //public bool IsMulti
        //{
        //    get
        //    {
        //        return isMulti;
        //    }
        //    set
        //    {
        //        isMulti = value;
        //    }
        //}

        //List<string> interTextList = new List<string>();
        //public List<string> InnerTexts
        //{
        //    get
        //    {
        //        if (Parent != null)
        //        {
        //            interTextList = MyLib.Web.GetTagContentList(Parent.Text, StartClip, EndClip);
        //        }

        //        return interTextList;
        //    }
        //    set
        //    {
        //        interTextList = value;
        //    }
        //}
        //List<string> outerTextList = new List<string>();
        //public List<string> OuterTexts
        //{
        //    get
        //    {
        //        if (Parent != null)
        //        {
        //            outerTextList = MyLib.Web.GetTagContentList(Parent.Text, StartClip, EndClip,true);
        //        }
        //        return outerTextList;
        //    }
        //    set
        //    {
        //        outerTextList = value;
        //    }
        //}
        #endregion

        bool userInner = true;
        /// <summary>
        /// 切り取った文字の内部を使う
        /// </summary>
        public bool UseInner
        {
            get
            {
                return userInner;
            }
            set
            {
                userInner = value;
            }
        }

       ///// <summary>
       ///// このオブジェクトのテキスト。
       ///// </summary>
       // public override string Text
       // {
       //     get
       //     {
       //         if (text == string.Empty && doClone == false)
       //         {
       //             Run(false);
       //         }
       //         return text;
       //     }

       // }

       // List<string> texts = new List<string>();
       // /// <summary>
       // /// このオブジェクトのテキスト（複数）
       // /// </summary>
       // public List<string> Texts
       // {
       //     get
       //     {
       //         return texts;
       //     }

       // }

        bool doClone = false;
        /// <summary>
        /// Cloneを作る。
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<ClipText>(parent);
        }
    }
}
