using college_events_desktop.DataModels;
using college_events_desktop.Model;
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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace college_events_desktop.View.Layers.Users
{
    public partial class page_user_navigation_layer : Page
    {
        MainWindow _mainWindow;
        DataService _dataService;
        page_UserAccount _userAccount;

        AuthorizedUser _authorizedUser;

        public page_user_navigation_layer(MainWindow mainWindow, DataService dataService, AuthorizedUser authorizedUser)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            _dataService = dataService;
            _authorizedUser = authorizedUser;

            _userAccount = new page_UserAccount(_mainWindow, _dataService, _authorizedUser);

            mainframe.Navigate(_userAccount);
            Loaded += Page_user_navigation_layer_Loaded;
        }

        private async void Page_user_navigation_layer_Loaded(object sender, RoutedEventArgs e)
        {
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
            mainframe.Navigate(_userAccount);
        }

        private void btn_goto_supervisorProfile_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_goto_organizerProfile_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
