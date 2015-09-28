using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rawler.Tool;

namespace RawlerTwitter
{
     class UserState:RawlerBase
    {
        public override void Run(bool runChildren)
        {
            var login = this.GetAncestorRawler().OfType<TwitterLogin>().First();
          //  login.Token.Statuses.Lookup()
            base.Run(runChildren);
        }
    }
}
