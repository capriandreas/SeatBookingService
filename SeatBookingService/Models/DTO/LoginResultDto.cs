using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeatBookingService.Models.DTO
{
    public class LoginResultDto
    {
        public int id { get; set; }
        public string username { get; set; }
        public string nickname { get; set; }
        public int role_id { get; set; }
        public string rolename { get; set; }
        public string token { get; set; }
    }
}
