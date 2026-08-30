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
    /// <summary>
    /// ApiClient - класс отправки запросов на сервер.
    /// </summary>
    public class ApiClient
    {
        /// <summary>
        /// Экземпляр класса установки подключения к серверу
        /// </summary>
        public readonly HttpClient _client = new HttpClient()
        {
            //Базовая строка подключения - строка, которая приписывается ко всем эндпоинтам в начало
            //BaseAddress = new Uri("http://192.168.1.253:33679/college/admin/")
            BaseAddress = new Uri("https://localhost:7280/college/admin/")
        };
        




        //УНИВЕРСАЛЬНЫЕ МЕТОДЫ ОТПРАВКИ RESTAPI ЗАПРОСА
        private async Task<T> GetAsync<T>(string endpoint)
        {
            try
            {
                //отправка запроса на сервер
                HttpResponseMessage response = await _client.GetAsync(endpoint);
                if (response.IsSuccessStatusCode)
                {
                    //конвертация json в класс
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(json);
                }
                return default;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException.Message ?? ex.Message);
            }
        }
        private async Task<T> PutAsync<T>(string endpoint, object body = null)
        {
            HttpContent content = body != null
                ? new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json")
                : null;
            HttpResponseMessage response = await _client.PutAsync(endpoint, content);
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(json);
            }
            return default;
        }
        private async Task<bool> PutActionAsync(string endpoint, object body = null)
        {
            HttpContent content = body != null
                ? new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json")
                : null;
            HttpResponseMessage response = await _client.PutAsync(endpoint, content);
            return response.IsSuccessStatusCode;
        }
        private async Task<T> PostAsync<T>(string endpoint, object body)
        {
            StringContent content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PostAsync(endpoint, content);
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(json);
            }
            return default;
        }





        
        //TIP: JsonConvert - это класс NuGet пакета Newtonsoft.Json.
        //     Используется для конвертации объектов в json и наоборот.
        //     ГЛАВНОЕ что бы ключи и значения (в json) совпадали с полями и их типами данных (в C#)
        //
        //     Например:
        //
        //     file.json:
        //     {
        //       "student": {
        //         "name": "Robert",
        //         "age": 18
        //       }
        //     }
        //     "student" - имя класса. Но если говорить строго на языке программирования,
        //     в самом JSON-файле нет классов.
        //     "student" — это имя объекта (или ключ), который внутри C# мы превращаем (маппим) в класс.
        //
        //     name и age - поля. "Robert" и 18 - их значения соотв.
        //     name - это string, age - это int.
        //     было бы 18 в кавычках, было бы тоже string
        //
        //
        //
        //     ЧАСТО САМ ПОПАДАЛСЯ НА ОШИБКУ, В КОТОРОЙ ДАННЫЕ НЕ УДАВАЛОСЬ ПОЛУЧИТЬ ТОЛЬКО ПОТОМУ, ЧТО
        //     НАЗВАНИЯ КЛЮЧЕЙ В JSON И ПОЛЯ В КЛАССЕ C# ОТЛИЧАЛИСЬ
        //     а вообще, если тебе понадобился этот гайд, иди доучиваться на metanit.com
        

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
            //данные для входа
            var loginData = new { login, passwordHash = password };
            //преобразование данных в json
            StringContent content = new StringContent(JsonConvert.SerializeObject(loginData), Encoding.UTF8, "application/json");
            
            //отправка запроса на сервер
            HttpResponseMessage response = await _client.PostAsync("auth/login", content);

            if (response.IsSuccessStatusCode)
            {
                //считываем json ответ в виде string
                var json = await response.Content.ReadAsStringAsync();
                //полученную строку ответа преобразовываем в класс AuthResponse
                AuthResponse authResponse = JsonConvert.DeserializeObject<AuthResponse>(json);
                return authResponse;
            }
            return null;
        }

        /// <summary>
        /// PUT запрос для обновления списка групп, записанных на мероприятие. Метод определяет на стороне сервера какие мероприятия были добавлены, какие изменены, а какие удалены
        /// </summary>
        /// <param name="body">Список групп, записанных на мероприятие</param>
        /// <param name="eventId">id мероприятия, на которое записан список групп</param>
        /// <returns></returns>
        public async Task<bool> UpdateEventGroups(EventUpdateDto body, int eventId) => await PutAsync<bool>($"events/update/{eventId}", body);

        /// <summary>
        /// Обновляет статус мероприятия
        /// </summary>
        /// <param name="EventId">идентификатор мероприятия</param>
        /// <param name="StatusId">новый статус</param>
        /// <returns>true или false в зависимости от успешности выполнения</returns>
        public async Task<bool> UpdateEventStatus(int EventId, int StatusId) => await PutActionAsync($"events/{EventId}/status/{StatusId}");

        /// <summary>
        /// Создаёт новое место проведения мероприятия
        /// </summary>
        /// <param name="body">новое место</param>
        /// <returns>true или false в зависимости от успешности выполнения</returns>
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
        /// Обновляет информацию по статистике посещения группами мероприятия
        /// </summary>
        /// <param name="EventId">уникальный идентификатор мероприятия</param>
        /// <param name="body">тело запроса</param>
        /// <returns></returns>
        public async Task<bool> UpdateEventGroupsStatistics(int EventId, List<EventGroupsActualAttendances> body) => await PutActionAsync($"events/{EventId}/statistics", body);


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

        internal async Task<List<EventGroupsActualAttendances>> GetListOfEventGroupsStatistics(int eventId) => await GetAsync<List<EventGroupsActualAttendances>>($"events/{eventId}/statistics");
    }
}
