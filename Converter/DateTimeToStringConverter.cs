using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace college_events_desktop.Converter
{
    internal class DateTimeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dateTime)
            {
                //если передан формат - используем его, иначе формат по умолчанию
                string format = parameter as string ?? "dd.MM.yyyy HH:mm";
                return dateTime.ToString(format, CultureInfo.InvariantCulture);
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str && DateTime.TryParse(str, out DateTime result))
                return result;
            return null;
        }
    }
}
