using System;

namespace SeatBookingService.Models
{
    public class TRTrip
    {
        public int id { get; set; }
        public int route_id { get; set; }
        public int trip_type_id { get; set; }
        public DateTime schedule_date { get; set; }
        public string created_by { get; set; }
        public DateTime created_date { get; set; }
        public string updated_by { get; set; }
        public DateTime updated_date { get; set; }
    }
}
