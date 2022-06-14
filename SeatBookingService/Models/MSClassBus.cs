using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeatBookingService.Models
{
    public class MSClassBus
    {
        public int id { get; set; }
        public string kelas_bus { get; set; }
        public string created_by { get; set; }
        public DateTime created_date { get; set; }
        public string updated_by { get; set; }
        public DateTime updated_date { get; set; }
    }
}
