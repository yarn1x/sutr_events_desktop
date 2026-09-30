using System;

namespace college_events_desktop.Model
{

    public static class EventConstants
    {
        /// <summary>
        /// Уникальный идентификатор для статуса предложенного, есть конфликт времени.
        /// </summary>
        /// <remarks>
        /// СТАТУС ИДЕНТИФИЦИРУЕТСЯ ТОЛЬКО НА КЛИЕНТЕ (в базе данных нет такого статуса).
        /// </remarks>
        public const int status_suggested_conflict = -1;
        
        
        /// <summary>
        /// Уникальный идентификатор для статуса предложенного
        /// </summary>
        public const int status_suggested = 1;


        /// <summary>
        /// Уникальный идентификатор для статуса запланированного
        /// </summary>
        public const int status_applied = 2;


        /// <summary>
        /// Уникальный идентификатор для статуса прошедшего, требуется составление отчёта
        /// </summary>
        public const int status_done_report_needed = 3;


        /// <summary>
        /// Уникальный идентификатор для статуса прошедшего
        /// </summary>
        public const int status_done = 4;


        /// <summary>
        /// Уникальный идентификатор для статуса перенесённого
        /// </summary>
        public const int status_rescheduled = 5;


        /// <summary>
        /// Уникальный идентификатор для статуса отменённого
        /// </summary>
        public const int status_rejected = 6;

    }

    public static class AuthorizedUserConstants
    {
        /// <summary>
        /// Уникальный идентификатор для роли администратора
        /// </summary>
        public const int administratorTypeId = 1;


        /// <summary>
        /// Уникальный идентификатор для роли куратора
        /// </summary>
        public const int supervisorTypeId = 2;


        /// <summary>
        /// Уникальный идентификатор для роли организатора
        /// </summary>
        public const int organizerTypeId = 3;
    }

    public static class StringConstants
    {
        public const string mainWindow_Title_EventsList = "Главное окно - список мероприятий";
        public const string mainWindow_Title_EventEdit = "Главное окно - редактирование информации о мероприятии";
        public const string mainWindow_Title_EventEditStatistic = "Главное окно - редактирование статистики посещения мероприятия";
        public const string mainWindow_Title_EventSeeStatistic = "Главное окно - просмотр статистики посещения мероприятия";

        public const string mainWindow_Title_OrganizerList = "Главное окно - список организаторов";
        public const string mainWindow_Title_OrganizerSeeStatistic = "Главное окно - просмотр статистики организатора";


        public const string mainWindow_Title_GroupList = "Главное окно - список групп";
        public const string mainWindow_Title_GroupSeeStatistic = "Главное окно - просмотр статистики группы";

        public const string mainWindow_Title_SupervisorList = "Главное окно - список кураторов";
        public const string mainWindow_Title_SupervisorSeeStatistic = "Главное окно - просмотр статистики куратора";

        public const string mainWindow_Title_UserList = "Главное окно - список пользователей";
        public const string mainWindow_Title_UserEditAccount = "Главное окно - редактирование информации о пользователе";

        public const string mainWindow_Title_UserNewAccount = "Главное окно - создание нового аккаунта";
    }
}
