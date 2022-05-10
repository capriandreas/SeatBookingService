using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeatBookingService.Models.DAO
{
    public interface IMasterDataDao
    {
        public List<MSBus> GetAllMasterBus();
    }
}
