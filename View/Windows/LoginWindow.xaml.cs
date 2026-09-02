using college_events_desktop.Model;
using college_events_desktop.Services;
using college_events_desktop.View.Controls;
using System;
using System.Windows;

namespace college_events_desktop.View.Windows
{
    public partial class LoginWindow : Window
    {
        DataService _dataService;
        public LoginWindow()
        {
            InitializeComponent();
            _dataService = new DataService(new ApiClient());
        }

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
    }
}
