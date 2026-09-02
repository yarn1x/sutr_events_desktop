using college_events_desktop.DataModels;
using System;
using System.Collections.Generic;
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

namespace college_events_desktop.View.Layers.Groups
{
    public partial class control_group_card : UserControl
    {
        public readonly Group _Group;


        public control_group_card(Group group)
        {
            InitializeComponent();
            _Group = group;
            this.DataContext = _Group;
        }
    }
}
