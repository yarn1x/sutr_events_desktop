using System.Windows;
using System.Windows.Controls;

namespace college_events_desktop.View.Controls.Containers
{
    public partial class ContentBox : UserControl
    {
        public ContentBox()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(
                nameof(Header),
                typeof(string),
                typeof(ContentBox),
                new PropertyMetadata("Добавьте заголовок"));

        public string Header
        {
            get => (string)GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }
    }
}