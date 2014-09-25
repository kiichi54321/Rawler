using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RawlerLib
{

    public static class IO
    {
        public static string FilterStringCreate(string extend)
        {
            return "<> files (*.<>)|*.<>|All files (*.*)|*.*".Replace("<>", extend);
        }

        /// <summary>
        /// 連番のファイル名を出力する。predicateで条件指定もできる。
        /// </summary>
        /// <param name="baseFile"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static string GenerateFileName(string baseFile, Predicate<string> predicate)
        {
            var file = System.IO.Path.GetFileNameWithoutExtension(baseFile);
            var ext = System.IO.Path.GetExtension(baseFile);
            int count = 1;
            var r  = string.Empty;
            while(true)
            {
                r = file+"_"+count+ext;
                if (predicate(r)) break;
            }
            return r;
        }
    }
}
