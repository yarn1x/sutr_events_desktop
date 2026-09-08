using college_events_desktop.DataModels;
using college_events_desktop.Model;
using college_events_desktop.Services;
using college_events_desktop.View.Controls;
using college_events_desktop.View.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace college_events_desktop.View.Layers.Groups
{
    public partial class page_GroupList : Page
    {
        #region Поля класса
        private MainWindow _mainWindow;
        private readonly DataService _dataService;
        #endregion



        #region Конструктор
        public page_GroupList(MainWindow mainWindow, DataService dataService)
        {
            InitializeComponent();
            Loaded += Page_GroupList_Loaded;
            Unloaded += Page_GroupList_Unloaded;

            _mainWindow = mainWindow;
            _dataService = dataService;
        }
        #endregion



        #region Обработчики событий
        private async void Page_GroupList_Loaded(object sender, RoutedEventArgs e)
        {
            using (LoadingService.StartLoading())
            {
                try
                {
                    await _dataService.LoadGroupsListAsync();
                    stack_groups.Children = BuildGroupsCards(_dataService.groups);
                    combobox_course.SelectionChanged += combobox_course_SelectionChanged;
                }
                catch (Exception ex)
                {
                    UserNotificationService.ShowError("Ошибка получения списка групп", ex, "page_GroupListAA001");
                }
            }
        }
        private void Page_GroupList_Unloaded(object sender, RoutedEventArgs e)
        {
            combobox_course.SelectionChanged -= combobox_course_SelectionChanged;
        }


        private void goback_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.mainframe.GoBack();
        }


        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!(sender is SearchBar textBox)) return;

            string filterText = textBox.SearchText?.Trim();
            var sortedList = _dataService.groups.Where(g => g.groupName.Contains(filterText)).ToList();
            stack_groups.Children = BuildGroupsCards(sortedList);

        }

        private void combobox_course_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (combobox_course.SelectedItem is int course)
                {
                    DateTime filterDate = GetFilterDate(course);
                    var filteredList = _dataService.groups.Where(g => g.creationDate.Year == filterDate.Year).ToList();
                    stack_groups.Children = BuildGroupsCards(filteredList);
                }
                else if (combobox_course.SelectedItem is string)
                {
                    stack_groups.Children = BuildGroupsCards(_dataService.groups);
                }
            }
            catch (Exception ex)
            {
                UserNotificationService.ShowError("Ошибка применения фильтра по курсу группы", ex, "page_GroupListAA002");
            }
        }
        #endregion



        #region Методы класса
        private DateTime GetFilterDate(int course)
        {
            var year = DateTime.Now.AddMonths(4).Year - course;
            return new DateTime(year, 1, 1);
        }

        private List<UIElement> BuildGroupsCards(List<Group> groups)
        {
            List<UIElement> cards = new List<UIElement>();
            groups.ForEach(group =>
            {
                var card = new control_group_card(group)
                {
                    Margin = new Thickness(0, 0, 0, 10)
                };
                cards.Add(card);
            });

            return cards;
        }
        #endregion
    }
}
