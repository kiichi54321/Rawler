using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace RawlerTwitter
{
    public class TweetData
    {
        public decimal Id { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
        public string ScreenName { get; set; }
        public string UsetId { get; set; }
        public string Location { get; set; }
        public string ProfileImageLocation { get; set; }
        public string Source { get; set; }
        public string Language { get; set; }
        public Dictionary<string, string> Annotations { get; set; }

        public void SetDate(string create_at)
        {
            DateTime parsedDate;
            string DateFormat = "ddd MMM dd HH:mm:ss zz00 yyyy";
            DateTime.TryParseExact(
                create_at,
                DateFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out parsedDate);
            Date = parsedDate;
        }

        public static TweetData Parse(string json)
        {
            dynamic item = Codeplex.Data.DynamicJson.Parse(json);
            TweetData t = new TweetData();
            try
            {
                 t = new TweetData()
                {
                    Text = item.text,
                    ScreenName = item.user.screen_name,
                    Id = decimal.Parse(item.id_str),
                    Location = item.user.location,
                    UsetId = item.user.id_str,
                    ProfileImageLocation = item.user.profile_image_url

                };
                t.SetDate(item.created_at);
            }
            catch
            {

            }
            return t;
        }

        public enum TweetDataElements
        {
            Id, Text, Date, ScreenName, UsetId, Location, ProfileImageLocation, Source, Language, Annotations
        }

        public string GetTweetDataElement(TweetDataElements element)
        {
            if (element == TweetDataElements.Id) return Id.ToString();
            else if (element == TweetDataElements.Date) return Date.ToLongDateString() + " " + Date.ToLongTimeString();
            else if (element == TweetDataElements.Language) return Language;
            else if (element == TweetDataElements.Location) return Location;
            else if (element == TweetDataElements.ProfileImageLocation) return ProfileImageLocation;
            else if (element == TweetDataElements.ScreenName) return ScreenName;
            else if (element == TweetDataElements.Source) return Source;
            else if (element == TweetDataElements.Text) return Text;
            else if (element == TweetDataElements.UsetId) return UsetId;
            else { return "TweetDataElementsが不正です"; }
        }
    }
}
