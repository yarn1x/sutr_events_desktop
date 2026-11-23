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
        public User user { get; set; }
        public string login { get; set; }
        public string PasswordHash { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public List<UserUserType> roles { get; set; }
        public List<Group> supervisorGroups { get; set; } = null;
    }

    public class User
    {
        public string firstName { get; set; }
        public string surName { get; set; }
        public string lastName { get; set; } = null;
    }

    public class UserUserType
    {
        public int userUsertypeId { get; set; }
        public int userTypeId { get; set; }
        public string typeName { get; set; }
    }

    public class UserCreateUpdateDto
    {
        public int authorizedUserId { get; set; }
        public string login { get; set; }
        public string password { get; set; }     // на сервере ждёт открытый пароль (или хеш — уточните)
        public string phone { get; set; }
        public string email { get; set; }
        public List<UserGroupDto> groups { get; set; } = new List<UserGroupDto>();
        public UserDto user { get; set; } = new UserDto();
        public List<UserUserTypeDto> userUsertypes { get; set; } = new List<UserUserTypeDto>();
    }

    public class UserGroupDto
    {
        public int groupId { get; set; }
        public string groupName { get; set; }
        public int studentsCount { get; set; }
    }

    public class UserDto
    {
        public int userId { get; set; }
        public string surName { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
    }

    public class UserUserTypeDto
    {
        public int userTypeId { get; set; }
    }
}
