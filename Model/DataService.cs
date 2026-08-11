using college_events_desktop.DataModels;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace college_events_desktop.Model
{
    public class DataService
    {
        internal readonly ApiClient apiClient;
        internal string _jwtToken { get; private set; }
        internal int _jwtExpiresIn { get; private set; }

        public List<Event> events { get; private set; }

        public List<Category> categories { get; private set; }

        public List<Organizer> organizers {  get; private set; }

        public List<Group> groups { get; private set; }

        public List<Location> places { get; private set; }

        public DataService(ApiClient apiClient)
        {
            this.apiClient = apiClient;
            events = new List<Event>();
            categories = new List<Category>();
            organizers = new List<Organizer>();
            groups = new List<Group>();
            places = new List<Location>();
        }

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
        public async Task LoadPlacesListAsync()
        {
            places = await apiClient.GetListOfPlaces();
        }
    }
}
