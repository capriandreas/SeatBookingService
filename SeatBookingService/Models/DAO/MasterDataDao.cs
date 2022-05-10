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

        public List<MSBus> GetAllMasterBus()
        {
            var query = @"select a.id, a.no_bus, a.no_polisi, a.jumlah_seat, a.kelas_id, b.kelas_bus 
                        from ms_bus a
                        left join ms_kelas_bus b on b.id = a.kelas_id";

            return _sQLHelper.queryList<MSBus>(query, null).Result;
        }

        
    }
}
