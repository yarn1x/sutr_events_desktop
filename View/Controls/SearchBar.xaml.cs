using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace college_events_desktop.View.Controls
{
    public partial class SearchBar : UserControl
    {
        public SearchBar()
        {
            InitializeComponent();
        }

        public string SearchText { get; set; }

        public event EventHandler<TextChangedEventArgs> TextChanged;

        private void search_gotFocus(object sender, RoutedEventArgs e)
        {
            TextBox edit_text = sender as TextBox;
            if (string.IsNullOrEmpty(edit_text.Text.Trim()))
                edit_placeholder.Text = string.Empty;
        }

        private void search_lostFocus(object sender, RoutedEventArgs e)
        {
            TextBox edit_text = sender as TextBox;

            if (string.IsNullOrEmpty(edit_text.Text.Trim()))
            {
                edit_placeholder.Text = "Поиск по содержанию";
            }
        }

        private void edit_searchbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchText = edit_searchbox.Text;
            TextChanged?.Invoke(this, e);
        }
    }
}
