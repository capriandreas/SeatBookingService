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

        public LoginResultDto GetDataLogin(MSUsers obj)
        {
            var query = @"select a.id, username, nickname, role_id, rolename
                        from ms_users a
                        left join ms_roles b on a.role_id = b.id
                        where username = @username and password = @password and is_active = 1";

            var param = new Dictionary<string, object> {
                { "username", obj.username },
                { "password", obj.password }
            };

            return _sQLHelper.querySingle<LoginResultDto>(query, param).Result;
        }

        public List<TRReservedSeatHeader2Dto> GetDataSeatValidation(int trip_schedule_id)
        {
            var query = @"select a.seat_id, a.reserved_seat_header_id, b.trip_schedule_id
                            from tr_reserved_seat a
                            left join tr_reserved_seat_header b on b.id = a.reserved_seat_header_id
                            left join tr_trip_schedule c on c.id = b.trip_schedule_id
                            where b.trip_schedule_id = @trip_schedule_id";

            var param = new Dictionary<string, object> {
                { "trip_schedule_id", trip_schedule_id }
            };

            return _sQLHelper.queryList<TRReservedSeatHeader2Dto>(query, param).Result;
        }

        public List<MSSeatDto> GetListAllSeat(int trip_schedule_id)
        {
            var query = @"select 
	                        c.id as seat_id,
	                        c.no_bus,
	                        c.seat_column,
	                        c.seat_row,
	                        a.users_id,
	                        CASE WHEN a.id is null then 'Available' else 'Booked' end as seat_status,
	                        d.id as trip_schedule_id
	                        from tr_reserved_seat_header a
	                        left join tr_reserved_seat b on b.reserved_seat_header_id = a.id
	                        right join ms_seat c on c.id = b.seat_id
	                        left join tr_trip_schedule d on d.id = a.trip_schedule_id
	                        left join tr_bus_trip_schedule e on e.trip_schedule_id = d.id and e.no_bus = c.no_bus
	                        where c.no_bus = (select no_bus from 
						                        tr_bus_trip_schedule a
						                        where a.trip_schedule_id = @trip_schedule_id)";

            var param = new Dictionary<string, object> {
                { "trip_schedule_id", trip_schedule_id }
            };

            return _sQLHelper.queryList<MSSeatDto>(query, param).Result;
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

        public List<TRReservedSeatHeaderBookedDto> GetListBookedTrip(int users_id)
        {
            var query = @"select 
                            a.id,
	                        a.users_id,
                            a.price,
                            a.additional_information,
                            c.no_bus,
                            b.schedule_date,
                            b.origin,
                            b.destination
                        from tr_reserved_seat_header a
                        left join tr_trip_schedule b on a.trip_schedule_id = b.id
                        left join tr_bus_trip_schedule c on c.trip_schedule_id = b.id 
                        where a.users_id = @users_id
                        order by a.created_date desc";

            var param = new Dictionary<string, object> {
                { "users_id", users_id }
            };

            return _sQLHelper.queryList<TRReservedSeatHeaderBookedDto>(query, param).Result;
        }

        public List<TRTripScheduleDto> GetListTripSchedule(TRTripSchedule obj)
        {
            var query = @"select 
                                a.id, a.schedule_date, a.origin, a.origin_additional_information, a.destination, 
                                a.destination_additional_information, a.created_by,
                                b.no_bus, c.jumlah_seat, c.no_polisi, c.kelas_id, d.kelas_bus
                            from tr_trip_schedule a
                            left join tr_bus_trip_schedule b on a.id = b.trip_schedule_id
                            left join ms_bus c on c.no_bus = b.no_bus
                            left join ms_kelas_bus d on d.id = c.kelas_id
                                where a.schedule_date = @schedule_date";

            var param = new Dictionary<string, object> {
                { "schedule_date", obj.schedule_date }
            };

            return _sQLHelper.queryList<TRTripScheduleDto>(query, param).Result;
        }

        public List<MSSeatDetailDto> GetSeatDetail(int reserved_seat_header_id)
        {
            var query = @"select 
	                        a.id,
                            a.seat_id,
                            b.seat_row,
                            b.seat_column
                        from tr_reserved_seat a
                        left join ms_seat b on b.id = a.seat_id
                        where a.reserved_seat_header_id  = @reserved_seat_header_id";

            var param = new Dictionary<string, object> {
                { "reserved_seat_header_id", reserved_seat_header_id }
            };

            return _sQLHelper.queryList<MSSeatDetailDto>(query, param).Result;
        }

        public TicketDto GetTicketDataHeader(int reserved_seat_header_id)
        {
            var query = @"select 
	                        b.schedule_date,
                            b.origin,
                            b.destination,
                            c.no_bus,
	                        a.price
                        from tr_reserved_seat_header a
                        left join tr_trip_schedule b on b.id = a.trip_schedule_id
                        left join tr_bus_trip_schedule c on c.trip_schedule_id = b.id
                        where a.id = @reserved_seat_header_id";

            var param = new Dictionary<string, object> {
                { "reserved_seat_header_id", reserved_seat_header_id }
            };

            return _sQLHelper.querySingle<TicketDto>(query, param).Result;
        }

        public bool InsertMasterSeat(List<MSSeat> obj)
        {
            string query = string.Empty;
            bool result = false;
            var param = new Dictionary<string, object>();

            foreach (var item in obj)
            {
                query = @"insert into ms_seat (no_bus, seat_column, seat_row) values (@no_bus, @seat_column, @seat_row);";

                param = new Dictionary<string, object> {
                    { "no_bus", item.no_bus },
                    { "seat_column", item.seat_column },
                    { "seat_row", item.seat_row }
                };

                result = _sQLHelper.queryInsert(query, param).Result > 0;
            }

            return result;
        }

        public bool SubmitSeatBooking(TRReservedSeatHeaderDto obj)
        {
            bool result = false;
            var query = string.Empty;
            var param = new Dictionary<string, object>();

            #region Insert into tr_trip_schedule
            query = @"insert into tr_reserved_seat_header 
                        (users_id, trip_schedule_id, price, additional_information, created_by, updated_by)
                        values (@users_id, @trip_schedule_id, @price, @additional_information, @created_by, @created_by)";

            param = new Dictionary<string, object> {
                    { "users_id", obj.users_id },
                    { "trip_schedule_id", obj.trip_schedule_id },
                    { "price", obj.price },
                    { "additional_information", obj.additional_information },
                    { "created_by", obj.users_id }
                };

            int reserved_seat_header_id = _sQLHelper.queryInsertWithReturningId(query, param).Result;
            #endregion

            #region Insert into tr_bus_trip_schedule
            foreach(var item in obj.seat_detail)
            {
                query = @"insert into tr_reserved_seat 
                        (seat_id, reserved_seat_header_id, created_by, updated_by)
                        values (@seat_id, @reserved_seat_header_id, @created_by, @created_by)";

                param = new Dictionary<string, object> {
                    { "seat_id", item.seat_id },
                    { "reserved_seat_header_id", reserved_seat_header_id },
                    { "created_by", obj.users_id }
                };

                result = _sQLHelper.queryInsert(query, param).Result > 0;
            }
            
            #endregion

            return result;
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
