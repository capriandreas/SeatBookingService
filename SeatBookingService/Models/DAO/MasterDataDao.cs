using SeatBookingService.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeatBookingService.Models.DAO
{
    public class MasterDataDao : IMasterDataDao
    {
        ISQLHelper _sQLHelper;

        public MasterDataDao(ISQLHelper sQLHelper)
        {
            _sQLHelper = sQLHelper;
        }

        public List<MSStationsRoutes> GetAllDestinationCity(MSStationsRoutes obj)
        {
            var query = @"select distinct city 
                            from ms_stations_routes a
                            where route in (SELECT distinct route from ms_stations_routes where city = @city and route_order = 1)
                            and a.city <> @city";

            var param = new Dictionary<string, object> { 
                { "city", obj.city } 
            };

            return _sQLHelper.queryList<MSStationsRoutes>(query, param).Result;
        }

        public List<MSBus> GetAllMasterBus()
        {
            var query = @"select a.id, a.no_bus, a.no_polisi, a.jumlah_seat, a.kelas_id, b.kelas_bus 
                        from ms_bus a
                        left join ms_kelas_bus b on b.id = a.kelas_id";

            return _sQLHelper.queryList<MSBus>(query, null).Result;
        }

        public List<MSStationsRoutes> GetAllOriginCity()
        {
            var query = @"select distinct city
                        from ms_stations_routes a
                        where route_order = 1";

            return _sQLHelper.queryList<MSStationsRoutes>(query, null).Result;
        }
    }
}
