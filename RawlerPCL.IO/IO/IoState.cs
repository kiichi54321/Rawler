using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PCLStorage;

namespace Rawler.IO
{
    public static class IoState
    {
        public static IFolder CurrentFolder { get; set; } = FileSystem.Current.LocalStorage;
        
        public static async Task<IFile> GetFileAsync(string filename)
        {
            var directory = System.IO.Path.GetDirectoryName(filename);
            if(directory == string.Empty)
            {
                return await CurrentFolder.GetFileAsync(filename);
            }
            else
            {
                return await FileSystem.Current.GetFileFromPathAsync(filename);
            }
        }

        internal static bool FileExists(string file)
        {
            var t = CurrentFolder.CheckExistsAsync(file);
            t.Wait();
            return t.Result.HasFlag(ExistenceCheckResult.FileExists);
        }

        internal static Task<IFile> GetFileAsync(string filename, FileSaveMode fileSaveMode)
        {
            return CurrentFolder.CreateFileAsync(filename, CreationCollisionOption.GenerateUniqueName);
        }
    }
}
