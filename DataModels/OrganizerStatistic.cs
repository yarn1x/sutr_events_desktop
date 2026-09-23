using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace college_events_desktop.DataModels
{
    public class OrganizerStatistic
    {
        public int studentsCount { get; set; }
        public int eventsCount { get; set; }
        public int supervisorsCount { get; set; }

        public List<OrganizerEvents> events { get; set; }
    }


    public class OrganizerEvents
    {
        public string title { get; set; }
        public DateTime startDatetime { get; set; }
        public DateTime endDatetime { get; set; }
        public string fullDescription { get; set; }
        public string CategoryName { get; set; }

        public int actualListenersCount { get; set; }
        public int actualParticipantsCount { get; set; }
        public int actualSuperParticipantsCount { get; set; }

    }
}
