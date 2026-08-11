using college_events_desktop.DataModels;
using college_events_desktop.Model;
using college_events_desktop.View.Layers.Tables;
using college_events_desktop.View.Windows;
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

namespace college_events_desktop.View.Layers
{
    public partial class page_EventList_save : Page
    {
        #region Поля класса
        private MainWindow mainWindow;
        private DataService _dataService;
        private Event _Event;
        #endregion

        #region Конструктор
        public page_EventList_save(Window win, DataService dataService, Event _event)
        {
            InitializeComponent();
            mainWindow = win as MainWindow;
            _dataService = dataService;
            _Event = _event;
            DataContext = _Event;
            Loaded += Page_EventList_save_Loaded;
        }
        #endregion

        #region Обработчики событий
        private async void Page_EventList_save_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadInformation();
            frame_table.Navigate(new page_table_EventGroup_save(this, _dataService, _Event));
        }

        private void goback_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.mainframe.GoBack();
        }

        private void btn_save_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        private async Task LoadInformation()
        {
            //данные о мероприятии
            //edit_organizer_name.Text = $"{_Event.organizerSurname} {_Event.organizerName} {_Event.organizerLastname}";
            //edit_event_direction.Text = _Event.categoryName;

            // добавление мест проведения мероприятия
            foreach (Location location in _Event.locations)
            {
                stack_places.Children.Add(CreateLocationButton(location));
            }
        }

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
