using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rawler.Tool;
using RawlerLib.MyExtend;
using Newtonsoft.Json.Linq;

namespace RawlerTwitter
{

    public class UserLookup : RawlerBatchMultiBase
    {
        public UserLookup()
            :base()
        {
            base.BatchSize = 100;
        }

        ParentUserIdType parentUserIdType = ParentUserIdType.ScreenName;

        public ParentUserIdType ParentUserIdType
        {
            get { return parentUserIdType; }
            set { parentUserIdType = value; }
        }

        /// <summary>
        /// BatchSizeを隠す用
        /// </summary>
        protected int BatchSize { get; set; }

        public override void RunBatch(IEnumerable<string> list)
        {
            var login = this.GetUpperRawler<TwitterLogin>();
            if (login == null)
            {
                ReportManage.ErrUpperNotFound<TwitterLogin>(this);
            }
            string[] list1 = null;
            bool flag = true;
            do
            {
                flag = true;
                try
                {

                    if (ParentUserIdType == ParentUserIdType.ScreenName)
                    {
                        var l = login.Token.Users.Lookup(list);
                        list1 = l.Select(n => Codeplex.Data.DynamicJson.Serialize(n)).ToArray();
                    }
                    else
                    {
                        long[] longList = null;
                        try
                        {
                            longList = list.Select(n => long.Parse(n)).ToArray();
                        }
                        catch (Exception ex)
                        {
                            ReportManage.ErrReport(this, "tweetIdが数値ではありません。" + ex.Message+"\t"+list.JoinText(","));
                        }
                        var l = login.Token.Users.Lookup(longList);
                        list1 = l.Select(n => Codeplex.Data.DynamicJson.Serialize(n)).ToArray();
                    }
                }
                catch (Exception ex1)
                {
                    ReportManage.ErrReport(this, ex1.Message);
                    flag = false;
                    System.Threading.Thread.Sleep(TimeSpan.FromMinutes(3));
                }
            } while (flag);
            base.RunBatch(list1);

        }


    }
}


