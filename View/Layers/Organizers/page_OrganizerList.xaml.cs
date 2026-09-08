using college_events_desktop.Model;
using college_events_desktop.View.Windows;
using college_events_desktop.View.Controls.Containers;
using System;
using System.Windows.Controls;
using System.Windows;
using System.Collections.Generic;
using college_events_desktop.DataModels;
using System.Net.Http;
using college_events_desktop.View.Controls;
using college_events_desktop.Services;
using System.Linq;

namespace college_events_desktop.View.Layers.Organizers
{
    public partial class page_OrganizerList : Page
    {
        #region Поля класса
        private readonly MainWindow _mainWindow;
        DataService _dataService;
        double_stack _Stack;
        #endregion



        #region Конструктор
        public page_OrganizerList(MainWindow mainWindow, DataService dataService)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            _dataService = dataService;
            _Stack = new double_stack();
            container_content.Content = _Stack;


            Loaded += Page_OrganizerList_Loaded;
        }
        #endregion



        #region Обработчики событий
        private async void Page_OrganizerList_Loaded(object sender, RoutedEventArgs e)
        {
            using (LoadingService.StartLoading())
            {
                try
                {
                    await _dataService.LoadOrganizerListAsync();
                    _Stack.Children = BuildOrganizersCards(_dataService.organizers);
                }
                catch (HttpRequestException ex)
                {
                    UserNotificationService.ShowError("Возникла ошибка отправки запроса на сервер.", ex, "page_OrganizerListAA001");
                }
                catch (Exception ex)
                {
                    UserNotificationService.ShowError("Непредвиденная ошибка загрузки списка организаторов.", ex, "page_OrganizerListAA002");
                }
            }
        }

        private void goback_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.mainframe.GoBack();
        }


        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!(sender is SearchBar textBox)) return;

            string text = textBox.SearchText;
            var sortedList = _dataService.organizers.Where(o => o.firstName.Contains(text) || o.surName.Contains(text) || o.lastName.Contains(text)).ToList();

            _Stack.Children = BuildOrganizersCards(sortedList);
        }

        #endregion



        #region Методы класса

        private List<UIElement> BuildOrganizersCards(List<Organizer> organizers)
        {
            List<UIElement> cards = new List<UIElement>();
            organizers.ForEach(organizer =>
            {
                var card = new control_organizer_card(_mainWindow, _dataService, organizer)
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
