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
        private readonly page_UserAccount _page;
        private bool _isInitialized;
        private Group _preselectedGroup;
        private int _preselectedStudentsCount;

        public Group SelectedGroup => combobox_groups.SelectedItem as Group;
        public int StudentsCount => int.TryParse(edit_student_count.Text, out var n) ? n : 0;

        public control_supervisor_group_tuple(page_UserAccount page)
        {
            InitializeComponent();
            _page = page;
            Loaded += Control_supervisor_group_tuple_Loaded;
        }

        public control_supervisor_group_tuple(page_UserAccount page, Group group, int studentsCount)
            : this(page)
        {
            _preselectedGroup = group;
            _preselectedStudentsCount = studentsCount;
        }

        private void Control_supervisor_group_tuple_Loaded(object sender, RoutedEventArgs e)
        {
            combobox_groups.ItemsSource = _page._groups;
            combobox_groups.DisplayMemberPath = "groupName";

            if (_preselectedGroup != null)
            {
                var match = _page._groups.FirstOrDefault(g => g.groupId == _preselectedGroup.groupId);
                combobox_groups.SelectedItem = match;
                edit_student_count.Text = _preselectedStudentsCount.ToString();
            }
            else
            {
                combobox_groups.SelectedIndex = -1;
            }

            _isInitialized = true;
        }

        private void combobox_groups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInitialized) return;
            if (combobox_groups.SelectedIndex == -1) return;

            // Если это последний кортеж — добавим ещё один пустой
            if (_page.stack_groups.Children.IndexOf(this) == _page.stack_groups.Children.Count - 1)
                _page.AddGroupTuple();
        }

        private void btn_delete_Click(object sender, RoutedEventArgs e)
        {
            _page.stack_groups.Children.Remove(this);
        }
    }
}
