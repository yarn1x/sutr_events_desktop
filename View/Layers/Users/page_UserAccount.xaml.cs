using college_events_desktop.DataModels;
using college_events_desktop.Model;
using college_events_desktop.Services;
using college_events_desktop.View.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace college_events_desktop.View.Layers.Users
{
    public partial class page_UserAccount : Page
    {
        /*
         * Класс разделяет 2 разных функционала:
         *   1. Просмотр страницы с инфой о существующем пользователе системы;
         *   2. Создание аккаунта для нового пользователя.
         */

        /*
         * Поля класса одни на оба конструктора
         */
        #region Поля класса
        private readonly MainWindow _mainWindow;
        private readonly DataService _dataService;
        private AuthorizedUser _authorizedUser;
        internal List<Group> _groups;
        #endregion


        private void AddGroupTuple()
        {
            var tuple = new control_supervisor_group_tuple(this);
            stack_groups.Children.Add(tuple);
        }


        /*
         * Просмотр страницы с инфой о существующем пользователе системы
         */
        #region Конструктор

        /// <summary>
        /// Экземпляр класса, представляющий собой страницу для просмотра существующего пользователя
        /// </summary>
        /// <param name="mainWindow"></param>
        /// <param name="dataService"></param>
        /// <param name="authorizedUser"></param>
        public page_UserAccount(MainWindow mainWindow, DataService dataService, AuthorizedUser authorizedUser)
        {
            InitializeComponent();

            _mainWindow = mainWindow;
            _dataService = dataService;
            _authorizedUser = authorizedUser;
            DataContext = authorizedUser;

            Loaded += Page_UserAccount_Loaded;
            btn_save.Click += btn_save_Click;
        }
        #endregion


        #region Обработчики событий
        private async void Page_UserAccount_Loaded(object sender, RoutedEventArgs e)
        {
            using (LoadingService.StartLoading())
            {
                var caughtErrors = new List<(string UserMessage, Exception Ex)>();
                try
                {
                    await _dataService.LoadGroupsListAsync();
                    _groups = _dataService.groups;

                    AddGroupTuple();
                }
                catch (Exception ex)
                {
                    caughtErrors.Add(("ошибка получения списка групп.", ex));
                }
                try
                {
                    await _dataService.LoadRolesListAsync();
                    multicombobox_roles.Items = _dataService.roles;

                    var grantedRoles = new List<UserType>();
                    foreach (var role in _authorizedUser.roles)
                    {
                        grantedRoles.Add(new UserType()
                        {
                            userTypeId = role.userTypeId,
                            typeName = role.typeName,
                        });
                    }

                    multicombobox_roles.SelectedItems = grantedRoles;
                }
                catch (Exception ex)
                {
                    caughtErrors.Add(("ошибка получения/отображения списка ролей.", ex));
                }
                if (caughtErrors.Count > 0)
                {
                    UserNotificationService.ShowError("Список ошибок:", caughtErrors, "page_UserAccountAA001");
                }
            }
        }


        private void btn_save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var roles = multicombobox_roles.SelectedItems;
                if (
                    stack_groups.Children.Count >= 1
                    && !roles.OfType<UserType>().Any(role => role.userTypeId == AuthorizedUserConstants.supervisorTypeId))
                {
                    UserNotificationService.ShowWarning("Вы прикрепили группу или несколько групп, но не присвоили роль куратора.\nПрисвойте роль куратора для продолжения.");
                }
                else
                {
                    UserNotificationService.ShowInformation("Сохранено!", "btn_save_Click()");
                }
            }
            catch (Exception ex)
            {
                UserNotificationService.ShowError("Ошибка сохранения", ex, "page_UserAccountAA002");
            }
        }
        #endregion


        #region Методы класса
        #endregion




        /*
         * Создание аккаунта для нового пользователя
         */
        #region Конструктор

        /// <summary>
        /// Экземпляр класса, представляющий собой страницу для заполнения информации по новому пользователю
        /// </summary>
        /// <param name="mainWindow">Экземпляр класса окна родителя</param>
        /// <param name="dataService">Экземпляр класса представления информации.</param>
        public page_UserAccount(MainWindow mainWindow, DataService dataService)
        {
            InitializeComponent();

            _mainWindow = mainWindow;
            _dataService = dataService;

            Loaded += Page_NewAccount_Loaded;
            btn_save.Click += btn_create_account_Click;
        }
        #endregion


        #region Обработчики событий
        private async void Page_NewAccount_Loaded(object sender, RoutedEventArgs e)
        {
            using (LoadingService.StartLoading())
            {
                var caughtErrors = new List<(string UserMessage, Exception Ex)>();
                try
                {
                    await _dataService.LoadGroupsListAsync();
                    _groups = _dataService.groups;

                    AddGroupTuple();
                }
                catch (Exception ex)
                {
                    caughtErrors.Add(($"произошла ошибка при получении списка групп.", ex));
                }
                try
                {
                    await _dataService.LoadRolesListAsync();
                    multicombobox_roles.Items = _dataService.roles;
                }
                catch (Exception ex)
                {
                    caughtErrors.Add(("ошибка получения списка ролей.", ex));
                }
                if (caughtErrors.Count > 0)
                {
                    UserNotificationService.ShowError("Список ошибок:", caughtErrors, "page_UserAccountAA001");
                }
                
            }
        }
        private void btn_create_account_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var roles = multicombobox_roles.SelectedItems;
                if (
                    stack_groups.Children.Count >= 1 
                    && !roles.OfType<UserType>().Any(role => role.userTypeId == AuthorizedUserConstants.supervisorTypeId))
                {
                    UserNotificationService.ShowWarning("Вы прикрепили группу или несколько групп, но не присвоили роль куратора.\nПрисвойте роль куратора для продолжения.");
                }
                else
                {
                    UserNotificationService.ShowInformation("Создан новый аккаунт!", "btn_create_account_Click");
                }
            }
            catch (Exception ex)
            {
                UserNotificationService.ShowError("Ошибка сохранения", ex, "page_UserAccountAA002");
            }
        }
        #endregion

        #region Методы класса
        #endregion

        private void btn_select_image_path_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                avatarImage.Source = ImageService.SelectImage().ImageSource;
                edit_image_path.Text = avatarImage.Source.ToString();
            }
            catch (Exception ex)
            {
                UserNotificationService.ShowError("Ошибка выбора изображения.", ex, "page_UserAccountAA003");
            }
        }
    }
}
