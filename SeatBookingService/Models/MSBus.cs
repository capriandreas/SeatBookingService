using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeatBookingService.Models
{
    public class MSBus
    {
        public int id { get; set; }
        public string no_bus { get; set; }
        public string no_polisi { get; set; }
        public int jumlah_seat { get; set; }
        public int kelas_id { get; set; }
        public string created_by { get; set; }
        public DateTime created_date { get; set; }
        public string updated_by { get; set; }
        public DateTime updated_date { get; set; }
    }
}
