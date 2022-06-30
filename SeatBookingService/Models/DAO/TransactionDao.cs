using SeatBookingService.Helper;
using SeatBookingService.Library.Const;
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

        public bool ApproveCancelSeat(TRCancellation obj)
        {
            var query = string.Empty;
            var param = new Dictionary<string, object>();

            //Set status_seat_id menjadi canceled dan action_id menjadi approved
            query = @"update tr_cancellation set status_seat_id = 1, action_id = 1 where id = @id";

            param = new Dictionary<string, object> {
                    { "id", obj.id }
                };

            return _sQLHelper.queryUpdate(query, param).Result > 0;
        }

        public bool RejectCancelSeat(TRCancellation obj)
        {
            var query = string.Empty;
            var param = new Dictionary<string, object>();

            //Set status_seat_id menjadi rejected dan action_id menjadi reject
            query = @"update tr_cancellation set status_seat_id = 3, action_id = 2 where id = @id";

            param = new Dictionary<string, object> {
                    { "id", obj.id }
                };

            return _sQLHelper.queryUpdate(query, param).Result > 0;
        }

        public bool CancelSeat(TRCancellation obj)
        {
            var query = string.Empty;
            var param = new Dictionary<string, object>();

            query = @"insert into tr_cancellation 
                        (reserved_seat_id, status_seat_id, reason, created_by, updated_by)
                        values (@reserved_seat_id, 2, @reason, (select created_by from tr_reserved_seat where id = @reserved_seat_id), (select created_by from tr_reserved_seat where id = @reserved_seat_id))";

            param = new Dictionary<string, object> {
                    { "reserved_seat_id", obj.reserved_seat_id },
                    { "reason", obj.reason }
                };

            return _sQLHelper.queryInsert(query, param).Result > 0;
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

        public List<TRReservedSeatHeader2Dto> GetDataSeatValidation(int trip_id, string joinSeatId)
        {
            var query = @"select a.seat_id, a.reserved_seat_header_id, b.trip_id, d.seat_column, d.seat_row
                            from tr_reserved_seat a
                            left join tr_reserved_seat_header b on b.id = a.reserved_seat_header_id
                            left join tr_trip c on c.id = b.trip_id
                            left join ms_seat d on d.id = a.seat_id
                            where b.trip_id = @trip_id and a.seat_id in (" + joinSeatId + ")";

            var param = new Dictionary<string, object> {
                { "trip_id", trip_id }
            };

            return _sQLHelper.queryList<TRReservedSeatHeader2Dto>(query, param).Result;
        }

        public List<TRExpeditionDto> GetExpedition(TRExpedition obj)
        {
            var query = @"select a.users_id, a.trip_id, a.price, a.goods_type, a.volume, a.additional_information
                            from tr_expedition a
                            where a.users_id = @users_id and a.trip_id = @trip_id";

            var param = new Dictionary<string, object> {
                { "users_id", obj.users_id },
                { "trip_id", obj.trip_id },
            };

            return _sQLHelper.queryList<TRExpeditionDto>(query, param).Result;
        }

        public List<HistoryDetailDto> GetHistoryDetail(int trip_id, int users_id)
        {
            var query = @"select a.id, a.price, a.additional_information 
                            from tr_reserved_seat_header a
                            where a.users_id = @users_id and a.trip_id = @trip_id";

            var param = new Dictionary<string, object> {
                { "trip_id", trip_id },
                { "users_id", users_id },
            };

            return _sQLHelper.queryList<HistoryDetailDto>(query, param).Result;
        }

        public List<HistoryExpeditionDetailDto> GetHistoryExpeditionDetail(int trip_id, int users_id)
        {
            var query = @"select a.id, a.price, a.goods_type, a.volume, a.additional_information
                            from tr_expedition a
                            where a.users_id = @users_id and a.trip_id = @trip_id";

            var param = new Dictionary<string, object> {
                { "trip_id", trip_id },
                { "users_id", users_id }
            };

            return _sQLHelper.queryList<HistoryExpeditionDetailDto>(query, param).Result;
        }

        public List<HistoryHeaderDto> GetHistoryHeader(int users_id)
        {
            var query = @"select 
								a.trip_id,
                                b.schedule_date,
                                a.users_id,
								a.price,
                                (select count(id) from tr_reserved_seat where reserved_seat_header_id = a.id) as total_tickets,
                                (select count(id) * a.price from tr_reserved_seat where reserved_seat_header_id = a.id) as total_price,
                                a.additional_information,
                                b.route_id,
                                b.trip_type_id,
                                 case 
								when b.trip_type_id = 1 then 
                                (
									select GROUP_CONCAT(a.city separator ' - ') as `Route` 
									from ms_stations_routes a
									left join ms_routes b on b.id = a.routes_id
                                    where b.is_active = 1 and b.id = b.route_id
                                    group by b.id
								) 
								when b.trip_type_id = 2 then 
                                (
									select CONCAT_WS (' - ', a.origin, a.destination) as `Route`
									from tr_trip_schedule a
									where a.id = b.route_id
								) end as route,
                                case 
								when b.trip_type_id = 1 then 
                                (
									select departure_hours 
									from ms_routes a
                                    where a.is_active = 1 and a.id = b.route_id
								) 
								when b.trip_type_id = 2 then 
                                (
									select departure_hours 
									from tr_trip_schedule a
									where a.id = b.route_id
								) end as departure_hours
							from tr_reserved_seat_header a
                            left join tr_trip b on b.id = a.trip_id
                            where a.users_id = @users_id";

            var param = new Dictionary<string, object> {
                { "users_id", users_id }
            };

            return _sQLHelper.queryList<HistoryHeaderDto>(query, param).Result;
        }

        public List<HistorySeatDetailDto> GetHistorySeatDetail(int reserved_seat_header_id)
        {
            var query = @"select 
                            a.id, 
                            a.seat_id, 
                            b.seat_row, 
                            b.seat_column, 
                            c.status_seat_id, 
                            d.status_name, 
                            c.action_id, 
                            e.action_name,
                            case when c.status_seat_id is null then true else false end as allow_cancel
                            from tr_reserved_seat a
                            left join ms_seat b on b.id = a.seat_id
                            left join tr_cancellation c on c.reserved_seat_id = a.id
                            left join ms_status_seat d on d.id = c.status_seat_id
                            left join ms_action e on e.id = c.action_id
                            where a.reserved_seat_header_id = @reserved_seat_header_id";

            var param = new Dictionary<string, object> {
                { "reserved_seat_header_id", reserved_seat_header_id }
            };

            return _sQLHelper.queryList<HistorySeatDetailDto>(query, param).Result;
        }

        public List<MSSeatDto> GetListAllSeat(int trip_id)
        {
            var query = @"select * from (
                            select 	
							a.id,
		                    b.id as reserved_seat_id,
		                    c.id as seat_id,
                            c.seat_order,
		                    c.class_bus_id,
		                    c.seat_column,
		                    c.seat_row,
		                    a.users_id,
                            'Booked' as seat_status,
		                    -- CASE WHEN a.id is null then 'Available' else 'Booked' end as seat_status,
		                    g.status_name,
		                    f.reason,
		                    a.trip_id,
		                    h.trip_type_id
		                    from tr_reserved_seat_header a
		                    left join tr_reserved_seat b on b.reserved_seat_header_id = a.id and b.reserved_seat_header_id in (select a.id from tr_reserved_seat_header a 
																											                    left join tr_trip b on b.id = a.trip_id 
																											                    where a.trip_id = @trip_id)
		                    left join ms_seat c on c.id = b.seat_id
		                    left join tr_trip h on h.id = a.trip_id 
		                    left join tr_cancellation f on f.reserved_seat_id = b.id
		                    left join ms_status_seat g on g.id = f.status_seat_id
		                    where c.class_bus_id = (select 
									                    case 
									                    when a.trip_type_id = 1 then (select class_bus_id from ms_routes where id = a.route_id) 
									                    when a.trip_type_id = 2 then (select class_bus_id from tr_trip_schedule where id = a.route_id) 
									                    end as class_bus_id
								                    from tr_trip a
								                    where a.id = @trip_id)
                                                    
                            UNION
                            select null as id, null as reserved_seat_id, a.id as seat_id, a.seat_order, a.class_bus_id, a.seat_column, a.seat_row, null as users_id, 'Available' as seat_status, null as status_name, null as reason, null as trip_id, null as trip_type_id
                            from ms_seat a 
                            where a.class_bus_id = (select 
							                            case 
							                            when a.trip_type_id = 1 then (select class_bus_id from ms_routes where id = a.route_id) 
							                            when a.trip_type_id = 2 then (select class_bus_id from tr_trip_schedule where id = a.route_id) 
							                            end as class_bus_id
						                            from tr_trip a
						                            where a.id = @trip_id)
                            and a.id not in (select 	
		                                                c.id as seat_id
		                                                from tr_reserved_seat_header a
		                                                left join tr_reserved_seat b on b.reserved_seat_header_id = a.id and b.reserved_seat_header_id in (select a.id from tr_reserved_seat_header a 
																											                                                left join tr_trip b on b.id = a.trip_id 
																											                                                where a.trip_id = @trip_id)
		                                                left join ms_seat c on c.id = b.seat_id
		                                                left join tr_trip h on h.id = a.trip_id 
		                                                left join tr_cancellation f on f.reserved_seat_id = b.id
		                                                left join ms_status_seat g on g.id = f.status_seat_id
		                                                where c.class_bus_id = (select 
									                                                case 
									                                                when a.trip_type_id = 1 then (select class_bus_id from ms_routes where id = a.route_id) 
									                                                when a.trip_type_id = 2 then (select class_bus_id from tr_trip_schedule where id = a.route_id) 
									                                                end as class_bus_id
								                                                from tr_trip a
								                                                where a.id = @trip_id))
                            ) a order by seat_order asc";

            var param = new Dictionary<string, object> {
                { "trip_id", trip_id }
            };

            return _sQLHelper.queryList<MSSeatDto>(query, param).Result;
        }

        public List<TRBusAssignStatusDto> GetListAssignedBus(TRBusAssignStatus obj)
        {
            var query = @"select h.no_bus, c.no_polisi, c.jumlah_seat, c.class_bus_id, h.status_bus_id, b.status_bus, h.assign_date, d.class_bus
                            from tr_bus_assign_status h
                            inner join (select a1.no_bus, a1.created_date
                            from tr_bus_assign_status a1
                            inner join (select no_bus, max(created_date) as created_date from tr_bus_assign_status group by no_bus) a2
                            on a1.no_bus = a2.no_bus and a1.created_date = a2.created_date) h2
                            on h.no_bus = h2.no_bus and h.created_date = h2.created_date
                            left join ms_status_bus b on b.id = h.status_bus_id
                            left join ms_bus c on c.no_bus = h.no_bus
                            left join ms_class_bus d on d.id = c.class_bus_id
                            where h.status_bus_id = @status_bus_id and h.assign_date = @assign_date";

            var param = new Dictionary<string, object> {
                { "status_bus_id", obj.status_bus_id },
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

        public List<TRCancellationDto> GetListCancelSeat()
        {
            var query = @"select 
			                a.id, 
			                a.reserved_seat_id, 
			                a.status_seat_id, 
			                b.status_name, 
			                a.action_id, 
			                c.action_name, 
			                a.reason,
                            e.trip_id,
                            f.route_id,
                            f.trip_type_id,
                            -- h.no_bus,
                            -- f.origin,
                            -- f.destination,
                            f.schedule_date,
                            g.seat_column,
                            g.seat_row,
                            i.nickname,
                            case 
								when f.trip_type_id = 1 then 
                                (
									select GROUP_CONCAT(a.city separator ' - ') as `Route` 
									from ms_stations_routes a
									left join ms_routes b on b.id = a.routes_id
                                    where b.is_active = 1 and b.id = f.route_id
                                    group by b.id
								) 
								when f.trip_type_id = 2 then 
                                (
									select CONCAT_WS (' - ', a.origin, a.destination) as `Route`
									from tr_trip_schedule a
									where a.id = f.route_id
								) 
							end as route,
                            case 
								when f.trip_type_id = 1 then 
                                (
									select departure_hours 
									from ms_routes a
                                    where a.is_active = 1 and a.id = f.route_id
								) 
								when f.trip_type_id = 2 then 
                                (
									select departure_hours 
									from tr_trip_schedule a
									where a.id = f.route_id
								) end as departure_hours,
                            a.created_by as cancel_by_users_id,
                            l.nickname as cancel_by_users_nickname
		                from tr_cancellation a 
		                left join ms_status_seat b on b.id = a.status_seat_id
		                left join ms_action c on c.id = a.action_id
		                left join tr_reserved_seat d on d.id = a.reserved_seat_id
		                left join tr_reserved_seat_header e on e.id = d.reserved_seat_header_id
                        left join tr_trip f on f.id = e.trip_id
		                -- left join tr_trip_schedule f on f.id = e.trip_schedule_id
                        left join ms_seat g on g.id = d.seat_id
                        -- left join tr_bus_trip_schedule h on h.trip_schedule_id = f.id
                        left join ms_users i on i.id = f.route_id
                        left join ms_routes j on j.id = f.route_id
                        left join tr_trip_schedule k on k.id = f.route_id
                        left join ms_users l on l.id = a.created_by
		                where a.action_id is null";

            return _sQLHelper.queryList<TRCancellationDto>(query, null).Result;
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
                query = @"insert into ms_seat (class_bus_id, seat_column, seat_row, seat_order) values (@class_bus_id, @seat_column, @seat_row, @seat_order);";

                param = new Dictionary<string, object> {
                    { "class_bus_id", item.class_bus_id },
                    { "seat_column", item.seat_column },
                    { "seat_row", item.seat_row },
                    { "seat_order", item.seat_order }
                };

                result = _sQLHelper.queryInsert(query, param).Result > 0;
            }

            return result;
        }

        public bool SubmitExpedition(TRExpedition obj)
        {
            var query = string.Empty;
            var param = new Dictionary<string, object>();

            #region Insert into tr_reserved_seat_header
            query = @"insert into tr_expedition 
                        (users_id, trip_id, price, goods_type, volume, additional_information, created_by, updated_by)
                        values (@users_id, @trip_id, @price, @goods_type, @volume, @additional_information, @created_by, @created_by)";

            param = new Dictionary<string, object> {
                    { "users_id", obj.users_id },
                    { "trip_id", obj.trip_id },
                    { "price", obj.price },
                    { "goods_type", obj.goods_type },
                    { "volume", obj.volume },
                    { "additional_information", obj.additional_information },
                    { "created_by", obj.users_id }
                };

            #endregion

            return _sQLHelper.queryInsert(query, param).Result > 0;
        }

        public bool SubmitSeatBooking(TRReservedSeatHeaderDto obj)
        {
            bool result = false;
            var query = string.Empty;
            var param = new Dictionary<string, object>();

            #region Insert into tr_reserved_seat_header
            query = @"insert into tr_reserved_seat_header 
                        (users_id, trip_id, price, additional_information, created_by, updated_by)
                        values (@users_id, @trip_id, @price, @additional_information, @created_by, @created_by)";

            param = new Dictionary<string, object> {
                    { "users_id", obj.users_id },
                    { "trip_id", obj.trip_id },
                    { "price", obj.price },
                    { "additional_information", obj.additional_information },
                    { "created_by", obj.users_id }
                };

            int reserved_seat_header_id = _sQLHelper.queryInsertWithReturningId(query, param).Result;
            #endregion

            #region Insert into tr_reserved_seat
            foreach (var item in obj.seat_detail)
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

        public bool InsertNewStationRoutes(TRStationRoutesDto obj)
        {
            bool result = false;
            string query = string.Empty;
            var param = new Dictionary<string, object>();

            #region Insert into ms_routes
            query = @"insert into ms_routes 
                        (class_bus_id, departure_hours, description, created_by, updated_by)
                        values (@class_bus_id, @departure_hours, @description, @created_by, @created_by)";

            param = new Dictionary<string, object> {
                    { "class_bus_id", obj.class_bus_id },
                    { "departure_hours", obj.departure_hours },
                    { "description", obj.description },
                    { "created_by", obj.created_by }
                };

            int routes_id = _sQLHelper.queryInsertWithReturningId(query, param).Result;
            #endregion

            #region Insert into ms_stations_routes
            foreach (var item in obj.stationRoutes)
            {
                query = @"insert into ms_stations_routes 
                        (routes_id, city, route_order, created_by, updated_by)
                        values (@routes_id, @city, @route_order, @created_by, @created_by)";

                param = new Dictionary<string, object> {
                    { "routes_id", routes_id },
                    { "city", item.city },
                    { "route_order", item.route_order },
                    { "created_by", obj.created_by }
                };

                result = _sQLHelper.queryInsert(query, param).Result > 0;
            }

            #endregion

            return result;
        }

        public bool CreateTripScheduleNonRegular(TRTripScheduleRoutesDto obj)
        {
            bool result = false;
            var query = string.Empty;
            var param = new Dictionary<string, object>();

            #region Insert into tr_trip_schedule
            query = @"insert into tr_trip_schedule 
                        (class_bus_id, schedule_date, departure_hours, description, created_by, updated_by)
                        values (@class_bus_id, @schedule_date, @departure_hours, @description, @created_by, @created_by)";

            param = new Dictionary<string, object> {
                    { "class_bus_id", obj.class_bus_id },
                    { "schedule_date", obj.schedule_date },
                    { "departure_hours", obj.departure_hours },
                    { "description", obj.description },
                    { "created_by", obj.created_by }
                };

            int trip_schedule_id = _sQLHelper.queryInsertWithReturningId(query, param).Result;
            #endregion

            #region Insert into ms_stations_routes
            foreach (var item in obj.tripRoutes)
            {
                query = @"insert into tr_trip_schedule_routes 
			            (trip_schedule_id, city, route_order, created_by, updated_by)
			            values (@trip_schedule_id, @city, @route_order, @created_by, @created_by)";

                param = new Dictionary<string, object> {
                        { "trip_schedule_id", trip_schedule_id },
                        { "city", item.city },
                        { "route_order", item.route_order },
                        { "created_by", obj.created_by }
                };

                result = _sQLHelper.queryInsert(query, param).Result > 0;
            }

            #endregion

            return result;
        }

        public List<MSTripDto> GetAllTrip(DateTime? schedule_date)
        {
            var param = new Dictionary<string, object>();
            var query = @"select 
                            a.id_route,
                            a.Route,
                            a.departure_hours,
                            a.class_bus,
                            a.description,
                            b.*,
                            -- c.*,
                            d.no_bus
                          from
                            (
	                            select 
                                    b.id as 'id_route',
		                            GROUP_CONCAT(a.city order by a.route_order separator ' - ') as `Route`,
		                            b.departure_hours,
		                            c.class_bus,
		                            b.description,
		                            1 as trip_type_id
	                            from ms_stations_routes a
	                            left join ms_routes b on b.id = a.routes_id
	                            left join ms_class_bus c on c.id = b.class_bus_id
	                            where b.is_active = 1
	                            group by b.id
                            UNION
	                            select 
                                    a.id as 'id_route',
		                            CONCAT_WS (' - ', a.origin, a.destination) as `Route`,
		                            a.departure_hours,
		                            b.class_bus,
		                            a.description,
		                            2 as trip_type_id
	                            from tr_trip_schedule a
                                left join ms_class_bus b on b.id = a.class_bus_id "
                                + (schedule_date != null && schedule_date.HasValue ? @"where a.schedule_date = @schedule_date" : string.Empty)
                            + @") a 
                            left join (select result.*
	                                    from ms_settings a,
                                         json_table(a.data,
                                                    '$[*]' columns (
                                                        trip_type_id int path '$.trip_type_id',
                                                        trip_type_name varchar(255) path '$.trip_type_name'
                                                        )
                                             ) result
	                                    where a.key = 'trip_type') b on b.trip_type_id = a.trip_type_id
                            left join tr_trip c on c.route_id = a.id_route and c.trip_type_id = b.trip_type_id and c.schedule_date = @schedule_date
                            left join tr_bus_trip_schedule d on d.trip_id = c.id";

            param = new Dictionary<string, object> {
                    { "schedule_date", schedule_date }
                };

            return _sQLHelper.queryList<MSTripDto>(query, param).Result;
        }

        public TRTrip GetTrTrip(TripDetailParamDto obj)
        {
            var query = @"select 
                            id, route_id, trip_type_id, schedule_date 
                          from tr_trip
                          where 
                            route_id = @route_id
                            and trip_type_id = @trip_type_id
                            and schedule_date = @schedule_date";

            var param = new Dictionary<string, object> {
                { "route_id", obj.route_id },
                { "trip_type_id", obj.trip_type_id },
                { "schedule_date", obj.schedule_date }
            };

            return _sQLHelper.querySingle<TRTrip>(query, param).Result;
        }

        public bool CreateTrTrip(TripDetailParamDto obj)
        {
            bool result = false;
            var query = string.Empty;
            var param = new Dictionary<string, object>();

            #region Insert into tr_reserved_seat_header
            query = @"insert into tr_trip 
                        (route_id, trip_type_id, schedule_date, created_by, updated_by)
                        values (@route_id, @trip_type_id, @schedule_date, @created_by, @created_by)";

            param = new Dictionary<string, object> {
                    { "route_id", obj.route_id },
                    { "trip_type_id", obj.trip_type_id },
                    { "schedule_date", obj.schedule_date },
                    { "created_by", obj.created_by }
                };

            #endregion

            return _sQLHelper.queryInsert(query, param).Result > 0;
        }

        public bool AssignBusStatus(List<TRBusAssignStatus> obj)
        {
            bool result = false;
            var query = string.Empty;
            var param = new Dictionary<string, object>();

            foreach (var item in obj)
            {
                query = @"insert into tr_bus_assign_status (no_bus, status_bus_id, assign_date, station, description) values (@no_bus, @status_bus_id, @assign_date, @station, @description);";

                param = new Dictionary<string, object> {
                    { "no_bus", item.no_bus },
                    { "status_bus_id", item.status_bus_id },
                    { "assign_date", item.assign_date },
                    { "station", item.station },
                    { "description", item.description }
                };

                result = _sQLHelper.queryInsert(query, param).Result > 0;
            }

            return result;
        }

        public List<TRCancellationDto> GetListHistoryCancelSeat()
        {
            var query = @"select 
			                a.id, 
			                a.reserved_seat_id, 
			                a.status_seat_id, 
			                b.status_name, 
			                a.action_id, 
			                c.action_name, 
			                a.reason,
                            e.trip_id,
                            f.route_id,
                            f.trip_type_id,
                            -- h.no_bus,
                            -- f.origin,
                            -- f.destination,
                            f.schedule_date,
                            g.seat_column,
                            g.seat_row,
                            i.nickname,
                            case 
								when f.trip_type_id = 1 then 
                                (
									select GROUP_CONCAT(a.city separator ' - ') as `Route` 
									from ms_stations_routes a
									left join ms_routes b on b.id = a.routes_id
                                    where b.is_active = 1 and b.id = f.route_id
                                    group by b.id
								) 
								when f.trip_type_id = 2 then 
                                (
									select CONCAT_WS (' - ', a.origin, a.destination) as `Route`
									from tr_trip_schedule a
									where a.id = f.route_id
								) 
							end as route,
                            case 
								when f.trip_type_id = 1 then 
                                (
									select departure_hours 
									from ms_routes a
                                    where a.is_active = 1 and a.id = f.route_id
								) 
								when f.trip_type_id = 2 then 
                                (
									select departure_hours 
									from tr_trip_schedule a
									where a.id = f.route_id
								) end as departure_hours,
                            a.created_by as cancel_by_users_id,
                            l.nickname as cancel_by_users_nickname
		                from tr_cancellation a 
		                left join ms_status_seat b on b.id = a.status_seat_id
		                left join ms_action c on c.id = a.action_id
		                left join tr_reserved_seat d on d.id = a.reserved_seat_id
		                left join tr_reserved_seat_header e on e.id = d.reserved_seat_header_id
                        left join tr_trip f on f.id = e.trip_id
		                -- left join tr_trip_schedule f on f.id = e.trip_schedule_id
                        left join ms_seat g on g.id = d.seat_id
                        -- left join tr_bus_trip_schedule h on h.trip_schedule_id = f.id
                        left join ms_users i on i.id = f.route_id
                        left join ms_routes j on j.id = f.route_id
                        left join tr_trip_schedule k on k.id = f.route_id
                        left join ms_users l on l.id = a.created_by
		                where a.action_id is not null";

            return _sQLHelper.queryList<TRCancellationDto>(query, null).Result;
        }

        public bool ChangePassword(ChangePasswordDto obj)
        {
            var query = string.Empty;
            var param = new Dictionary<string, object>();

            query = @"update ms_users set password = @encrypted_password where id = @user_id";

            param = new Dictionary<string, object> {
                    { "user_id", obj.user_id },
                    { "encrypted_password", obj.encrypted_password },
                    { "verify_password", obj.verify_password }
                };

            return _sQLHelper.queryUpdate(query, param).Result > 0;
        }

        public bool AssignBusTrip(TRBusTripSchedule obj)
        {
            var query = string.Empty;
            var param = new Dictionary<string, object>();

            query = @"insert into tr_bus_trip_schedule 
                        (trip_id, no_bus, created_by, updated_by)
                        values (@trip_id, @no_bus, @created_by, @created_by)";

            param = new Dictionary<string, object> {
                    { "trip_id", obj.trip_id },
                    { "no_bus", obj.no_bus },
                    { "created_by", obj.created_by }
                };

            return _sQLHelper.queryInsert(query, param).Result > 0;
        }

        public List<MSBus> GetAllBusToAssign(DateTime? schedule_date)
        {
            var query = @"select a.id, a.no_bus, a.no_polisi, a.jumlah_seat, a.class_bus_id, b.class_bus 
                        from ms_bus a
                        left join ms_class_bus b on b.id = a.class_bus_id
                        where a.no_bus not in (select a.no_bus from (select no_bus, max(created_date) as created_date from tr_bus_assign_status where assign_date = @schedule_date group by no_bus) a)";

            var param = new Dictionary<string, object> {
                { "schedule_date", schedule_date }
            };

            return _sQLHelper.queryList<MSBus>(query, param).Result;
        }

        public List<MSBus> GetAllBusAssignValidation(TRBusTripSchedule obj)
        {
            var query = @"select a.no_bus, a.trip_id
                        from tr_bus_trip_schedule a
                        left join tr_trip b on b.id = a.trip_id
                        where no_bus = @no_bus and b.schedule_date = (select schedule_date from tr_trip where id = @trip_id)";

            var param = new Dictionary<string, object> {
                { "trip_id", obj.trip_id },
                { "no_bus", obj.no_bus }
            };

            return _sQLHelper.queryList<MSBus>(query, param).Result;
        }

        public List<GetSummaryReportDto> GetSummaryReport(DateTime? schedule_date)
        {
            var query = @"select *, 
	                        case 
		                        when a.trip_type_id = 1 then 
		                        (
			                        select GROUP_CONCAT(a.city separator ' - ') as `Route` 
			                        from ms_stations_routes a
			                        left join ms_routes b on b.id = a.routes_id
			                        where b.is_active = 1 and b.id = a.route_id
			                        group by b.id
		                        ) 
		                        when a.trip_type_id = 2 then 
		                        (
			                        select CONCAT_WS (' - ', a.origin, a.destination) as `Route`
			                        from tr_trip_schedule a
			                        where a.id = a.route_id
		                        ) 
	                        end as route
                        from (
	                        select a.no_bus, b.no_polisi, c.class_bus, d.status_bus, a.station, a.description,
	                        (select a.trip_type_id from tr_trip a
		                        left join tr_bus_trip_schedule b on a.id = b.trip_id 
		                        where b.no_bus = a.no_bus and a.schedule_date = @schedule_date) as trip_type_id,
	                        (select a.route_id from tr_trip a
	                        left join tr_bus_trip_schedule b on a.id = b.trip_id 
	                        where b.no_bus = a.no_bus and a.schedule_date = @schedule_date) as route_id
	                        from tr_bus_assign_status a
	                        left join ms_bus b on a.no_bus = b.no_bus
	                        left join ms_class_bus c on c.id = b.class_bus_id
	                        left join ms_status_bus d on d.id = a.status_bus_id
	                        where a.assign_date = @schedule_date and a.created_date = (select max(b.created_date) from tr_bus_assign_status b where b.no_bus = a.no_bus) 
                        ) a
                        UNION 
                        select a.no_bus, a.no_polisi, b.class_bus, '' as status_bus, '' as station, '' as description, null as trip_type_id, null as route_id, '' as route
                        from ms_bus a
                        left join ms_class_bus b on b.id = a.class_bus_id 
                        where a.no_bus not in (select a.no_bus
                        from tr_bus_assign_status a
                        left join ms_bus b on a.no_bus = b.no_bus
                        left join ms_class_bus c on c.id = b.class_bus_id
                        left join ms_status_bus d on d.id = a.status_bus_id
                        where a.assign_date = @schedule_date and a.created_date = (select max(b.created_date) from tr_bus_assign_status b where b.no_bus = a.no_bus))";

            var param = new Dictionary<string, object> {
                { "schedule_date", schedule_date }
            };

            return _sQLHelper.queryList<GetSummaryReportDto>(query, param).Result;
        }
    }
}
