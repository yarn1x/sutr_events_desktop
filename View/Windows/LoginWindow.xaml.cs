using college_events_desktop.Model;
using college_events_desktop.Services;
using college_events_desktop.View.Controls;
using college_events_desktop.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace college_events_desktop.View.Windows
{
    public partial class LoginWindow : Window
    {
        #region Поля класса
        
        private readonly ApiClient _apiClient;
        private readonly SessionService _session;

        #endregion


        #region Конструктор
        public LoginWindow(ApiClient apiClient, SessionService session)
        {
            InitializeComponent();
            _apiClient = apiClient;
            _session = session;
        }
        #endregion


        #region Обработчики событий
        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            btn_login.IsEnabled = false;
            try
            {
                var response = await _apiClient.LoginAsync(edit_login.Text, edit_password.Password);

                if (response == null || string.IsNullOrEmpty(response.accessToken))
                {
                    UserNotificationService.ShowWarning("Неверный логин или пароль.");
                    return;
                }

                _session.SetSession(response); // Сохраняем токены и данные пользователя

                // Открываем MainWindow через DI
                var mainWindow = App.Services.GetRequiredService<MainWindow>();
                mainWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                UserNotificationService.ShowError("Ошибка авторизации.", ex, "LoginWindowAA001");
            }
            finally
            {
                btn_login.IsEnabled = true;
            }
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
