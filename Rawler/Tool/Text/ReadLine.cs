using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rawler.Tool
{
    public class ReadLines:RawlerMultiBase
    {
        public override void Run(bool runChildren)
        {
            var t = GetText();
            if (t.Length > 0)
            {
                System.IO.StringReader sr = new System.IO.StringReader(t);
                List<string> list = new List<string>();
                while (sr.Peek() > -1)
                {
                    string line = sr.ReadLine();
                    if (line.Length > 0)
                    {
                        list.Add(line);
                    }
                }
                sr.Close();
                base.RunChildrenForArray(runChildren, list);
            }
        }


        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<ReadLines>(parent);
        }

        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }
        #endregion
    }
}
