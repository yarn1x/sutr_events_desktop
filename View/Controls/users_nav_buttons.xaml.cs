using college_events_desktop.View.Layers;
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

namespace college_events_desktop.View.Controls
{
    public partial class users_nav_buttons : UserControl
    {
        MainWindow mainWindow;
        public users_nav_buttons(Window win)
        {
            InitializeComponent();
            mainWindow = win as MainWindow;
        }
        page_NewAccount page = new page_NewAccount();
        private void create_new_user_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.mainframe.Navigate(page);
        }
    }
}
