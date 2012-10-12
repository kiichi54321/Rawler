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

        /// <summary>
        /// 子を実行する。
        /// </summary>
        /// <param name="runChildren"></param>
        /// <param name="list"></param>
        protected void RunChildrenForArray(bool runChildren, IEnumerable<string> list)
        {

            if (runChildren)
            {
                
                if (Query != null)
                {
                    list = Query.RunQuery(list);
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
                if (EmptyTree != null)
                {
                    if (list.Any() == false)
                    {
                        EmptyTree.SetParent(this);
                        EmptyTree.Run();
                    }
                }
            }
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
