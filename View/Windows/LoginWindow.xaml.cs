using college_events_desktop.Model;
using college_events_desktop.Services;
using college_events_desktop.View.Controls;
using college_events_desktop.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace college_events_desktop.View.Windows
{
    public partial class LoginWindow : Window
    {
        #region Поля класса
        /// <summary>
        /// Экземпляр класса, предоставляющий доступ к хранимым данным в оперативной памяти
        /// </summary>
        DataService _dataService;
        #endregion


        #region Конструктор
        public LoginWindow()
        {
            InitializeComponent();
            _dataService = new DataService(new ApiClient());
        }
        #endregion


        #region Обработчики событий
        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            btn_login.IsEnabled = false;
            loading_interface loading_Interface = new loading_interface();
            loading_Interface.AddInterfaceToContainer(grid_main, new Thickness(0, 90, 0, 0));

            try
            {
                await _dataService.GetSessionToken(edit_login.Text, edit_password.Password);

                if (_dataService._jwtToken == null)
                {
                    btn_login.IsEnabled = true;
                    loading_Interface.RemoveInterface(grid_main);
                    UserNotificationService.ShowWarning("Логин или пароль введён неверно, либо у вас нет прав администратора.");
                    return;
                }

                new MainWindow(_dataService).Show();
                Close();
            }
            catch (Exception ex)
            {
                loading_Interface.RemoveInterface(grid_main);
                UserNotificationService.ShowError("Система не смогла проверить ваши введённые данные. Возможны проблемы с доступом к серверу. Попробуйте позже.", ex, "LoginWindowAA001");
            }
            btn_login.IsEnabled = true;
        }


        #region Показать пароль функционал
        private void edit_password_view_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (edit_password.Password != edit_password_view.Text)
            {
                edit_password.Password = edit_password_view.Text;
            }
        }

        private void edit_password_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (edit_password_view.Text != edit_password.Password)
            {
                edit_password_view.Text = edit_password.Password;
            }
        }

        private bool isPasswordVisible = false;
        private async void tgbtn_see_pw_Click(object sender, RoutedEventArgs e)
        {
            if (!isPasswordVisible)
            {
                Color newColor = Color.FromArgb(0xFF, 0x00, 0x8C, 0xFF);
                await UIAnimations.ChangeColorAsync<ToggleButton>(sender as ToggleButton, newColor, 1, System.Windows.Media.Animation.EasingMode.EaseOut);
                isPasswordVisible = !isPasswordVisible;
            }
            else
            {
                Color newColor = Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF);
                await UIAnimations.ChangeColorAsync<ToggleButton>(sender as ToggleButton, newColor, 1, System.Windows.Media.Animation.EasingMode.EaseOut);
                isPasswordVisible = !isPasswordVisible;
            }
        }
        #endregion

        #endregion

    }
}
