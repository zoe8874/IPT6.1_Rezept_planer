using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Wpf
{
    /// <summary>
    /// Interaktionslogik für Kulschrakübersicht.xaml
    /// </summary>
    public partial class Kulschrakübersicht : Window
    {
      // List<Zutaten>mienkulschrkan;
        public Kulschrakübersicht()
        {
            InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var text = SearchTextBox.Text?.Trim();
            if (string.IsNullOrEmpty(text))
                return;

            // Add to ComboBox suggestions so the entry is available next time
            if (!SearchTextBox.Items.Contains(text))
            {
                SearchTextBox.Items.Add(text);
            }

            SearchTextBox.Text = string.Empty;
            SearchTextBox.Focus();
        }


    }
}
