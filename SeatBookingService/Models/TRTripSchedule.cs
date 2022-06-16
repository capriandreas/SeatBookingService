using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeatBookingService.Models
{
    public class TRTripSchedule
    {
        public int id { get; set; }
        public int class_bus_id { get; set; }
        public DateTime? schedule_date { get; set; }
        public string origin { get; set; }
        public string origin_additional_information { get; set; }
        public string destination { get; set; }
        public string destination_additional_information { get; set; }
        public string departure_hours { get; set; }
        public string description { get; set; }
        public string created_by { get; set; }
        public DateTime created_date { get; set; }
        public string updated_by { get; set; }
        public DateTime updated_date { get; set; }
    }
}
