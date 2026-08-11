using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace college_events_desktop.DataModels
{
    internal class Supervisor
    {
        public string firstName {  get; set; }
        public string lastName {  get; set; }
        public string middleName {  get; set; }
        public string email {  get; set; }
        public string mobilePhone {  get; set; }
        public List<Group> groups { get; set; }
    }
}
