using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rawler.Core;
using System.IO;

namespace Rawler.IO
{
    public class DownLoadFile : Rawler.Core.IFileBase
    {
        public RawlerBase Parent
        {
            get;
            set;
        }

        public void Close()
        {
            stream.Dispose();
        }

        public string FileName { get; set; }
        StreamWriter stream;
        public async void InitFile()
        {
            var f = await Windows.Storage.DownloadsFolder.CreateFileAsync(FileName.Convert(Parent), Windows.Storage.CreationCollisionOption.GenerateUniqueName);
            var f1 = await f.OpenTransactedWriteAsync();
            stream = new StreamWriter(f1.Stream.AsStreamForWrite());
        }

        public void SequentialWriteLine(string line)
        {
            stream.WriteLine(line);
        }

        public void WriteLines(IEnumerable<string> lines)
        {
            foreach (var item in lines)
            {
                stream.WriteLine(item);
            }
        }
    }
}
