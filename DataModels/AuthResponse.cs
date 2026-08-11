using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace college_events_desktop.DataModels
{
    public class AuthResponse
    {
        public string token {  get; set; }
        public int expiresIn { get; set; }
    }
}
