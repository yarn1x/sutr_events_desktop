using college_events_desktop.DataModels;
using college_events_desktop.Model;
using college_events_desktop.View.Layers.Tables;
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

namespace college_events_desktop.View.Layers
{
    public partial class page_EventList_seeEvent : Page
    {

        private MainWindow mainWindow;
        private DataService _dataService;
        private Event _Event;
        //page_table_EventGroup_edit _table;


        public page_EventList_seeEvent(Window win, DataService dataService, Event _event)
        {
            InitializeComponent();

            mainWindow = win as MainWindow;
            _dataService = dataService;
            _Event = _event;
            DataContext = _Event;
        }

        private void goback_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.mainframe.GoBack();
        }
    }
}
