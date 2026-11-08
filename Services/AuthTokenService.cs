using college_events_desktop.DataModels;
using college_events_desktop.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace college_events_desktop.Services
{
    public class AuthTokenService
    {
        private readonly SessionService _session;
        private readonly HttpClient _httpClient; //"Голый" клиент для запроса refresh

        public AuthTokenService(SessionService session, HttpClient httpClient)
        {
            _session = session;
            _httpClient = httpClient;
        }

        public async Task<bool> RefreshTokenAsync()
        {
            if (string.IsNullOrEmpty(_session.RefreshToken)) return false;
            if (_session.RefreshTokenExpiresUtc < DateTime.UtcNow) return false;

            var request = new RefreshRequest { refreshToken = _session.RefreshToken };
            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("auth/refresh", content);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var newAuth = JsonConvert.DeserializeObject<AuthResponse>(json);
                _session.SetSession(newAuth);
                return true;
            }
            return false;
        }
    }
}
