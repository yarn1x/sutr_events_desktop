using college_events_desktop.DataModels;
using college_events_desktop.Services;
using college_events_desktop.View.Windows;
using System.Windows.Controls;

namespace college_events_desktop.View.Layers.Groups
{
    public partial class control_group_card : UserControl
    {
        private readonly MainWindow _mainWindow;
        private readonly DataService _dataService;
        public Group _Group => DataContext as Group;

        /// <summary>
        /// Конструктор по умолчанию. Необходим для автоматической сборки через DataTemplate.
        /// </summary>
        public control_group_card()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Старый конструктор (для обратной совместимости, если где-то остался ручной вызов)
        /// </summary>
        public control_group_card(MainWindow mainWindow, DataService dataService, Group group) : this()
        {
            _mainWindow = mainWindow;
            _dataService = dataService;
            this.DataContext = group;
        }

        private void btn_card_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _mainWindow.mainframe.Navigate(new page_Group(_mainWindow, _dataService, _Group));
        }
    }
}
