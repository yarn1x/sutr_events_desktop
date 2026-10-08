using college_events_desktop.DataModels;
using college_events_desktop.Model;
using college_events_desktop.Services;
using System;
using System.Collections.Generic;
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
        private bool isUserAdded;
        #endregion


        #region Конструктор
        public table_tuple_EventGroup_save(page_EventList_save page, EventGroups group, page_table_EventGroup_save table, bool isUserAdded)
        {
            InitializeComponent();
            _page = page;
            _group = group;
            _table = table;
            Loaded += OnLoaded;
            this.isUserAdded = isUserAdded;
        }
        #endregion


        #region Обработчики событий
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            edit_actualListenersCount.TextChanged += edit_actualCount_TextChanged;
            edit_actualParticipantsCount.TextChanged += edit_actualCount_TextChanged;
            edit_actualSuperParticipantsCount.TextChanged += edit_actualCount_TextChanged;

            if (_group != null)
            {
                Tag = _group.eventGroupId;
                combobox_group.Text = _group.name;
                text_supervisor_name.Text = $"{_group.supervisorSurname} {_group.supervisorName} {_group.supervisorLastname}";
                edit_expectedListenersCount.Text = _group.expectedListenersCount.ToString();
                edit_expectedParticipantsCount.Text = _group.expectedParticipantsCount.ToString();
                edit_expectedSuperParticipantsCount.Text = _group.expectedSuperParticipantsCount.ToString();

            }
            if (isUserAdded)
            {
                MouseEnter += UserControl_MouseEnter;
                MouseLeave += UserControl_MouseLeave;

            }

            RecalculateScore();
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
            var callback = MessageBox.Show(
                $"Хотите удалить группу {combobox_group.Text} из списка?",
                "Удаление",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question,
                MessageBoxResult.No);

            if (callback == MessageBoxResult.Yes)
            {
                _table.stack_table_rows.Children.Remove(this);
            }
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

        private void edit_actualCount_TextChanged(object sender, TextChangedEventArgs e)
        {
            RecalculateScore();
        }

        #endregion


        #region Методы класса


        /// <summary>
        /// Безопасно пересчитывает балл мероприятия.
        /// Не бросает исключений при невалидных значениях — просто пропускает пересчёт.
        /// </summary>
        private void RecalculateScore()
        {
            if (_group == null) return;

            // Парсим значения; при неудаче — не пересчитываем (пользователь ещё вводит)
            if (!int.TryParse(edit_actualListenersCount.Text, out int listeners) ||
                !int.TryParse(edit_actualParticipantsCount.Text, out int participants) ||
                !int.TryParse(edit_actualSuperParticipantsCount.Text, out int superParticipants))
            {
                return;
            }

            // Защита от null в словаре критериев
            if (!_table._dataService.criteriaDict.TryGetValue(
                    CriteriaConstants.listener_score_id, out var listenerCriterion) ||
                !_table._dataService.criteriaDict.TryGetValue(
                    CriteriaConstants.participant_score_id, out var participantCriterion) ||
                !_table._dataService.criteriaDict.TryGetValue(
                    CriteriaConstants.superparticipant_score_id, out var superParticipantCriterion))
            {
                UserNotificationService.ShowError(
                    "Ошибка подсчёта баллов",
                    new KeyNotFoundException("Не найдены критерии для подсчёта баллов"),
                    "table_tuple_EventGroup_editAA002");
                return;
            }

            try
            {
                int studentsCount = _group.studentsCount;

                text_score.Text = GroupScoreService.GetGroupScore(
                    listeners,
                    participants,
                    superParticipants,
                    listenerCriterion,
                    participantCriterion,
                    superParticipantCriterion,
                    studentsCount
                ).ToString();
            }
            catch (Exception ex)
            {
                UserNotificationService.ShowError(
                    "Ошибка подсчёта баллов", ex, "table_tuple_EventGroup_editAA001");
            }
        }


        #endregion

        
    }
}
