using System;

namespace SeatBookingService.Models
{
    public class MSRoutes
    {
        public int id { get; set; }
        public int kelas_bus_id { get; set; }
        public string departure_hours { get; set; }
        public string description { get; set; }
        public string created_by { get; set; }
        public DateTime created_date { get; set; }
        public string updated_by { get; set; }
        public DateTime updated_date { get; set; }
        public string kelas_bus { get; set; }
    }
}
