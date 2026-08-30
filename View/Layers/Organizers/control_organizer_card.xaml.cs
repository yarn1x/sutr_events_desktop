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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace college_events_desktop.View.Layers.Organizers
{
    public partial class control_organizer_card : UserControl
    {
        MainWindow _mainWindow;
        DataService _dataService;
        public Organizer _Organizer { get; private set; }

        public control_organizer_card(MainWindow mainWindow, DataService dataService, Organizer organizer)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            _dataService = dataService;
            _Organizer = organizer;
            DataContext = _Organizer;

            Loaded += Control_organizer_card_Loaded;
        }

        private void Control_organizer_card_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
