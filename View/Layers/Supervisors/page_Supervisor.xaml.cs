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

        private async void Page_Supervisor_Loaded(object sender, RoutedEventArgs e)
        {
            _mainWindow.Title = StringConstants.mainWindow_Title_SupervisorSeeStatistic;
            using (LoadingService.StartLoading())
            {
                try
                {
                    await _dataService.LoadSupervisorStatisticAsync(_authorizedUser.AuthorizedUserId);
                    DataContext = _dataService.supervisorStatistic;

                    await _dataService.LoadCategoriesAsync();
                    var categories = new List<string> { "Не выбрано" };
                    categories.AddRange(_dataService.categories.Select(c => c.name));

                    combobox_categories.ItemsSource = categories;
                    combobox_categories.SelectedIndex = 0;

                    foreach (var group in _dataService.supervisorStatistic.supervisorGroups)
                    {
                        SimpleTextPlaceholder text = new SimpleTextPlaceholder(group.groupName, Colors.White, Color.FromArgb(255, 102, 102, 102), Color.FromRgb(102, 102, 102))
                        {
                            Margin = new Thickness(0, 0, 10, 0)
                        };
                        stack_groups.Children.Add(text);
                    }

                    datagrid_events.ItemsSource = _dataService.supervisorStatistic.events;
                }
                catch (Exception ex)
                {
                    UserNotificationService.ShowError("Ошибка получения информации куратора.", ex, "page_SupervisorAA001");
                }
            }
        }

        private void search_textChanged(object sender, TextChangedEventArgs e)
        {
            if (!(sender is SearchBar textBox)) return;
            string searchText = textBox.SearchText.ToLower().Trim();
            try
            {
                datagrid_events.ItemsSource = _dataService.supervisorStatistic.events.Where(ev => 
                    ev.title.ToLower().Trim().Contains(searchText) 
                    || ev.groupName.ToLower().Trim().Contains(searchText) 
                );
            }
            catch (Exception ex)
            {
                UserNotificationService.ShowError("Ошибка применения поиска по тексту.", ex, "page_SupervisorAA002");
            }
        }

        private void combobox_categories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(sender is ComboBox comboBox)) return;
            if (comboBox.SelectedIndex == 0)
            {
                datagrid_events.ItemsSource = _dataService.supervisorStatistic.events;
                return;
            }

            string searchText = comboBox.SelectedItem.ToString() ?? string.Empty;
            string lowerSearchText = searchText.ToLower().Trim();

            try
            {
                datagrid_events.ItemsSource = _dataService.supervisorStatistic.events.Where(ev => ev.categoryName.ToLower().Trim().Contains(lowerSearchText));
            }
            catch (Exception ex)
            {
                UserNotificationService.ShowError("Ошибка применения фильтра по направлению (категории).", ex, "page_SupervisorAA003");
            }
        }
    }
}
