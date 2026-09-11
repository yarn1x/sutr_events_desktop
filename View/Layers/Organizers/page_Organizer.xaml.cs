using college_events_desktop.DataModels;
using college_events_desktop.Model;
using college_events_desktop.Services;
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

namespace college_events_desktop.View.Layers.Organizers
{
    public partial class page_Organizer : Page
    {
        MainWindow _mainWindow;
        DataService _dataService;
        AuthorizedUser _authorizedUser;
        OrganizerStatistic _organizerStatistic;

        public page_Organizer(MainWindow mainWindow, DataService dataService, AuthorizedUser authorizedUser)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            _dataService = dataService;
            _authorizedUser = authorizedUser;
            DataContext = _authorizedUser;

            Loaded += Page_Organizer_Loaded;
        }

        private async void Page_Organizer_Loaded(object sender, RoutedEventArgs e)
        {
            using (LoadingService.StartLoading())
            {
                await _dataService.LoadOrganizerStatistic(_authorizedUser.AuthorizedUserId);
                DataContext = _dataService.organizerStatistic;
                datagrid_events.ItemsSource = _dataService.organizerStatistic.events;
            }
        }
    }
}
