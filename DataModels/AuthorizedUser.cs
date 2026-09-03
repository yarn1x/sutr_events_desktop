using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace college_events_desktop.DataModels
{
    public class AuthorizedUser
    {
        public int AuthorizedUserId { get; set; }

        public string firstName { get; set; }
        
        public string surName { get; set; }
        
        public string lastName { get; set; }
        
        public string Login { get; set; }

        public string PasswordHash { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public List<UserUserType> roles { get; set; }
    }

    public class UserUserType
    {
        public int userUsertypeId { get; set; }

        public int userTypeId { get; set; }
        public string typeName { get; set; }
    }
}
