using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RawlerLib
{
    public class Io
    {
        public static string FilterStringCreate(string extend)
        {
            return "<> files (*.<>)|*.<>|All files (*.*)|*.*".Replace("<>", extend);
        }
    }
}
