using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RawlerConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length>0)
            {
                try
                {
                    foreach (var item in args.Skip(1))
                    {
                        var d = item.Split('=');
                        if (d.Length > 1)
                        {
                            Rawler.Tool.TempVar.SetVar(d[0], d[1]);
                        }
                    }
                    Rawler.Tool.RawlerBase rawler = (Rawler.Tool.RawlerBase)System.Xaml.XamlServices.Load(args[0]);
                    rawler.Run();
                }
                catch(Exception e)
                {
                    System.Console.WriteLine(e.ToString());
                }
            }
        }
    }
}
