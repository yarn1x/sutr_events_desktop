using college_events_desktop.DataModels;
using college_events_desktop.Model;
using college_events_desktop.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace college_events_desktop.View.Layers.Events.Tables
{
    public partial class table_tuple_EventGroup_edit : UserControl
    {
        #region Поля класса
        private readonly page_EventList_edit _page;
        private readonly page_table_EventGroup_edit _table;
        private bool _isInitialized;

        public EventGroups _group { get; private set; }
        #endregion

        #region Конструктор
        public table_tuple_EventGroup_edit(page_EventList_edit page, EventGroups group, object table)
        {
            InitializeComponent();
            _page = page;
            _group = group;
            _table = table as page_table_EventGroup_edit;

            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }
        #endregion

        #region Обработчики событий

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            MouseEnter += OnMouseEnter;
            MouseLeave += OnMouseLeave;

            edit_expectedListenersCount.TextChanged += OnExpectedCountTextChanged;
            edit_expectedParticipantsCount.TextChanged += OnExpectedCountTextChanged;
            edit_expectedSuperParticipantsCount.TextChanged += OnExpectedCountTextChanged;

            if (_group != null)
            {
                Tag = _group;
                combobox_group.Text = _group.name;
                text_supervisor_name.Text =
                    $"{_group.supervisorSurname} {_group.supervisorName} {_group.supervisorLastname}";
                edit_expectedListenersCount.Text = _group.expectedListenersCount.ToString();
                edit_expectedParticipantsCount.Text = _group.expectedParticipantsCount.ToString();
                edit_expectedSuperParticipantsCount.Text = _group.expectedSuperParticipantsCount.ToString();
            }

            LoadGroups();
            _isInitialized = true;

            RecalculateScore();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            // Отписываемся, чтобы не держать ссылки
            MouseEnter -= OnMouseEnter;
            MouseLeave -= OnMouseLeave;
            edit_expectedListenersCount.TextChanged -= OnExpectedCountTextChanged;
            edit_expectedParticipantsCount.TextChanged -= OnExpectedCountTextChanged;
            edit_expectedSuperParticipantsCount.TextChanged -= OnExpectedCountTextChanged;
        }

        private void combobox_group_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (combobox_group.SelectedItem == null) return;

            string selectedGroup = combobox_group.SelectedItem.ToString();

            //если это новая (пустая) строка, добавляем ещё одну пустую для следующего выбора
            if (IsEmptyRow())
            {
                AddNewEmptyRow();
            }

            text_supervisor_name.Text = GetSupervisor(selectedGroup);

            //находим группу и обновляем _group, СОХРАНЯЯ studentsCount
            var matchedGroup = _table._groups.FirstOrDefault(n => n.groupName == selectedGroup);
            if (matchedGroup == null) return;

            _group = new EventGroups
            {
                groupId = matchedGroup.groupId,
                name = matchedGroup.groupName,
                studentsCount = matchedGroup.studentsCount,
            };

            Tag = _group;
            RecalculateScore();
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
            => AnimateDeleteButton(20, EasingMode.EaseOut);

        private void OnMouseLeave(object sender, MouseEventArgs e)
            => AnimateDeleteButton(0, EasingMode.EaseIn);

        private void AnimateDeleteButton(double to, EasingMode easing)
        {
            var anim = new DoubleAnimation
            {
                From = btn_delete_group.Width,
                To = to,
                Duration = TimeSpan.FromMilliseconds(200),
                EasingFunction = new QuadraticEase { EasingMode = easing }
            };
            btn_delete_group.BeginAnimation(WidthProperty, anim);
        }

        private void btn_delete_group_Click(object sender, RoutedEventArgs e)
        {
            if (IsEmptyRow())
            {
                UserNotificationService.ShowWarning("Это строка для добавления новой группы. Её нельзя удалить.");
                return;
            }

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

        private void edit_expectedCount_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added)
                _page.validation_errors_count++;
            else if (e.Action == ValidationErrorEventAction.Removed)
                _page.validation_errors_count--;
        }

        /// <summary>
        /// Один общий обработчик для всех трёх полей количества.
        /// Пересчитывает балл, не падая на невалидных данных.
        /// </summary>
        private void OnExpectedCountTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isInitialized) return; // не считаем во время первичной инициализации
            RecalculateScore();
        }

        #endregion

        #region Методы класса

        /// <summary>
        /// Строка считается "пустой", если куратор не назначен.
        /// </summary>
        private bool IsEmptyRow()
            => text_supervisor_name.Text == "-" || string.IsNullOrWhiteSpace(text_supervisor_name.Text);

        private void AddNewEmptyRow()
        {
            edit_expectedListenersCount.Text = "0";
            edit_expectedParticipantsCount.Text = "0";
            edit_expectedSuperParticipantsCount.Text = "0";

            _table.stack_table_rows.Children.Add(
                new table_tuple_EventGroup_edit(_page, null, _table));
        }

        /// <summary>
        /// Безопасно пересчитывает балл мероприятия.
        /// Не бросает исключений при невалидных значениях — просто пропускает пересчёт.
        /// </summary>
        private void RecalculateScore()
        {
            if (_group == null) return;

            // Парсим значения; при неудаче — не пересчитываем (пользователь ещё вводит)
            if (!int.TryParse(edit_expectedListenersCount.Text, out int listeners) ||
                !int.TryParse(edit_expectedParticipantsCount.Text, out int participants) ||
                !int.TryParse(edit_expectedSuperParticipantsCount.Text, out int superParticipants))
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

        /// <summary>
        /// Загружает в комбобокс только те группы, которые ещё не выбраны в других строках.
        /// </summary>
        private void LoadGroups()
        {
            var usedGroups = _table.stack_table_rows.Children
                .OfType<table_tuple_EventGroup_edit>()
                .Where(t => !ReferenceEquals(t, this))
                .Select(t => t.combobox_group.Text)
                .ToHashSet();

            combobox_group.ItemsSource = _table._groups
                .Select(g => g.groupName)
                .Where(name => !usedGroups.Contains(name))
                .ToList();
        }

        /// <summary>
        /// Поиск куратора по названию группы.
        /// </summary>
        public string GetSupervisor(string groupName)
        {
            if (string.IsNullOrEmpty(groupName))
                return "Куратор не указан";

            return _table.GroupSupervisor.TryGetValue(groupName, out string supervisor)
                ? supervisor
                : "Куратор не назначен";
        }

        #endregion
    }
}
