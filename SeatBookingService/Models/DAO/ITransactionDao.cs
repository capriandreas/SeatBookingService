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
        public List<MSSeatDto> GetListAllSeat(TripDetailParamDto obj);
        public bool SubmitSeatBooking(TRReservedSeatHeaderDto obj);
        public List<TRReservedSeatHeaderBookedDto> GetListBookedTrip(int users_id);
        public List<MSSeatDetailDto> GetSeatDetail(int reserved_seat_header_id);
        public TicketDto GetTicketDataHeader(int reserved_seat_header_id);
        public LoginResultDto GetDataLogin(MSUsers obj);
        public List<TRReservedSeatHeader2Dto> GetDataSeatValidation(int trip_id, string joinSeatId);
        public bool SubmitExpedition(TRExpedition obj);
        public List<TRExpeditionDto> GetExpedition(TRExpedition obj);
        public List<HistoryHeaderDto> GetHistoryHeader(int users_id);
        public List<HistoryDetailDto> GetHistoryDetail(int trip_schedule_id, int users_id);
        public List<HistorySeatDetailDto> GetHistorySeatDetail(int reserved_seat_header_id);
        public List<HistoryExpeditionDetailDto> GetHistoryExpeditionDetail(int trip_schedule_id, int users_id);
        public bool CancelSeat(TRCancellation obj);
        public List<TRCancellationDto> GetListCancelSeat();
        public bool ApproveCancelSeat(TRCancellation obj);
        public bool RejectCancelSeat(TRCancellation obj);
        public bool InsertNewStationRoutes(TRStationRoutesDto obj);
        public bool CreateTripScheduleNonRegular(TRTripSchedule obj);
        public List<MSTripDto> GetAllTrip(DateTime? schedule_date);
        public TRTrip GetTrTrip(TripDetailParamDto obj);
        public bool CreateTrTrip(TripDetailParamDto obj);
        public bool AssignBusStatus(List<TRBusAssignStatus> obj);
    }
}
