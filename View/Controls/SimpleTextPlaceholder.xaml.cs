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
    public partial class SimpleTextPlaceholder : UserControl
    {
        public string Text;

        public SimpleTextPlaceholder(string text, Color Background, Color BorderBrush, Color Foreground)
        {
            InitializeComponent();

            Text = text;
            text_body.Text = Text;
            border_bg.Background = Brush(Background);
            border_bg.BorderBrush = Brush(BorderBrush);
            text_body.Foreground = Brush(Foreground);
        }

        private SolidColorBrush Brush(Color color) => new SolidColorBrush(color);
    }
}
