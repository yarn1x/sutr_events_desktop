using college_events_desktop.DataModels;
using college_events_desktop.Model;
using college_events_desktop.Services;
using college_events_desktop.View.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;

namespace college_events_desktop.View.Layers.Users
{
    public partial class page_UserAccount : Page
    {
        
        #region Поля класса

        private readonly MainWindow _mainWindow;
        private readonly DataService _dataService;
        private readonly AuthorizedUser _authorizedUser; //если null, тогда режим создания новой учётки
        internal List<Group> _groups;

        private bool IsEditMode => _authorizedUser != null;

        #endregion


        #region Конструктор
        public page_UserAccount(MainWindow mainWindow, DataService dataService, AuthorizedUser authorizedUser = null)
        {
            InitializeComponent();

            _mainWindow = mainWindow;
            _dataService = dataService;
            _authorizedUser = authorizedUser;

            if (IsEditMode)
            {
                DataContext = authorizedUser;
            }

            Loaded += Page_UserAccount_Loaded;
            btn_save.Click += Btn_save_Click;
        }
        #endregion


        #region Обработчики событий

        private async void Page_UserAccount_Loaded(object sender, RoutedEventArgs e)
        {
            _mainWindow.Title = IsEditMode
                ? StringConstants.mainWindow_Title_UserEditAccount
                : StringConstants.mainWindow_Title_UserNewAccount;

            using (LoadingService.StartLoading())
            {
                var caughtErrors = new List<(string UserMessage, Exception Ex)>();

                //подгружаем учебные группы
                try
                {
                    ///загрузка данных с сервера
                    await _dataService.LoadGroupsListAsync();
                    ///передаем группы сюдя в класс
                    _groups = _dataService.groups;

                    ///если открыли редактор существующей учётки
                    if (IsEditMode)
                    {
                        stack_groups.Children.Clear();
                        foreach (var sg in _authorizedUser.supervisorGroups)
                        {
                            var tuple = new control_supervisor_group_tuple(this, sg, sg.studentsCount) { Margin = new Thickness(0, 0, 0, 5) };
                            stack_groups.Children.Add(tuple);
                        }
                        AddGroupTuple();
                    }
                    ///елси открыли чистую страницу
                    else
                    {
                        stack_groups.Children.Clear();
                        AddGroupTuple();
                    }
                }
                catch (Exception ex) { caughtErrors.Add(("ошибка получения списка групп.", ex)); }

                //подгружаем роли для пользователя
                try
                {
                    ///загрузка данных с сервера
                    await _dataService.LoadRolesListAsync();
                    ///указываем комбобоксу какие у нас будут элементы
                    multicombobox_roles.Items = _dataService.roles;

                    ///это загрузка ролей для существующего пользователя (уже выбранные роли)
                    if (IsEditMode && _authorizedUser.roles != null)
                    {
                        var grantedRoles = _authorizedUser.roles
                            .Select(r => new UserType { userTypeId = r.userTypeId, typeName = r.typeName })
                            .ToList();
                        multicombobox_roles.SelectedItems = grantedRoles;
                    }
                }
                catch (Exception ex) { caughtErrors.Add(("ошибка получения или отображения списка ролей.", ex)); }

                if (caughtErrors.Count > 0)
                {
                    UserNotificationService.ShowError("Список ошибок:", caughtErrors, "page_UserAccountAA001");
                }
            }
        }

        private async void Btn_save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var roles = multicombobox_roles.SelectedItems?.OfType<UserType>().ToList() ?? new List<UserType>();

                ///валидация
                var filledTuples = stack_groups.Children
                    .OfType<control_supervisor_group_tuple>()
                    .Where(t => t.SelectedGroup != null)
                    .ToList();
                if (filledTuples.Count >= 1
                    && !roles.Any(r => r.userTypeId == AuthorizedUserConstants.supervisorTypeId))
                {
                    UserNotificationService.ShowWarning(
                        "Вы прикрепили группу, но не присвоили роль куратора.\nПрисвойте роль куратора для продолжения.");
                    return;
                }

                var dto = HarvestData(roles);
                bool ok = false;
                HttpResponseMessage response;

                if (IsEditMode)
                {
                    response = await _dataService.apiClient.UpdateUserAccountAsync(_authorizedUser.AuthorizedUserId, dto);
                }
                else
                {
                    response = await _dataService.apiClient.CreateUserAccountAsync(dto);
                }

                if (response.IsSuccessStatusCode)
                {
                    UserNotificationService.ShowInformation(IsEditMode ? "Сохранено!" : "Создан новый аккаунт!", "Успех");
                }
                else
                {
                    UserNotificationService.ShowWarning($"Сервер вернул ошибку при сохранении.\n\nПричина={response.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                UserNotificationService.ShowError("Ошибка сохранения", ex, "page_UserAccountAA002");
            }
        }

        private void btn_select_image_path_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var brush = ImageService.SelectImage();
                if (brush == null) return;
                avatarImage.Source = brush.ImageSource;
                edit_image_path.Text = brush.ImageSource?.ToString() ?? string.Empty;
            }
            catch (Exception ex)
            {
                UserNotificationService.ShowError("Ошибка выбора изображения.", ex, "page_UserAccountAA003");
            }
        }

        private void go_UserHelp(object sender, RoutedEventArgs e)
        {
        }

        #endregion


        #region Методы класса

        /// <summary>
        /// Визуально добавляет новую строчку для списка закреплённых групп
        /// </summary>
        public void AddGroupTuple()
        {
            var tuple = new control_supervisor_group_tuple(this)
            {
                Margin = new Thickness(0, 0, 0, 5)
            };
            stack_groups.Children.Add(tuple);
        }


        /// <summary>
        /// Собирает DTO из полей формы.
        /// </summary>
        private UserCreateUpdateDto HarvestData(List<UserType> roles)
        {
            var dto = new UserCreateUpdateDto
            {
                login = edit_login.Text,
                password = edit_password.Password,
                phone = string.IsNullOrEmpty(edit_phone.Text) ? null : edit_phone.Text,
                email = string.IsNullOrEmpty(edit_email.Text) ? null : edit_email.Text,
                user = new UserDto
                {
                    surName = edit_surname.Text,
                    firstName = edit_firstname.Text,
                    lastName = edit_lastname.Text,
                },
                userUsertypes = roles.Select(r => new UserUserTypeDto { userTypeId = r.userTypeId }).ToList(),
                groups = new List<UserGroupDto>()
            };

            foreach (var tuple in stack_groups.Children.OfType<control_supervisor_group_tuple>())
            {
                var group = tuple.SelectedGroup;
                if (group == null) continue;
                dto.groups.Add(new UserGroupDto
                {
                    groupId = group.groupId,
                    studentsCount = tuple.StudentsCount
                });
            }

            return dto;
        }

        #endregion

    }
}