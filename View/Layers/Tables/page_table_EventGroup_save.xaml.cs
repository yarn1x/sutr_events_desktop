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

namespace college_events_desktop.View.Layers.Tables
{
    /// <summary>
    /// Класс представляет собой контейнер для помещения внутрь элементов с информацией в виде пользовательских элементов. 
    /// </summary>
    public partial class page_table_EventGroup_save : Page
    {
        #region Поля класса
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
            //Показываем иконку загрузки, пока не загрузим контент
            stack_table_rows.Children.Clear();
            var element = new loading_interface();
            element.AddInterfaceToContainer(stack_table_rows, new Thickness(0, 10, 0, 0));
            try
            {
                await LoadInformationAsync();
            }
            catch //При возникновении ошибки, добавим в контейнер таблицы информацию об ошибке
            {
                TextBlock MessageText = new TextBlock()
                {
                    Text = $"Ошибка получения списка групп.\n\nCode=page_table_EventGroup_saveAA001",
                    TextAlignment = TextAlignment.Center,
                    Margin = new Thickness(0, 10, 0, 0)
                };
                stack_table_rows.Children.Clear();
                stack_table_rows.Children.Add(MessageText);
            }
        }
        #endregion

        #region Методы класса
        private async Task LoadInformationAsync()
        {
            var groupsList = await _dataService.apiClient.GetEventGroupsByEventId(_Event.eventId);

            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                stack_table_rows.Children.Clear();
                foreach (var group in groupsList)
                {
                    stack_table_rows.Children.Add(new table_tuple_EventGroup_save(_page, group, this));
                }
                stack_table_rows.Children.Add(new table_tuple_EventGroup_save(_page, null, this));
                //TODO: пустая строчка после списка
            });
        }
        #endregion
    }
}
