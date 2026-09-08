using college_events_desktop.DataModels;
using college_events_desktop.Model;
using college_events_desktop.Services;
using college_events_desktop.View.Controls.Containers;
using college_events_desktop.View.Windows;
using college_events_desktop.ViewModels;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace college_events_desktop.View.Layers.Users
{
    public partial class page_UserList : Page
    {
        MainWindow _mainWindow;
        DataService _dataService;
        double_stack _Stack;


        public page_UserList(MainWindow mainWindow, DataService dataService)
        {
            InitializeComponent();

            _mainWindow = mainWindow;
            _dataService = dataService;
            _Stack = new double_stack();
            container_content.Content = _Stack;

            Loaded += Page_UserList_Loaded;
        }

        private async void Page_UserList_Loaded(object sender, RoutedEventArgs e)
        {
            await Update();
        }


        private async Task Update()
        {
            using (LoadingService.StartLoading())
            {
                try
                {
                    await _dataService.LoadUsersListAsync();
                    _Stack.Children = BuildOrganizersCards(_dataService.authorizedUsers);
                }
                catch (HttpRequestException ex)
                {
                    UserNotificationService.ShowError("Возникла ошибка отправки запроса на сервер.", ex, "page_UserListAA001");
                }
                catch (Exception ex)
                {
                    UserNotificationService.ShowError("Непредвиденная ошибка загрузки списка организаторов.", ex, "page_UserListAA002");
                }
            }
        }

        private async void btn_filter_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            UIAnimations.lightForegroundCase = new SolidColorBrush(Colors.White);
            UIAnimations.darkForegroundCase = new SolidColorBrush(Color.FromArgb(0xFF, 76, 76, 76));

            switch (button.Tag)
            {
                case 1:
                    await UIAnimations.ChangeColorAsync(button, Color.FromArgb(255, 250, 215, 172), 200, System.Windows.Media.Animation.EasingMode.EaseOut);
                    break;
                case 2:
                    await UIAnimations.ChangeColorAsync(button, Color.FromArgb(255, 225, 213, 231), 200, System.Windows.Media.Animation.EasingMode.EaseOut);
                    break;
                case 3:
                    await UIAnimations.ChangeColorAsync(button, Color.FromArgb(255, 176, 227, 230), 200, System.Windows.Media.Animation.EasingMode.EaseOut);
                    break;
            }
        }

        private List<UIElement> BuildOrganizersCards(List<AuthorizedUser> users)
        {
            List<UIElement> cards = new List<UIElement>();
            users.ForEach(user =>
            {
                var card = new control_user_card(_mainWindow, _dataService, user)
                {
                    Margin = new Thickness(0, 0, 0, 10)
                };
                cards.Add(card);
            });

            return cards;
        }

        private async void btn_update_page_Click(object sender, RoutedEventArgs e)
        {
            await Update();
        }
    }
}
