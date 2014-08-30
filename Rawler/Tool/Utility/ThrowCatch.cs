using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Rawler.Tool
{
    public interface IThrowCatch
    {
         string Name { get; set; }
         void Catch(string text);
    }

    /// <summary>
    /// 上流にテキストを投げる。汎用に上流に投げる処理を行うよう。
    /// </summary>
    public class Throw:RawlerBase
    {
        public string TypeName { get; set; }
        public string ObjectName { get; set; }
        public override void Run(bool runChildren)
        {
            var list = this.GetAncestorRawler().OfType<IThrowCatch>();
            if(string.IsNullOrEmpty(TypeName)==false)
            {
                list = list.Where(n => n.GetType().Name == TypeName);
            }
            if(string.IsNullOrEmpty(ObjectName)==false)
            {
                list = list.Where(n => n.Name == ObjectName);
            }
            if(list.Any())
            {
                list.First().Catch(GetText());
            }
            else
            {
                ReportManage.ErrReport(this, "上流に対象が見つかりません");
            }
            base.Run(runChildren);
        }
    }
}
