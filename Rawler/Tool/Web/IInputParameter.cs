using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rawler.Tool
{
    /// <summary>
    /// Get Post用のパラメータをセットする
    /// </summary>
    public interface IInputParameter
    {
        /// <summary>
        /// パラメータ追加する
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void AddParameter(string key, string value);
        void ReplaceParameter(string key, string value);
    }
}
