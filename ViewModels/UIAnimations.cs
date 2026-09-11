using System;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

namespace college_events_desktop.ViewModels
{
    internal static class UIAnimations
    {

        /// <summary>
        /// Асинхронное перемещение UI элемента
        /// </summary>
        /// <typeparam name="T">Класс перемещаемого элемента</typeparam>
        /// <param name="element">Экземпляр класса или объект</param>
        /// <param name="startPos">Точка начального положения UI элемента</param>
        /// <param name="targetPos">Точка конечного положения UI элемента</param>
        /// <param name="durationMs">Продолжительность анимации в миллисекундах</param>
        /// <param name="easingMode">Модификация плавности анимации</param>
        public static async Task MoveObjectAsync<T>(
            T element, 
            Point startPos, 
            Point targetPos, 
            int durationMs = 1000, 
            EasingMode? easingMode = null) where T : FrameworkElement
        {
            var startX = startPos.X;
            var startY = startPos.Y;
            var targetX = targetPos.X;
            var targetY = targetPos.Y;

            //настройка анимации
            IEasingFunction easingFunction = null;
            if (easingMode.HasValue) { easingFunction = new QuadraticEase { EasingMode = easingMode.Value }; }
            var animationX = new ThicknessAnimation
            {
                From = new Thickness(startX, startY, 0, 0),
                To = new Thickness(targetX, targetY, 0, 0),
                Duration = TimeSpan.FromMilliseconds(durationMs),
                EasingFunction = easingFunction
            };

            var tcs = new TaskCompletionSource<bool>();
            await Application.Current.Dispatcher.InvokeAsync(() => {
                
                //подписываемся на завершение анимации
                EventHandler handler = null;
                handler = (s, e) =>
                {
                    animationX.Completed -= handler;
                    tcs.SetResult(true);
                };

                animationX.Completed += handler;

                //запускаем анимации
                element.BeginAnimation(FrameworkElement.MarginProperty, animationX);
            });
            await tcs.Task;
        }



        /// <summary>
        /// Асинхронное изменение значения альфа-канала элемента
        /// </summary>
        /// <typeparam name="T">Класс элемента</typeparam>
        /// <param name="element">Экземпляр класса или объект</param>
        /// <param name="startOpacity">Начальное значение альфа (от 0 до 1)</param>
        /// <param name="targetOpacity">Конечное значение альфа (от 0 до 1)</param>
        /// <param name="durationMs">Продолжительность анимации в миллисекундах</param>
        /// <param name="easingMode">Модификация плавности анимации</param>
        public static async Task ChangeObjectOpacityAsync<T>(
            T element, 
            double startOpacity, 
            double targetOpacity, 
            int durationMs = 1000, 
            EasingMode? easingMode = null) where T : FrameworkElement
        {
            //настройка анимации
            IEasingFunction easingFunction = null;
            if (easingMode.HasValue) { easingFunction = new QuadraticEase { EasingMode = easingMode.Value }; }
            DoubleAnimation animation = new DoubleAnimation()
            {
                From = startOpacity,
                To = targetOpacity,
                Duration = TimeSpan.FromMilliseconds(durationMs),
                EasingFunction = easingFunction
            };

            var tcs = new TaskCompletionSource<bool>();
            await Application.Current.Dispatcher.InvokeAsync(() => {
                
                //подписываемся на завершение анимации
                EventHandler handler = null;
                handler = (s, e) =>
                {
                    animation.Completed -= handler;
                    tcs.SetResult(true);
                };

                animation.Completed += handler;

                //запускаем анимации
                element.BeginAnimation(FrameworkElement.OpacityProperty, animation);
            });
            await tcs.Task;
        }



        /// <summary>
        /// Параметр для пользовательского цвета шрифта у UI элемента. Используется в <see cref="ChangeColorAsync{T}(T, Color, int, EasingMode?)"/>
        /// </summary>
        /// <remarks>
        /// Применяется когда задний фон элемента становится слишком тёмный
        /// </remarks>
        public static SolidColorBrush lightForegroundCase { get; set; }

        /// <summary>
        /// Параметр для пользовательского цвета шрифта у UI элемента. Используется в <see cref="ChangeColorAsync{T}(T, Color, int, EasingMode?)"/>
        /// </summary>
        /// <remarks>
        /// Применяется когда задний фон элемента становится слишком светлый
        /// </remarks>
        public static SolidColorBrush darkForegroundCase { get; set; }

        /// <summary>
        /// Метод изменяющий цвет UI элемента на заданный через анимацию
        /// </summary>
        /// <typeparam name="T">Класс UI элемента</typeparam>
        /// <param name="element">Экземпляр класса UI элемента</param>
        /// <param name="color">Цвет для перекрашивания</param>
        /// <param name="durationMs">Длительность анимации в миллисекундах</param>
        /// <param name="easingMode">Модификатор плавности анимации</param>
        /// <returns></returns>
        public static async Task ChangeColorAsync<T>(
            T element,
            Color color,
            int durationMs = 1000,
            EasingMode? easingMode = null) where T : Control
        {
            if (element == null) return;

            if (element.Background is SolidColorBrush currentBrush)
            {
                if (currentBrush.IsFrozen)
                {
                    element.Background = currentBrush.Clone();
                }
            }
            else
            {
                element.Background = new SolidColorBrush(Colors.Transparent);
            }

            IEasingFunction easingFunction = null;
            if (easingMode.HasValue)
            {
                easingFunction = new QuadraticEase { EasingMode = easingMode.Value };
            }

            ColorAnimation animation = new ColorAnimation()
            {
                To = color,
                Duration = TimeSpan.FromMilliseconds(durationMs),
                EasingFunction = easingFunction,
            };

            var tcs = new TaskCompletionSource<bool>();
            animation.Completed += (s, e) => tcs.SetResult(true);


            //формула вычисляет значение яркости полученного цвета
            //точные коэффициенты взяты из международного стандарта ITU - R BT.601
            double luminance = (0.299 * color.R) + (0.587 * color.G) + (0.155 * color.B);
            SolidColorBrush light = lightForegroundCase != null ? lightForegroundCase : new SolidColorBrush(Colors.White);
            SolidColorBrush dark = darkForegroundCase != null ? darkForegroundCase : new SolidColorBrush(Colors.Black);
            Brush targetForeground = luminance < 128 ? light : dark;
            element.Foreground = targetForeground;

            element.Background.BeginAnimation(SolidColorBrush.ColorProperty, animation);
            await tcs.Task;
        }

    }
}
