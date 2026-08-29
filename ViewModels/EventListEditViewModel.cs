using college_events_desktop.DataModels;
using college_events_desktop.View.Layers.Events;
using System.Collections.Generic;

namespace college_events_desktop.ViewModels
{
    public class EventListEditViewModel
    {


        public Event eventData { get; private set; }
        public List<EventGroups> groupsData { get; private set; }
        

        public EventListEditViewModel(page_EventList_edit page)
        {

        }
    }
}
