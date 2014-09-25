using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rawler.Tool;

namespace Rawler.Tool
{
    public class CaseInt : CaseBase
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<CaseInt>(parent);
        }

        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }
        #endregion

        public int? Start { get; set; }
        public int? End { get; set; }

        private bool errReport = false;

        public bool ErrReport
        {
            get { return errReport; }
            set { errReport = value; }
        }

        public override bool Check(string txt)
        {
            int dt;

            if (Start == null && End == null)
            {
                return false;
            }

            if (int.TryParse(txt, out dt))
            {
                if (Start == null && End != null)
                {
                    if (dt < End)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (Start != null && End != null)
                {
                    if (dt >= Start && dt < End)
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
                    if (dt >= Start)
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
                    ReportManage.ErrReport(this, "int型のキャストに失敗：" + txt);
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
