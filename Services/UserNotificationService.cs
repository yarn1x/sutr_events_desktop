using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace college_events_desktop.Services
{
    /// <summary>
    /// Статический класс для обратной связи с пользователем
    /// </summary>
    public static class UserNotificationService
    {

        /// <summary>
        /// Возвращает значение приставки для вывода сообщения исключения
        /// </summary>
        private const string MessagePrefix = "Детали: ";
        /// <summary>
        /// Возвращает значение приставки для вывода кода ошибки
        /// </summary>
        private const string CodePrefix = "Код ошибки: ";
        /// <summary>
        /// Возвращает значение приставки для элемента списка
        /// </summary>
        private const string BulletSymbol = " — ";




        /// <summary>
        /// Построение шаблона для сообщения
        /// </summary>
        /// <param name="message">Сообщение пользователю</param>
        /// <param name="exception">Исключение, вызвавшее ошибку</param>
        /// <param name="codeword">Кодовое сочетание символов для отслеживания места вызова ошибки</param>
        private static string BuildTemplate(string message, Exception exception = null, string codeword = null)
        {
            string result = message?.TrimEnd('\r', '\n') ?? string.Empty;

            if (exception != null)
            {
                var excMessage = exception.InnerException?.Message ?? exception.Message;
                result += $"\n\n{MessagePrefix}{excMessage}";
            }

            if (!string.IsNullOrEmpty(codeword))
            {
                result += $"\n\n{CodePrefix}{codeword}";
            }

            return result;
        }



        /// <summary>
        /// Построение шаблона для сообщения с множеством ошибок
        /// </summary>
        /// <param name="title">Общий заголовок списка (пример "Список ошибок:")</param>
        /// <param name="errors">Коллекция пар (Пользовательское описание, Системное исключение)</param>
        /// <param name="codeword">Код места возникновения ошибки</param>
        private static string BuildTemplate(string title, IEnumerable<(string UserMessage, Exception Ex)> errors, string codeword = null)
        {
            var sb = new StringBuilder();
            sb.AppendLine(title);

            foreach (var error in errors)
            {
                sb.AppendLine();
                sb.Append(BulletSymbol).AppendLine(error.UserMessage);

                if (error.Ex != null)
                {
                    var excMessage = error.Ex.InnerException?.Message ?? error.Ex.Message;
                    sb.Append(MessagePrefix).AppendLine(excMessage);
                }
            }

            if (!string.IsNullOrEmpty(codeword))
            {
                sb.AppendLine();
                sb.Append(CodePrefix).Append(codeword);
            }

            return sb.ToString().TrimEnd();
        }




        /// <summary>
        /// Вывод <see cref="MessageBox"/> окна со списком агрегированных ошибок и их исключений.
        /// </summary>
        /// <remarks>Построение шаблона происходит в методе <see cref="BuildTemplate"/></remarks>
        /// <param name="title">Общий заголовок списка (пример "Список ошибок:")</param>
        /// <param name="errors">Коллекция исключений (Пользовательское описание, Системное исключение)</param>
        /// <param name="codeword">Код места возникновения ошибки</param>
        public static void ShowError(string title, IEnumerable<(string UserMessage, Exception Ex)> errors, string codeword)
        {
            string fullMessage = BuildTemplate(title, errors, codeword);
            MessageBox.Show(fullMessage, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
        }



        /// <summary>
        /// Вывод <see cref="MessageBox"/> окна с шаблонным форматом сообщения в виде ошибки.
        /// </summary>
        /// <remarks>Построение шаблона происходит в методе <see cref="BuildTemplate"/></remarks>
        /// <param name="message">Сообщение пользователю</param>
        /// <param name="exception">Исключение, вызвавшее ошибку</param>
        /// <param name="codeword">Кодовое сочетание символов для отслеживания места вызова ошибки</param>
        public static void ShowError(string message, Exception exception, string codeword)
        {
            MessageBox.Show(BuildTemplate(message, exception, codeword), "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
        }



        /// <summary>
        /// Вывод <see cref="MessageBox"/> окна с шаблонным форматом сообщения в виде ошибки
        /// </summary>
        /// <remarks>Построение шаблона происходит в методе <see cref="BuildTemplate"/></remarks>
        /// <param name="message">Сообщение пользователю</param>
        /// <param name="codeword">Кодовое сочетание символов для отслеживания места вызова ошибки</param>
        public static void ShowError(string message, string codeword)
        {
            MessageBox.Show(BuildTemplate(message, null as Exception, codeword), "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
        }



        /// <summary>
        /// Вывод <see cref="MessageBox"/> окна с шаблонным форматом сообщения в виде предупреждения
        /// </summary>
        /// <remarks>Построение шаблона происходит в методе <see cref="BuildTemplate"/></remarks>
        /// <param name="message">Сообщение пользователю</param>
        public static void ShowWarning(string message)
        {
            MessageBox.Show(BuildTemplate(message), "Предупреждение!", MessageBoxButton.OK, MessageBoxImage.Warning);
        }



        /// <summary>
        /// Вывод <see cref="MessageBox"/> окна с шаблонным форматом сообщения в виде предупреждения
        /// </summary>
        /// <remarks>Построение шаблона происходит в методе <see cref="BuildTemplate"/></remarks>
        /// <param name="message">Сообщение пользователю</param>
        /// <param name="codeword">Кодовое сочетание символов для отслеживания места вызова ошибки</param>
        /// <param name="exception">Исключение, вызвавшее ошибку</param>
        public static void ShowWarning(string message, Exception exception, string codeword)
        {
            MessageBox.Show(BuildTemplate(message, exception, codeword), "Предупреждение!", MessageBoxButton.OK, MessageBoxImage.Warning);
        }



        /// <summary>
        /// Вывод <see cref="MessageBox"/> в виде информационного окна
        /// </summary>
        /// <param name="message">Сообщение пользователю</param>
        /// <param name="caption">Заголовок <see cref="MessageBox"/></param>
        public static void ShowInformation(string message, string caption)
        {
            MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Information);
        }


        //ебать я собой горжусь этим классом
    }
}
