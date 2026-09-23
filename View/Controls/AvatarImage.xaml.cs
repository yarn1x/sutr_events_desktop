using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace college_events_desktop.View.Controls
{
    public partial class AvatarImage : UserControl
    {
        private static readonly ImageSource DefaultPlaceholder = LoadDefaultPlaceholder();

        public AvatarImage()
        {
            InitializeComponent();

            // Применяем значения по умолчанию вручную
            border_image_mask.CornerRadius = CornerRadius;
            image_brush.ImageSource = Source ?? DefaultPlaceholder;
        }

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register(
                nameof(Source),
                typeof(ImageSource),
                typeof(AvatarImage),
                new PropertyMetadata(null, OnSourceChanged));

        public ImageSource Source
        {
            get => (ImageSource)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (AvatarImage)d;
            var newSource = e.NewValue as ImageSource;

            control.image_brush.ImageSource = newSource ?? DefaultPlaceholder;
        }


        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(
                nameof(CornerRadius),
                typeof(CornerRadius),
                typeof(AvatarImage),
                new PropertyMetadata(new CornerRadius(10), OnCornerRadiusChanged));

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        private static void OnCornerRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (AvatarImage)d;
            control.border_image_mask.CornerRadius = (CornerRadius)e.NewValue;
        }

        private static ImageSource LoadDefaultPlaceholder()
        {
            try
            {
                return new ImageSourceConverter().ConvertFromString(
                    "pack://application:,,,/View/Resources/images/img_patient.png"
                ) as ImageSource;
            }
            catch
            {
                return null;
            }
        }
    }
}