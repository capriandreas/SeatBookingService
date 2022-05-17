using SeatBookingService.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeatBookingService.Models.DAO
{
    public interface ITransactionDao
    {
        public List<TRBusAssignStatusDto> GetListAssignedBus(TRBusAssignStatus obj);
        public bool SubmitTripSchedule(TRTripScheduleDto obj);
        public List<TRTripScheduleDto> GetListTripSchedule(TRTripSchedule obj);
        public bool InsertMasterSeat(List<MSSeat> obj);
        public List<MSSeatDto> GetListAllSeat(int trip_schedule_id);
        public bool SubmitSeatBooking(TRReservedSeatHeaderDto obj);
        public List<TRReservedSeatHeaderBookedDto> GetListBookedTrip(int users_id);
        public List<MSSeatDetailDto> GetSeatDetail(int reserved_seat_header_id);
    }
}
