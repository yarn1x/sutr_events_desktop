using college_events_desktop.DataModels;
using college_events_desktop.Model;
using college_events_desktop.Services;
using college_events_desktop.View.Controls;
using college_events_desktop.View.Controls.Containers;
using college_events_desktop.View.Windows;
using college_events_desktop.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace college_events_desktop.View.Layers.Users
{
    public partial class page_UserList : Page
    {
        #region Поля класса
        MainWindow _mainWindow;
        DataService _dataService;
        double_stack _Stack;

        private Dictionary<int, bool> filterStates = new Dictionary<int, bool>()
        {
            {AuthorizedUserConstants.administratorTypeId, false},
            {AuthorizedUserConstants.supervisorTypeId, false},
            {AuthorizedUserConstants.organizerTypeId, false},
        };
        #endregion



        #region Конструктор
        public page_UserList(MainWindow mainWindow, DataService dataService)
        {
            InitializeComponent();

            _mainWindow = mainWindow;
            _dataService = dataService;
            _Stack = new double_stack();
            container_content.Content = _Stack;

            Loaded += Page_UserList_Loaded;
        }
        #endregion



        #region Обработчики событий
        private async void Page_UserList_Loaded(object sender, RoutedEventArgs e)
        {
            await Update();
        }



        private void btn_filter_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            
            switch (button.Tag)
            {
                case AuthorizedUserConstants.administratorTypeId:
                    _ = filterStates[AuthorizedUserConstants.administratorTypeId]
                        ? UIAnimations.ChangeColorAsync(button, Color.FromArgb(255, 255, 255, 255), 200, System.Windows.Media.Animation.EasingMode.EaseOut)
                        : UIAnimations.ChangeColorAsync(button, Color.FromArgb(255, 250, 215, 172), 200, System.Windows.Media.Animation.EasingMode.EaseOut); 
                    filterStates[AuthorizedUserConstants.administratorTypeId] = !filterStates[AuthorizedUserConstants.administratorTypeId];
                break;

                case AuthorizedUserConstants.supervisorTypeId:
                    _ = filterStates[AuthorizedUserConstants.supervisorTypeId]
                        ? UIAnimations.ChangeColorAsync(button, Color.FromArgb(255, 255, 255, 255), 200, System.Windows.Media.Animation.EasingMode.EaseOut)
                        : UIAnimations.ChangeColorAsync(button, Color.FromArgb(255, 225, 213, 231), 200, System.Windows.Media.Animation.EasingMode.EaseOut);
                    filterStates[AuthorizedUserConstants.supervisorTypeId] = !filterStates[AuthorizedUserConstants.supervisorTypeId];
                break;

                case AuthorizedUserConstants.organizerTypeId:
                    _ = filterStates[AuthorizedUserConstants.organizerTypeId]
                        ? UIAnimations.ChangeColorAsync(button, Color.FromArgb(255, 255, 255, 255), 200, System.Windows.Media.Animation.EasingMode.EaseOut)
                        : UIAnimations.ChangeColorAsync(button, Color.FromArgb(255, 176, 227, 230), 200, System.Windows.Media.Animation.EasingMode.EaseOut);
                    filterStates[AuthorizedUserConstants.organizerTypeId] = !filterStates[AuthorizedUserConstants.organizerTypeId];
                break;
            }

            apply_filters();
        }



        private async void btn_update_page_Click(object sender, RoutedEventArgs e)
        {
            await Update();
        }



        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!(sender is SearchBar textBox)) return;

            string text = textBox.SearchText.ToLower();
            var sortedList = _dataService.authorizedUsers.Where(o => 
            o.firstName.ToLower().Contains(text) 
            || o.surName.ToLower().Contains(text) 
            || o.lastName.ToLower().Contains(text)
            || o.roles.Any(role => role.typeName.ToLower().Contains(text))
            ).ToList();

            _Stack.Children = BuildUsersCards(sortedList);
        }
        #endregion



        #region Методы класса
        private async Task Update()
        {
            using (LoadingService.StartLoading())
            {
                try
                {
                    await _dataService.LoadUsersListAsync();
                    _Stack.Children = BuildUsersCards(_dataService.authorizedUsers);
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

        private List<UIElement> BuildUsersCards(List<AuthorizedUser> users)
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

        private void apply_filters()
        {
            //получаем список ID ролей, которые сейчас активны (имеют значение true)
            var activeRoleIds = filterStates
                .Where(pair => pair.Value)
                .Select(pair => pair.Key)
                .ToList();

            List<AuthorizedUser> sortedList;

            //если ни один фильтр не выбран, показываем всех пользователей
            if (activeRoleIds.Count == 0)
            {
                sortedList = _dataService.authorizedUsers.ToList();
            }
            else
            {
                //если фильтры активны, выбираем пользователей, у которых 
                //ID хотя бы одной из ролей совпадает с активными фильтрами
                sortedList = _dataService.authorizedUsers
                    .Where(user => user.roles.Any(role => activeRoleIds.Contains(role.userTypeId)))
                    .ToList();
            }

            _Stack.Children = BuildUsersCards(sortedList);
        }
        #endregion
    }
}
