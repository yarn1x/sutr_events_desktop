using college_events_desktop.DataModels;
using college_events_desktop.Model;
using college_events_desktop.View.Controls;
using college_events_desktop.View.Windows;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace college_events_desktop.View.Layers.Events
{
    public partial class page_EventList_seeEvent : Page
    {

        private MainWindow mainWindow;
        private DataService _dataService;
        private Event _Event;


        public page_EventList_seeEvent(Window win, DataService dataService, Event _event)
        {
            InitializeComponent();

            mainWindow = win as MainWindow;
            _dataService = dataService;
            _Event = _event;
            DataContext = _Event;

            Loaded += Page_EventList_seeEvent_Loaded;
        }

        private async void Page_EventList_seeEvent_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //добавление локаций
                _Event.locations.ForEach(location => stack_places.Children.Add(CreateLocationButton(location)));

                //запрос на получение групп из ActualAttendances
                var groups = await _dataService.apiClient.GetListOfEventGroupsStatistics(_Event.eventId);
                //вывод в DataGrid
                datagrid_groups.ItemsSource = groups;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void goback_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.mainframe.GoBack();
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

        private async void btn_export_Click(object sender, RoutedEventArgs e)
        {
            Point point = new Point()
            {
                X = 100, Y = 0
            };
            TextPlaceholder text = new TextPlaceholder(stack_nav, point, "Пока в разработке")
            {
                VerticalAlignment = VerticalAlignment.Center,
            };
            await text.ShowAsync();
            await Task.Delay(1000);
            await text.HideAsync();
        }
    }
}
