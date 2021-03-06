using System;

namespace SeatBookingService.Models
{
    public class MSSeat
    {
        public int id { get; set; }
        public int class_bus_id { get; set; }
        public string seat_column { get; set; }
        public string seat_row { get; set; }
        public int seat_order { get; set; }
        public string created_by { get; set; }
        public DateTime created_date { get; set; }
        public string updated_by { get; set; }
        public DateTime updated_date { get; set; }
        public string kelas_bus { get; set; }
    }
}
