using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rawler.Core;

namespace Rawler
{
    public class UseCounter : RawlerBase
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<UseCounter>(parent);
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
            if (this.Parent != null) SetText(this.Parent.Text);
            base.Run(runChildren);
        }

        Dictionary<string, int> dic = new Dictionary<string, int>();

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

        public void Clear(string key)
        {
            if (key == null)
            {
                dic = new Dictionary<string, int>();
            }
            else
            {

                if (dic.ContainsKey(key))
                {
                    dic.Remove(key);
                }
            }
        }

        public void Add(string key, int count)
        {
            if (dic.ContainsKey(key))
            {
                dic[key] += count;
            }
            else
            {
                dic.Add(key, count);
            }

        }

        public int GetCount(string key)
        {
            if (dic.ContainsKey(key))
            {
                return dic[key];
            }
            else
            {
                return 0;
            }
        }
    }

    public class AddCounter : RawlerBase
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<AddCounter>(parent);
        }

        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }
        #endregion

        int count = 1;

        public int Count
        {
            get { return count; }
            set { count = value; }
        }

        public string Key { get; set; }

        /// <summary>
        /// このクラスでの実行すること。
        /// </summary>
        /// <param name="runChildren"></param>
        public override void Run(bool runChildren)
        {
            if (this.GetAncestorRawler().OfType<UseCounter>().Any() == false)
            {
                ReportManage.ErrReport(this, "上流にUseCounterがありません");
            }
            else
            {
                this.GetAncestorRawler().OfType<UseCounter>().First().Add(this.Key, Count);
            }
            base.Run(runChildren);
        }
    }

    public class ClearCounter : RawlerBase
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<ClearCounter>(parent);
        }

        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }
        #endregion


        public string Key { get; set; }

        /// <summary>
        /// このクラスでの実行すること。
        /// </summary>
        /// <param name="runChildren"></param>
        public override void Run(bool runChildren)
        {
            if (this.GetAncestorRawler().OfType<UseCounter>().Any() == false)
            {
                ReportManage.ErrReport(this, "上流にUseCounterがありません");
            }
            else
            {
                this.GetAncestorRawler().OfType<UseCounter>().First().Clear(Key);
            }
            base.Run(runChildren);
        }
    }

    public class GetCounter : RawlerBase
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<GetCounter>(parent);
        }

        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }
        #endregion


        public string Key { get; set; }

        /// <summary>
        /// このクラスでの実行すること。
        /// </summary>
        /// <param name="runChildren"></param>
        public override void Run(bool runChildren)
        {
            if (this.GetAncestorRawler().OfType<UseCounter>().Any() == false)
            {
                ReportManage.ErrReport(this, "上流にUseCounterがありません");
            }
            else
            {
                SetText(this.GetAncestorRawler().OfType<UseCounter>().First().GetCount(Key).ToString());
            }
            base.Run(runChildren);
        }
    }
}
