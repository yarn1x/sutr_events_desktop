using college_events_desktop.DataModels;
using college_events_desktop.Model;
using college_events_desktop.View.Controls;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace college_events_desktop.View.Layers.Events.Tables
{
    /// <summary>
    /// Класс представляет собой контейнер для помещения внутрь элементов с информацией в виде пользовательских элементов. 
    /// </summary>
    public partial class page_table_EventGroup_save : Page
    {
        #region Поля класса
        public List<EventGroupsActualAttendances> EventGroupsActualAttendances { get => CollectGroupsFromUI(); }

        private page_EventList_save _page;
        private readonly DataService _dataService;
        private readonly Event _Event;
        #endregion

        #region Конструктор
        public page_table_EventGroup_save(page_EventList_save page, DataService dataService, Event Event)
        {
            InitializeComponent();
            _page = page;
            _Event = Event;
            _dataService = dataService;
            Loaded += Page_table_EventGroup_save_Loaded;
        }
        #endregion

        #region Обработчики событий
        private async void Page_table_EventGroup_save_Loaded(object sender, RoutedEventArgs e)
        {
            //показываем иконку загрузки, пока не загрузим контент
            stack_table_rows.Children.Clear();
            var element = new loading_interface();
            element.AddInterfaceToContainer(stack_table_rows, new Thickness(0, 10, 0, 0));
            try //совершаем попытку загрузки информуции
            {
                await LoadInformationAsync();
            }
            catch (Exception ex)//при возникновении ошибки, добавим в контейнер таблицы информацию об ошибке
            {
                TextBlock MessageText = new TextBlock()
                {
                    Text = $"Ошибка получения списка групп.\n\nMessage={ex.InnerException?.Message ?? ex.Message}\n\nCode=page_table_EventGroup_saveAA001",
                    TextAlignment = TextAlignment.Center,
                    Margin = new Thickness(0, 10, 0, 0)
                };
                stack_table_rows.Children.Clear();
                stack_table_rows.Children.Add(MessageText);
            }
        }
        #endregion

        #region Методы класса

        /// <summary>
        /// Извлекает список групп из интерфейса
        /// </summary>
        /// <returns>Список групп, зарегистрированных на мероприятие</returns>
        private List<EventGroupsActualAttendances> CollectGroupsFromUI()
        {
            try
            {
                if (stack_table_rows.Children.Count <= 1)
                {
                    throw new Exception("Нет зарегистрированных групп. Попробуйте обновить страницу.");
                }
                //TIP: OfType<table_tuple_EventGroup_save>() - будем собирать только элементы указанного класса
                //TIP: .Where(g => g._group != null) - условие того, что элемент должен содержать экземпляр класса зарегистрированной группы
                //TIP: .Select(child => ...) - выбираем в каком виде и что извлекаем из UI элемента
                return stack_table_rows.Children
                    .OfType<table_tuple_EventGroup_save>()
                    .Where(g => g._group != null)
                    .Select(child => new EventGroupsActualAttendances
                    {
                        
                        eventGroupId = (int)child.Tag,
                        actualListenersCount = SafeParseInt(child.edit_actualListenersCount.Text),
                        actualParticipantsCount = SafeParseInt(child.edit_actualParticipantsCount.Text),
                        actualSuperParticipantsCount = SafeParseInt(child.edit_actualSuperParticipantsCount.Text)
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Безопасный способ получения целочисленного значения из строки
        /// </summary>
        /// <param name="text">Текст для извлечения целочисленного значения</param>
        private int SafeParseInt(string text) => int.TryParse(text, out int result) ? result : 0;

        /// <summary>
        /// Метод, загружающий на страницу информацию
        /// </summary>
        private async Task LoadInformationAsync()
        {
            var groupsList = await _dataService.apiClient.GetEventGroupsByEventId(_Event.eventId);

            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                //отчистка контейнера перед заполнением
                stack_table_rows.Children.Clear();
                //добавление зарегистрированных групп в контейнер
                foreach (var group in groupsList)
                {
                    stack_table_rows.Children.Add(new table_tuple_EventGroup_save(_page, group, this));
                }
                //добавление в конец списка групп пустую строчку (для добавления новых групп).
                stack_table_rows.Children.Add(new table_tuple_EventGroup_save(_page, null, this));
                //TODO: SAVE сделать активную пустую строчку после списка /\ (по требованию заказчика)

                //TIP: если такая надобность возникнет, помимо запроса к API college/admin/events/{EventId}/statistics (изменение записей в actual_attendances)
                //     потребуется изначально изменить весь состав зарегистрированных групп (изменение в event_groups)
                //     college/admin/events/update/{EventId}/groups - эндпоинт редактирует список зарегистрированных групп. Нет метода на клиенте
                //     ИЛИ college/admin/events/update/{EventId} - эндпоинт редактирует как информацию о мероприятии, так и о группах. Уже существует метод в ApiClient
                //     Дата создания заметки: 08-15-2026
            });
        }
        #endregion
    }
}
