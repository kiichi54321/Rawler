using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rawler.Core
{
    public interface IFileBase
    {
        RawlerBase Parent { get; set; }
        void WriteLines(IEnumerable<string> lines);
        void InitFile();
        void SequentialWriteLine(string line);
        void Close();
    }

    public interface IFolderBase
    {
        RawlerBase Parent { get; set; }
        void WriteFile(string fileName, byte[] data);
    }


}
