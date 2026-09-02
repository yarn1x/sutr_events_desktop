using college_events_desktop.View.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace college_events_desktop.Services
{
    public static class LoadingService
    {
        private static int _loadingCounter = 0;
        private static MainWindow _mainWindow;

        public static void Register(MainWindow mainWindow)
        {
            _mainWindow = mainWindow ?? throw new ArgumentNullException(nameof(mainWindow));
        }

        /// <summary>
        /// Отображение оверлея загрузки (увеличивает счетчик задач).
        /// </summary>
        public static void Show()
        {
            // На всякий случай перенаправляем в UI-поток, если вызов прилетел из фона
            Application.Current.Dispatcher.VerifyAccess();

            _loadingCounter++;
            _mainWindow?.ShowLoading(true);
        }

        /// <summary>
        /// Скрытие оверлея загрузки (уменьшает счетчик задач).
        /// </summary>
        public static void Hide()
        {
            Application.Current.Dispatcher.VerifyAccess();

            _loadingCounter--;
            if (_loadingCounter <= 0)
            {
                _loadingCounter = 0;
                _mainWindow?.ShowLoading(false);
            }
        }

        /// <summary>
        /// Удобный запуск загрузки через конструкцию using. 
        /// Оверлей автоматически скроется, когда выполнение выйдет за пределы блока.
        /// </summary>
        /// <returns>Объект, вызывающий метод <see cref="Hide"/> при утилизации.</returns>
        public static IDisposable StartLoading()
        {
            //для тех, кто в танке, для использования данного метода, нужно:
            //1. создать поле ILoadingService
            //2. присвоить полю новый экземпляр LoadingService
            //3. в месте, где нужно показать интерфейс загрузки, использовать конструкцию using
            //4. внутрь помещаем всё, что должно выполниться при ожидании
            //5. после выполнения тела конструкции using, интерфейс сам скроется (произойдёт вызов new DisposableAction(Hide);)
            Show();
            return new DisposableAction(Hide);
        }
    }

    /// <summary>
    /// Класс-обертка для выполнения действия при уничтожении объекта.
    /// </summary>
    public class DisposableAction : IDisposable
    {
        private readonly Action _action;
        public DisposableAction(Action action) => _action = action;
        public void Dispose() => _action?.Invoke();
    }
}
