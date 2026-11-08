using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace college_events_desktop.DataModels
{
    public class AuthResponse
    {
        public string FirstName { get; set; }
        public string SurName { get; set; }
        public string LastName { get; set; }
        public int UserTypeId { get; set; }
        public string accessToken { get; set; }
        public string refreshToken { get; set; }
        public DateTime accessTokenExpiresInUTC { get; set; }
        public DateTime refreshTokenExpiresInUTC { get; set; }
    }

    public class RefreshRequest
    {
        public string refreshToken { get; set; }
    }
}
