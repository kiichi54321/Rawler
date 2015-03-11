using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rawler.Tool;

namespace Rawler.Tool
{
    /// <summary>
    /// ごみ
    /// </summary>
    class DataGrouping : RawlerBase
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<DataGrouping>(parent);
        }

        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }
        #endregion


        Dictionary<string,Dictionary<string,List<string>>> dic = new Dictionary<string,Dictionary<string,List<string>>>();

        /// <summary>
        /// このクラスでの実行すること。
        /// </summary>
        /// <param name="runChildren"></param>
        public override void Run(bool runChildren)
        {
            base.Run(runChildren);

            Save();
        }

        string SaveFileName { get; set; }

        public void Save()
        {
            var file = System.IO.File.CreateText(SaveFileName);
            foreach (var item in dic.OrderBy(n=>n.Key))
            {
                file.Write(item.Key + "\t");

            }
        }

        public void AddData(string group, string key, string value)
        {
            if (dic.ContainsKey(group))
            {
                if (dic[group].ContainsKey(key))
                {
                    dic[group][key].Add(value);
                }
                else
                {
                    dic[group].Add(key, new List<string>() { value });
                }                       
            }
            else
            {
                dic.Add(group, new Dictionary<string, List<string>>());
                dic[group].Add(key, new List<string>() { value });
            }
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
