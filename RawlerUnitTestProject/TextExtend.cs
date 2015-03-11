using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RawlerExpressLib.TestExtend
{
    public static class TestExtend
    {
        public static IEnumerable<T> ConsoleWriteLine<T>(this IEnumerable<T> list, Func<T, string> func)
        {
            foreach (var item in list)
            {
                System.Console.WriteLine(func(item));
            }
            return list;
        }

        public static string ConsoleWriteLine(this string text)
        {
            System.Console.WriteLine(text);
            return text;
        }

        public static IEnumerable<T> ConsoleWriteLine<T>(this IEnumerable<T> list)
        {
            foreach (var item in list)
            {
                System.Console.WriteLine(item.ToString());
            }
            return list;
        }

        public static IEnumerable<T> FileWriteLine<T>(this IEnumerable<T> list, string fileName, Func<T, string> func)
        {
            using (var file = System.IO.File.CreateText(fileName))
            {
                foreach (var item in list)
                {
                    try
                    {
                        file.Encoding.GetBytes(func(item));
                        file.WriteLine(func(item));
                    }
                    catch { }
                }
            }
            return list;
        }
    }
}
