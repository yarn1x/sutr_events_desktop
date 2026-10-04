using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace college_events_desktop.DataModels
{
    public class GroupStatistic
    {
        public int eventsCount { get; set; }
        public int categoriesCount { get; set; }
        public int totalScore { get; set; }

        public List<GroupEvents> events { get; set; }
    }

    public class GroupEvents
    {
        public string title { get; set; }
        public string categoryName { get; set; }
        public int actualListenersCount { get; set; }
        public int actualParticipantsCount { get; set; }
        public int actualSuperParticipantsCount { get; set; }
        public int totalScore { get; set; }
    }
}
