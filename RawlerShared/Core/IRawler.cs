using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Rawler.Core
{
    /// <summary>
    /// Rawlerクラスのインタフェース
    /// </summary>
    public interface IRawler
    {
        RawlerBase Parent { get;  }
        RawlerCollection Children { get; }
   //     bool IsMulti { get; set; }
        string Text { get; }
   //     List<string> Texts { get; set; }
        void Run();
        void Run(bool runChildren);
        void AddChildren(RawlerBase rawler);
        RawlerBase Clone();
        RawlerBase Clone(RawlerBase parent);
    }


    /// <summary>
    /// このオブジェクトは、末尾に来ないとダメ。
    /// </summary>
    public interface ILastObject
    {
        RawlerBase Parent { get; }
    }

    /// <summary>
    /// 繰り返しを行うクラスであることを示すインターフェース
    /// </summary>
    public interface ILoopEnd
    {
        /// <summary>
        /// 繰り返しが終了したことを示すイベント
        /// </summary>
        event EventHandler LoopEndEvent;
    }


    public interface Imulti
    {
        bool IsMulti { get; set; }
        List<string> Texts { get;  }
    }







    //public interface ITagReader : IRawler
    //{
    //    string InnerText { get; set; }
    //    string OuterText { get; set; }
    //    List<string> InnerTexts { get; set; }
    //    List<string> OuterTexts { get; set; }
    //    bool UseInner { get; set; }
    //}
}
