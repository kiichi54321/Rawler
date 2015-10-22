using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rawler.Tool
{
    /// <summary>
    /// KeyでGroup化されたものにする。
    /// </summary>
    public class GroupBy : RawlerBase
    {
        Dictionary<string, List<string>> dic = new Dictionary<string, List<string>>();
        bool connectEndEvent = false;

        public string Key { get; set; }

        public override void Run(bool runChildren)
        {          
            if (connectEndEvent == false)
            {
                var parent = (ILoopEnd)this.GetUpperInterface<ILoopEnd>();
                parent.LoopEndEvent += Parent_EndRunEvent;
                connectEndEvent = true;
            }

            var key = Key.Convert(this);

            if(dic.ContainsKey(key))
            {
                dic[key].Add(GetText());
            }
            else
            {
                dic.Add(key, new List<string>() { GetText() });
            }
        }


        /// <summary>
        /// 蓄積されたものを実行するところです。
        /// </summary>
        /// <param name="dic"></param>
        public virtual void RunGroupBy(Dictionary<string,List<string>> dic)
        {
            foreach (var item in dic)
            {
                currentKeyValue = item;
                SetText(item.Key);
                RunChildren(true);
            }
        }

        KeyValuePair<string, List<string>> currentKeyValue;

        public KeyValuePair<string,List<string>> GetCurrentKeyValue()
        {
            return currentKeyValue;
        }

        private void Parent_EndRunEvent(object sender, EventArgs e)
        {
            RunGroupBy(dic);
            dic.Clear();
        }
    }

    /// <summary>
    /// Groupbyの子で、Keyを受け取る。
    /// </summary>
    public class GetGroupbyKey:RawlerBase
    {
        public override void Run(bool runChildren)
        {
            var g = this.GetUpperRawler<GroupBy>();
            if(g == null)
            {
                ReportManage.ErrUpperNotFound<GroupBy>(this);
                return;
            }
            SetText(g.GetCurrentKeyValue().Key);
            base.Run(runChildren);
        }
    }

    public class GetGroupbyValue:RawlerMultiBase
    {
        public override void Run(bool runChildren)
        {
            var g = this.GetUpperRawler<GroupBy>();
            if (g == null)
            {
                ReportManage.ErrUpperNotFound<GroupBy>(this);
                return;
            }
            RunChildrenForArray(runChildren, g.GetCurrentKeyValue().Value);
        }
    }

    public class GroupByCount : RawlerBase
    {
        Dictionary<string, int> dic = new Dictionary<string, int>();
        bool connectEndEvent = false;

        public string Key { get; set; }

        public override void Run(bool runChildren)
        {
            if (connectEndEvent == false)
            {
                var parent = (ILoopEnd)this.GetUpperInterface<ILoopEnd>();
                parent.LoopEndEvent += Parent_EndRunEvent;
                connectEndEvent = true;
            }

            var key = Key.Convert(this);

            if (dic.ContainsKey(key))
            {
                dic[key] = dic[key] + 1;
            }
            else
            {
                dic.Add(key, 1);
            }
        }


        /// <summary>
        /// 蓄積されたものを実行するところです。
        /// </summary>
        /// <param name="dic"></param>
        public virtual void RunGroupBy(Dictionary<string, int> dic)
        {
            foreach (var item in dic)
            {
                currentKeyValue = item;
                SetText(item.Key+"\t"+item.Value);
                RunChildren(true);
            }
        }

        KeyValuePair<string, int> currentKeyValue;

        public KeyValuePair<string, int> GetCurrentKeyValue()
        {
            return currentKeyValue;
        }

        private void Parent_EndRunEvent(object sender, EventArgs e)
        {
            RunGroupBy(dic);
            dic.Clear();
        }
    }
}
