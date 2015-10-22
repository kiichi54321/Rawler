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
        long cursor = -1;
        public int Max { get; set; } = int.MaxValue;

        public IEnumerable<string> ReadAPI()
        {
            var login = this.GetUpperRawler<TwitterLogin>();
            if (login == null)
            {
                ReportManage.ErrReport(this, "TwitterLoginをTweetUserTimelineの上流に配置してください");
                yield break;
            }
     
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
            var result = login.Token.Friends.Ids(dic);
            foreach (var item in result.Result)
            {
                yield return item.ToString();
            }
            if (result.NextCursor > 0) cursor = result.NextCursor;
            else
            {
                cursor = -1;
           }

        }

        public override void Run(bool runChildren)
        {
            cursor = -1;
            int c = 0;
            do
            {
                bool flag = true;
                List<string> list = new List<string>();
                try
                {
                    list = ReadAPI().ToList();
                }
                catch(Exception ex)
                {
                    ReportManage.ErrReport(this, ex.Message);
                    if (ex.Message == "Rate limit exceeded")
                    {
                        System.Threading.Thread.Sleep(TimeSpan.FromMinutes(3));
                    }
                    else
                    {
                        flag = false;
                    }                    
                }
                if (flag == false) break;
                RunChildrenForArray(runChildren, list);
                c = c + list.Count;
                if (c >= Max) break;
            }
            while (cursor > 0);
        }
    }
}
