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

namespace college_events_desktop.View.Layers.Settings
{
    public partial class page_Settings : Page
    {
        public page_Settings()
        {
            InitializeComponent();
        }

        private void btn_reset_moving_menus_Click(object sender, RoutedEventArgs e)
        {
            //сбрасываем сохраненные координаты панели
            Properties.Settings.Default.eventList_moving_panel_X = 0;
            Properties.Settings.Default.eventList_moving_panel_Y = 0;
            Properties.Settings.Default.Save();

            //показываем MessageBox с кнопками Да/Нет и иконкой вопроса
            MessageBoxResult result = MessageBox.Show("Для восстановления исходного положения панелей необходимо перезапустить приложение.\n\nПерезапустить сейчас?", "Сброс положения панелей", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // Запускает новую копию текущего приложения
                System.Windows.Forms.Application.Restart();
                // Завершает работу текущего процесса приложения
                System.Windows.Application.Current.Shutdown();
            }
        }
    }
}
