using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rawler.Tool;

namespace Rawler.Tool
{
    public class QueryFirst : RawlerQuery
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerQuery Clone()
        {
            return base.Clone<QueryFirst>();
        }

        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }
        #endregion

        public override IEnumerable<string> Query(IEnumerable<string> list)
        {
            List<string> l = new List<string>();
            if (list.Any() )
            {
                l.Add(list.First());
            }
            return l;
        }
    }

    public class QueryLast : RawlerQuery
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerQuery Clone()
        {
            return base.Clone<QueryLast>();
        }

        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }
        #endregion

        public override IEnumerable<string> Query(IEnumerable<string> list)
        {
            List<string> l = new List<string>();
            if (list.Any())
            {
                l.Add(list.Last());
            }
            return l;
            
        }
    }

    public class QueryContain : RawlerQuery
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerQuery Clone()
        {
            return base.Clone<QueryContain>();
        }

        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }
        #endregion

        public string ContainCSV { get; set; }
        public string ContainText { get; set; }
        private bool result = true;

        public bool Result
        {
            get { return result; }
            set { result = value; }
        }
        
        public override IEnumerable<string> Query(IEnumerable<string> list)
        {
            if (string.IsNullOrEmpty(ContainCSV) == false || string.IsNullOrEmpty(ContainText) == false)
            {
                if (string.IsNullOrEmpty(ContainText) == false)
                {
                    return list.Where(n => n.Contains(ContainText) == Result);
                }
                var array = ContainCSV.Split(',');

                return list.Where(n => array.Any(m=> n.Contains(m))  == Result);
            }
            else
            {
                ReportManage.ErrReport(null, "QueryContain で、ContainTextかContainCSVが空です。");
                return new List<string>();
            }
        }
    }



    public class QueryShuffle : RawlerQuery
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerQuery Clone()
        {
            return base.Clone<QueryShuffle>();
        }


        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }
        #endregion

        public override IEnumerable<string> Query(IEnumerable<string> list)
        {
            return list.OrderBy(n => Guid.NewGuid());
        }
    }

    public class QueryOrderBy:RawlerQuery
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerQuery Clone()
        {
            return base.Clone<QueryOrderBy>();
        }


        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }
        #endregion

        public RawlerBase KeyTree { get; set; }

        public override IEnumerable<string> Query(IEnumerable<string> list)
        {
            if (KeyTree == null)
            {
                return list.OrderBy(n => n);
            }
            else
            {
                return list.OrderBy(n => RawlerBase.GetText(n, KeyTree));
            }
        }
    }

    public class QueryOrderByStringLength : RawlerQuery
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerQuery Clone()
        {
            return base.Clone<QueryOrderByStringLength>();
        }

        bool descending = false;

        public bool Descending
        {
            get { return descending; }
            set { descending = value; }
        }

        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }
        #endregion

        public override IEnumerable<string> Query(IEnumerable<string> list)
        {
            if (descending)
            {
                return list.OrderByDescending(n => n.Length);
            }
            else
            {
                return list.OrderBy(n => n.Length);
            }
        }
    }


    public class QueryTake : RawlerQuery
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerQuery Clone()
        {
            return base.Clone<QueryTake>();
        }


        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }
        #endregion
        public int Count { get; set; }

        public override IEnumerable<string> Query(IEnumerable<string> list)
        {                  
            if (Count > 0)
            {
                return list.Take(Count);
            }
            else
            {
                return list.Take(0);
            }
        }
    }

    public class QueryDistinct : RawlerQuery
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerQuery Clone()
        {
            return base.Clone<QueryDistinct>();
        }

        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }
        #endregion

        public override IEnumerable<string> Query(IEnumerable<string> list)
        {
            return list.Distinct();
        }



    }

    public class QuerySkip : RawlerQuery
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerQuery Clone()
        {
            return base.Clone<QuerySkip>();
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
        /// 数
        /// </summary>
        public int Num { get; set; }

        public override IEnumerable<string> Query(IEnumerable<string> list)
        {

            if (Num >= 0)
            {
                return list.Skip(Num);
            }
            else
            {
                ReportManage.ErrReport(new RawlerBase(), "QuerySkipのNumの値がありません。");
                return list;
            }
        }
    }

    public class QueryElementAt : RawlerQuery
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerQuery Clone()
        {
            return base.Clone<QueryElementAt>();
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
        /// 数
        /// </summary>
        public int Index { get; set; }

        public override IEnumerable<string> Query(IEnumerable<string> list)
        {
            if (Index >= 0 && list.Count()> Index)
            {
                List<string> l = new List<string>();
                l.Add(list.ElementAt(Index));
                return l;
            }
            else
            {
                ReportManage.ErrReport(new RawlerBase(), "QueryElementAtのIndexの値が範囲外です。");
                List<string> l = new List<string>();
                 return l;
            }
        }
    }

    /// <summary>
    /// 割り算をして、余りが一致するか？
    /// </summary>
    public class QuerySkipExtend : RawlerQuery
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerQuery Clone()
        {
            return base.Clone<QuerySkipExtend>();
        }

        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }
        #endregion

        private int quotient = 2;

        public int Quotient
        {
            get { return quotient; }
            set { quotient = value; }
        }
        int remainder = 0;

        public int Remainder
        {
            get { return remainder; }
            set { remainder = value; }
        }

        public override IEnumerable<string> Query(IEnumerable<string> list)
        {
            int count = 0;
            foreach (var item in list)
            {
                count++;
                if (count % Quotient == Remainder)
                {
                    yield return item;
                }
            }
        }
    }

    public class QueryAddCounter : RawlerQuery
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerQuery Clone()
        {
            return base.Clone<QueryAddCounter>();
        }

        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }
        #endregion

        public int StartCount { get; set; }
        private string separater = ":";

        public string Separater
        {
            get { return separater; }
            set { separater = value; }
        }

        public override IEnumerable<string> Query(IEnumerable<string> list)
        {
            int count = 1;
            foreach (var item in list)
            {
                yield return count+separater+item;
                count++;
            }
        }
    }


    public class QueryCreatePair : RawlerQuery
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerQuery Clone()
        {
            return base.Clone<QueryCreatePair>();
        }

        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }
        #endregion

        bool noOverlap = true;

        public bool NoOverlap
        {
            get { return noOverlap; }
            set { noOverlap = value; }
        }
        public override IEnumerable<string> Query(IEnumerable<string> list)
        {
            if (noOverlap)
            {
                int i = 1;
                foreach (var item in list.Take(list.Count()-1))
                {
                    foreach (var item2 in list.Skip(i))
                    {
                        yield return item + "\t" + item2;
                    }
                    i++;
                }
            }
            else
            {
                foreach (var item in list)
                {
                    foreach (var item2 in list)
                    {
                        if (item != item2)
                        {
                            yield return item + "\t" + item2;
                        }
                    }
                }
            }
        }
    }
}
