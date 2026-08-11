using college_events_desktop.ViewModels;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace college_events_desktop.View.Controls
{
    public partial class TextPlaceholder : UserControl
    {
        Panel _parentContainer;
        Point _position;

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
            //определение координат
            Margin = new Thickness
            (
                _position.X - (ActualWidth / 2) <= 0 ? 0 : _position.X - (ActualWidth / 2), //корды по Х вычисляются как позиция курсора - ширина плашки / 2. Если корды получились отрицательными, то Х = 0
                _position.Y - 40, // позиция курсора - 40 пикселей, что бы плашка была повыше над курсором
                0,
                0
            );
        }

        public async Task ShowAsync()
        {
            await Application.Current.Dispatcher.InvokeAsync(() => 
            { 
                _parentContainer.Children.Add(this);        
            });
        }

        public async Task HideAsync()
        {
            await Task.WhenAll(
                UIAnimations.MoveObjectAsync(this, new Point(Margin.Left, Margin.Top), new Point(Margin.Left, Margin.Top - 70), 200, EasingMode.EaseInOut),
                UIAnimations.ChangeObjectOpacityAsync(this, 1, 0, 200, EasingMode.EaseOut)
            ).ContinueWith(_ =>
            {
                //после окончания двух объектов MoveObject, ChangeObjectOpacity удаляется плашка с контейнера
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (_parentContainer.Children.Contains(this))
                    {
                        _parentContainer.Children.Remove(this);
                    }
                });
            });
        }
    }
}
