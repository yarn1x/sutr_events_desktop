using college_events_desktop.DataModels;
using college_events_desktop.Model;
using college_events_desktop.Services;
using college_events_desktop.View.Controls;
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
    public partial class page_Group : Page
    {
        #region Поля класса


        MainWindow _mainWindow;
        DataService _dataService;
        Group _group;
        
        
        #endregion



        #region Конструктор

        public page_Group(MainWindow mainWindow, DataService dataService, Group group)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            _dataService = dataService;
            _group = group;
            DataContext = _group;

            Loaded += OnLoaded;
        }


        #endregion



        #region Обработчики событий

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            _mainWindow.Title = StringConstants.mainWindow_Title_GroupSeeStatistic;
            using (LoadingService.StartLoading())
            {
                try
                {
                    await _dataService.LoadGroupStatisticAsync(_group.groupId);
                    //DataContext = _dataService.groupStatistic;
                    datagrid_events.ItemsSource = _dataService.groupStatistic.events;

                    await _dataService.LoadCategoriesAsync();
                    var categories = new List<string> { "Не выбрано" };
                    categories.AddRange(_dataService.categories.Select(c => c.name));

                    combobox_categories.ItemsSource = categories;
                    combobox_categories.SelectedIndex = 0;
                }
                catch (Exception ex)
                {
                    UserNotificationService.ShowError("Ошибка получения сведений о группе.", ex, "page_GroupAA001");
                }
            }
        }

        private void search_textChanged(object sender, TextChangedEventArgs e)
        {
            if (!(sender is SearchBar textBox)) return;
            string searchText = textBox.SearchText.ToLower().Trim();
            try
            {
                datagrid_events.ItemsSource = _dataService.groupStatistic.events.Where(ev => ev.title.ToLower().Trim().Contains(searchText));
            }
            catch (Exception ex)
            {
                UserNotificationService.ShowError("Ошибка применения поиска по тексту.", ex, "page_GroupAA002");
            }
        }

        private void combobox_categories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(sender is ComboBox comboBox)) return;
            if (comboBox.SelectedIndex == 0)
            {
                datagrid_events.ItemsSource = _dataService.groupStatistic.events;
                return;
            }

            string searchText = comboBox.SelectedItem.ToString() ?? string.Empty;
            string lowerSearchText = searchText.ToLower().Trim();

            try
            {
                datagrid_events.ItemsSource = _dataService.groupStatistic.events.Where(ev => ev.categoryName.ToLower().Trim().Contains(lowerSearchText));
            }
            catch (Exception ex)
            {
                UserNotificationService.ShowError("Ошибка применения фильтра по направлению (категории).", ex, "page_GroupAA003");
            }
        }

        private void goback_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.mainframe.GoBack();
        }

        #endregion

        #region Методы класса


        #endregion
    }
}
