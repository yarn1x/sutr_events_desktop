using college_events_desktop.DataModels;
using college_events_desktop.Services;
using college_events_desktop.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace college_events_desktop.View.Layers.Users
{
    public partial class control_supervisor_group_tuple : UserControl
    {

        page_UserAccount _page;

        public control_supervisor_group_tuple(page_UserAccount page)
        {
            InitializeComponent();
            _page = page;

            Loaded += Control_supervisor_group_tuple_Loaded;
        }

        private void Control_supervisor_group_tuple_Loaded(object sender, RoutedEventArgs e)
        {
            combobox_groups.SelectedIndex = -1;
            combobox_groups.ItemsSource = _page._groups.Select(g => g.groupName);   
        }

        private void btn_delete_Click(object sender, RoutedEventArgs e)
        {
            _page.stack_groups.Children.Remove(this);
        }

        private void combobox_groups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (combobox_groups.SelectedIndex != -1)
            {
                _page.stack_groups.Children.Add(new control_supervisor_group_tuple(_page));
            }
        }
    }
}
