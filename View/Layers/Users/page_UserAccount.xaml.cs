using college_events_desktop.DataModels;
using college_events_desktop.Model;
using college_events_desktop.Services;
using college_events_desktop.View.Controls.Containers;
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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
        #endregion


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
                    UserNotificationService.ShowError("Ошибка получения или отображения информации.", ex, "page_UserAccountAA001");
                }
            }
        }


        private void btn_save_Click(object sender, RoutedEventArgs e)
        {
            UserNotificationService.ShowInformation("Сохранено!", "btn_save_Click()");
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
                try
                {
                    await _dataService.LoadRolesListAsync();
                    multicombobox_roles.Items = _dataService.roles;
                }
                catch (Exception ex)
                {
                    UserNotificationService.ShowError("Ошибка получения или отображения информации.", ex, "page_UserAccountAA001");
                }
            }
        }
        private void btn_create_account_Click(object sender, RoutedEventArgs e)
        {
            UserNotificationService.ShowInformation("Создан новый аккаунт!", "btn_create_account_Click");
        }
        #endregion


        #region Методы класса
        #endregion
    }
}
