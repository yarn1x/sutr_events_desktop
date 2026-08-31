using college_events_desktop.DataModels;
using college_events_desktop.Model;
using college_events_desktop.View.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace college_events_desktop.View.Layers.Events.Tables
{
    /// <summary>
    /// Класс представляет собой контейнер для помещения внутрь элементов с информацией в виде пользовательских элементов. 
    /// </summary>
    public partial class page_table_EventGroup_edit : Page
    {
        #region Поля класса

        /// <summary>
        /// Получает список групп, зарегистрированных на мероприятие
        /// </summary>
        public List<EventGroups> EventGroups { get => CollectGroupsFromUI(); }

        private page_EventList_edit _page;
        private readonly DataService _dataService;
        private readonly Event _Event;
        internal List<Group> _groups;
        internal Dictionary<string, string> GroupSupervisor = new Dictionary<string, string>();

        #endregion


        #region Конструктор
        public page_table_EventGroup_edit(page_EventList_edit parentPage, DataService dataService, Event _event)
        {
            InitializeComponent();
            Loaded += Page_table_EventGroup_Loaded;
            _page = parentPage;
            _dataService = dataService;
            _Event = _event;
        }
        #endregion


        #region Обработчики событий
        private void Page_table_EventGroup_Loaded(object sender, RoutedEventArgs e)
        {
            ShowLoadingIndicator();
            LoadInformationAsync();
        }
        #endregion


        #region Методы класса 
        private async void LoadInformationAsync()
        {
            try
            {
                await _dataService.LoadEventGroupsAsync(_Event.eventId);
                _groups = _dataService.groups;
                BuildSupervisorDictionary();

                DisplayEventGroups();
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex);
            }
        }

        private void BuildSupervisorDictionary()
        {
            GroupSupervisor.Clear();

            foreach (var group in _groups)
            {
                var supervisorFullName = $"{group.supervisorSurname} {group.supervisorName} {group.supervisorLastname}";
                GroupSupervisor[group.groupName] = supervisorFullName;
            }
        }

        private void DisplayEventGroups()
        {
            stack_table_rows.Children.Clear();

            // Отображаем группы мероприятия
            foreach (var group in _dataService.eventGroups)
            {
                AddEventGroupRow(group);
            }

            // Добавляем пустую строку для новых групп
            AddEventGroupRow(null);
        }

        private void AddEventGroupRow(EventGroups group)
        {
            var row = new table_tuple_EventGroup_edit(_page, group, this);
            stack_table_rows.Children.Add(row);
        }

        private void ShowLoadingIndicator()
        {
            stack_table_rows.Children.Clear();
            var loadingElement = new loading_interface();
            loadingElement.AddInterfaceToContainer(stack_table_rows, new Thickness(0, 10, 0, 0));
        }

        private void ShowErrorMessage(Exception ex)
        {
            // Логируем ошибку для отладки
            Debug.WriteLine($"Error in page_table_EventGroup_edit: {ex.Message}");

            var messageText = new TextBlock()
            {
                Text = $"Ошибка получения списка групп.\n\nCode=page_table_EventGroup_editAA001",
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 10, 0, 0)
            };

            stack_table_rows.Children.Clear();
            stack_table_rows.Children.Add(messageText);
        }

        /// <summary>
        /// Извлекает список групп из интерфейса
        /// </summary>
        /// <returns>Список групп, зарегистрированных на мероприятие</returns>
        private List<EventGroups> CollectGroupsFromUI()
        {
            return stack_table_rows.Children
                .OfType<table_tuple_EventGroup_edit>()
                .Where(g => g._group != null) 
                .Select(child => new EventGroups
                {
                    eventGroupId = child._group.eventGroupId,
                    groupId = child._group.groupId,
                    expectedListenersCount = SafeParseInt(child.edit_expectedListenersCount.Text),
                    expectedParticipantsCount = SafeParseInt(child.edit_expectedParticipantsCount.Text),
                    expectedSuperParticipantsCount = SafeParseInt(child.edit_expectedSuperParticipantsCount.Text)
                })
                .ToList();
        }

        /// <summary>
        /// Безопасный метод получения целочисленного значения из строки
        /// </summary>
        /// <param name="text">Сырая строка для извлечения числа</param>
        private int SafeParseInt(string text) => int.TryParse(text, out int result) ? result : 0;

        #endregion
    }
}
