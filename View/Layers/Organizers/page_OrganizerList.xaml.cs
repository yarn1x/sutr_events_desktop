using college_events_desktop.Model;
using college_events_desktop.View.Windows;
using System;
using System.Windows.Controls;

namespace college_events_desktop.View.Layers.Organizers
{
    public partial class page_OrganizerList : Page
    {
        MainWindow _mainWindow;
        DataService _dataService;

        public page_OrganizerList(MainWindow mainWindow, DataService dataService)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
        }

        private void goback_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _mainWindow.mainframe.GoBack();
        }
    }
}
