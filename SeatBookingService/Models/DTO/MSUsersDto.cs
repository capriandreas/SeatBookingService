using System;

namespace SeatBookingService.Models.DTO
{
    public class MSUsersDto
    {
        public string username { get; set; }
        public string nickname { get; set; }
        public int role_id { get; set; }
        public string rolename { get; set; }
        public DateTime created_date { get; set; }
    }
}
