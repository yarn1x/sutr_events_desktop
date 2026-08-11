using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace college_events_desktop.DataModels
{
    public class Group
    {
        public int groupId { get; set; }
        public string groupName { get; set; }
        public string supervisorName { get; set; }
        public string supervisorSurname { get; set; }
        public string supervisorMiddlename { get; set; }
        public string supervisorEmail { get; set; }
        public string supervisorPhone { get; set; }
        public int eventsCount { get; set; }
    }
}
