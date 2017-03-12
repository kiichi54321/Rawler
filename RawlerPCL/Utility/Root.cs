using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rawler
{
    /// <summary>
    /// Rootに配置するもの。
    /// </summary>
    public class Root:KeyValueStore
    {
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
