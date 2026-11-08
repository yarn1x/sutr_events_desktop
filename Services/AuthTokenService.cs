using college_events_desktop.DataModels;
using college_events_desktop.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace college_events_desktop.Services
{
    public class AuthTokenService
    {
        private readonly ApiClient _apiClient;
        private string _accessToken;
        private string _refreshToken;
        private DateTime _accessTokenExpiresUtc;
        private DateTime _refreshTokenExpiresUtc;

        // Событие для UI: если сессию не удалось восстановить, нужно выкинуть на окно логина
        public event Action OnSessionExpired;

        public AuthTokenService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public bool IsAuthenticated => !string.IsNullOrEmpty(_accessToken) && _accessTokenExpiresUtc > DateTime.UtcNow;

        public async Task<bool> LoginAsync(string login, string password)
        {
            // ВАЖНО: Обратите внимание, сервер ждет поле "passwordHash", 
            // если там хеш - нужно хешировать здесь. Если пароль в открытом виде - передавайте как есть.
            var response = await _apiClient.LoginAsync(login, password);

            if (response == null) return false;

            ApplyTokens(response);
            return true;
        }

        /// <summary>
        /// Проверяет, не истек ли Access-токен. Если истек - обновляет его через Refresh-токен.
        /// </summary>
        public async Task<bool> EnsureValidTokenAsync()
        {
            // Если Access-токен еще жив (с запасом в 1 минуту) - ничего не делаем
            if (_accessTokenExpiresUtc > DateTime.UtcNow.AddMinutes(1))
                return true;

            // Если Refresh-токен протух - сессию не спасти
            if (_refreshTokenExpiresUtc <= DateTime.UtcNow)
            {
                Logout();
                return false;
            }

            // Пытаемся обновить
            var response = await _apiClient.RefreshTokenAsync(_refreshToken);
            if (response == null)
            {
                Logout();
                return false;
            }

            ApplyTokens(response);
            return true;
        }

        private void ApplyTokens(AuthResponse response)
        {
            _accessToken = response.accessToken;
            _refreshToken = response.refreshToken;
            _accessTokenExpiresUtc = response.accessTokenExpiresInUTC;
            _refreshTokenExpiresUtc = response.refreshTokenExpiresInUTC;

            _apiClient.SetAuthorizationHeader(_accessToken);
        }

        public void Logout()
        {
            _accessToken = null;
            _refreshToken = null;
            _accessTokenExpiresUtc = DateTime.MinValue;
            _refreshTokenExpiresUtc = DateTime.MinValue;

            _apiClient.ClearAuthorizationHeader();
            OnSessionExpired?.Invoke();
        }
    }
}
