using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rawler.Tool;

namespace Rawler.Tool
{
    public class SetFunctionTree : RawlerBase
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<SetFunctionTree>(parent);
        }

        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }
        #endregion

        string key = string.Empty;
        public string Key { get { return key; } set { key = value; TreeFunctionManage.SetTree(Key, this); } }
        bool useClone = false;

        public bool UseClone
        {
            get { return useClone; }
            set { useClone = value; }
        }
        /// <summary>
        /// このクラスでの実行すること。
        /// </summary>
        /// <param name="runChildren"></param>
        public override void Run(bool runChildren)
        {
//            TreeFunctionManage.SetTree(Key, this);
//            base.Run(runChildren);
        }

        public void RunTree()
        {
            base.Run(true);
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

    public class RunFunctionTree : RawlerBase
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<RunFunctionTree>(parent);
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
            var tree = TreeFunctionManage.GetTree(Key);
            if (tree == null)
            {
                ReportManage.ErrReport(this, "FunctionTree:「"+Key+"」が見つかりません。");
                return;
            }
            if (tree.UseClone)
            {
                tree = tree.Clone() as SetFunctionTree;
            }
            tree.SetParent(this);
            tree.RunTree();

            base.Run(runChildren);
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

    public static class TreeFunctionManage
    {
        static Dictionary<string, SetFunctionTree> treeDic = new Dictionary<string, SetFunctionTree>();

        public static void SetTree(string key, SetFunctionTree r)
        {
            if (treeDic.ContainsKey(key))
            {
                treeDic[key] = r;
            }
            else
            {
                treeDic.Add(key, r);
            }
        }

        public static SetFunctionTree GetTree(string key)
        {
            if (treeDic.ContainsKey(key))
            {
                return treeDic[key];
            }
            else
            {
                return null;
            }
        }
    }
}
