using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace college_events_desktop.Services
{
    internal static class ApplicationResourceService
    {
        /// <summary>
        /// Возвращает ресурс из App.xaml с автоматическим приведением к нужному типу.
        /// </summary>
        /// <typeparam name="T">Тип возвращаемого ресурса (например, Color, Brush, Style).</typeparam>
        /// <param name="resourceKey">Строковый ключ ресурса из XAML.</param>
        /// <returns>Экземпляр ресурса приведенного типа.</returns>
        /// <exception cref="ResourceReferenceKeyNotFoundException">Выбрасывается, если ключ не найден.</exception>
        /// <exception cref="InvalidCastException">Выбрасывается, если ресурс найден, но имеет другой тип.</exception>
        public static T GetResource<T>(string resourceKey)
        {
            if (string.IsNullOrWhiteSpace(resourceKey))
                throw new ArgumentException("Ключ ресурса не может быть пустым.", nameof(resourceKey));

            // FindResource ищет в App.xaml, а также проверяет системные ресурсы
            object resource = Application.Current.FindResource(resourceKey);

            return (T)resource;
        }

        /// <summary>
        /// Безопасно возвращает ресурс. Если ресурс не найден или имеет неверный тип, возвращает значение по умолчанию.
        /// </summary>
        public static T GetResourceOrDefault<T>(string resourceKey, T defaultValue = default)
        {
            try
            {
                if (Application.Current.TryFindResource(resourceKey) is object resource)
                {
                    return resource is T typedResource ? typedResource : defaultValue;
                }
            }
            catch
            {
                //подавляем воможные исключения при обращении к ресурсам (по желанию)
            }

            return defaultValue;
        }
    }
}
