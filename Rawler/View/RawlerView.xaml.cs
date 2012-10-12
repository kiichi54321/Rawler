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
using System.Reflection;

namespace Rawler.View
{
    /// <summary>
    /// RawlerView.xaml の相互作用ロジック
    /// </summary>
    public partial class RawlerView : UserControl
    {
        /// <summary>
        /// Rawlerのビューワー
        /// </summary>
        public RawlerView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Rawler をセットする。
        /// </summary>
        /// <param name="rawler"></param>
        public void SetRawler(Tool.RawlerBase rawler)
        {
            var list =  new List<Tool.RawlerBase>();
            list.Add(rawler);
            treeView1.ItemsSource = list;
            rootRawler = rawler;
            int i = 0;
            foreach (var item in rawler.GetDescendantRawler())
            {
                item.SetId(i);
                i++;
                item.EndRunEvent += new EventHandler(item_EndRunEvent);
            }
        }



        Rawler.Tool.RawlerBase rootRawler = null;

        public Rawler.Tool.RawlerBase GetRawler()
        {
            return rootRawler;
        }

        void item_EndRunEvent(object sender, EventArgs e)
        {
            while (true)
            {
                var rawler = sender as Rawler.Tool.RawlerBase;
                if (nextMove)
                {
                    break;
                }
                if (stopHash.Contains(rawler.GetId()))
                {
                    System.Threading.Thread.Sleep(1000);
                }
                else
                {
                    break;
                }
            }
            nextMove = false;
        }
        bool nextMove = false;
        HashSet<int> stopHash = new HashSet<int>();

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var check = sender as CheckBox;
            var tag = check.Tag as Tool.RawlerBase;
            if (stopHash.Contains(tag.GetId()))
            {
                stopHash.Remove(tag.GetId());
            }
            else
            {
                stopHash.Add(tag.GetId());
            }
        }

        //private void button1_Click(object sender, RoutedEventArgs e)
        //{
        //    comboBox1.ItemsSource = Rawler.Tool.ViewManage.Keys;
        //    listBox1.ItemsSource = Rawler.Tool.ViewManage.GetValue(comboBox1.SelectedItem.ToString());
        //}

        //private void comboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{  
        //    listBox1.ItemsSource = Rawler.Tool.ViewManage.GetValue(comboBox1.SelectedItem.ToString());
        //}

        //private void listBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //}

        //private void ContextMenu_MouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    var list = Rawler.Tool.ViewManage.GetValue(comboBox1.SelectedItem.ToString());
        //    StringBuilder str = new StringBuilder();

        //    foreach (var item in list)
        //    {
        //        str.AppendLine(item);
        //        str.AppendLine("-----");
        //        str.AppendLine();
        //    }
        //    Clipboard.SetText(str.ToString());
        //}

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            popup.PlacementTarget = sender as UIElement;
            popup.IsOpen = !popup.IsOpen;
        //    propertyGrid.SelectedObject = (sender as Button).Tag;
            var obj =  (sender as Button).Tag;
 //           rawlerLabel.Content = (obj as Rawler.Tool.RawlerBase).ObjectName;
        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void TextBlock_MouseDown_1(object sender, MouseButtonEventArgs e)
        {

        }

        private void TextBlock_MouseDown_2(object sender, MouseButtonEventArgs e)
        {
            popup2.PlacementTarget = sender as UIElement;
            popup2.IsOpen = true;
        }

        private void TextBlock_MouseDown_3(object sender, MouseButtonEventArgs e)
        {

        }

        private void TextBlock_MouseDown_4(object sender, MouseButtonEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            popup2.PlacementTarget = sender as UIElement;
            popup2.IsOpen = !popup2.IsOpen;
            rawlerListBox.ItemsSource = GetRawlerClass();
          
        }

        private List<string> GetRawlerClass()
        {
            List<string> list = new List<string>();


            foreach (var assem in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var t in assem.GetExportedTypes())
                {
                    if (t.IsSubclassOf(typeof(Rawler.Tool.RawlerBase)))
                    {
                        list.Add(t.Name);
                    }

                }
            }
            list.Sort();
            return list;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textbox = sender as TextBox;
            var list = GetRawlerClass().Where(n => n.ToLower().Contains(textbox.Text.ToLower()));
            if(list.Count()>3)
            {
                rawlerListBox.ItemsSource = list;
            }
        }

        private void rawlerListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(e.AddedItems.Count>0)
            {
                kohoTextbox.Text = e.AddedItems[0].ToString();
            }
        }

        private void kohoTextbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                rawlerListBox.Focus();
            }
        }

        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            
        }

        private void StackPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var obj = (sender as Button).Tag;
            PropertyGrid1.Instance = obj;
 
        }

        private void treeView1_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            PropertyGrid1.Instance = treeView1.SelectedItem;
        }
    }

    class Single2ListConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            List<object> list = new List<object>();
            list.Add(value);
            return list;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is IEnumerable<object>)
            {
                return ((IEnumerable<object>)value).First();
            }
            else
            {
                return null;
            }
        }
    }


}
