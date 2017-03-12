using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rawler.Core;

namespace Rawler
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

        /// <summary>
        /// 指定したキーが上流のKeyValueStoreにあるかを調べる
        /// </summary>
        /// <param name="rawler"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static bool ContainsKey(RawlerBase rawler, params string[] keys)
        {
            var r = rawler.GetUpperRawler<KeyValueStore>();
            if (r != null)
            {
                return keys.All((n) => r.dic.ContainsKey(n));
            }
            else
            {
                ReportManage.ErrUpperNotFound<KeyValueStore>(rawler);
            }
            return false;
        }

        static Dictionary<RawlerBase, WeakReference<KeyValueStore[]>> ancestorKeyValueStoreDic = new Dictionary<RawlerBase, WeakReference<KeyValueStore[]>>();

        /// <summary>
        /// 指定したキーで上流のKeyValueStoreから値を取得する。
        /// </summary>
        /// <param name="rawler"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetValueByKey(RawlerBase rawler, string key)
        {
            KeyValueStore[] r = null;
            if (ancestorKeyValueStoreDic.ContainsKey(rawler))
            {
                ancestorKeyValueStoreDic[rawler].TryGetTarget(out r);
            }
            if (r == null)
            {
                r = rawler.GetAncestorRawler().OfType<KeyValueStore>().ToArray();
                if (ancestorKeyValueStoreDic.ContainsKey(rawler))
                {
                    ancestorKeyValueStoreDic[rawler].SetTarget(r);
                }
                else
                {
                    ancestorKeyValueStoreDic.Add(rawler, new WeakReference<KeyValueStore[]>(r));
                }
            }
            string val = null;
            foreach (var item in r)
            {
                if (item.dic.ContainsKey(key))
                {
                    val = item.dic[key];
                    break;
                }
            }
            if (val == null)
            {
                ReportManage.ErrReport(rawler, "key:" + key + "が見つかりません");
            }
            return val;
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

        KeyValueStore keyValueStore;
        /// <summary>
        /// このクラスでの実行すること。
        /// </summary>
        /// <param name="runChildren"></param>
        public override void Run(bool runChildren)
        {
            if (keyValueStore == null)
            {
                keyValueStore = this.GetUpperRawler<KeyValueStore>();
            }
            if (keyValueStore != null)
            {
                if (string.IsNullOrEmpty(Value))
                {
                    keyValueStore.SetKeyValue(Key, GetText());
                }
                else
                {
                    keyValueStore.SetKeyValue(Key, Value);
                }
            }
            else
            {
                ReportManage.ErrUpperNotFound<KeyValueStore>(this);
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


    public static class KeyValueLib
    {
        static System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"\[[^\]]*\]", System.Text.RegularExpressions.RegexOptions.None);

        static Dictionary<string, StrBlock[]> keyDic = new Dictionary<string, StrBlock[]>();
        class StrBlock
        {
            public string Text { get; set; }

            public virtual string GetText(RawlerBase rawler)
            {
                return Text;
            }
        }

        class StrBlock_Key : StrBlock
        {
            public string Key { get; set; }
            protected new string Text { get; set; }
            public override string GetText(RawlerBase rawler)
            {
                return KeyValueStore.GetValueByKey(rawler, Key);
            }
        }

        class StrBlock_Key2 : StrBlock
        {
            public string Key { get; set; }
            public string Header { get; set; }
            protected new string Text { get; set; }
            public override string GetText(RawlerBase rawler)
            {
                string v = string.Empty;
                v = KeyValueStore.GetValueByKey(rawler, Key);
                try
                {
                    switch (Header)
                    {
                        case "File":
                            v = System.IO.Path.GetFileNameWithoutExtension(v);
                            break;
                        case "Directory":
                            v = System.IO.Path.GetDirectoryName(v);
                            break;
                        case "FullFile":
                            v = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(v), System.IO.Path.GetFileNameWithoutExtension(v));
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    ReportManage.ErrReport(rawler, "値:" + Header + "で" + ex.Message);
                }
                if (v == null)
                {
                    ReportManage.ErrReport(rawler, "値:" + Header + "は、存在しない識別子です。" + v);
                }

                return v;
            }
        }

        static string GetConvertStr(string text, RawlerBase rawler)
        {
            if (keyDic.ContainsKey(text))
            {
                StrBlock[] l = keyDic[text];
                if (l.Length == 1)
                {
                    return l[0].GetText(rawler);
                }
                else
                {
                    StringBuilder sb1 = new StringBuilder();
                    foreach (var item in l)
                    {
                        sb1.Append(item.GetText(rawler));
                    }
                    return sb1.ToString();
                }
            }
            var text2 = text.Replace("[[", "&(").Replace("]]", "&)");
            // System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"\[\w*\]");
            int postion = 0;
            List<StrBlock> blockList = new List<StrBlock>();
            foreach (var item in r.Matches(text2).OfType<System.Text.RegularExpressions.Match>())
            {
                var key = item.Value.Replace("[", "").Replace("]", "");

                if (item.Index != postion)
                {
                    blockList.Add(new StrBlock() { Text = text2.Substring(postion, item.Index - postion) });
                }
                postion = item.Index + item.Value.Length;

                var d = key.Split(':');
                if (d.Length == 1)
                {
                    blockList.Add(new StrBlock_Key() { Key = key });
                }
                else
                {
                    blockList.Add(new StrBlock_Key2() { Key = d.Last(), Header = d.First() });
                }
            }
            if (postion < text2.Length)
            {
                blockList.Add(new StrBlock() { Text = text2.Substring(postion, text2.Length - postion) });
            }

            if (keyDic.ContainsKey(text))
            {
                keyDic[text] = blockList.ToArray();
            }
            else
            {
                keyDic.Add(text, blockList.ToArray());
            }
            StringBuilder sb = new StringBuilder();
            foreach (var item in blockList)
            {
                sb.Append(item.GetText(rawler));
            }
            return sb.ToString();
        }
        
    


        /// <summary>
        /// {key} という記法で上流のKeyValueの値を取得する。
        /// </summary>
        /// <param name="text"></param>
        /// <param name="rawler"></param>
        /// <returns></returns>
        public static string Convert(this string text, RawlerBase rawler)
        {
            if (text == null)
            {
                return null;
            }
            else
            {
                return GetConvertStr(text, rawler);
        //        var text2 = text.Replace("[[", "&(").Replace("]]", "&)");
        //// System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"\[\w*\]");
                
        //        foreach (var item in r.Matches(text).OfType<System.Text.RegularExpressions.Match>())
        //        {
        //            var key = item.Value.Replace("[", "").Replace("]", "");
        //            var key1 = HeaderConvert(key, rawler);
        //            if(key1.Length==0)
        //            {
        //                var val = KeyValueStore.GetValueByKey(rawler, key);                       
        //                if (val.Length > 0)
        //                {
        //                    text2 = text2.Replace(item.Value, val);
        //                }
        //            }
        //            else
        //            {
        //                text2 = text2.Replace(item.Value, key1);
        //            }
                   
        //        }
        //        return text2.Replace("&(", "[").Replace("&)", "]");
            }
        }

        /// <summary>
        /// [File:key]でファイル名のみを取得するなどの方法を提供。
        /// </summary>
        /// <param name="val"></param>
        /// <param name="rawler"></param>
        /// <returns></returns>
        public static string HeaderConvert(string val, RawlerBase rawler)
        {
            var d = val.Split(':');
            if(d.Length==1)
            {
                return string.Empty ;
            }
            else
            {
                var h = d.First();
                string v = string.Empty;
                v = KeyValueStore.GetValueByKey(rawler, d[1]);
                try
                {
                    switch (h)
                    {
                        case "File":
                            v = System.IO.Path.GetFileNameWithoutExtension(v);
                            break;
                        case "Directory":
                            v = System.IO.Path.GetDirectoryName(v);
                            break;
                        case "FullFile":
                            v = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(v), System.IO.Path.GetFileNameWithoutExtension(v));
                            break;
                        default:
                            break;
                    }
                }
                catch(Exception ex)
                {
                    ReportManage.ErrReport(rawler, "値:" + val+"で"+ex.Message);
                }
                if(v == null)
                {
                    ReportManage.ErrReport(rawler, "値:" + h+"は、存在しない識別子です。"+ val);
                }
                return v;
            }


        }


        //public static string Convert(this string text,RawlerBase rawler)
        //{
        //    var list = RawlerLib.Web.GetTagContentList(text, "{", "}", true);

        //}
    }
}
