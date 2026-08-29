using college_events_desktop.DataModels;
using college_events_desktop.Model;
using college_events_desktop.View.Controls;
using System.Collections.Generic;
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
        private async void Page_table_EventGroup_Loaded(object sender, RoutedEventArgs e)
        {
            await Application.Current.Dispatcher.InvokeAsync(async () => 
            { 
                //Показываем иконку загрузки, пока не загрузим контент
                stack_table_rows.Children.Clear();
                var element = new loading_interface();
                element.AddInterfaceToContainer(stack_table_rows, new Thickness(0, 10, 0, 0));
                LoadInformationAsync();
            });
        }
        #endregion


        #region Методы класса 
        private async void LoadInformationAsync()
        {
            try
            {
                //В отдельном потоке, совершаем запрос к API на получение групп, участвовавших в мероприятии
                var eventGroupsList = await _dataService.apiClient.GetEventGroupsByEventId(_Event.eventId);
                // и полный список групп (для выбора из выпадающего списка)
                var groups = _dataService.groups;
                _groups = groups;
                foreach (var group in groups)
                {
                    GroupSupervisor.Add(group.groupName, $"{group.supervisorSurname} {group.supervisorName} {group.supervisorLastname}");
                }

                stack_table_rows.Children.Clear();
                //По каждой найденной записанной группе в мероприятие, выводим в контейнер таблицы строчку с информацией.
                foreach (var group in eventGroupsList)
                {
                    stack_table_rows.Children.Add(new table_tuple_EventGroup_edit(_page, group, this));
                }
                stack_table_rows.Children.Add(new table_tuple_EventGroup_edit(_page, null, this));
            }
            catch //При возникновении ошибки, добавим в контейнер таблицы информацию об ошибке
            {
                TextBlock MessageText = new TextBlock()
                {
                    Text = $"Ошибка получения списка групп.\n\nCode=page_table_EventGroup_editAA001",
                    TextAlignment = TextAlignment.Center,
                    Margin = new Thickness(0, 10, 0, 0)
                };
                stack_table_rows.Children.Clear();
                stack_table_rows.Children.Add(MessageText);
            }
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
