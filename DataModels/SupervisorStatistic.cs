using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace college_events_desktop.DataModels
{
    public class SupervisorStatistic
    {
        public int eventsCount { get; set; }
        public int categoriesCount { get; set; }
        public int totalScore { get; set; }

        public List<SupervisorGroup> supervisorGroups { get; set; }
        public List<SupervisorEvent> events { get; set; }
    }

    public class SupervisorGroup
    {
        public string groupName { get; set; }
        public DateTime creationTime { get; set; }
    }

    public class SupervisorEvent
    {
        public string title { get; set; }
        public string categoryName { get; set; }
        public string groupName { get; set; }
        public int actualListenersCount { get; set; }
        public int actualParticipantsCount { get; set; }
        public int actualSuperParticipantsCount { get; set; }
        public int totalScore { get; set; }
    }
}
