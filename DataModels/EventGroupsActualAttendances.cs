using System;
using System.ComponentModel;

namespace college_events_desktop.DataModels
{
    public class EventGroupsActualAttendances : IDataErrorInfo
    {
        public int eventGroupId { get; set; }
        public int actualListenersCount { get; set; }
        public int actualParticipantsCount { get; set; }
        public int actualSuperParticipantsCount { get; set; }
        public string this[string columnName]
        {
            get
            {
                string error = String.Empty;
                switch (columnName)
                {
                    case "actualListenersCount":
                        if (actualListenersCount < 0)
                        {
                            error += "Число слушателей не может быть отрицательным! ";
                        }
                        break;
                    case "actualParticipantsCount":
                        if (actualParticipantsCount < 0)
                        {
                            error += "Число участников не может быть отрицательным! ";
                        }
                        break;
                    case "actualSuperParticipantsCount":
                        if (actualSuperParticipantsCount < 0)
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
            get { throw new NotImplementedException(); }
        }
    }
}
