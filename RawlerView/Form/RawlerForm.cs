using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rawler.Tool;
using RawlerLib.MyExtend;
using System.Windows.Controls;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using System.Threading.Tasks;
using System.Threading;

namespace RawlerView.Form
{
    public class FormProperties:List<BaseProperty>
    {

    }

    public class RawlerForm:Rawler.Tool.KeyValueStore
    {
        FormProperties properties = new  FormProperties();
        public FormProperties Properties { get { return properties; }  }
        public string SettingFileName { get; set; }

        public override void Run(bool runChildren)
        {
            LoadSetting();
            bool flag = true;
            Task reportProgressTask = Task.Factory.StartNew(() =>
              {
                  FormWindow fw = new FormWindow();
                  fw.SetUp(Properties);
                  if (fw.ShowDialog() == true)
                  {
                      PropertiesUpdate();
                      SaveSetting();
                  }
                  else
                  {
                      flag = false;
                  }

              },
       CancellationToken.None,
       TaskCreationOptions.None,
       RawlerLib.UIData.UISyncContext);
            reportProgressTask.Wait();
            if (flag)
            {
                base.Run(runChildren);
            }
            else
            {
                ReportManage.Report(this, "Formがキャンセルされました。終了します。", true, true);
            }
        }




        void PropertiesUpdate()
        {
            foreach (var item in Properties)
            {
                this.SetKeyValue(item.Key, item.Value);
            }
        }


        void LoadSetting()
        {
            if(string.IsNullOrEmpty( SettingFileName) == false)
            {
                if(System.IO.File.Exists(SettingFileName))
                {
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    foreach (var item in System.IO.File.ReadLines(SettingFileName))
                    {
                        var d = item.Split('\t');
                        if(d.Length > 1) dic.Add(d[0], d[1]);
                    }

                    foreach (var item in Properties)
                    {
                        if(dic.ContainsKey( item.Key))
                        {
                            item.Value = dic[item.Key];
                        }
                    }
                }
            }
        }

        void SaveSetting()
        {
            if (string.IsNullOrEmpty(SettingFileName) == false)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                if (System.IO.File.Exists(SettingFileName))
                {
                    foreach (var item in System.IO.File.ReadLines(SettingFileName))
                    {
                        var d = item.Split('\t');
                        if (d.Length > 1) dic.Add(d[0], d[1]);
                    }
                }
                foreach (var item in Properties)
                {
                    if (dic.ContainsKey(item.Key))
                    {
                        dic[item.Key] = item.Value;
                    }
                    else
                    {
                        dic.Add(item.Key, item.Value);
                    }
                }

                using (var f = System.IO.File.CreateText(SettingFileName))
                {
                    dic.ToList().ForEach(n => f.WriteLine(n.Key + "\t" + n.Value));
                }
            }
        }
    }
}
