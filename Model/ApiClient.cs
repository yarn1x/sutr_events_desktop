using college_events_desktop.DataModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace college_events_desktop.Model
{
    public class ApiClient
    {
        public readonly HttpClient _client = new HttpClient()
        {
            //BaseAddress = new Uri("http://192.168.1.253:33679/college/admin/")
            BaseAddress = new Uri("https://localhost:7280/college/admin/")
        };
        

        public double CalculateTimeDuration(DateTime start, DateTime end)
        {
            return (end - start).TotalMinutes;
        }

        private async Task<T> GetAsync<T>(string endpoint)
        {
            try
            {
                HttpResponseMessage response = await _client.GetAsync(endpoint);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(json);
                }
                return default;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GET Api error: {ex}");
                return default;
            }
        }

        /// <summary>
        /// POST запрос к API
        /// </summary>
        /// <param name="login">логин авторизации</param>
        /// <param name="password">пароль авторизации</param>
        /// <returns>
        /// AuthResponse - класс с параметрами: token - jwt токен; expiresIn - время действия токена
        /// </returns>
        public async Task<AuthResponse> LoginAsync(string login, string password)
        {
            var loginData = new { login, passwordHash = password };
            StringContent content = new StringContent(JsonConvert.SerializeObject(loginData), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _client.PostAsync("auth/login", content);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                AuthResponse authResponse = JsonConvert.DeserializeObject<AuthResponse>(json);
                return authResponse;
            }
            return null;
        }

        /// <summary>
        /// PUT запрос для обновления списка групп, записанных на мероприятие. Метод определяет на стороне сервера какие мероприятия были добавлены, какие изменены, а какие удалены
        /// </summary>
        /// <param name="groups">Список групп, записанных на мероприятие</param>
        /// <param name="eventId">id мероприятия, на которое записан список групп</param>
        /// <returns></returns>
        public async Task<bool> UpdateEventGroups(EventUpdateDto body, int eventId)
        {
            StringContent content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _client.PutAsync($"events/update/{eventId}", content);
            string json = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> UpdateEventStatus(int EventId, int StatusId)
        {
            HttpResponseMessage response = await _client.PutAsync($"events/{EventId}/status/{StatusId}", null);

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> CreateLocation(Location body)
        {
            StringContent content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _client.PostAsync("places/place", content);
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                return true;
            }
            return false;
        }

        /// <summary>
        /// GET Запрос к API
        /// </summary>
        /// <returns>
        /// Список мероприятий. Новое мероприятие добавляется в начало списка
        /// </returns>
        internal async Task<List<Event>> GetAllEventsAsync() => await GetAsync<List<Event>>("events");

        /// <summary>
        /// GET Запрос к API
        /// </summary>
        /// <returns>
        /// Список всех категорий (направлений мероприятий).
        /// </returns>
        internal async Task<List<Category>> GetListOfCategories() => await GetAsync<List<Category>>("events/categories");

        /// <summary>
        /// GET Запрос к API
        /// </summary>
        /// <param name="eventId">Уникальный идентификатор мероприятия</param>
        /// <returns>Список групп, которые закреплены к мероприятию</returns>
        internal async Task<List<EventGroups>> GetEventGroupsByEventId(int eventId) => await GetAsync<List<EventGroups>>($"events/{eventId}/groups");

        /// <summary> GET Запрос к API </summary>
        /// <returns> Список локаций, в которых проходят мероприятия. </returns>
        internal async Task<List<Location>> GetListOfPlaces() => await GetAsync<List<Location>>("places");

        /// <summary>
        /// GET Запрос к API
        /// </summary>
        /// <returns>
        /// Список всех студенческих групп.
        /// </returns>
        internal async Task<List<Group>> GetListOfGroups() => await GetAsync<List<Group>>("groups");

        /// <summary>
        /// GET Запрос к API
        /// </summary>
        /// <returns>
        /// Список всех организаторов.
        /// </returns>
        internal async Task<List<Organizer>> GetListOfOrganizers() => await GetAsync<List<Organizer>>("organizers");
    }
}
