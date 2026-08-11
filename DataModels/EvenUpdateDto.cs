using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace college_events_desktop.DataModels
{
    public class EventUpdateDto
    {
        public Event Event { get; set; }
        public List<EventGroups> Groups { get; set; }
    }

    public class EventPostDto
    {
        public int EventId { get; set; }

        public string Title { get; set; }

        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }

        public string FullDescription { get; set; }

        public string ShortDescription { get; set; }

        public int OrganizerId { get; set; }

        public string OrganizerPosition { get; set; }

        public string OrganizerOrganization { get; set; }

        public int CategoryId { get; set; }

        public int MaxListenersCount { get; set; }

        public int MaxParticipantsCount { get; set; }

        public string AdditionalInfo { get; set; }

        public List<int> EventLocationsIds { get; set; }
    }

    public class EventGroupPost
    {

        public int GroupId { get; set; }

        public int ExpectedListenersCount { get; set; }

        public int ExpectedParticipantsCount { get; set; }

        public int ExpectedSuperParticipantsCount { get; set; }
    }
}
