using college_events_desktop.DataModels;
using college_events_desktop.Model;
using college_events_desktop.Services;
using college_events_desktop.View.Controls;
using college_events_desktop.View.Helpers;
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
            mainWindow.Title = StringConstants.mainWindow_Title_EventSeeStatistic;

            try
            {
                //добавление локаций
                _Event.locations.ForEach(location => stack_places.Children.Add
                (
                    LocationButtonFactory.CreateLocationButton(stack_places, location, (Style)TryFindResource("ButtonStyle_X"))
                ));

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
