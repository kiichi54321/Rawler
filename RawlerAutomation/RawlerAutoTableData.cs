using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RawlerLib.MyExtend;
using RawlerLib.Extend;
using Rawler.Tool;

namespace RawlerExpressLib.Automation
{
    public enum TableType {Normal,CrossTable}

    public class RawlerAutoTable:Rawler.Tool.RawlerBase
    {
        public override void Run(bool runChildren)
        {
            RawlerLib.Timer.StopWatch.Write("RawlerAutoTable Create");

            var html = GetText();
            var tables = html.ToHtml().GetTag("table");
            var dic = this.GetDescendantRawler().OfType<Tags>().ToDictionary(n => n.ParameterFilter, n => n);

            foreach (var item in tables.GroupBy(n=>n.Parameter))
            {
                if (dic.ContainsKey(item.Key) == false)
                {
                    this.Add(new Data()).Add(new GetPageHtml()).Add(new Tags() { Tag = "table", ParameterFilter = item.Key }).Add(new RawlerAutoTableDataWrite() { AppendTree = new DataWriteSoruceUrl() });
                }
            }

            RawlerLib.Timer.StopWatch.Write("RawlerAutoTable  autoNextLink.Run");

            base.Run(runChildren);
            RawlerLib.Timer.StopWatch.Write("RawlerAutoTable  autoNextLink.Run End");
        }
    }


    public class RawlerAutoTableDataWrite:Rawler.Tool.RawlerBase
    {
        public RawlerBase AppendTree { get; set; }

        TableType readTableType = TableType.Normal;

        public TableType ReadTableType
        {
            get { return readTableType; }
            set { readTableType = value; }
        }

        int keyColumn = 0;

        public int KeyColumn
        {
            get { return keyColumn; }
            set { keyColumn = value; }
        }

