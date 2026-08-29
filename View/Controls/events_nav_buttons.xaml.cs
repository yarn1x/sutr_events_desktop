using college_events_desktop.View.Layers.Events;
using college_events_desktop.View.Windows;
using college_events_desktop.ViewModels;
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

namespace college_events_desktop.View.Controls
{
    public partial class events_nav_buttons : UserControl
    {
        MainWindow mainWindow;
        page_EventList eventList;
        public events_nav_buttons(Window win, page_EventList _eventList)
        {
            InitializeComponent();

            mainWindow = win as MainWindow;
            eventList = _eventList;

            EventNotification.StatusChanged += EventNotification_StatusChanged;
        }

        private void EventNotification_StatusChanged(object sender, EventStatusChangedEventArgs e)
        {
            //показываем, если есть элементы с tag 1 или -1
            ellipse_new_event_status.Visibility = e.HasElements
                ? Visibility.Visible
                : Visibility.Collapsed;   
        }

        private void open_event_list_Click(object sender, RoutedEventArgs e)
        {
            var frame = mainWindow.mainframe;
            frame.Navigate(eventList);
        }
    }
}
