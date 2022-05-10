using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeatBookingService.Helper
{
    public interface ISQLHelper
    {
        public Task<List<T>> queryList<T>(string query, Dictionary<string, object> param) where T : new();
    }
}
