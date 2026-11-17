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
        private MainWindow _mainWindow;
        private page_EventList _eventList;
        private bool _isSubscribed;

        public events_nav_buttons(Window win, page_EventList eventList)
        {
            InitializeComponent();
            _mainWindow = win as MainWindow;
            _eventList = eventList;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (_isSubscribed) return;
                EventNotification.StatusChanged += EventNotification_StatusChanged;
            _isSubscribed = true;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (!_isSubscribed) return;
                EventNotification.StatusChanged -= EventNotification_StatusChanged;
            _isSubscribed = false;
        }

        private void EventNotification_StatusChanged(object sender, EventStatusChangedEventArgs e)
        {
            ellipse_new_event_status.Visibility = e.HasElements
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void open_event_list_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.mainframe.Navigate(_eventList);
        }
    }
}
