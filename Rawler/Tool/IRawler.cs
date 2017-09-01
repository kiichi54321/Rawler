using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Xml;
using System.Xml.Linq;

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
            Init.Run(this);
            if (enable)
            {
                try
                {
                    OnBeginRunEvent();
                    ReportComment();
                    Run(true);
                    Completed();
                    OnEndRunEvent();
                }
                catch(Exception e)
                {
                    ReportManage.ErrReport(this, e.Message);
                }
            }
        }

        InitTreeCollection initTree = new InitTreeCollection();

        public InitTreeCollection Init
        {
            get { return initTree; }
        } 

        /// <summary>
        /// 終了時に実行する。
        /// </summary>
        public virtual void Completed()
        { }

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

        /// <summary>
        /// commentを出力する。
        /// </summary>
        protected void ReportComment()
        {
            if(!string.IsNullOrEmpty( this.commnet))
            {
                ReportManage.Report(this, this.Comment);
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
            SetParent();
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

        /// <summary>
        /// 最初に挿入する。
        /// </summary>
        /// <param name="rawler"></param>
        public void AddFirst(RawlerBase rawler)
        {
            children.Insert(0, rawler);
            rawler.SetParent(this);            
        }

        /// <summary>
        /// 子たちを追加する。返りは、自分自身。
        /// </summary>
        /// <param name="children"></param>
        /// <returns></returns>
        public RawlerBase AddRange(params Rawler.Tool.RawlerBase[] children)
        {
            foreach (var item in children)
            {
                this.AddChildren(item);
            }
            return this;
        }

        /// <summary>
        /// 子を追加する。メソッドチェーン用。
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        public RawlerBase Add( Rawler.Tool.RawlerBase child)
        {
            this.AddChildren(child);
            return child;
        }

        /// <summary>
        /// rootとなるRawlerBaseを取得する
        /// </summary>
        /// <returns></returns>
        public RawlerBase GetRoot()
        {
            return this.GetAncestorRawler().Last();
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            xamlWithoutChildren = null;
            xaml = null;
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
        /// 直近の上流に指定の型があったら取得する。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetUpperRawler<T>()
            where T:RawlerBase 
        {
            T result = null;
            RawlerBase rawler = this.Parent;
            if (rawler != null)
            {
                while (true)
                {
                    if (rawler is T)
                    {
                        result = rawler as T;
                        break;
                    }
                    else if (rawler.Parent == null)
                    {
                        break;
                    }
                    else
                    {
                        rawler = rawler.Parent;
                    }
                }
            }
            return result;
        }

        public RawlerBase GetUpperInterface<T>()
        {
            bool flag = false;
            RawlerBase rawler = this.Parent;
            while (true)
            {
                if (rawler == null) break;
                if (rawler is T)
                {
                    flag = true;
                    break;
                }
                else if (rawler.Parent == null)
                {
                    break;
                }
                else
                {
                    rawler = rawler.Parent;
                }
            }
            if(flag )
            {
                return rawler;
            }
            else
            {
                return null;
            }
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

        /// <summary>
        /// Dispose()
        /// </summary>
        public virtual void Dispose()
        {
            this.text = null;
            this.parent = null;
            this.children = null;
            this.commnet = null;
            this.preTree = null;            
        }

        /// <summary>
        /// XAMLを解釈してオブジェクトを返す。
        /// </summary>
        /// <param name="xaml"></param>
        /// <param name="err"></param>
        /// <returns></returns>
        public static RawlerBase Parse(string xaml,out string err)
        {
            object obj = null;
            err = string.Empty;
            try
            {
                obj = System.Xaml.XamlServices.Parse(xaml);
            }
            catch (Exception ex)
            {
                err = "XAMLの形式がおかしいです" + ex.Message;
            }
            if (obj == null)
            {
                return null;
            }
            if ((obj is Rawler.Tool.RawlerBase) == false)
            {
                err = "キャストできませんでした。XAMLの形式がおかしいです";
                return null;
            }
            else
            {
                 ((RawlerBase)obj).SetParent();
                return (RawlerBase)obj;
            }
            
        }

        /// <summary>
        /// 簡易型XAML生成
        /// </summary>
        /// <returns></returns>
        public string CreateXAML()
        {
            StringBuilder sb = new StringBuilder();
            var type = this.GetType();
            sb.Append("<" + this.ToObjectString() + ">");
            foreach (var item in this.Children)
            {
                sb.Append(item.CreateXAML());
            }
            sb.Append("</" + type.Name + ">");
            return sb.ToString();
        }

        /// <summary>
        /// 高速にXAMLっぽい出力を得る
        /// </summary>
        /// <returns></returns>
        public virtual string ToObjectString()
        {
            StringBuilder sb = new StringBuilder();
            var type = this.GetType();
            sb.Append(type.Name);
            sb.Append(" ");

             System.Reflection.PropertyInfo[] properties = type.GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                //読込み可能なプロパティのみを対象とする。
                //    if (properties[i].CanRead && properties[i].CanWrite)
                if (properties[i].CanWrite)
                {
                    System.Reflection.ParameterInfo[] param =
                             properties[i].GetGetMethod().GetParameters();
                    if ((param != null) && (param.Length > 0))
                    {
                        continue;
                    }

                    //プロパティから値を取得し、その文字列表記を保存する。
                    object v = properties[i].GetValue(this, null);

                    if (v != null )
                    {
                        if(v is RawlerBase)
                        {
                            sb.Append(properties[i].Name);
                            sb.Append("=");
                            sb.Append("'{" + ((RawlerBase)v).CreateXAML() + "}' ");
                        }
                        else if (v.ToString().Length > 0)
                        {
                            sb.Append(properties[i].Name);
                            sb.Append("=");
                            sb.Append("'" + v.ToString() + "' ");
                        }
                    }

                }
            }

            return sb.ToString();
        }

        protected string xaml = null;
        protected string xamlWithoutChildren = null;

        /// <summary>
        /// XAML化する。
        /// </summary>
        /// <returns></returns>
        public string ToXAML()
        {
            if(xaml == null) xaml = System.Xaml.XamlServices.Save(this);
            return xaml;
        }

        /// <summary>
        /// 比較的きれいなXAMLを返す
        /// </summary>
        /// <returns></returns>
        public string ToCleanXAML()
        {
            StringBuilder xaml = new StringBuilder(System.Xaml.XamlServices.Save(this));
            xaml = xaml.Replace("\"{x:Null}\"", "Null").Replace(" Enable=\"True\"", "").Replace(" Comment=\"\"", "");

            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"\w*=Null");
            List<string> list = new List<string>();
            foreach (System.Text.RegularExpressions.Match item in regex.Matches(xaml.ToString()))
            {
                list.Add(item.Value);
            }

            foreach (var item in list.Distinct())
            {
                xaml = xaml.Replace(" " + item, string.Empty);
            }

            return xaml.ToString();
        }

        /// <summary>
        /// 子供なしでXAML化する。
        /// </summary>
        /// <returns></returns>
        public string ToXAMLWithoutChildren()
        {
            if (xamlWithoutChildren == null)
            {
                string xaml = this.ToXAML();
                var xml = XElement.Parse(xaml);

                xml.RemoveNodes();
                xamlWithoutChildren = xml.ToString();
            }
            return xamlWithoutChildren;
        }

        /// <summary>
        /// 子のrawlerをマージする。
        /// </summary>
        public void MargeChildren()
        {
            List<KeyValuePair<string, RawlerBase>> list = new List<KeyValuePair<string, RawlerBase>>();
            foreach (var item in this.Children)
            {
                list.Add(new KeyValuePair<string, RawlerBase>(item.ToObjectString(), item));
            }
            List<RawlerBase> delList = new List<RawlerBase>();
            foreach (var group in list.GroupBy(n=>n.Key))
            {
                var r = group.First();
                foreach (var item in group.Where(n=>n.Value != r.Value))
                {
                    r.Value.AddRange(item.Value.Children.ToArray());
                    delList.Add(item.Value);
                }
            }
            foreach (var item in delList)
            {
                this.Children.Remove(item);
            }
            ///同じ兄弟にDataWriteがあったら、一つだけにする。
            if(this.Children.OfType<DataWrite>().Count()>1)
            {
                foreach (var item in this.Children.OfType<DataWrite>().Skip(1).ToArray())
                {
                    if(item.Children.Any()==false) this.children.Remove(item);                    
                }
            }
            //LastObjectを持つものを末尾にする。
            foreach (RawlerBase item in this.Children.OfType<ILastObject>().ToArray())
            {
                this.Children.Remove(item);
                this.Children.Add(item);
            }

            foreach (var item in this.Children)
            {
                item.MargeChildren();
            }
           
        }
    }

    /// <summary>
    /// 子の集合クラス
    /// </summary>
    [ContentProperty()]
    [Serializable]
    public class RawlerCollection : System.Collections.ObjectModel.ObservableCollection<RawlerBase>
    {
        public void Run(RawlerBase root,string text)
        {
            if (this.Any())
            {
                foreach (var item in this)
                {
                    RawlerBase.GetText(text, item, root);
                }
            }
        }
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
