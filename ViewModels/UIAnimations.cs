using System;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using System.Windows;

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
    }
}
