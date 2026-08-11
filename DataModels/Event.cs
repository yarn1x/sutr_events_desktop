using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace college_events_desktop.DataModels
{
    public class Event : IDataErrorInfo
    {
        public int eventId { get; set; }
        public string title { get; set; }

        public DateTime startDateTime { get; set; }
        public int duration { get; set; }
        public DateTime endDateTime { get; set; }

        public string fullDescription { get; set; }
        public string shortDescription { get; set; }

        public int categoryId { get; set; }
        public string categoryName { get; set; }

        public List<Location> locations { get; set; }
        public List<int> eventLocationsIds { get; set; }

        public int statusId { get; set; }
        public string statusName { get; set; }

        public int organizerId { get; set; }
        public string organizerName { get; set; }
        public string organizerSurname { get; set; }
        public string organizerLastname { get; set; }
        public string organizerPosition { get; set; }
        public string organizerOrganization { get; set; }

        public string additionalInfo { get; set; }
        public int maxListenersCount { get; set; }
        public int maxParticipantsCount { get; set; }


        public string this[string columnName]
        {
            get
            {
                string error = String.Empty;
                switch (columnName)
                {
                    case nameof(startDateTime):
                        if (startDateTime.Date < DateTime.Now.Date)
                        {
                            error += "Дата начала мероприятия не может быть раньше настоящего";
                        }
                        break;


                    case nameof(endDateTime):
                        if (endDateTime.TimeOfDay < startDateTime.TimeOfDay)
                        {
                            error += "Время окончания не может быть раньше начала";
                        }
                        break;


                }

                return error;
            }
        }
        public string Error
        {
            get { return string.Empty; }
        }
    }
}
