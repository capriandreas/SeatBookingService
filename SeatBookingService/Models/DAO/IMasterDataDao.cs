using SeatBookingService.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeatBookingService.Models.DAO
{
    public interface IMasterDataDao
    {
        public List<MSBus> GetAllMasterBus();
        public List<MSStationsRoutes> GetAllOriginCity();
        public List<MSStationsRoutes> GetAllDestinationCity(MSStationsRoutes obj);
        public List<MSUsersDto> GetAllListUsers();
        public List<MSClassBusDto> GetAllMasterClassBus();
    }
}
