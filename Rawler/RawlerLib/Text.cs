using System;
using System.Collections.Generic;
using System.Text;

namespace RawlerLib
{
    public class String
    {
        /// <summary>
        /// 改行コードを削除します
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        public static string ChopReturnCode(string txt)
        {
            return txt.Replace("\r","").Replace("\n","");
        }

        /// <summary>
        /// リストの中のnullや空白のものを削除
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<string> EmptyDelList(string[] list)
        {
            List<string> list2 = new List<string>();
            foreach (string tmp in list)
            {
                if (tmp != null && tmp.Length > 0)
                {
                    list2.Add(tmp);
                }
            }
            return list2;
        }

        /// <summary>
        /// リストの中のnullや空白のものを削除
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<string> EmptyDelList(List<string> list)
        {
            List<string> list2 = new List<string>();
            foreach (string tmp in list)
            {
                if (tmp != null && tmp.Length > 0)
                {
                    list2.Add(tmp);
                }
            }
            return list2;
        }

        /// <summary>
        /// 配列を指定した文字列で区切った文字列を返す。
        /// </summary>
        /// <param name="array"></param>
        /// <param name="separetor"></param>
        /// <returns></returns>
        public static string JoinString(string[] array, string separetor)
        {
            StringBuilder strBuider = new StringBuilder();
            foreach (string str in array)
            {
                strBuider.Append(str);
                strBuider.Append(separetor);
            }
            if (strBuider.Length > separetor.Length)
            {
                strBuider.Length = strBuider.Length - separetor.Length;
            }
            return strBuider.ToString();
        }

        

        /// <summary>
        /// 指定したｃのところで改行した文字列を返す
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static string AddReturnCode(string txt, char c,int maxLength)
        {
            StringBuilder strBuilder = new StringBuilder();
            StringBuilder lineBuilder = new StringBuilder();
            int count = 0;
            foreach (char a in txt.ToCharArray())
            {
                count++;
                lineBuilder.Append(a);
                if (a.Equals(c))
                {
                    strBuilder.Append(AddReturnCode(lineBuilder.ToString(), maxLength));
                    lineBuilder.Length = 0;
                    if (txt.Length > count)
                    {
                        strBuilder.AppendLine();
                    }
                }

            }
            if (lineBuilder.Length > 0)
            {
                strBuilder.Append(AddReturnCode(lineBuilder.ToString(), maxLength));
            }
      
            return strBuilder.ToString();
        }

        public static string AddReturnCode(string txt, int maxLength)
        {
            StringBuilder strBuilder = new StringBuilder();
            StringBuilder lineBuilder = new StringBuilder();
            lineBuilder.Append(txt);
            if (lineBuilder.Length > maxLength)
            {
                int k = (int)Math.Floor((((double)lineBuilder.Length / (double)maxLength) + 0.5));
                int length = lineBuilder.Length / k;
                int cc = 0;
                for (int i = 0; i < k; i++)
                {
                    if (lineBuilder.Length > cc + length)
                    {
                        strBuilder.AppendLine(lineBuilder.ToString(i * length, length));
                        cc += length;
                    }
                    else
                    {
                        strBuilder.Append(lineBuilder.ToString(i * length, lineBuilder.Length - cc));
                        break;
                    }
                }
            }
            else
            {
                strBuilder.Append(lineBuilder.ToString());
            }
            return strBuilder.ToString();
        }

        /// <summary>
        /// 00123 のような文字列を作る。
        /// </summary>
        /// <param name="i"></param>
        /// <param name="figure">桁数</param>
        /// <returns></returns>
        public static string Int2String(int number, int figure)
        {
            string tmp ="";
            for (int i = 0; i < figure - number.ToString().Length; i++)
            {
                tmp = "0" + tmp;
            }
            tmp = tmp + number.ToString();
            return tmp;
        }

