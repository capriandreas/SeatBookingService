using SeatBookingService.Helper;
using SeatBookingService.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeatBookingService.Models.DAO
{
    public class TransactionDao : ITransactionDao
    {
        ISQLHelper _sQLHelper;

        public TransactionDao(ISQLHelper sQLHelper)
        {
            _sQLHelper = sQLHelper;
        }

        public List<TRBusAssignStatusDto> GetListAssignedBus(TRBusAssignStatus obj)
        {
            var query = @"select h.no_bus, c.no_polisi, c.jumlah_seat, c.kelas_id, h.status_bus_id, b.status_bus, h.assign_date, d.kelas_bus
                            from tr_bus_assign_status h
                            inner join (select a1.no_bus, a1.created_date
                            from tr_bus_assign_status a1
                            inner join (select no_bus, max(created_date) as created_date from tr_bus_assign_status group by no_bus) a2
                            on a1.no_bus = a2.no_bus and a1.created_date = a2.created_date) h2
                            on h.no_bus = h2.no_bus and h.created_date = h2.created_date
                            left join ms_status_bus b on b.id = h.status_bus_id
                            left join ms_bus c on c.no_bus = h.no_bus
                            left join ms_kelas_bus d on d.id = c.kelas_id
                            where h.status_bus_id = 2 and h.assign_date = @assign_date";

            var param = new Dictionary<string, object> {
                { "assign_date", obj.assign_date }
            };

            return _sQLHelper.queryList<TRBusAssignStatusDto>(query, param).Result;
        }

        public bool SubmitTripSchedule(TRTripScheduleDto obj)
        {
            bool result = false;
            var query = string.Empty;
            var param = new Dictionary<string, object>();

            #region Insert into tr_trip_schedule
            query = @"insert into tr_trip_schedule 
                        (schedule_date, origin, origin_additional_information, destination, destination_additional_information, created_by, updated_by)
                        values (@schedule_date, @origin, @origin_additional_information, @destination, @destination_additional_information, @created_by, @created_by)";

            param = new Dictionary<string, object> {
                    { "schedule_date", obj.schedule_date },
                    { "origin", obj.origin },
                    { "origin_additional_information", obj.origin_additional_information },
                    { "destination", obj.destination },
                    { "destination_additional_information", obj.destination_additional_information },
                    { "created_by", obj.created_by }
                };
            
            int trip_schedule_id = _sQLHelper.queryInsertWithReturningId(query, param).Result;
            #endregion

            #region Insert into tr_bus_trip_schedule
            query = @"insert into tr_bus_trip_schedule 
                        (trip_schedule_id, no_bus, created_by, updated_by)
                        values (@trip_schedule_id, @no_bus, @created_by, @created_by)";

            param = new Dictionary<string, object> {
                    { "trip_schedule_id", trip_schedule_id },
                    { "no_bus", obj.no_bus },
                    { "created_by", obj.created_by }
                };

            result = _sQLHelper.queryInsert(query, param).Result > 0;

            #endregion

            return result;
        }
    }
}
