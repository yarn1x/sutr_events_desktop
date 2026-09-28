using college_events_desktop.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace college_events_desktop.View.Helpers
{
    public static class LocationButtonFactory
    {
        public static Button CreateLocationButton(
            StackPanel container, 
            Location location, 
            Style style,
            bool isEnabled = false)
        {
            var btn = new Button()
            {
                Tag = location.locationId,
                Content = location.place,
                Style = style,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                Padding = new Thickness(5, 1, 5, 1),
                Margin = new Thickness(5),
                MinWidth = 80,
                FontSize = 14,
                Cursor = System.Windows.Input.Cursors.Hand,
                IsEnabled = isEnabled
            };

            btn.Click += (s, e) => container.Children.Remove(btn);
            return btn;
        }
    }
}
