using college_events_desktop.DataModels;
using college_events_desktop.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace college_events_desktop.Services
{
    /// <summary>
    /// DataService - класс для представления информации, взятой из БД.
    /// </summary>
    public class DataService
    {
        /// <summary> Экземпляр класса для совершения http запросов к серверу </summary>
        internal readonly ApiClient apiClient;


        /// <summary> Возвращает jwt токен сессии</summary>
        internal string _jwtToken { get; private set; }


        /// <summary> Возвращает "Срок годности" токена сессии в минутах </summary>
        internal DateTime _jwtExpiresIn { get; private set; }


        /// <summary> Возвращает полученный список мероприятий из БД. Для получения вызвать метод <see cref="LoadEventsAsync"/> </summary>
        public List<Event> events { get; private set; }


        /// <summary> Возвращает полученный список критериев оценивания групп на мероприятии. Для получения вызвать метод <see cref="LoadCriteriaAsync"/> </summary>
        public List<Criterion> criteria { get; private set; }


        public Dictionary<int, int> criteriaDict { get; private set; } = new Dictionary<int, int>();


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


        /// <summary> Возвращает полученную статистическую информацию об организаторской деятельности пользователя из БД. Для получения вызвать метод <see cref="LoadOrganizerStatistic(int)"/> </summary>
        public OrganizerStatistic organizerStatistic { get; private set; }


        /// <summary> Возвращает полученную статистическую информацию о группе из БД. Для получения вызвать метод <see cref="LoadGroupStatistic(int)"/> </summary>
        public GroupStatistic groupStatistic { get; private set; }


        /// <summary> Возвращает полученную статистическую информацию о группе из БД. Для получения вызвать метод <see cref="LoadGroupStatistic(int)"/> </summary>
        public SupervisorStatistic supervisorStatistic { get; private set; }



        public DataService(ApiClient apiClient)
        {
            this.apiClient = apiClient;
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
                _jwtExpiresIn = response.expiresInUTC;
            }
            apiClient._client.DefaultRequestHeaders.Clear();
            apiClient._client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_jwtToken}");
        }

        public async Task LoadEventsAsync()
        {    
            events = await apiClient.GetAllEventsAsync();
        }
        public async Task LoadCriteriaAsync()
        {
            criteria = await apiClient.GetCriteriaAsync();
            foreach (var e in criteria)
            {
                criteriaDict.Add(e.criterionId, e.score);
            }
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
        public async Task LoadGroupStatistic(int groupId)
        {
            groupStatistic = await apiClient.GetGroupStatisticAsync(groupId);
        }
        public async Task LoadSupervisorStatistic(int supervisorId)
        {
            supervisorStatistic = await apiClient.GetSupervisorStatistic(supervisorId);
        }

        #endregion
    }
}
