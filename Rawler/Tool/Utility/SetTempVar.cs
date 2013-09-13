using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rawler.Tool;

namespace Rawler.Tool
{
    public static class TempVar
    {
        static Dictionary<string, string> dic = new Dictionary<string, string>();
        public static void SetVar(string key, string val)
        {
            if (dic.ContainsKey(key))
            {
                dic[key] = val;
            }
            else
            {
                dic.Add(key, val);
            }
        }

        public static string GetVar(string key)
        {
            if (dic.ContainsKey(key))
            {
                return dic[key];
            }
            else
            {
                ReportManage.ErrReport(null, "TempVarクラスのGetVarで「"+key+"」がありませんでした。");
                return string.Empty;
            }
            
        }

    }



    public class SetTempVar : RawlerBase
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<SetTempVar>(parent);
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
        public string Value { get; set; }
        /// <summary>
        /// このクラスでの実行すること。
        /// </summary>
        /// <param name="runChildren"></param>
        public override void Run(bool runChildren)
        {
            if (string.IsNullOrEmpty(Key))
            {
                ReportManage.ErrReport(this, "SetTempVarのKeyが空です。");
            }
            else
            {
                if (string.IsNullOrEmpty(Value))
                {
                    TempVar.SetVar(Key, GetText());
                    this.SetText(GetText());
                }
                else
                {
                    TempVar.SetVar(Key, Value);
                    this.SetText(Value);
                }
            }
            base.Run(runChildren);
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

    public class GetTempVar : RawlerBase
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<GetTempVar>(parent);
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
            if (string.IsNullOrEmpty(Key))
            {
                ReportManage.ErrReport(this, "GetTempVarのKeyが空です。");
            }
            else
            {
                this.SetText(TempVar.GetVar(Key));
            } 
            base.Run(runChildren);
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

    public class CheckTempVar : RawlerBase
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<CheckTempVar>(parent);
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
        public string Value { get; set; }
        bool result = true;

        public bool Result
        {
            get { return result; }
            set { result = value; }
        }
        /// <summary>
        /// このクラスでの実行すること。
        /// </summary>
        /// <param name="runChildren"></param>
        public override void Run(bool runChildren)
        {
            if (string.IsNullOrEmpty(Key))
            {
                ReportManage.ErrReport(this, "GetTempVarのKeyが空です。");
            }
            else
            {
                if ((TempVar.GetVar(Key) == Value) == result)
                {
                    this.SetText(this.GetText());
                    base.Run(runChildren);
                }
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

    public class GetTempVarCsv : RawlerBase
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<GetTempVarCsv>(parent);
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
        public int Num { get; set; }
        /// <summary>
        /// このクラスでの実行すること。
        /// </summary>
        /// <param name="runChildren"></param>
        public override void Run(bool runChildren)
        {
            if (string.IsNullOrEmpty(Key))
            {
                ReportManage.ErrReport(this, "GetTempVarのKeyが空です。");
            }
            else
            {
                var d = TempVar.GetVar(Key).Split(',');
                if (d.Length > Num && Num >-1)
                {
                    this.SetText(d.ElementAt(Num));
                }
                else
                {
                    ReportManage.ErrReport(this, "GetTempVarのNumが範囲外です。");
                }
            }
            base.Run(runChildren);

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
