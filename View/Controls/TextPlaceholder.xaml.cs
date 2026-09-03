using college_events_desktop.ViewModels;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace college_events_desktop.View.Controls
{
    public partial class TextPlaceholder : UserControl
    {
        private readonly Panel _parentContainer;
        private readonly Point _position;

        public TextPlaceholder(Panel parentContainer, Point position, string text)
        {
            InitializeComponent();
            _parentContainer = parentContainer;
            _position = position;
            text_body.Text = text;

            Loaded += TextPlaceholder_Loaded;
        }

        private void TextPlaceholder_Loaded(object sender, RoutedEventArgs e)
        {
            // Вычисляем X: позиция курсора минус половина ширины плашки. Если вышли за экран (меньше 0), то 0.
            double left = _position.X - (ActualWidth / 2);
            if (left < 0) left = 0;

            Margin = new Thickness(left, _position.Y, 0, 0);
        }

        public async Task ShowAsync()
        {
            // Добавляем элемент в контейнер
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                _parentContainer.Children.Add(this);
            });
        }

        public async Task HideAsync()
        {
            // Ждем завершения анимаций перемещения и прозрачности
            await Task.WhenAll(
                UIAnimations.MoveObjectAsync(this, new Point(Margin.Left, Margin.Top), new Point(Margin.Left, Margin.Top - 70), 200, EasingMode.EaseInOut),
                UIAnimations.ChangeObjectOpacityAsync(this, 1, 0, 200, EasingMode.EaseOut)
            );

            // Код после await гарантированно выполнится в UI-потоке. Спокойно удаляем элемент.
            _parentContainer.Children.Remove(this);
        }
    }
}