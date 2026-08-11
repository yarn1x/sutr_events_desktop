using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace college_events_desktop.View.Controls
{
    public partial class loading_interface : UserControl
    {
        public loading_interface()
        {
            InitializeComponent();
        }
        public void AddInterfaceToContainer(Grid container, Thickness margins = default)
        {
            this.Margin = margins;
            container.Children.Add(this);
        }
        public void AddInterfaceToContainer(StackPanel container, Thickness margins = default)
        {
            this.Margin = margins;
            container.Children.Add(this);
        }
        public void AddInterfaceToContainer(Canvas container, int left = 0, int top = 0)
        {
            Canvas.SetLeft(this, left);
            Canvas.SetTop(this, top);
            container.Children.Add(this);
        }
        public void RemoveInterface(Grid container)
        {
            container.Children.Remove(this);
        }
        public void RemoveInterface(StackPanel container)
        {
            container.Children.Remove(this);
        }
        public void RemoveInterface(Canvas container)
        {
            container.Children.Remove(this);
        }
    }
}
