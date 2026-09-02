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

namespace college_events_desktop.View.Layers.Organizers
{
    public partial class page_OrganizerList : Page
    {
        private readonly MainWindow _mainWindow;
        DataService _dataService;
        double_stack _Stack;


        public page_OrganizerList(MainWindow mainWindow, DataService dataService)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            _dataService = dataService;
            _Stack = new double_stack();
            container_content.Content = _Stack;


            Loaded += Page_OrganizerList_Loaded;
        }

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
                    UserNotificationService.ShowError("Возникла ошибка отправки запроса на сервер.", ex, "page_EventList_saveAA001");
                }
                catch (Exception ex)
                {
                    UserNotificationService.ShowError("Непредвиденная ошибка загрузки списка организаторов.", ex, "page_EventList_saveAA002");
                }
            }
        }

        private void goback_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.mainframe.GoBack();
        }

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
    }
}
