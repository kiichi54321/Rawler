using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rawler.Tool;

namespace Rawler.Tool
{
    public class RawlerMultiBase : RawlerBase
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            var obj = base.Clone<RawlerMultiBase>(parent);
            return obj;
        }

        public override T Clone<T>(RawlerBase parent)
        {
            var obj = base.Clone<T>(parent);
            if (obj is RawlerMultiBase)
            {
                (obj as RawlerMultiBase).Query = this.Query.Clone();
            }
            return obj;
        }

        protected IEnumerable<string> texts = new List<string>();
        public IEnumerable<string> Texts
        {
            get { return texts; }
        }

        protected void SetTexts(IEnumerable<string> list)
        {
            texts = list;
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
        /// Query
        /// </summary>
        public RawlerQuery Query { get; set; }

        public RawlerBase EmptyTree { get; set; }
        public RawlerBase AnyTree { get; set; }
        public RawlerBase ConvertTree { get; set; }
        /// <summary>
        /// 繰り返しが終わったことを知らせる。
        /// </summary>
        public event EventHandler LoopEndEvent;


        /// <summary>
        /// 子を実行する。
        /// </summary>
        /// <param name="runChildren"></param>
        /// <param name="list"></param>
        protected void RunChildrenForArray(bool runChildren, IEnumerable<string> list)
        {
            if (runChildren)
            {
                if (ConvertTree != null)
                {
                    list = Convert(list);
                }
                if (Query != null)
                {                    
                    list = Query.RunQuery(list,this);
                }
                SetTexts(list);
        
                foreach (var item in list)
                {
                    this.SetText(item);

                    foreach (RawlerBase item2 in children)
                    {
                        item2.Run();
                        if (item2.GetBreakFlag())
                        {
                            break;
                        }
                    }

                }
                if(LoopEndEvent !=null)
                {
                    LoopEndEvent(this, new EventArgs());
                }
                if (EmptyTree != null)
                {
                    if (list.Any() == false)
                    {
                        EmptyTree.SetParent(this);
                        EmptyTree.Run();
                    }
                }
                if (AnyTree != null)
                {
                    if (list.Any() == true)
                    {
                        AnyTree.SetParent(this);
                        AnyTree.Run();
                    }
                }
            }
        }

        /// <summary>
        /// 子を実行する。
        /// </summary>
        /// <param name="runChildren"></param>
        /// <param name="list"></param>
        protected void RunChildrenForArray<T>(bool runChildren, IEnumerable<T> list,Func<T,string> textFunc,Action<T> action)
        {
            if (runChildren)
            {
                List<TextPair<T>> dataList = list.Select(n => new TextPair<T> { Text = textFunc(n), Obj = n }).ToList();
                if (ConvertTree != null)
                {
                    dataList = Convert<T>(dataList,textFunc).ToList();
                }
                if (Query != null)
                {
                    dataList = Query.RunQuery(dataList, this).ToList();
                }
                SetTexts(dataList.Select(n=>n.Text));

                foreach (var item in dataList)
                {
                    this.SetText(item.Text);
                    action(item.Obj);
                    foreach (RawlerBase item2 in children)
                    {
                        item2.Run();
                        if (item2.GetBreakFlag())
                        {
                            break;
                        }
                    }

                }
                if (LoopEndEvent != null)
                {
                    LoopEndEvent(this, new EventArgs());
                }
                if (EmptyTree != null)
                {
                    if (list.Any() == false)
                    {
                        EmptyTree.SetParent(this);
                        EmptyTree.Run();
                    }
                }
                if (AnyTree != null)
                {
                    if (list.Any() == true)
                    {
                        AnyTree.SetParent(this);
                        AnyTree.Run();
                    }
                }
            }
        }



        private IEnumerable<string> Convert(IEnumerable<string> list)
        {
            if (ConvertTree != null)
            {
                ConvertTree.SetParent(this.Parent);
                foreach (var item in list)
                {
                    yield return RawlerBase.GetText(item, ConvertTree, this.Parent);
                }
            }
            else
            {
                foreach (var item in list)
                {
                    yield return item;
                }

            }
        }

        private IEnumerable<TextPair<T>> Convert<T>(IEnumerable<TextPair<T>> list,Func<T,string> textFunc)
        {
            if (ConvertTree != null)
            {
                ConvertTree.SetParent(this.Parent);
                foreach (var item in list)
                {
                    item.Text = RawlerBase.GetText(item.Text, ConvertTree, this.Parent);
                    yield return item;
                    //yield return RawlerBase.GetText(item, ConvertTree, this.Parent);
                }
            }
            else
            {
                foreach (var item in list)
                {
                    yield return item;
                }
            }
        }

        public class TextPair<T>
        {
            public T Obj { get; set; }
            public string Text { get; set; }
        }

        /// <summary>
        /// このクラスでの実行すること。
        /// </summary>
        /// <param name="runChildren"></param>
        public override void Run(bool runChildren)
        {
            base.Run(runChildren);
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


    }
}
