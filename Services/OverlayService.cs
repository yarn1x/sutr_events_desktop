using college_events_desktop.View.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace college_events_desktop.Services
{
    public interface IOverlayService
    {
        void Open(UIElement child);

        void Close();

    }

    public class OverlayService : IOverlayService
    {
        private readonly MainWindow window;

        public OverlayService(MainWindow window)
        {
            this.window = window;
        }

        public void Open(UIElement child)
        {
            window.grid_overlay.Visibility = Visibility.Visible;
            window.frame_overlay.Content = child;
        }
        public void Close()
        {
            window.grid_overlay.Visibility = Visibility.Collapsed;
            window.frame_overlay.Content = null;
        }
    }
}
