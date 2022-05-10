using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeatBookingService
{
    public class AppConfig
    {
        public MySql MySqlConfig { get; set; }
    }

    public class MySql
    {
        public string Url { get; set; }
    }
}