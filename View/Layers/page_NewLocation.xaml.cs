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
        private readonly ILoadingService _loadingService;
        public page_NewLocation(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            _loadingService = new LoadingService(mainWindow);
            InitializeComponent();
        }


        private async void btn_upload_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(edit_location.Text))
            {
                return;
            }
            using (_loadingService.StartLoading())
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
                        MessageBox.Show("Успешно!", $"{edit_location.Text} создано!");
                    }
                    else
                    {
                        MessageBox.Show($"Ошибка создания новой локации! Попробуйте снова. Если не получилось, проверьте введённые данные.\n\nCode=page_NewLocationAA002", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка создания новой локации!\n\nCode=page_NewLocationAA001\n\n{ex.Message}", "Что-то пошло не так", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }

        }
    }
}