        /// <summary>
        /// 文字列を改行区切りでリストにする。
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string[] String2Lines(string str)
        {
            List<string> separator = new List<string>();
            separator.Add("\r\n");
            separator.Add("\n");
            separator.Add("\r");
            return str.Split(separator.ToArray(), StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// テキストについて、配列の文字列が見つかった最初の位置を返す。
        /// </summary>
        /// <param name="str"></param>
        /// <param name="array"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public static int IndexOfAny(string str, string[] array,int startIndex,out string useString)
        {
            int i = int.MaxValue;
            string tmpStr = string.Empty;
            foreach (string d in array)
            {
                int tmp = str.IndexOf(d, startIndex);
                if (tmp >= startIndex)
                {
                    if (i > tmp)
                    {
                        i = tmp;
                        tmpStr = d;
                    }
                }
            }
            if (i == int.MaxValue)
            {
                i = -1;
            }
            useString = tmpStr;
            return i;
        }

        /// <summary>
        /// テキストをデリミタによって分割し配列にします。
        /// </summary>
        /// <param name="html"></param>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        public static string[] TextToListByDelimiter(string html, string[] delimiter,string footer)
        {
            List<string> list = new List<string>();
            int index = 0;
            int endIndex = 0;
            string useDelimiter;
            //    string tmpTxt = html;
            while (true)
            {
                endIndex = RawlerLib.String.IndexOfAny(html, delimiter, index, out useDelimiter);
                if (endIndex > -1)
                {
                    list.Add(html.Substring(index, endIndex + useDelimiter.Length - index)+footer);
                    index = endIndex + useDelimiter.Length;
                }
                else
                {
                    break;
                }
            }
            if (index < html.Length)
            {
                list.Add(html.Substring(index, html.Length - index)+footer);
            }
            return list.ToArray();
        }

        public static string[] TextToListByDelimiter(string html, string[] delimiter)
        {
            return TextToListByDelimiter(html, delimiter, "");
        }

        /// <summary>
        /// 配列の中身も含めて同じかどうかをみる。同じならTrue
        /// </summary>
        /// <param name="array1"></param>
        /// <param name="array2"></param>
        /// <returns></returns>
        public static bool IsSameArray(string[] array1, string[] array2)
        {
            if (array1 == null || array2 == null)
            {
                return false;
            }
            if (array1.Length != array2.Length)
            {
                return false;
            }
            else
            {
                bool flag = true;
                for (int i = 0; i < array1.Length; i++)
                {
                    if (array1[i] != array2[i])
                    {
                        flag = false;
                        break;
                    }
                }
                return flag;
            }
        }

        /// <summary>
        /// 配列の中身と部分マッチしたら、True。配列が空の場合は無条件でTrue
        /// </summary>
        /// <param name="str"></param>
        /// <param name="array"></param>
        /// <returns></returns>
        public static bool ContainsInArray(string str, string[] array)
        {
            if (array.Length > 0)
            {
                foreach (string a in array)
                {
                    if (str.Contains(a))
                    {
                        return true;
                    }
                }
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 末尾が配列を一致したらTrue
        /// </summary>
        /// <param name="target"></param>
        /// <param name="array"></param>
        /// <returns></returns>
        public static bool EndsWith(string target, string[] array)
        {
            bool flag = false;
            foreach (string a in array)
            {
                if (target.EndsWith(a))
                {
                    flag = true;
                    break;
                }
            }
            return flag;
        }



        /// <summary>
        /// n字以上を切り捨てる
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string RoundString(string str,int n)
        {
            if (str.Length > n)
            {
                return str.Substring(0, n);
            }
            else
            {
                return str;
            }
        }

        public static string IntArray2String(int[] array)
        {
            StringBuilder strBuilder = new StringBuilder();
            foreach (int i in array)
            {
                strBuilder.Append(i);
                strBuilder.Append(",");
            }
            strBuilder.Length = strBuilder.Length - 1;
            return strBuilder.ToString();
        }

        public static int[] String2IntArray(string str)
        {
            string[] strArray = str.Split(',');
            List<int> list = new List<int>();
            int r;
            foreach (string s in strArray)
            {
                if (int.TryParse(s, out r))
                {
                    list.Add(r);
                }
            }
            return list.ToArray();
        }

        /// <summary>
        /// 文字列配列をカンマ区切りに変更する。
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static string StringArray2Csv(string[] array)
        {
            StringBuilder strBuilder = new StringBuilder();
            foreach (string i in array)
            {
                strBuilder.Append(i);
                strBuilder.Append(",");
            }
            strBuilder.Length = strBuilder.Length - 1;
            return strBuilder.ToString();

        }

        /// <summary>
        /// 配列の中身すべてと部分マッチしたらTrue。配列が空の場合は無条件でTrue
        /// </summary>
        /// <param name="str"></param>
        /// <param name="array"></param>
        /// <returns></returns>
        public static bool ContainsAllInArray(string str, string[] array)
        {
            if (array.Length > 0)
            {
                foreach (string a in array)
                {
                    if (str.Contains(a) == false)
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                return true;
            }

        }

        public static IEnumerable<string> TextReadLines(string text)
        {
            System.IO.StringReader reader = new System.IO.StringReader(text);
            while (reader.Peek() > -1)
            {
                yield return reader.ReadLine();
            }
        }
    }
}
