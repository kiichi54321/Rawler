using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RawlerLib
{
    /// <summary>
    /// http://dobon.net/vb/dotnet//internet/urlencode.html から拝借
    /// </summary>
    public class RFC3986Uri
    {
        public static readonly string UnreservedCharacters =
            "-._~";
        public static readonly string ReservedCharacters =
            UnreservedCharacters + ":/?#[]@!$&'()*+,;=";

        /// <summary>
        /// RFC3986に基づいてURLエンコードを行います。
        /// </summary>
        /// <param name="stringToEscape">
        /// URLエンコードする文字列。
        /// </param>
        /// <param name="escapeEncoding">
        /// エンコード方式を指定するEncoding オブジェクト。
        /// </param>
        /// <returns>
        /// URLエンコードされた文字列。
        /// </returns>
        public static string EscapeDataString(string stringToEscape,
            System.Text.Encoding escapeEncoding)
        {
            return PercentEncodeString(
                stringToEscape, UnreservedCharacters, escapeEncoding);
        }
        public static string EscapeDataString(string stringToEscape)
        {
            return EscapeDataString(stringToEscape, System.Text.Encoding.UTF8);
        }

        /// <summary>
        /// RFC3986に基づいてURI文字列のURLエンコードを行います。
        /// </summary>
        /// <param name="stringToEscape">
        /// URLエンコードする文字列。
        /// </param>
        /// <param name="escapeEncoding">
        /// エンコード方式を指定するEncoding オブジェクト。
        /// </param>
        /// <returns>
        /// URLエンコードされた文字列。
        /// </returns>
        public static string EscapeUriString(string stringToEscape,
            System.Text.Encoding escapeEncoding)
        {
            return PercentEncodeString(
                stringToEscape, ReservedCharacters, escapeEncoding);
        }
        public static string EscapeUriString(string stringToEscape)
        {
            return EscapeUriString(stringToEscape, System.Text.Encoding.UTF8);
        }

        internal static string PercentEncodeString(string stringToEscape,
            string dontEscapeCharacters,
            System.Text.Encoding escapeEncoding)
        {
            System.Text.StringBuilder encodedString =
                new System.Text.StringBuilder();

            foreach (char c in stringToEscape)
            {
                if (('0' <= c && c <= '9') ||
                    ('a' <= c && c <= 'z') || ('A' <= c && c <= 'Z') ||
                    (0 <= dontEscapeCharacters.IndexOf(c)))
                {
                    //エンコードしない文字の場合
                    encodedString.Append(c);
                }
                else
                {
                    //エンコードする文字の場合
                    encodedString.Append(HexEscape(c, escapeEncoding));
                }
            }

            return encodedString.ToString();
        }

        /// <summary>
        /// 指定した文字のパーセントエンコーディング（百分率符号化）を行います。
        /// </summary>
        /// <param name="character">
        /// パーセントエンコーディングする文字。
        /// </param>
        /// <param name="escapeEncoding">
        /// エンコード方式を指定するEncoding オブジェクト。
        /// </param>
        /// <returns>
        /// パーセントエンコーディングされた文字列。
        /// </returns>
        public static string HexEscape(char character,
            System.Text.Encoding escapeEncoding)
        {
            if (255 < (int)character)
            {
                //characterが255を超えるときはUri.HexEscapeが使えない
                System.Text.StringBuilder buf = new System.Text.StringBuilder();
                byte[] characterBytes =
                    escapeEncoding.GetBytes(character.ToString());
                foreach (byte b in characterBytes)
                {
                    buf.AppendFormat("%{0:X2}", b);
                }

                return buf.ToString();
            }

            return Uri.HexEscape(character);
        }
    }
}
