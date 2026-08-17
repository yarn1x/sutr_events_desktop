using college_events_desktop.DataModels;
using college_events_desktop.Model;
using college_events_desktop.Services;
using college_events_desktop.View.Layers.Tables;
using college_events_desktop.View.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace college_events_desktop.View.Layers
{
    public partial class page_EventList_save : Page
    {
        #region Поля класса
        private MainWindow mainWindow;
        private DataService _dataService;
        private Event _Event;
        page_table_EventGroup_save _table;

        ILoadingService _loadingService;
        #endregion

        #region Конструктор
        public page_EventList_save(Window win, DataService dataService, Event _event)
        {
            InitializeComponent();
            mainWindow = win as MainWindow;
            _dataService = dataService;
            _Event = _event;
            DataContext = _Event;
            _table = new page_table_EventGroup_save(this, _dataService, _Event);
            
            _loadingService = new LoadingService(mainWindow);
            Loaded += Page_EventList_save_Loaded;
        }
        #endregion

        #region Обработчики событий
        private void Page_EventList_save_Loaded(object sender, RoutedEventArgs e)
        {
            LoadInformation();
            frame_table.Navigate(_table);
        }

        private void goback_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.mainframe.GoBack();
        }

        private async void btn_save_Click(object sender, RoutedEventArgs e)
        {
            btn_save.IsEnabled = false;
            using (_loadingService.StartLoading())
            {
                try
                {
                    //TODO: SAVE валидация данных
                    //если в ячейке данные не валидны, система заменяет их автоматом на 0
                    bool response = await _dataService.apiClient.UpdateEventGroupsStatistics(_Event.eventId, _table.EventGroupsActualAttendances);
                    bool updStatusResponse = await _dataService.apiClient.UpdateEventStatus(_Event.eventId, 4);
                    if (!response && !updStatusResponse)
                    {
                        MessageBox.Show("Произошла ошибка обновления.\n\nCode=page_EventList_saveAA003", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    MessageBox.Show("Отчёт успешно составлен!", "Успешно!", MessageBoxButton.OK, MessageBoxImage.Information);
                    mainWindow.mainframe.GoBack();
                }
                catch (HttpRequestException httpEx)
                {
                    MessageBox.Show($"Возникла ошибка отправки запроса на сервер.\n\nCode=page_EventList_saveAA001\nMessage={httpEx.Message}", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Возникла непредвиденная ошибка.\n\nCode=page_EventList_saveAA002\nMessage={ex}", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            btn_save.IsEnabled = true;
        }
        #endregion

        private void LoadInformation()
        {
            _Event.locations.ForEach(g => stack_places.Children.Add(CreateLocationButton(g)));
            // добавление мест проведения мероприятия
            //foreach (Location location in _Event.locations)
            //{
            //    stack_places.Children.Add(CreateLocationButton(location));
            //}
        }

        //TODO: CODE REVEAL: поместить в отдельный класс
        private Button CreateLocationButton(Location location)
        {
            var btn = new Button()
            {
                Tag = location.locationId,
                Content = location.place,
                Style = (Style)TryFindResource("ButtonStyle_X"),
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                Padding = new Thickness(5, 1, 5, 1),
                Margin = new Thickness(5),
                MinWidth = 80,
                FontSize = 14,
                Cursor = System.Windows.Input.Cursors.Hand,
                IsEnabled = false
            };

            btn.Click += (s, e) => stack_places.Children.Remove(btn);
            return btn;
        }

    }
}
