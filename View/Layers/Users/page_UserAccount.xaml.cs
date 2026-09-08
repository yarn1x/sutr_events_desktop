using college_events_desktop.DataModels;
using college_events_desktop.Model;
using college_events_desktop.Services;
using college_events_desktop.View.Controls.Containers;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace college_events_desktop.View.Layers.Users
{
    public partial class page_UserAccount : Page
    {
        private readonly MainWindow _mainWindow;
        private readonly DataService _dataService;
        private AuthorizedUser _authorizedUser;

        public page_UserAccount(MainWindow mainWindow, DataService dataService, AuthorizedUser authorizedUser)
        {
            InitializeComponent();

            _mainWindow = mainWindow;
            _dataService = dataService;
            _authorizedUser = authorizedUser;
            DataContext = authorizedUser;

            Loaded += Page_UserAccount_Loaded; ;
        }

        private async void Page_UserAccount_Loaded(object sender, RoutedEventArgs e)
        {
            using (LoadingService.StartLoading())
            {
                await _dataService.LoadRolesListAsync();
                
                multicombobox_roles.Items = _dataService.roles;

                var grantedRoles = new List<UserType>();
                foreach (var role in _authorizedUser.roles)
                {
                    grantedRoles.Add(new UserType()
                    {
                        userTypeId = role.userTypeId,
                        typeName = role.typeName,
                    });
                }

                multicombobox_roles.SelectedItems = grantedRoles;
            }
        }



        public page_UserAccount(MainWindow mainWindow, DataService dataService)
        {
            InitializeComponent();

            _mainWindow = mainWindow;
            _dataService = dataService;

            Loaded += Page_NewAccount_Loaded;
        }

        private async void Page_NewAccount_Loaded(object sender, RoutedEventArgs e)
        {
            using (LoadingService.StartLoading())
            {
                await _dataService.LoadRolesListAsync();
                
                multicombobox_roles.Items = _dataService.roles;
            }
        }

        private void goback_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_save_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
