using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace college_events_desktop.DataModels
{
    public class Location
    {
        public int locationId { get; set; }
        public string place { get; set; }
        public bool inCollege { get; set; }
    }
}
