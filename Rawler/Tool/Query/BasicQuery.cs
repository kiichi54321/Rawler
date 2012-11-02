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

        public string ContainText { get; set; }
        private bool result = true;

        public bool Result
        {
            get { return result; }
            set { result = value; }
        }
        
        public override IEnumerable<string> Query(IEnumerable<string> list)
        {
            if (string.IsNullOrEmpty(ContainText) == false)
            {
                return list.Where(n => n.Contains(ContainText) == Result);
            }
            else
            {
                ReportManage.ErrReport(null, "ContainTextが空です。");
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

        public override IEnumerable<string> Query(IEnumerable<string> list)
        {
            return list.OrderBy(n => n);
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
