using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rawler.Tool;

namespace Rawler.Tool
{
    public class Switch : RawlerBase
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<Switch>(parent);
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

            string switchValue = GetSwitchValue();
            bool flag = true;
            foreach (var item in this.Children)
            {
                if (item is CaseBase)
                {
                    var node = item as CaseBase;
                    if (node.Check(switchValue))
                    {
                        node.Run();
                        if (node.IsBreak)
                        {
                            flag = false;
                            break;
                        }
                    }
                }
            }
            if (flag)
            {
                if (OutsideTree != null)
                {
                    OutsideTree.SetParent();
                    OutsideTree.SetParent(this);

                    OutsideTree.Run();
                }

            }

         //   base.Run(runChildren);
        }



        protected string GetSwitchValue()
        {
            if (SwitchValueTree != null)
            {
                SwitchValueTree.SetParent();
                return  RawlerBase.GetText(this.Parent.Text, SwitchValueTree, this.Parent);
            }
            else
            {
                return GetText();
            }
        }

        public RawlerBase SwitchValueTree { get; set; }
        public RawlerBase OutsideTree { get; set; }

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

    public class CaseBase : RawlerBase
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<CaseBase>(parent);
        }

        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }
        #endregion

        bool isBreak = true;

        public bool IsBreak
        {
            get { return isBreak; }
            set { isBreak = value; }
        }

        public virtual bool Check(string txt)
        {
            return false;
        }


        /// <summary>
        /// このクラスでの実行すること。
        /// </summary>
        /// <param name="runChildren"></param>
        public override void Run(bool runChildren)
        {
            base.Run(runChildren);
        }


        public RawlerBase SwitchValueTree { get; set; }

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
}
