using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RawlerView.WPF
{
    /// <summary>
    /// RawlerParameterControl.xaml の相互作用ロジック
    /// </summary>
    public partial class RawlerParameterControl : UserControl,IGroupParameterValue
    {
        public RawlerParameterControl()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(RawlerParameterControl_Loaded);
        }

        void RawlerParameterControl_Loaded(object sender, RoutedEventArgs e)
        {
            RawlerRunMange.AddParameterObjcet(this);
        }

        public TextBlock TextBlock
        {
            get
            {
                return textBlock;
            }
        }

        public TextBox TextBox
        {
            get
            {
                return textBox;
            }
        }

       
        public string Group { get; set; }
        protected string parameterName = string.Empty;
        public string ParameterName {
            get { return parameterName; }
            set
            {
                parameterName = value;
                if (ParameterText.Count == 0)
                {
                    TextBlock t = new System.Windows.Controls.TextBlock();
                    t.Text = value;
                    ParameterText = t.Inlines; 
                }
            }
        }
        public InlineCollection ParameterText
        {
            get
            {
                return this.textBlock.Inlines;
            }
            set
            {
                this.textBlock.Inlines.Clear();
                foreach (var item in value)
                {
                    this.textBlock.Inlines.Add(item);
                }
            }
        }

        public Func<string, string> TextConveter;
        

        public string Value
        {
            get
            {
                string t = this.textBox.Text;
                if (TextConveter != null)
                {
                    t = TextConveter(t);
                }
                return t;
            }
            set
            {
                this.textBox.Text = value;
            }
        }

       
    }

 

    public class RawlerCommonParameterControl:RawlerParameterControl
    {
        public RawlerCommonParameterControl()
        {
            this.Group = "Common";
        }
    }
}
