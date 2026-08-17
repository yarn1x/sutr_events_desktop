using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace college_events_desktop.DataModels
{
    public class Organizer
    {
        public int userId { get; set; }
        public string firstName { get; set; }
        public string surName { get; set; }
        public string lastName { get; set; }
        public int typeId { get; set; }
        public string typeName { get; set; }
        public string email {  get; set; }
        public string mobilePhone { get; set; }
    }
}
