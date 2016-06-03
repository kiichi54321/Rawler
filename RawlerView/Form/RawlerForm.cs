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
using RawlerView.Form.Core;

namespace RawlerView.Form
{
    public class FormProperties:List<FormParts>
    {

    }

    public class RawlerForm:Rawler.Tool.KeyValueStore
    {
        FormProperties properties = new  FormProperties();
        public FormProperties Properties { get { return properties; }  }
        public string SettingFileName { get; set; }
        public string Title { get; set; } = string.Empty;

        public override void Run(bool runChildren)
        {
            LoadSetting();
            bool flag = true;
            Properties.ForEach(n => n.SetParent(this));

            Task reportProgressTask = Task.Factory.StartNew(() =>
              {
                  FormWindow fw = new FormWindow();
                  fw.Title = Title.Convert(this);
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
            foreach (var item in Properties.OfType<BaseProperty>())
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
                    var dic = KeyValueDic.Load(SettingFileName);
                    if(dic !=null)
                    {
                        foreach (var item in Properties.OfType<BaseProperty>())
                        {
                            if (dic.ContainsKey(item.Key))
                            {
                                item.Value = dic[item.Key];
                            }
                        }
                    }
                }
            }
        }

        void SaveSetting()
        {
            if (string.IsNullOrEmpty(SettingFileName) == false)
            {

                KeyValueDic dic = new KeyValueDic();
                if (System.IO.File.Exists(SettingFileName))
                {
                    dic = KeyValueDic.Load(SettingFileName);
                    //foreach (var item in System.IO.File.ReadLines(SettingFileName))
                    //{
                    //    var d = item.Split('\t');
                    //    if (d.Length > 1) dic.Add(d[0], d[1]);
                    //}
                }
                if (dic == null) dic = new KeyValueDic();
                foreach (var item in Properties.OfType<BaseProperty>().Where(n=>n.DoSave==true))
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
                dic.Save(SettingFileName);
            }
        }
    }

    public class KeyValueDic:Dictionary<string,string>
    {
        public static KeyValueDic Load(string file)
        {
            try
            {
                var obj = System.Xaml.XamlServices.Load(file);
                if (obj is KeyValueDic)
                {
                    return (KeyValueDic)obj;
                }
            }
            catch(Exception)
            {
                
            }
            return null;
        }

        public void Save(string file)
        {
            System.Xaml.XamlServices.Save(file,this);
        }
    }
}
