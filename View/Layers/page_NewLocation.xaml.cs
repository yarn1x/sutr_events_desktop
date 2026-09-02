using college_events_desktop.DataModels;
using college_events_desktop.Model;
using college_events_desktop.Services;
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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace college_events_desktop.View.Layers
{
    public partial class page_NewLocation : Page
    {
        MainWindow mainWindow; 
        public page_NewLocation(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            InitializeComponent();
        }


        private async void btn_upload_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(edit_location.Text))
            {
                return;
            }
            using (LoadingService.StartLoading())
            {
                Location body = new Location()
                {
                    place = edit_location.Text,
                    inCollege = (bool)check_in_college.IsChecked
                };
                try
                {
                    bool response = await mainWindow._dataService.apiClient.CreateLocation(body);
                    if (response)
                    {
                        UserNotificationService.ShowInformation($"{edit_location.Text} создано!\n\nПЕРЕЗАЙДИТЕ НА СТРАНИЦУ ИЛИ ОБНОВИТЕ ИНФОРМАЦИЮ ДЛЯ ОТОБАЖЕНИЯ НОВОГО МЕСТА", "Успешно!");
                    }
                    else
                    {
                        UserNotificationService.ShowError("Ошибка создания новой локации! Попробуйте снова. Если не получилось, проверьте введённые данные.", "page_NewLocationAA002");
                    }
                }
                catch (Exception ex)
                {
                    UserNotificationService.ShowError("Ошибка создания новой локации!", ex, "page_NewLocationAA001");
                }
            }

        }
    }
}
