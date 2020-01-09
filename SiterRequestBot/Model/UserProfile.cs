using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SiteRequest.Model
{
    public class UserProfile
    {
        public string Name { get; set; }
        public string ShowCard { get; set; }
        public string Description { get; set; }
        public DateTime callbackTime { get; set; }

        public string PhoneNumber { get; set; }
        public string bug { get; set; }

    }
}
