using Rawler.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RawlerLib.MyExtend;

namespace RawlerTwitter
{
    public class GetFollowerIds:Rawler.Tool.RawlerMultiBase
    {
        ParentUserIdType parentUserIdType = ParentUserIdType.ScreenName;

        public ParentUserIdType ParentUserIdType
        {
            get { return parentUserIdType; }
            set { parentUserIdType = value; }
        }

        public string ScreenName { get; set; }
        public string UserId { get; set; }
        long cursor = -1;

        public IEnumerable<string> ReadAPI()
        {
            var login = this.GetUpperRawler<TwitterLogin>();
            if (login == null)
            {
                ReportManage.ErrReport(this, "TwitterLoginをTweetUserTimelineの上流に配置してください");
                return new List<string>();
            }

            List<long> list = new List<long>();

            Dictionary<string, object> dic = new Dictionary<string, object>()
                {
                       {"cursor", cursor},
                       {"count", 5000}
                };
            if (ScreenName.IsNullOrEmpty() == false)
            {
                dic.Add("screen_name", ScreenName.Convert(this));
            }
            else if (UserId.IsNullOrEmpty() == false)
            {
                dic.Add("user_id", UserId.Convert(this));
            }
            else
            {
                if (ParentUserIdType == RawlerTwitter.ParentUserIdType.ScreenName)
                {
                    dic.Add("screen_name", GetText());
                }
                else if (ParentUserIdType == RawlerTwitter.ParentUserIdType.UserId)
                {
                    dic.Add("user_id", GetText());
                }
            }

            var result = login.Token.Followers.Ids(dic);

            foreach (var item in result.Result)
            {
                list.Add(item);
            }
            if (result.NextCursor > 0) cursor = result.NextCursor;
            else
            {
                cursor = -1;
            }


            return list.Select(n => n.ToString()).ToList();
        }

        public override void Run(bool runChildren)
        {
            cursor = -1;
            do
            {
                try
                {
                    var list = ReadAPI();
                    RunChildrenForArray(runChildren, list);
                }
                catch (Exception ex)
                {
                    if (ex.Message == "Rate limit exceeded")
                    {
                        ReportManage.ErrReport(this, ex.Message);
                        System.Threading.Thread.Sleep(1000 * 60 * 3);
                    }
                    else
                    {
                        cursor = -1;
                    }
                }
            } while (cursor > 0);
        }
    }
}
