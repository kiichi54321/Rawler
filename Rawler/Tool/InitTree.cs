using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;

namespace Rawler.Tool
{
    /// <summary>
    /// 初期化するプロパティ
    /// </summary>
    [ContentProperty("Child")]
    public class InitProperty
    {

        public string PropertyName { get; set; }
        public RawlerBase Child { get; set; }
        public void Run(RawlerBase parent)
        {
            try
            {
                var type = parent.GetType();
                var field = type.GetProperty(PropertyName);
                var text = RawlerBase.GetText(string.Empty, Child, parent);
                if (field.PropertyType == typeof(string))
                {
                    field.SetValue(parent, text,null);
                }
                else if (field.PropertyType == typeof(int))
                {
                    int num;
                    if (int.TryParse(text, out num))
                    {
                        field.SetValue(parent, num,null);
                    }
                    else
                    {
                        ReportManage.ErrReport(parent, "InitTreeで" + PropertyName + "の値をint型に変換に失敗しました");
                    }
                }
                else if (field.PropertyType == typeof(double))
                {
                    double num;
                    if (double.TryParse(text, out num))
                    {
                        field.SetValue(parent, num,null);
                    }
                    else
                    {
                        ReportManage.ErrReport(parent, "InitTreeで" + PropertyName + "の値をdouble型に変換に失敗しました");
                    }
                }
                else if(field.PropertyType == typeof(bool))
                {
                    if(text.ToLower()=="true")
                    {
                        field.SetValue(parent, true, null);
                    }
                    else if(text.ToLower()=="false")
                    {
                        field.SetValue(parent, false, null);                       
                    }
                    else
                    {
                        ReportManage.ErrReport(parent, "InitTreeで" + PropertyName + "の値をbool型に変換に失敗しました。Valueは"+text);
                    }
                }
            }
            catch(Exception ex)
            {
                ReportManage.ErrReport(parent, "InitTreeで" + PropertyName + "でエラーが発生しました。" + ex.Message);
            }

        }
    }

    /// <summary>
    /// 初期化の集合
    /// </summary>
    public class InitTreeCollection:List<InitProperty>
    {
        /// <summary>
        /// 初期化実行
        /// </summary>
        /// <param name="root"></param>
        public void Run(RawlerBase root)
        {
            if (this.Count == 0) return;
            foreach (var item in this)
            {
                item.Run(root);
            }
        }
    }
}
