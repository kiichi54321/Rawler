using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;

namespace Rawler.Tool
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

    public interface IDataWrite
    {
        string Attribute { get; set; }
        DataWriteType WriteType { get; set; }
    }


    public interface Imulti
    {
        bool IsMulti { get; set; }
        List<string> Texts { get;  }
    }

    /// <summary>
    /// Rawlerクラスの基底クラス。
    /// </summary>
    [ContentProperty("Children")]
    [RuntimeNameProperty("Name")]
    [Serializable]
    public class RawlerBase : IRawler,System.ComponentModel.INotifyPropertyChanged,IDisposable
    {
//        private string objectName = "RawlerBase";

        public virtual string ObjectName
        {
            get { return this.GetType().Name; }
        }


        /// <summary>
        /// Text の中身のtext
        /// </summary>
        protected string text = string.Empty;

        /// <summary>
        /// Text の中身のtext
        /// </summary>
        public virtual string Text
        {
            get { return text; }
        }

        private RawlerBase preTree = null;

        public RawlerBase PreTree
        {
            get { return preTree; }
            set { preTree = value; }
        }

        
        public static string GetText(string text, RawlerBase rawler)
        {
            Document doc = new Document();
            doc.SetText(text);
            rawler.SetParent(doc);
            var last = rawler.GetDescendantRawler().Last();
            rawler.Run();
            return last.Text;
        }

        public static string GetText(string text, RawlerBase rawler,RawlerBase parent)
        {
            Document doc = new Document();
            doc.SetText(text);
            if (parent != null)
            {
                doc.SetParent(parent);
            }
            rawler.SetParent(doc);
            rawler.SetParent();
            var last = rawler.GetDescendantRawler().Last();
            rawler.Run();
            return last.Text;
        }


        /// <summary>
        /// PreTreeを通したあとのText
        /// </summary>
        /// <returns></returns>
        protected string GetText()
        {
            if (this.Parent != null)
            {
                if (preTree != null)
                {
                    preTree.SetParent();
                    return RawlerBase.GetText(this.Parent.Text, PreTree,this.Parent);
                }
                else
                {
                    return this.Parent.Text;
                }
            }
            else
            {
                return this.text;
            }


        }

        
        /// <summary>
        /// 指定したテキストをPreTreeにかける。
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        protected string GetText(string preText)
        {
            if (preTree != null)
            {
                return RawlerBase.GetText(preText, PreTree);
            }
            else
            {
                return preText;
            }
        }


        protected string commnet = string.Empty;

        /// <summary>
        /// 次の兄弟要素に進むのをやめるかのフラグ。
        /// </summary>
        protected bool breakFlag = false;

        public bool GetBreakFlag()
        {
            bool flag = breakFlag;
            breakFlag = false;
            return flag; 
        }

        /// <summary>
        /// コメント。
        /// </summary>
        public string Comment
        {
            get
            {
                return commnet;
            }
            set
            {
                commnet = value;
                OnPropertyChanged("Comment");
            }
        }

        /// <summary>
        /// textをセットする。
        /// </summary>
        /// <param name="text"></param>
        public void SetText(string text)
        {
            this.text = text;
            OnPropertyChanged("Text");
        }

        /// <summary>
        /// 実行。
        /// </summary>
        public void Run()
        {
            if (enable)
            {
                OnBeginRunEvent();
                Run(true);
                OnEndRunEvent();
            }
        }

        bool enable = true;

        /// <summary>
        /// Falseの時、このオブジェクトは実行されない。
        /// </summary>
        public bool Enable
        {
            get { return enable; }
            set { enable = value; }
        }

        /// <summary>
        /// 実行
        /// </summary>
        /// <param name="runChildren"></param>
        public virtual void Run(bool runChildren)
        {

            RunChildren(runChildren);
        }

        /// <summary>
        /// 子を実行する。
        /// </summary>
        /// <param name="runChildren"></param>
        protected void RunChildren(bool runChildren)
        {
            
            if (runChildren)
            {
                foreach (RawlerBase item in children)
                {
                    item.Run();
                    if (item.GetBreakFlag())
                    {
                        break;
                    }
                }
            }

        }



        public void SetParent()
        {
            foreach (var item in this.children)
            {
                item.parent = this;
                item.SetParent();
            }
        }


        /// <summary>
        /// 実行を始める前のイベント起動
        /// </summary>
        protected void OnBeginRunEvent()
        {
            if (BeginRunEvent != null)
            {
                BeginRunEvent(this, new EventArgs());
            }
        }

        /// <summary>
        /// 実行が終わった後のイベント起動
        /// </summary>
        protected void OnEndRunEvent()
        {
            if (EndRunEvent != null)
            {
                EndRunEvent(this, new EventArgs());
            }
        }

        /// <summary>
        /// クローンを作る
        /// </summary>
        /// <returns></returns>
        public RawlerBase Clone()
        {
            return Clone(this.Parent);
        }


        /// <summary>
        /// クローンを作る
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public virtual RawlerBase Clone(RawlerBase parent)
        {
            RawlerBase clone = new RawlerBase();
            RawlerLib.ObjectLib.FildCopy(this, clone);
            clone.SetParent(parent);
            CloneEvent(clone);
            clone.Children.Clear();
            foreach (var item in this.Children)
            {
                var child = item.Clone(clone);
                clone.AddChildren(child);
            }
            return clone;
        }

        /// <summary>
        /// クローンを作る
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent"></param>
        /// <returns></returns>
        public virtual T Clone<T>(RawlerBase parent)
            where T : RawlerBase, new()
        {
            T clone = new T();
            RawlerLib.ObjectLib.FildCopy(this as T, clone);
            clone.SetParent(parent);
            CloneEvent(clone);
            clone.Children.Clear();

            foreach (var item in clone.GetType().GetProperties())
            {
                if (item.PropertyType.IsSubclassOf(typeof(RawlerBase)))
                {
                    var r = item.GetValue(this as T, null) as RawlerBase;
                    item.SetValue(clone, r.Clone(), null);
                }
                //else if(item.PropertyType == typeof(RawlerCollection))
                //{
                //    var col = item.GetValue(clone, null) as RawlerCollection;
                //    col.Clear();
                //    foreach (var item2 in item.GetValue(this as T,null) as RawlerCollection)
                //    {
                //        col.Add(item2);
                //    }
                //}
            }

            foreach (var item in this.Children)
            {
                var child = item.Clone(clone);
                clone.AddChildren(child);
            }
            return clone;
        }


        /// <summary>
        /// イベントのコピー
        /// </summary>
        /// <param name="rawler"></param>
        protected virtual void CloneEvent(RawlerBase rawler)
        {
            rawler.BeginRunEvent = this.BeginRunEvent;
            rawler.EndRunEvent = this.EndRunEvent;
        }

        /// <summary>
        /// 実行前に起すイベント
        /// </summary>
        public event EventHandler BeginRunEvent;
        /// <summary>
        /// 実行後に起すイベント
        /// </summary>
        public event EventHandler EndRunEvent;
 
        /// <summary>
        /// 親Rawlerクラス
        /// </summary>
        protected RawlerBase parent;
        /// <summary>
        /// 親Rawlerクラス
        /// </summary>
        public RawlerBase Parent
        {
            get
            {
                return parent;
            }
            //set
            //{
            //    parent = value;
            //}

        }

        public void SetParent(RawlerBase rawler)
        {
            parent = rawler;
        }

        /// <summary>
        /// 子Rawlerクラス
        /// </summary>
        protected RawlerCollection children = new RawlerCollection();
        /// <summary>
        /// 子Rawlerクラス
        /// </summary>
        public RawlerCollection Children
        {
            get { return children; }
        }

        /// <summary>
        /// 子を追加する。
        /// </summary>
        /// <param name="rawler"></param>
        public void AddChildren(RawlerBase rawler)
        {
            children.Add(rawler);
            rawler.SetParent(this);
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(name));
            }
        }

        int id = 0;
        public int GetId() { return id; }
        public void SetId(int value) { id = value; }

        public string Name { get; set; }


        /// <summary>
        /// 自身を含む、すべての子、孫を取得する。
        /// </summary>
        /// <returns></returns>
        public IEnumerable<RawlerBase> GetDescendantRawler()
        {
            List<RawlerBase> list = new List<RawlerBase>();
            Queue<RawlerBase> queueRawler = new Queue<RawlerBase>();
            queueRawler.Enqueue(this);
            while (queueRawler.Count > 0)
            {
                RawlerBase tmp = queueRawler.Dequeue();
                list.Add(tmp);
                foreach (var item in tmp.Children)
                {
                    queueRawler.Enqueue(item);
                }
            }
            return list;
        }

        /// <summary>
        /// 自身を含む、すべての祖先を取得する。
        /// </summary>
        /// <returns></returns>
        public IEnumerable<RawlerBase> GetAncestorRawler()
        {
            List<RawlerBase> list = new List<RawlerBase>();
            Queue<RawlerBase> queueRawler = new Queue<RawlerBase>();
            queueRawler.Enqueue(this);
            while (queueRawler.Count > 0)
            {
                RawlerBase tmp = queueRawler.Dequeue();
                list.Add(tmp);
                if (tmp.Parent != null)
                {
                    queueRawler.Enqueue(tmp.Parent);
                }
            }
            return list;
        }

        /// <summary>
        /// 自身を含む、繋がっているすべてを取得する。
        /// </summary>
        /// <returns></returns>
        public ICollection<RawlerBase> GetConectAllRawler()
        {
            List<RawlerBase> list = new List<RawlerBase>();
            foreach (var item in GetAncestorRawler())
            {
                list.AddRange(item.GetDescendantRawler());
            }
            List<RawlerBase> list2 = new List<RawlerBase>(list.Distinct());
            return list2;
        }

        public virtual void Dispose()
        {
            this.text = null;
            this.parent = null;
            this.children = null;
            this.commnet = null;
            this.preTree = null;            
        }
    }

    /// <summary>
    /// 子の集合クラス
    /// </summary>
    [ContentProperty("Items")]
    [Serializable]
    public class RawlerCollection : System.Collections.ObjectModel.ObservableCollection<RawlerBase>
    {

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
