using System;
using System.ComponentModel;

namespace college_events_desktop.DataModels
{
    public class EventGroups : IDataErrorInfo
    {
        public int eventGroupId { get; set; }
        public int eventId { get; set; }
        public int groupId { get; set; }
        public string name { get; set; }
        public string supervisorName { get; set; }
        public string supervisorSurname { get; set; }
        public string supervisorLastname { get; set; }
        public int expectedListenersCount { get; set; }
        public int expectedParticipantsCount { get; set; }
        public int expectedSuperParticipantsCount { get; set; }
        public string this[string columnName]
        {
            get
            {
                string error = String.Empty;
                switch (columnName)
                {
                    case nameof(expectedListenersCount):
                        if (expectedListenersCount < 0)
                        {
                            error += "Число слушателей не может быть отрицательным! ";
                        }
                        break;
                    case nameof(expectedParticipantsCount):
                        if (expectedParticipantsCount < 0)
                        {
                            error += "Число участников не может быть отрицательным! ";
                        }
                        break;
                    case nameof(expectedSuperParticipantsCount):
                        if (expectedSuperParticipantsCount < 0)
                        {
                            error += "Число супер-участников не может быть отрицательным! ";
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
