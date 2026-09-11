using college_events_desktop.DataModels;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace college_events_desktop.Model
{
    /// <summary>
    /// DataService - класс для представления информации, взятой из БД. Класс хранит, но не получает информацию. Для получения исп. ApiClient
    /// </summary>
    public class DataService
    {
        /// <summary> Экземпляр класса для совершения http запросов к серверу </summary>
        internal readonly ApiClient apiClient;

        /// <summary> Возвращает jwt токен сессии</summary>
        internal string _jwtToken { get; private set; }

        /// <summary> Возвращает "Срок годности" токена сессии в минутах </summary>
        internal int _jwtExpiresIn { get; private set; }

        /// <summary> Возвращает полученный список мероприятий из БД. Для получения вызвать метод <see cref="LoadEventsAsync"/> </summary>
        public List<Event> events { get; private set; }

        /// <summary> Возвращает полученный список направлений из БД. Для получения вызвать метод <see cref="LoadCategoriesAsync"/> </summary>
        public List<Category> categories { get; private set; }

        /// <summary> Возвращает полученный список организаторов из БД. Для получения вызвать метод <see cref="LoadOrganizerListAsync"/> </summary>
        public List<Organizer> organizers {  get; private set; }

        /// <summary> Возвращает полученный список всех доступных групп из БД. Для получения вызвать метод <see cref="LoadGroupsListAsync"/> </summary>
        public List<Group> groups { get; private set; }

        /// <summary> Возвращает полученный список групп зарегистрированных на мероприятие. Для получения вызвать метод <see cref="LoadEventGroupsAsync(int)"/> </summary>
        public List<EventGroups> eventGroups { get; private set; }

        /// <summary> Возвращает полученный список локаций из БД. Для получения вызвать метод <see cref="LoadPlacesListAsync"/> </summary>
        public List<Location> places { get; private set; }

        /// <summary> Возвращает полученный список пользователей из БД. Для получения вызвать метод <see cref="LoadUsersListAsync"/> </summary>
        public List<AuthorizedUser> authorizedUsers { get; private set; }

        /// <summary> Возвращает полученный список ролей из БД. Для получения вызвать метод <see cref="LoadRolesListAsync"/> </summary>
        public List<UserType> roles { get; private set; }


        /// <summary> Возвращает полученную статистическую информацию об организаторской деятельности пользователя из БД. Для получения вызвать метод <see cref="LoadRolesListAsync"/> </summary>
        public OrganizerStatistic organizerStatistic { get; private set; }

        public DataService(ApiClient apiClient)
        {
            this.apiClient = apiClient;
            events = new List<Event>();
            categories = new List<Category>();
            organizers = new List<Organizer>();
            groups = new List<Group>();
            eventGroups = new List<EventGroups>();
            places = new List<Location>();
            authorizedUsers = new List<AuthorizedUser>();
            roles = new List<UserType>();
            organizerStatistic = new OrganizerStatistic();
        }

        #region Методы получения данных

        /// <summary>
        /// Метод, получающий от сервера jwt токен
        /// </summary>
        /// <param name="login">логин пользователя</param>
        /// <param name="password">пароль пользователя</param>
        /// <returns></returns>
        public async Task GetSessionToken(string login, string password)
        {
            AuthResponse response = await apiClient.LoginAsync(login, password);
            if (response != null)
            {
                _jwtToken = response.token;
                _jwtExpiresIn = response.expiresIn;
            }
            apiClient._client.DefaultRequestHeaders.Clear();
            apiClient._client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_jwtToken}");
        }

        public async Task LoadEventsAsync()
        {    
            events = await apiClient.GetAllEventsAsync();
        }
        public async Task LoadCategoriesAsync()
        {
            categories = await apiClient.GetListOfCategories();
        }
        public async Task LoadOrganizerListAsync()
        {
            organizers = await apiClient.GetListOfOrganizers();
        }
        public async Task LoadGroupsListAsync()
        {
            groups = await apiClient.GetListOfGroups();
        }
        public async Task LoadEventGroupsAsync(int eventId)
        {
            eventGroups = await apiClient.GetEventGroupsByEventId(eventId);
        }
        public async Task LoadPlacesListAsync()
        {
            places = await apiClient.GetListOfPlaces();
        }
        public async Task LoadUsersListAsync()
        {
            authorizedUsers = await apiClient.GetListOfAuthorizedUsers();
        }
        public async Task LoadRolesListAsync()
        {
            roles = await apiClient.GetListOfRolesAsync();
        }
        public async Task LoadOrganizerStatistic(int organizerId)
        {
            organizerStatistic = await apiClient.GetOrganizerStatisticAsync(organizerId);
        }


        #endregion
    }
}
