using college_events_desktop.View.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace college_events_desktop.Services
{
    public interface ILoadingService
    {
        void Show();
        void Hide();

        /// <summary>
        /// Магия! исп. using (_.StartLoading) { } для автоматического dispose
        /// </summary>
        IDisposable StartLoading();
    }

    /// <summary>
    /// Класс реализует интерфейс ILoadingService и предназначен для отображения загрузочного оверлея
    /// </summary>
    internal class LoadingService : ILoadingService
    {
        private readonly MainWindow _mainWindow;
        private int _loadingCounter = 0;


        public LoadingService(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
        }


        /// <summary>
        /// Отображение оверлея
        /// </summary>
        public void Show()
        {
            _loadingCounter++;
            _mainWindow.ShowLoading(true);
        }


        /// <summary>
        /// Скрытие оверлея
        /// </summary>
        public void Hide()
        {
            _loadingCounter--;
            if (_loadingCounter <= 0)
            {
                _loadingCounter = 0;
                _mainWindow.ShowLoading(false);
            }
        }


        /// <summary>
        /// для автоматического dispose
        /// </summary>
        /// <returns>Объект, выполняющий метод скрытия оверлея загрузки</returns>
        public IDisposable StartLoading()
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

    public class DisposableAction : IDisposable
    {
        private Action _action;
        public DisposableAction(Action action) => _action = action;
        public void Dispose() => _action?.Invoke();
    }
}
