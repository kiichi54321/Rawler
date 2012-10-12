using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rawler.Tool
{
    /// <summary>
    /// Keyに登録していないものなら、子に続きます。
    /// </summary>
    public class Distinct:RawlerBase
    {
        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }

        /// <summary>
        /// このオブジェクトのテキスト
        /// </summary>
        public override string Text
        {
            get
            {
                return GetText();
            }

        }

        private string key = string.Empty;

        /// <summary>
        /// Keyです。デフォルトは空文字。指定すると複数の縛りがやれます。
        /// </summary>
        public string Key
        {
            get { return key; }
            set { key = value; }
        }

        private DistinctMode mode = DistinctMode.add;

        /// <summary>
        /// 通常はadd　リストを空にしたいときはClearを
        /// </summary>
        public DistinctMode Mode
        {
            get { return mode; }
            set { mode = value; }
        }

        /// <summary>
        /// 実行
        /// </summary>
        /// <param name="runChildren"></param>
        public override void Run(bool runChildren)
        {
            if (mode == DistinctMode.add)
            {
                if (DistinctManage.Add(key, this.Parent.Text))
                {
                    this.RunChildren(runChildren);
                }
            }
            else if (mode == DistinctMode.clear)
            {
                DistinctManage.Clear(key);
            }

        }

        /// <summary>
        /// クローンを作る。
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            var clone = new Distinct();
            RawlerLib.ObjectLib.FildCopy(this, clone);
            clone.SetParent(parent);
            this.CloneEvent(clone);
            clone.children.Clear();
            foreach (var item in this.Children)
            {
                var child = item.Clone(clone);
                clone.AddChildren(child);
            }
            return clone;
        }
    }

    /// <summary>
    /// Distinctの動作を決めます。
    /// </summary>
    public enum DistinctMode
    {
        add,clear
    }

    /// <summary>
    /// Distinctの管理を行います。静的クラスです。
    /// </summary>
    public static class DistinctManage
    {
        static Dictionary<string, HashSet<string>> dic = new Dictionary<string, HashSet<string>>();

        public static bool Contain(string key, string val)
        {
            if (dic.ContainsKey(key))
            {
                return dic[key].Contains(val);
            }
            else
            {
                return false;
            }
        }

        public static bool Add(string key,string val)
        {
            if (dic.ContainsKey(key))
            {
                return dic[key].Add(val);
            }
            else
            {
                dic.Add(key, new HashSet<string>());
                return dic[key].Add(val);
            }

        }

        public static bool Clear(string key)
        {
            if (dic.ContainsKey(key))
            {
                dic[key].Clear();
                return true;
            }
            else
            {
                return false;
            }

        }
    }
}
