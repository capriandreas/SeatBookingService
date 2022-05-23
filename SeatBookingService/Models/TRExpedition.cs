using System;

namespace SeatBookingService.Models
{
    public class TRExpedition
    {
        public int id { get; set; }
        public int users_id { get; set; }
        public int trip_schedule_id { get; set; }
        public decimal price { get; set; }
        public string goods_type { get; set; }
        public string volume { get; set; }
        public string additional_information { get; set; }
        public string created_by { get; set; }
        public DateTime created_date { get; set; }
        public string updated_by { get; set; }
        public DateTime updated_date { get; set; }
    }
}
