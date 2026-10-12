using college_events_desktop.DataModels;
using college_events_desktop.Model;
using college_events_desktop.Services;
using college_events_desktop.View.Controls;
using college_events_desktop.View.Windows;
using System;
using System.Collections.Generic;
using System.Data;
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

namespace college_events_desktop.View.Layers.Users
{
    public partial class control_user_card : UserControl
    {
        MainWindow _mainWindow;
        DataService _dataService;
        AuthorizedUser _User;


        public control_user_card(MainWindow mainWindow, DataService dataService, AuthorizedUser user)
        {
            InitializeComponent();

            _mainWindow = mainWindow;
            _dataService = dataService;
            _User = user;
            DataContext = _User;

            Loaded += Control_user_card_Loaded;
        }

        private void Control_user_card_Loaded(object sender, RoutedEventArgs e)
        {
            if (_User.roles == null) return;

            stack_user_roles.Children.Clear();
            foreach (UserUserType role in _User.roles)
            {
                //маппим ID роли на строковый ключ из App.xaml
                string resourceKey;
                switch (role.userTypeId)
                {
                    case AuthorizedUserConstants.administratorTypeId:
                        resourceKey = "AdministratorColor";
                    break;
                    
                    case AuthorizedUserConstants.supervisorTypeId:
                        resourceKey = "SupervisorColor";
                    break;
                
                    case AuthorizedUserConstants.organizerTypeId:
                        resourceKey = "OrganizerColor";
                    break;
                
                    case AuthorizedUserConstants.moderatorTypeId:
                        resourceKey = "ModeratorColor";
                    break;
                
                    default:
                        resourceKey = "DefaultWhiteColor";
                    break;
                }

                //достаем цвет через статический метод
                Color roleColor = ApplicationResourceService.GetResourceOrDefault(resourceKey, Colors.White);

                //создаем элемент
                SimpleTextPlaceholder text = new SimpleTextPlaceholder(role.typeName, roleColor, Colors.Transparent, Color.FromRgb(102, 102, 102))
                {
                    Margin = new Thickness(0, 0, 10, 0)
                };
                stack_user_roles.Children.Add(text);
            }
        }

        private void btn_user_card_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.mainframe.Navigate(new page_user_navigation_layer(_mainWindow, _dataService, _User));
        }
    }
}
