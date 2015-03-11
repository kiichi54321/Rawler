using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rawler.Tool;

namespace Rawler.Tool
{
    public class KeyValueStore : RawlerBase
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<KeyValueStore>(parent);
        }

        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }
        #endregion

        /// <summary>
        /// このクラスでの実行すること。
        /// </summary>
        /// <param name="runChildren"></param>
        public override void Run(bool runChildren)
        {
            SetText(GetText());
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

        Dictionary<string, string> dic = new Dictionary<string, string>();

        public void SetKeyValue(string key, string value)
        {
            if (dic.ContainsKey(key))
            {
                dic[key] = value;
            }
            else
            {
                dic.Add(key, value);
            }
        }

        public string GetKeyValue(string key)
        {
            if (dic.ContainsKey(key))
            {
                return dic[key];
            }
            else
            {
                ReportManage.ErrReport(this, "Key（" + key + "）が見つかりません");
                return string.Empty;
            }
        }

        public void Clear()
        {
            dic.Clear();
        }
    }


    public class GetKeyValue : RawlerBase
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<GetKeyValue>(parent);
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
            var list = this.GetAncestorRawler().OfType<KeyValueStore>();
            if (list.Any())
            {
                SetText(list.First().GetKeyValue(Key));
            }
            else
            {
                ReportManage.ErrReport(this, "上流にKeyValueStoreがありません");
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

    public class SetKeyValue : RawlerBase
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<SetKeyValue>(parent);
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
            var list = this.GetAncestorRawler().OfType<KeyValueStore>();
            if (list.Any())
            {
                if (string.IsNullOrEmpty(Value))
                {
                    list.First().SetKeyValue(Key, GetText());
                }
                else
                {
                    list.First().SetKeyValue(Key, Value);
                }
            }
            else
            {
                ReportManage.ErrReport(this, "上流にKeyValueStoreがありません");
            }


            SetText(GetText());
            base.Run(runChildren);
        }

        public string Value { get; set; }

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

    public class CheckKeyValue : RawlerBase
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<CheckKeyValue>(parent);
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
                ReportManage.ErrReport(this, "CheckKeyValueのKeyが空です。");
            }
            else
            {
                var list = this.GetAncestorRawler().OfType<KeyValueStore>();
                if (list.Any())
                {
                    if ((list.First().GetKeyValue(Key) == Value) == result)
                    {
                        this.SetText(this.GetText());
                        base.Run(runChildren);
                    }
                }
                else
                {
                    ReportManage.ErrReport(this, "上流にKeyValueStoreがありません");
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


    public class KeyValueStoreClear : RawlerBase
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<KeyValueStoreClear>(parent);
        }

        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }
        #endregion

        /// <summary>
        /// このクラスでの実行すること。
        /// </summary>
        /// <param name="runChildren"></param>
        public override void Run(bool runChildren)
        {
            var list = this.GetAncestorRawler().OfType<KeyValueStore>();
            if (list.Any())
            {
                list.First().Clear();
            }
            else
            {
                ReportManage.ErrReport(this, "上流にKeyValueStoreがありません");
            }
            SetText(GetText());
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
