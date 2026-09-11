using college_events_desktop.DataModels;
using college_events_desktop.Model;
using college_events_desktop.View.Layers.Organizers;
using college_events_desktop.View.Windows;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace college_events_desktop.View.Layers.Users
{
    public partial class page_user_navigation_layer : Page
    {

        #region Поля класса
        MainWindow _mainWindow;
        DataService _dataService;
        page_UserAccount _userAccount;
        page_Organizer _pageOrganizer;

        AuthorizedUser _authorizedUser;
        #endregion

        #region Конструктор
        public page_user_navigation_layer(MainWindow mainWindow, DataService dataService, AuthorizedUser authorizedUser)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            _dataService = dataService;
            _authorizedUser = authorizedUser;

            _userAccount = new page_UserAccount(_mainWindow, _dataService, _authorizedUser);
            _pageOrganizer = new page_Organizer(_mainWindow, _dataService, _authorizedUser);

            subframe.Navigate(_userAccount);
            Loaded += Page_user_navigation_layer_Loaded;
        }
        #endregion

        #region Обработчики событий
        private async void Page_user_navigation_layer_Loaded(object sender, RoutedEventArgs e)
        {
            if (_authorizedUser.roles.Any(r => r.userTypeId == 2))
            {
                btn_goto_supervisorProfile.Visibility = Visibility.Visible;
            }
            if (_authorizedUser.roles.Any(r => r.userTypeId == 3))
            {
                btn_goto_organizerProfile.Visibility = Visibility.Visible;
            }

            await Task.Delay(500);
            DoubleAnimation widthAnimation = new DoubleAnimation
            {
                To = 60,
                Duration = TimeSpan.FromMilliseconds(500),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
            };
            grid_user_nav_layer_basement.BeginAnimation(Border.WidthProperty, widthAnimation);

        }

        private void btn_goback_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.mainframe.GoBack();
        }

        private void btn_goto_userAccount_Click(object sender, RoutedEventArgs e)
        {
            subframe.Navigate(_userAccount);
        }

        private void btn_goto_supervisorProfile_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_goto_organizerProfile_Click(object sender, RoutedEventArgs e)
        {
            subframe.Navigate(_pageOrganizer);
        }
        #endregion

        #region Методы класса
        #endregion



    }
}
