using college_events_desktop.DataModels;
using college_events_desktop.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace college_events_desktop.Services
{
    public class SessionService
    {
        public string AccessToken { get; private set; }
        public string RefreshToken { get; private set; }
        public DateTime AccessTokenExpiresUtc { get; private set; }
        public DateTime RefreshTokenExpiresUtc { get; private set; }

        public string FirstName { get; private set; }
        public string SurName { get; private set; }
        public string LastName { get; private set; }
        public int UserTypeId { get; private set; }

        public bool IsAuthenticated => !string.IsNullOrEmpty(AccessToken) && AccessTokenExpiresUtc > DateTime.UtcNow;
        public bool IsAdministrator => UserTypeId == AuthorizedUserConstants.administratorTypeId;
        public bool IsModerator => UserTypeId == AuthorizedUserConstants.moderatorTypeId;

        public void SetSession(AuthResponse response)
        {
            AccessToken = response.accessToken;
            RefreshToken = response.refreshToken;
            AccessTokenExpiresUtc = response.accessTokenExpiresInUTC;
            RefreshTokenExpiresUtc = response.refreshTokenExpiresInUTC;

            FirstName = response.FirstName;
            SurName = response.SurName;
            LastName = response.LastName;
            UserTypeId = response.UserTypeId;
        }

        public void Clear()
        {
            AccessToken = null;
            RefreshToken = null;
            AccessTokenExpiresUtc = DateTime.MinValue;
            RefreshTokenExpiresUtc = DateTime.MinValue;
            UserTypeId = 0;
        }
    }
}
