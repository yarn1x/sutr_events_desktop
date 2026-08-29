using college_events_desktop.DataModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace college_events_desktop.View.Layers.Events.Tables
{
    public partial class table_tuple_EventGroup_save : UserControl
    {
        #region Поля класса
        private readonly page_table_EventGroup_save _table;
        public EventGroups _group { get; private set; }
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
                Tag = _group.eventGroupId;
                combobox_group.Text = _group.name;
                text_supervisor_name.Text = $"{_group.supervisorSurname} {_group.supervisorName} {_group.supervisorLastname}";
                edit_expectedListenersCount.Text = _group.expectedListenersCount.ToString();
                edit_expectedParticipantsCount.Text = _group.expectedParticipantsCount.ToString();
                edit_expectedSuperParticipantsCount.Text = _group.expectedSuperParticipantsCount.ToString();
                //TODO: SAVE спросить про удаление строчек таблицы
                //MouseEnter += UserControl_MouseEnter;
                //MouseLeave += UserControl_MouseLeave;
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
            //TODO: SAVE спросить про удаление строчек таблицы
            //MouseEnter += UserControl_MouseEnter;
            //MouseLeave += UserControl_MouseLeave;
        }

        private void combobox_group_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //TODO: SAVE спросить про добавление новых строчек
        }

        private void edit_actualCount_Error(object sender, ValidationErrorEventArgs e)
        {
            //если ошибка валидации данных (допустим отрицательное число или символы недопустимые), то добавляем к счётчику ошибок +1
            if (e.Action == ValidationErrorEventAction.Added)
            {
                _page.validation_errors_count++;
            }
            //если ошибку исправили, убираем со счётчика -1
            else if (e.Action == ValidationErrorEventAction.Removed)
            {
                _page.validation_errors_count--;
            }
            //проверка валидности происходит в классе EventGroupsActualAttendances (там все условия валидности)
        }
    }
}
