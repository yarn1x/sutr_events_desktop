using college_events_desktop.View.Layers.Users;
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
        page_NewAccount page_NewAccount;
        page_UserList page_UserList;
        public users_nav_buttons(Window win, page_UserList page_UserList, page_NewAccount page_NewAccount)
        {
            InitializeComponent();
            mainWindow = win as MainWindow;
            this.page_NewAccount = page_NewAccount;
            this.page_UserList = page_UserList;
        }
        private void create_new_user_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.mainframe.Navigate(page_NewAccount);
        }

        private void btn_users_list_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.mainframe.Navigate(page_UserList);
        }
    }
}
