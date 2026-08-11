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
    public partial class page_EventListHelp : Page
    {
        private MainWindow _win;
        public page_EventListHelp(MainWindow win)
        {
            InitializeComponent();
            _win = win;
        }

        private void go_back(object sender, RoutedEventArgs e)
        {
            _win.mainframe.GoBack();
        }
    }
}