        public override void Run(bool runChildren)
        {
            var html = GetText();
            var tr = html.ToHtml().GetTag("tr");
            var data = this.GetUpperRawler<Data>();
            Page page = this.GetUpperRawler<Page>();
            var url = "";
            if(page !=null) url = page.GetCurrentUrl();
            if (AppendTree != null) AppendTree.SetParent(this);

            if (readTableType == Automation.TableType.Normal)
            {
                //初めの行にTHがあり、TDがないとき
                if (tr.First().Inner.ToHtml().GetTag("th").Any() && tr.First().Inner.ToHtml().GetTag("td").Any() == false)
                {
                    var header = tr.First().Inner.ToHtml().GetTag("th").Select(n => n.Inner).ToArray();
                    foreach (var item in tr.Where(n => n.Inner.ToHtml().GetTag("th").Count() != header.Count()))
                    {
                        foreach (var th in item.Inner.ToHtml().GetTag("th").Select((n, id) => new { n.Inner, id }))
                        {
                            RawlerAutoDataLib.AutoHtmlDataWrite(data, "th_" + th.id, th.Inner, url);
                        }

                        var line = item.Inner.ToHtml().GetTag("td").Select(n => n.Inner).ToArray();
                        for (int i = 0; i < Math.Min(header.Length, line.Length); i++)
                        {
                            RawlerAutoDataLib.AutoHtmlDataWrite(data, header[i], line[i], url);
                        }
                        if (AppendTree != null) AppendTree.Run();
                        data.NextDataRow();
                    }
                }
                //初めの行にTHとTDが両方あるとき
                else if (tr.First().Inner.ToHtml().GetTag("th").Any() && tr.First().Inner.ToHtml().GetTag("td").Any())
                {
                    foreach (var item in tr.Select(n => n.Inner))
                    {
                        var attribute = item.ToHtml().GetTag("th").First().Inner;
                        foreach (var td in item.ToHtml().GetTag("td").Select((n, id) => new { n.Inner, id }))
                        {
                            RawlerAutoDataLib.AutoHtmlDataWrite(data, attribute + "_" + td.id, td.Inner, url);
                        }
                    }
                    if (AppendTree != null) AppendTree.Run();
                    data.NextDataRow();
                }
                else
                {
                    foreach (var item in tr)
                    {
                        foreach (var th in item.Inner.ToHtml().GetTag("th").Select((n, id) => new { n.Inner, id }))
                        {
                            RawlerAutoDataLib.AutoHtmlDataWrite(data, "th_" + th.id, th.Inner, url);
                        }

                        foreach (var td in item.Inner.ToHtml().GetTag("td").Select((n, id) => new { n.Inner, id }))
                        {
                            RawlerAutoDataLib.AutoHtmlDataWrite(data, "col_" + td.id, td.Inner, url);
                        }

                        if (AppendTree != null) AppendTree.Run();
                        data.NextDataRow();
                    }
                }
            }
            if(readTableType == Automation.TableType.CrossTable)
            {
                //初めの行にTHがあり、TDがないとき
                if (tr.First().Inner.ToHtml().GetTag("th").Any() && tr.First().Inner.ToHtml().GetTag("td").Any() == false)
                {
                    var header = tr.First().Inner.ToHtml().GetTag("th").Select(n => n.Inner).ToArray();
                    foreach (var item in tr.Where(n => n.Inner.ToHtml().GetTag("th").Count() != header.Count()))
                    {

                        //
                        foreach (var th in item.Inner.ToHtml().GetTag("th").Select((n, id) => new { n.Inner, id }))
                        {
                            RawlerAutoDataLib.AutoHtmlDataWrite(data, "th_" + th.id, th.Inner, url);
                        }

                        var line = item.Inner.ToHtml().GetTag("td").Select(n => n.Inner).ToArray();
                        var key = line.ElementAt(keyColumn);
                        var keyHeader = header.ElementAt(keyColumn);
                        for (int i = 0; i < Math.Min(header.Length, line.Length); i++)
                        {
                            if (i != keyColumn)
                            {
                                RawlerAutoDataLib.AutoHtmlDataWrite(data, keyHeader ,key , url);
                                RawlerAutoDataLib.AutoHtmlDataWrite(data, "Column", header[i], url);
                                RawlerAutoDataLib.AutoHtmlDataWrite(data, "Value", line[i], url);
                                if(AppendTree != null) AppendTree.Run();
                                data.NextDataRow();
                            }
                        }
                        //if (AppendTree != null) AppendTree.Run();
                        //data.NextDataRow();
                    }
                }
                ////初めの行にTHとTDが両方あるとき
                //else if (tr.First().Inner.ToHtml().GetTag("th").Any() && tr.First().Inner.ToHtml().GetTag("td").Any())
                //{
                //    foreach (var item in tr.Select(n => n.Inner))
                //    {
                //        var attribute = item.ToHtml().GetTag("th").First().Inner;
                //        foreach (var td in item.ToHtml().GetTag("td").Select((n, id) => new { n.Inner, id }))
                //        {
                //            RawlerAutoDataLib.AutoHtmlDataWrite(data, attribute + "_" + td.id, td.Inner, url);
                //        }
                //    }
                //    if (AppendTree != null) AppendTree.Run();
                //    data.NextDataRow();
                //}
                //else
                //{
                //    foreach (var item in tr)
                //    {
                //        foreach (var th in item.Inner.ToHtml().GetTag("th").Select((n, id) => new { n.Inner, id }))
                //        {
                //            RawlerAutoDataLib.AutoHtmlDataWrite(data, "th_" + th.id, th.Inner, url);
                //        }

                //        foreach (var td in item.Inner.ToHtml().GetTag("td").Select((n, id) => new { n.Inner, id }))
                //        {
                //            RawlerAutoDataLib.AutoHtmlDataWrite(data, "col_" + td.id, td.Inner, url);
                //        }

                //        if (AppendTree != null) AppendTree.Run();
                //        data.NextDataRow();
                //    }
                //}

            }

            base.Run(runChildren);
        }
    }
}
