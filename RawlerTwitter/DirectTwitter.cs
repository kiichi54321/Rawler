using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Security.Cryptography;

namespace RawlerTwitter
{
    class DirectTwitter
    {
         string consumer_key = "**********************";
         string consumer_secret = "**********************";
         string token = "**********************";
         string token_secret = "**********************";

        //APIのURLとパラメータでAPIにアクセス
        public string GetAPI(string APIURL)
        {
            return GetAPI(APIURL, null);
        }

        public void SetToken(string consumer_key, string consumer_secret, string token, string token_secret)
        {
            this.consumer_key = consumer_key;
            this.consumer_secret = consumer_secret;
            this.token = token;
            this.token_secret = token_secret;
        }

        //APIのURLとパラメータでAPIにアクセス
        public string GetAPI(string APIURL, Dictionary<string, string> query)
        {
            string result, queryString;
            result = queryString = string.Empty;

            //signature生成
            string signature = GenerateSignature(APIURL, "GET", query, consumer_secret, token_secret, out queryString);

            //生成したsignatureは小文字でパーセントエンコード
            string postString = queryString + string.Format("&oauth_signature={0}", UrlEncodeSmall(signature));

            //取得開始    
            byte[] data = Encoding.ASCII.GetBytes(postString);
            WebRequest req = WebRequest.Create(string.Format("{0}?{1}", APIURL, postString));
            WebResponse res;
            try
            {
                res = req.GetResponse();
                Stream stream = res.GetResponseStream();
                StreamReader reader = new StreamReader(stream);

                result = reader.ReadToEnd();
                reader.Close();
                stream.Close();
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    if (((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.Unauthorized)
                    {
                        /*401 Unauthorized                    
                        *認証失敗*/
                        return "401 Unauthorized";
                    }
                    else if (((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.BadRequest)
                    {
                        /*400 Bad Request                    
                        *リクエストが不正*/
                        return "400 Bad Request";
                    }
                }
                else
                {
                    return ex.Message;
                }

            }
            return result;
        }

        //APIのURL,HttpMethod(POST/GET),パラメータ,consumer_secret,token_secretでsignature生成
        private string GenerateSignature(string url, string httpMethod, Dictionary<string, string> query, string consumer_secret, string token_secret, out string conectedQuery)
        {
            //SortedDictionaryでパラメータをkey順でソート
            SortedDictionary<string, string> sortedParams;
            if (query == null)
                sortedParams = new SortedDictionary<string, string>();
            else
                sortedParams = new SortedDictionary<string, string>(query);

            string timestamp = GenerateTimestamp();
            string nonce = GenerateNonce();

            sortedParams["oauth_consumer_key"] = consumer_key;
            sortedParams["oauth_token"] = token;
            sortedParams["oauth_version"] = "1.0";
            sortedParams["oauth_timestamp"] = timestamp;
            sortedParams["oauth_nonce"] = nonce;
            sortedParams["oauth_signature_method"] = "HMAC-SHA1";

            StringBuilder sb = new StringBuilder();
            bool first = true;
            foreach (var p in sortedParams)
            {
                if (first)
                {
                    sb.Append(p.Key + "=" + p.Value);
                    first = false;
                }
                else
                    sb.Append(@"&" + p.Key + "=" + p.Value);
            }
            conectedQuery = sb.ToString();
            string signatureBace = string.Format(@"{0}&{1}&{2}", httpMethod, UrlEncode(url), UrlEncode(sb.ToString()));

            //consumer_secretとtoken_secretを鍵にしてハッシュ値を求める
            HMACSHA1 hmacsha1 = new HMACSHA1();
            hmacsha1.Key = Encoding.ASCII.GetBytes(string.Format("{0}&{1}", UrlEncode(consumer_secret), UrlEncode(token_secret)));
            byte[] dataBuffer = System.Text.Encoding.ASCII.GetBytes(signatureBace);
            byte[] hashBytes = hmacsha1.ComputeHash(dataBuffer);

            return Convert.ToBase64String(hashBytes);
        }

        private string GenerateNonce()
        {
            string letters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            StringBuilder result = new StringBuilder(8);
            Random random = new Random();
            for (int i = 0; i < 8; ++i)
                result.Append(letters[random.Next(letters.Length)]);
            return result.ToString();
        }

        private string GenerateTimestamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

        private string UrlEncode(string value)
        {
            string unreserved = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";
            StringBuilder result = new StringBuilder();
            byte[] data = Encoding.UTF8.GetBytes(value);
            foreach (byte b in data)
            {
                if (b < 0x80 && unreserved.IndexOf((char)b) != -1)
                    result.Append((char)b);
                else
                    result.Append('%' + String.Format("{0:X2}", (int)b));
            }
            return result.ToString();
        }
        private string UrlEncodeSmall(string value)
        {
            string unreserved = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";

            StringBuilder result = new StringBuilder();
            byte[] data = Encoding.UTF8.GetBytes(value);
            foreach (byte b in data)
            {
                if (b < 0x80 && unreserved.IndexOf((char)b) != -1)
                    result.Append((char)b);
                else
                    result.Append('%' + String.Format("{0:x2}", (int)b));
            }
            return result.ToString();
        }
    }
}

