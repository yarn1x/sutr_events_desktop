using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

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
        public Brush statusColor { get => GetStatusColor(statusId); }

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

        private Brush GetStatusColor(int statusId)
        {
            try
            {
                Dictionary<int, Color> markerColors = new Dictionary<int, Color>()
                {
                {-1, Color.FromArgb(0xFF, 0xFF, 0x49, 0x49)}, //красный
                {1, Color.FromArgb(0xFF, 0xFF, 0xDD, 0x3C)}, //жёлтый
                {2, Color.FromArgb(0xFF, 0x66, 0xFF, 0x3D)}, //зелёный
                {3, Color.FromArgb(0xFF, 0x48, 0xAF, 0xFF)}, //голубой
                {4, Color.FromArgb(0xFF, 0xA7, 0xA7, 0xA7)}, //серый
                {5, Color.FromArgb(0xFF, 0xA6, 0xFF, 0x7D)}, //перенесено мероприятие (цвет чуть светлее зелёного)
                {6, Color.FromArgb(0xFF, 0x8C, 0x4F, 0x1B)}, //тёмно-оранжевый
                };
                return new SolidColorBrush(markerColors[statusId]);
            }
            catch
            {
                return new SolidColorBrush(Color.FromArgb(0xFF, 0x67, 0x67, 0x67));
            }
        }
    }
}
