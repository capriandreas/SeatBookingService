using SeatBookingService.Models;
using SeatBookingService.Models.DTO;
using System;

namespace SeatBookingService.BusinessLogic
{
    public class TransactionLogic
    {
        public static BusinessLogicResult SubmitTripScheduleValidation(TRTripScheduleDto obj)
        {
            BusinessLogicResult res = new BusinessLogicResult();

            string errMsg = string.Empty;

            if (obj.schedule_date == DateTime.MinValue || obj.schedule_date == null)
            {
                errMsg = "Schedule Date cannot be empty";
            }
            else if (string.IsNullOrWhiteSpace(obj.origin))
            {
                errMsg = "Origin cannot be empty";
            }
            else if (string.IsNullOrWhiteSpace(obj.destination))
            {
                errMsg = "Destination cannot be empty";
            }
            else if (string.IsNullOrWhiteSpace(obj.created_by))
            {
                errMsg = "Created By cannot be empty";
            }
            else if (obj.schedule_date.Value.Date < DateTime.Now.Date)
            {
                errMsg = "Schedule Date cannot backdate";
            }
            else if (obj.origin == obj.destination)
            {
                errMsg = "Origin and Destination should be different";
            }
            else if (string.IsNullOrWhiteSpace(obj.no_bus) || string.IsNullOrEmpty(obj.no_bus))
            {
                errMsg = "No Bus cannot be empty";
            }

            res.result = !string.IsNullOrEmpty(errMsg) ? false : true;
            res.message = errMsg;

            return res;
        }

        public static BusinessLogicResult GetListTripScheduleValidation(TRTripSchedule obj)
        {
            BusinessLogicResult res = new BusinessLogicResult();

            string errMsg = string.Empty;

            if(obj.schedule_date == DateTime.MinValue || obj.schedule_date == null)
            {
                errMsg = "Schedule Date cannot be empty";
            }

            res.result = !string.IsNullOrEmpty(errMsg) ? false : true;
            res.message = errMsg;

            return res;
        }
    }
}
