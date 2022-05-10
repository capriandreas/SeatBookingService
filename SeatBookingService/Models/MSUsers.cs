using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeatBookingService.Models
{
    public class MSUsers
    {
        public int id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string nickname { get; set; }
        public int role_id { get; set; }
        public string created_by { get; set; }
        public DateTime created_date { get; set; }
        public string updated_by { get; set; }
        public DateTime updated_date { get; set; }
        public string rolename { get; set; }
    }
}
