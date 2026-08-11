using college_events_desktop.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace college_events_desktop.View.Layers.Tables
{
    public partial class table_tuple_EventGroup_save : UserControl
    {
        #region Поля класса
        private readonly page_table_EventGroup_save _table;
        private EventGroups _group;
        private page_EventList_save _page;
        #endregion

        public table_tuple_EventGroup_save(page_EventList_save page, EventGroups group, page_table_EventGroup_save table)
        {
            InitializeComponent();
            _page = page;
            _group = group;
            _table = table;
            Loaded += Table_tuple_EventGroup_save_Loaded;
        }

        private void Table_tuple_EventGroup_save_Loaded(object sender, RoutedEventArgs e)
        {
            if (_group != null)
            {
                combobox_group.Text = _group.name;
                text_supervisor_name.Text = $"{_group.supervisorSurname} {_group.supervisorName} {_group.supervisorLastname}";
                edit_expectedListenersCount.Text = _group.expectedListenersCount.ToString();
                edit_expectedParticipantsCount.Text = _group.expectedParticipantsCount.ToString();
                edit_expectedSuperParticipantsCount.Text = _group.expectedSuperParticipantsCount.ToString();
            }
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            DoubleAnimation anim = new DoubleAnimation()
            {
                From = btn_delete_group.Width,
                To = 20,
                Duration = TimeSpan.FromMilliseconds(200),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };
            btn_delete_group.BeginAnimation(WidthProperty, anim);
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            DoubleAnimation anim = new DoubleAnimation()
            {
                From = btn_delete_group.Width,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(200),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            };
            btn_delete_group.BeginAnimation(WidthProperty, anim);
        }

        private void btn_delete_group_Click(object sender, RoutedEventArgs e)
        {

        }

        private void combobox_group_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void edit_actualCount_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added)
            {
                
            }
            else if (e.Action == ValidationErrorEventAction.Removed)
            {
                
            }
        }
}
}
