using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using RawlerLib.MyExtend;
using RawlerView.Form.Core;
using Rawler.Tool;

namespace RawlerView.Form.Core
{
    public class FormParts
    {
        public string Help { get; set; }
        public Visibility HelpVisibility
        {
            get
            {
                if (string.IsNullOrEmpty(Help))
                {
                    return Visibility.Collapsed;
                }
                else
                {
                    return Visibility.Visible;
                }
            }
        }
        
        public RawlerBase Parent { get;private set; }
        public void SetParent(RawlerBase p)
        {
            this.Parent = p;
        }
 
    }

    public class BaseProperty : FormParts, System.ComponentModel.INotifyPropertyChanged
    {
        public string Key { get; set; }
        public string Name { get; set; }
        string _value;
        public string Value { get { return _value; } set { this._value = value; RaisePropertyChanged("Value"); } }

        bool doSave = true;

        public bool DoSave
        {
            get { return doSave; }
            set { doSave = value; }
        }


        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(name));
            }
        }
    }
}


namespace RawlerView.Form
{
  

    public class ImagePart:FormParts
    {
        string imageUrl = string.Empty;
        public string ImageUrl {
            get
            {
                return imageUrl.Convert(this.Parent);
            }
            set
            {
                imageUrl = value;
            }
        }
    }

    public class TextBlockPart:FormParts
    {
        string text = string.Empty;
        public string Text
        {
            get
            {
                return text.Convert(this.Parent);
            }
            set
            {
                text = value;
            }
        }
    }

    public class TextBoxPart:FormParts
    {
        string text = string.Empty;
        public string Text
        {
            get
            {
                return text.Convert(this.Parent);
            }
            set
            {
                text = value;
            }
        }
    }
    

 


    public class TextProperty:BaseProperty
    {
        int maxlines = 1;

        public int Lines
        {
            get { return maxlines; }
            set { maxlines = value; }
        }

        public bool AcceptsReturn
        {
            get
            {
                if(Lines>1)
                {
                    return true;
                }
                else { return false; }
            }
        }
    }

    public class IntProperty:BaseProperty
    {
        int intValue = -1;
        public int IntValue
        {
            get
            {
                if(intValue == -1)
                {
                    int.TryParse(Value, out intValue);
                }
                return intValue;
            }
            set
            {
                if( intValue !=value)
                {
                    intValue = value;
                    Value = value.ToString();
                    RaisePropertyChanged("IntValue");
                    RaisePropertyChanged("Vaule");
                }
            }
        }

        int max = 100;

        public int Max
        {
            get { return max; }
            set { max = value; }
        }
        int min = 0;

        public int Min
        {
            get { return min; }
            set { min = value; }
        }
    }

    public class FileProperty:BaseProperty
    {
        FileDialogType fileDialogType = FileDialogType.OpenFile;

        public FileDialogType FileDialogType
        {
            get { return fileDialogType; }
            set { fileDialogType = value; }
        }

        public RelayCommand Command
        {
            get
            {
                if(fileDialogType == Form.FileDialogType.OpenFile)
                {
                    return OpenFileCommand;
                }
                else
                {
                    return SaveFileCommand;
                }
            }
        }

        public bool MultiSelect { get; set; } = false;

        public string DefaultExt { get; set; }

        RelayCommand openFileCommand;
        protected RelayCommand OpenFileCommand
        {
            get
            {
                if (openFileCommand == null)
                {
                    openFileCommand = new RelayCommand(() =>
                    {
                        Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
                        openFileDialog.FileName = this.Value;
                        openFileDialog.DefaultExt = this.DefaultExt;
                        openFileDialog.Title = this.Name;
                        openFileDialog.Multiselect = MultiSelect;
                        if (openFileDialog.ShowDialog() == true)
                        {
                            this.Value = openFileDialog.FileNames.JoinText("\n");
                        }
                    });
                }
                return openFileCommand;
            }
        }

        RelayCommand saveFileCommand;
        protected RelayCommand SaveFileCommand
        {
            get
            {
                if (saveFileCommand == null)
                {
                    saveFileCommand = new RelayCommand(() =>
                    {
                        Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
                        saveFileDialog.FileName = this.Value;
                        saveFileDialog.DefaultExt = this.DefaultExt;
                        saveFileDialog.Title = this.Name;
                        if (saveFileDialog.ShowDialog() == true)
                        {
                            this.Value = saveFileDialog.FileName;
                        }
                    });
                }
                return saveFileCommand;
            }
        }
    }

    public enum FileDialogType
    {
        OpenFile,
        SaveFile,
    }

    public class FormPropertySelector : DataTemplateSelector
    {
        public override System.Windows.DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            if(item is BaseProperty)
            {
                return ((FrameworkElement)container).FindResource(item.GetType().Name) as DataTemplate;
            }
            if (item is FormParts)
            {
                return ((FrameworkElement)container).FindResource(item.GetType().Name) as DataTemplate;
            }

            return null;

        }
    }
}
