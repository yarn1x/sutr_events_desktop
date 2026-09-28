using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace college_events_desktop.Services
{
    public static class AcademicYearCalculatorService
    {
        /// <summary>
        /// Возвращает дату начала существования учебных групп с учётом года курса
        /// </summary>
        /// <remarks>
        /// Смотреть <see cref="GetGroupStartYearByCourse"/> для пояснения
        /// </remarks>
        /// <param name="course">Курс</param>
        /// берём настоящее время и вычитаем значение курса = год начала существования группы
        /// + 4 месяца, что бы фильтр срабатывал не когда приходит новый год,
        /// а когда приходит новый УЧЕБНЫЙ год.
        /// пример: в сентябре 2026, когда 23-КИС-1 уходят уже на 4 курс, программа считывает
        /// год создания группы = 2023-01-01
        /// если не добавлять 4 месяца, фильтр будет думать, что 23-КИС-1 всё ещё на 3 курсе
        /// тк 2026 - 3 = 2023
        public static DateTime GetGroupStartYearByCourse(int course)
        {
            var year = DateTime.Now.AddMonths(4).Year - course;
            return new DateTime(year, 1, 1);
        }
    }
}
