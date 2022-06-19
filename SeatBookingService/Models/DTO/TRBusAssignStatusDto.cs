using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeatBookingService.Models.DTO
{
    public class TRBusAssignStatusDto
    {
        public string no_bus { get; set; }
        public string no_polisi { get; set; }
        public int jumlah_seat { get; set; }
        public int class_bus_id { get; set; }
        public string class_bus { get; set; }
        public int status_bus_id { get; set; }
        public string status_bus { get; set; }
        public DateTime assign_date { get; set; }
    }
}
