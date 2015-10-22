using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rawler.Tool;

namespace Rawler.Tool
{
    public class HashSet:RawlerBase
    {
        HashSet<string> hash = new HashSet<string>();

        public override void Run(bool runChildren)
        {
            SetText(GetText());
            base.Run(runChildren);
        }

        public bool CheckContains(string text)
        {
            return hash.Contains(text);
        }

        public void AddHash(string text)
        {
            hash.Add(text);
        }

        public void ClearHash()
        {
            hash.Clear();
        }

    }
    /// <summary>
    /// Hashを調べてResultと一致したら以下を実行する。
    /// </summary>
    public class HashContains:RawlerBase
    {
        public bool Result { get; set; } = true;
        HashSet hash;
        public override void Run(bool runChildren)
        {
            SetText(GetText());
            if(hash == null)
            {
                hash = this.GetUpperRawler<HashSet>();
                if(hash == null)
                {
                    ReportManage.ErrUpperNotFound<HashSet>(this);
                    return;
                }
            }
            if(hash.CheckContains(GetText())==Result)
            {
                base.Run(runChildren);
            }
        }
    }

    public class HashAdd:RawlerBase
    {
        HashSet hash;

        public override void Run(bool runChildren)
        {
            if (hash == null)
            {
                hash = this.GetUpperRawler<HashSet>();
                if (hash == null)
                {
                    ReportManage.ErrUpperNotFound<HashSet>(this);
                    return;
                }
            }
            hash.AddHash(GetText());
            base.Run(runChildren);
        }
    }
}
