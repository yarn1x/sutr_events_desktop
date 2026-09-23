using college_events_desktop.DataModels;
using college_events_desktop.Model;
using college_events_desktop.Services;
using college_events_desktop.View.Controls;
using college_events_desktop.View.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace college_events_desktop.View.Layers.Organizers
{
    public partial class page_Organizer : Page
    {
        #region Поля класса
        MainWindow _mainWindow;
        DataService _dataService;
        AuthorizedUser _authorizedUser;
        #endregion


        #region Конструктор
        public page_Organizer(MainWindow mainWindow, DataService dataService, AuthorizedUser authorizedUser)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            _dataService = dataService;
            _authorizedUser = authorizedUser;
            DataContext = _authorizedUser;

            Loaded += Page_Organizer_Loaded;
        }
        #endregion


        #region Обработчики событий
        private async void Page_Organizer_Loaded(object sender, RoutedEventArgs e)
        {
            using (LoadingService.StartLoading())
            {
                try
                {
                    await _dataService.LoadOrganizerStatistic(_authorizedUser.AuthorizedUserId);
                    DataContext = _dataService.organizerStatistic;
                    datagrid_events.ItemsSource = _dataService.organizerStatistic.events;

                    await _dataService.LoadCategoriesAsync();
                    var categories = new List<string> { "Не выбрано" };
                    categories.AddRange(_dataService.categories.Select(c => c.name));

                    combobox_categories.ItemsSource = categories;
                    combobox_categories.SelectedIndex = 0;
                }
                catch (Exception ex)
                {
                    UserNotificationService.ShowError("Ошибка получения данных.", ex, "page_OrganizerAA001");
                }
            }
        }

        private void search_textChanged(object sender, TextChangedEventArgs e)
        {
            if (!(sender is SearchBar textBox)) return;
            string searchText = textBox.SearchText.ToLower().Trim();
            try
            {
                datagrid_events.ItemsSource = _dataService.organizerStatistic.events.Where(ev => ev.title.ToLower().Trim().Contains(searchText));
            }
            catch (Exception ex)
            {
                UserNotificationService.ShowError("Ошибка применения поиска по тексту.", ex, "page_OrganizerAA002");
            }
        }

        private void combobox_categories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(sender is ComboBox comboBox)) return;
            if (comboBox.SelectedIndex == 0)
            {
                datagrid_events.ItemsSource = _dataService.organizerStatistic.events;
                return;
            }

            string searchText = comboBox.SelectedItem.ToString() ?? string.Empty;
            string lowerSearchText = searchText.ToLower().Trim();

            try
            {
                datagrid_events.ItemsSource = _dataService.organizerStatistic.events.Where(ev => ev.CategoryName.ToLower().Trim().Contains(lowerSearchText));
            }
            catch (Exception ex)
            {
                UserNotificationService.ShowError("Ошибка применения фильтра по направлению (категории).", ex, "page_OrganizerAA003");
            }
        }

        private void datepicker_date_changed(object sender, SelectionChangedEventArgs e)
        {
            ApplyDateFilter();
        }
        #endregion


        #region Методы класса
        private void ApplyDateFilter()
        {
            DateTime? from = datepicker_from.SelectedDate;
            DateTime? to = datepicker_to.SelectedDate;
            if (from != null && to == null)
            {
                datagrid_events.ItemsSource = _dataService.organizerStatistic.events.Where(evnt => evnt.startDatetime.Date == from);
            }
            if (from != null && to != null)
            {
                datagrid_events.ItemsSource = _dataService.organizerStatistic.events.Where(evnt => evnt.startDatetime >= from && evnt.startDatetime <= to);
            }
        }
        #endregion
    }
}
