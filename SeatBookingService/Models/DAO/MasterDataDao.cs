using SeatBookingService.Helper;
using SeatBookingService.Models.DTO;
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
            var query = @"
                        select distinct city
                        from
                        (
	                        select *, max(case when city = @city then route_order else 0 end) over (partition by route) min_route
	                        from ms_stations_routes
                        )a
                        where route_order > min_route";

            var param = new Dictionary<string, object> { 
                { "city", obj.city } 
            };

            return _sQLHelper.queryList<MSStationsRoutes>(query, param).Result;
        }

        public List<MSUsersDto> GetAllListUsers()
        {
            var query = @"select a.username, a.nickname, a.role_id, b.rolename, a.created_date
                            from ms_users a
                            left join ms_roles b on a.role_id = b.id
                            where a.role_id <> 1 and a.is_active = 1 order by role_id";

            return _sQLHelper.queryList<MSUsersDto>(query, null).Result;
        }

        public List<MSBus> GetAllMasterBus()
        {
            var query = @"select a.id, a.no_bus, a.no_polisi, a.jumlah_seat, a.class_bus_id, b.class_bus 
                        from ms_bus a
                        left join ms_class_bus b on b.id = a.class_bus_id";

            return _sQLHelper.queryList<MSBus>(query, null).Result;
        }

        public List<MSClassBusDto> GetAllMasterClassBus()
        {
            var query = @"select id, class_bus from ms_class_bus";

            return _sQLHelper.queryList<MSClassBusDto>(query, null).Result;
        }

        public List<MSRolesDto> GetAllMasterRoles()
        {
            var query = @"select id, rolename from ms_roles where rolename not in ('SuperAdmin')";

            return _sQLHelper.queryList<MSRolesDto>(query, null).Result;
        }

        public List<MSStationsRoutes> GetAllOriginCity()
        {
            var query = @"select distinct city
                        from ms_stations_routes a
                        where route_order = 1";

            return _sQLHelper.queryList<MSStationsRoutes>(query, null).Result;
        }

        public List<MSStationsRoutesDto> GetAllStationRoutes()
        {
            var query = @" select 
                                    b.id as 'id_route',
		                            GROUP_CONCAT(a.city separator ' - ') as `Route`,
		                            b.departure_hours,
		                            c.class_bus,
		                            b.description,
		                            1 as trip_type_id
	                            from ms_stations_routes a
	                            left join ms_routes b on b.id = a.routes_id
	                            left join ms_class_bus c on c.id = b.class_bus_id
	                            where b.is_active = 1
	                            group by b.id";

            return _sQLHelper.queryList<MSStationsRoutesDto>(query, null).Result;
        }
    }
}
