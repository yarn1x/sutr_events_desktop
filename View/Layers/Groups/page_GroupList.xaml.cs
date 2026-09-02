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

namespace college_events_desktop.View.Layers.Groups
{
    public partial class page_GroupList : Page
    {
        private MainWindow _mainWindow;
        private readonly DataService _dataService;

        public page_GroupList(MainWindow mainWindow, DataService dataService)
        {
            InitializeComponent();
            Loaded += Page_GroupList_Loaded;

            _mainWindow = mainWindow;
            _dataService = dataService;
        }

        private async void Page_GroupList_Loaded(object sender, RoutedEventArgs e)
        {
            using (LoadingService.StartLoading())
            {
                try
                {
                    await _dataService.LoadGroupsListAsync();
                    foreach (var item in _dataService.groups)
                    {
                        stack_groups.Children.Add(new control_group_card(item));
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"");
                }
            }
        }

        private void goback_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.mainframe.GoBack();
        }
    }
}
