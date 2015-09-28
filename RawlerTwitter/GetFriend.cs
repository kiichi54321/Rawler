using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rawler.Tool;
using RawlerLib.MyExtend;


namespace RawlerTwitter
{
    public class GetFriendIds : Rawler.Tool.RawlerMultiBase
    {
        ParentUserIdType parentUserIdType = ParentUserIdType.ScreenName;

        public ParentUserIdType ParentUserIdType
        {
            get { return parentUserIdType; }
            set { parentUserIdType = value; }
        }

        public string ScreenName { get; set; }
        public string UserId { get; set; }

        public IEnumerable<string> ReadAPI()
        {
            var login = this.GetUpperRawler<TwitterLogin>();
            if (login == null)
            {
                ReportManage.ErrReport(this, "TwitterLoginをTweetUserTimelineの上流に配置してください");
                yield break;
            }
            long cursor = -1;

            while (true)
            {
                Dictionary<string, object> dic = new Dictionary<string, object>()
                {
                       {"cursor", cursor},
                       {"count", 5000}
                };
                if (ScreenName.IsNullOrEmpty() == false)
                {
                    dic.Add("screen_name", ScreenName);
                }
                else if (UserId.IsNullOrEmpty() == false)
                {
                    dic.Add("user_id", UserId);
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
                var result = login.Token.Friends.Ids(dic);
                foreach (var item in result.Result)
                {
                    yield return item.ToString();
                }
                if (result.NextCursor > 0) cursor = result.NextCursor;
                else
                {
                    break;
                }
            }
        }

        public override void Run(bool runChildren)
        {
            RunChildrenForArray(runChildren, ReadAPI());
        }
    }
}
