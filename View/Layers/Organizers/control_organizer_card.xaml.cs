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
    public partial class control_organizer_card : UserControl
    {
        MainWindow _mainWindow;
        DataService _dataService;
        public AuthorizedUser _User { get; private set; }

        public control_organizer_card(MainWindow mainWindow, DataService dataService, Organizer organizer)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            _dataService = dataService;
            _User = new AuthorizedUser
            {
                AuthorizedUserId = organizer.userId,
                user = new User
                {
                    firstName = organizer.firstName,
                    lastName = organizer.lastName,
                    surName = organizer.surName,
                },
                Email = organizer.email,
                Phone = organizer.mobilePhone,
            };
            DataContext = organizer;

        }

        private void btn_open_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.mainframe.Navigate(new page_Organizer(_mainWindow, _dataService, _User));
        }
    }
}
