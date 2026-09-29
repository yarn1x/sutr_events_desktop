using college_events_desktop.DataModels;
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

namespace college_events_desktop.View.Layers.Supervisors
{
    /// <summary>
    /// Логика взаимодействия для page_Supervisor.xaml
    /// </summary>
    public partial class page_Supervisor : Page
    {
        #region Поля класса
        MainWindow _mainWindow;
        DataService _dataService;
        AuthorizedUser _authorizedUser;
        #endregion


        public page_Supervisor(MainWindow mainWindow, DataService dataService, AuthorizedUser authorizedUser)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            _dataService = dataService;
            _authorizedUser = authorizedUser;
            DataContext = _authorizedUser;

            Loaded += Page_Supervisor_Loaded;
        }

        private void Page_Supervisor_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void search_textChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void combobox_categories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
