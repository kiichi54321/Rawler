using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rawler.Tool;

namespace Rawler.Tool
{
    public class CaseDateTime :CaseBase
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<CaseDateTime>(parent);
        }

        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }
        #endregion
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        private bool errReport = false;

        public bool ErrReport
        {
            get { return errReport; }
            set { errReport = value; }
        }

        public override bool Check(string txt)
        {
            DateTime dt;

            if (StartDate == null && EndDate == null)
            {
                return false;
            }

            if (DateTime.TryParse(txt, out dt))
            {
                if (StartDate == null && EndDate != null)
                {
                    if (dt < EndDate)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (StartDate != null && EndDate != null)
                {
                    if (dt >= StartDate && dt < EndDate)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    if (dt >= StartDate)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }


            }
            else
            {
                if (ErrReport)
                {
                    ReportManage.ErrReport(this, "DateTime型のキャストに失敗："+txt);
                }
                return false;
            }

//            return base.Check(txt);
        }

        /// <summary>
        /// このクラスでの実行すること。
        /// </summary>
        /// <param name="runChildren"></param>
        public override void Run(bool runChildren)
        {
            base.Run(runChildren);
        }

        /// <summary>
        /// 子が参照するテキスト。
        /// </summary>
        public override string Text
        {
            get
            {
                return GetText();
            }
        }


    }
}
