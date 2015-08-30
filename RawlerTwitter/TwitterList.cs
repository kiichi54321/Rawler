using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rawler.Tool;

namespace RawlerTwitter
{
    public class TwitterListStatuses : RawlerMultiBase
    {
        public override void Run(bool runChildren)
        {
            base.RunChildrenForArray(runChildren, ReadList());
            GC.Collect();
        }

        public double SleepSecond { get; set; }

        private Dictionary<string, Tuple<decimal, decimal>> searchWordDic = new Dictionary<string, Tuple<decimal, decimal>>();

        public string list_id { get; set; }
        public string slug { get; set; }
        public string owner_id { get; set; }
        public string owner_screen_name { get; set; }

        bool _include_rts = true;

        public bool include_rts
        {
            get
            {
                return _include_rts;
            }

            set
            {
                _include_rts = value;
            }
        }

        string GetKey()
        {
            return "list_id:" + list_id + ",slug:" + slug + ",owner_id" + owner_id + ",owner_screen_name" + owner_screen_name;
        }

        protected IEnumerable<string> ReadList()
        {
            var login = this.GetAncestorRawler().OfType<TwitterLogin>().First();
            bool flag = true;
            var key = GetKey();

            decimal maxId = -1;
            decimal minId = decimal.MaxValue;
            string list_id = GetText();
            if (string.IsNullOrEmpty(list_id) == false)
            {
                list_id = this.list_id;
            }
            bool isFrist = true;
            if (searchWordDic.ContainsKey(key)) isFrist = false;

            int loop = 1;
            while (flag)
            {
                ReportManage.Report(this, loop + "回目", true, true);
                loop++;

                Dictionary<string, object> dic = new Dictionary<string, object>()
                {
                       {"list_id", list_id},
                       {"slug", this.slug},
                       {"owner_id", this.owner_id},
                       {"owner_screen_name", this.owner_screen_name},
                       {"include_rts", this.include_rts},
                        {"count", 100}, 
                };

                if (minId != decimal.MaxValue) dic.Add("max_id", minId - 1);


                if (isFrist == false && searchWordDic.ContainsKey(key))
                {
                    dic.Add("since_id", searchWordDic[key].Item1);
                }

                var result = login.Token.Lists.Statuses(dic.Where(n => n.Value != null));


                int c = 0;
                int d = 0;
                if (result != null)
                {
                    
                    //Jsonを下流に流す
                    foreach (var item in result)
                    {
                        maxId = Math.Max(maxId, item.Id);
                        minId = Math.Min(minId, item.Id);

                        if (searchWordDic.ContainsKey(key))
                        {
                            if (searchWordDic[key].Item1 < item.Id || searchWordDic[key].Item2 > item.Id)
                            {
                                c++;
                                yield return Newtonsoft.Json.Linq.JObject.FromObject(item).ToString();
                                //  yield return Codeplex.Data.DynamicJson.Serialize(item);
                            }
                            else
                            {
                                d++;
                            }
                        }
                        else
                        {
                            c++;
                            yield return Newtonsoft.Json.Linq.JObject.FromObject(item).ToString();
                            // yield return Codeplex.Data.DynamicJson.Serialize(item);
                            //   yield return Codeplex.Data.DynamicJson.Serialize(item);
                        }
                    }

                    //すでに取得したものを含むとき
                    if (d > 0 || c == 0)
                    {
                        flag = false;
                    }
                }
                else
                {
                    flag = false;
                }

                if (searchWordDic.ContainsKey(key))
                {
                    searchWordDic[key] = new Tuple<decimal, decimal>(maxId, minId);
                }
                else
                {
                    searchWordDic.Add(key, new Tuple<decimal, decimal>(maxId, minId));
                }
                if (SleepSecond > 0)
                {
                    System.Threading.Thread.Sleep((int)(SleepSecond * 1000));
                }

            }

        }

    }
}
