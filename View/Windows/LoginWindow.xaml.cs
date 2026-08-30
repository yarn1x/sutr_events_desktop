using college_events_desktop.Model;
using college_events_desktop.View.Controls;
using System;
using System.Windows;

namespace college_events_desktop.View.Windows
{
    public partial class LoginWindow : Window
    {
        DataService dataService;
        public LoginWindow()
        {
            InitializeComponent();
            dataService = new DataService(new ApiClient());
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            btn_login.IsEnabled = false;
            loading_interface loading_Interface = new loading_interface();
            loading_Interface.AddInterfaceToContainer(grid_main, new Thickness(0, 90, 0, 0));

            try
            {
                await dataService.GetSessionToken(edit_login.Text, edit_password.Password);

                if (dataService._jwtToken == null)
                {
                    btn_login.IsEnabled = true;
                    loading_Interface.RemoveInterface(grid_main);
                    MessageBox.Show("Логин или пароль введён неверно, либо у вас нет прав администратора.", "Аккаунт не найден!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                new MainWindow(dataService).Show();
                Close();
            }
            catch (Exception ex)
            {
                loading_Interface.RemoveInterface(grid_main);
                MessageBox.Show($"Система не смогла проверить ваши введённые данные. Возможны проблемы с доступом к серверу. Попробуйте позже.\n\nMessage={ex.InnerException?.Message ?? ex.Message}\n\nCode=LoginWindowAA001", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            btn_login.IsEnabled = true;
        }
    }
}
