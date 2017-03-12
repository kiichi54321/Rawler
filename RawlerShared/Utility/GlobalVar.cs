using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rawler.Core;

namespace Rawler
{
    /// <summary>
    /// 一時変数という扱いにしたかったけど、便利すぎたから名前を変える予定。
    /// </summary>
    public static class GlobalVar
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
                ReportManage.ErrReport(null, "TempVarクラスのGetVarで「" + key + "」がありませんでした。");
                return string.Empty;
            }

        }

    }



    public class SetGlobalVar : RawlerBase
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
                    GlobalVar.SetVar(Key, GetText());
                    this.SetText(GetText());
                }
                else
                {
                    GlobalVar.SetVar(Key, Value);
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

    public class GetGlobalVar : RawlerBase
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
                this.SetText(GlobalVar.GetVar(Key));
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

    public class CheckGlobalVar : RawlerBase
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
                if ((GlobalVar.GetVar(Key) == Value) == result)
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

    public class GetGlobalVarCsv : RawlerBase
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
                ReportManage.ErrReport(this, "GetGlobalVarCsvのKeyが空です。");
            }
            else
            {
                var d = GlobalVar.GetVar(Key).Split(',');
                if (d.Length > Num && Num > -1)
                {
                    this.SetText(d.ElementAt(Num));
                }
                else
                {
                    ReportManage.ErrReport(this, "GetGlobalVarCsvのNumが範囲外です。");
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
