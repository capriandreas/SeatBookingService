using SeatBookingService.Models;
using SeatBookingService.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public static BusinessLogicResult SubmitSeatBooking(TRReservedSeatHeaderDto obj, List<TRReservedSeatHeader2Dto> seat)
        {
            BusinessLogicResult res = new BusinessLogicResult();

            string errMsg = string.Empty;
            List<string> seatRowColumn = new List<string>();

            if (string.IsNullOrWhiteSpace(obj.users_id.ToString()))
            {
                errMsg = "Users Id cannot be empty";
            }
            else if (string.IsNullOrWhiteSpace(obj.trip_schedule_id.ToString()))
            {
                errMsg = "Trip Schedule Id cannot be empty";
            }
            else if (obj.price <= 0 || string.IsNullOrWhiteSpace(obj.price.ToString()))
            {
                errMsg = "Price cannot be empty";
            }
            else if (obj.seat_detail.Count <= 0)
            {
                errMsg = "Seat Id cannot be empty";
            }
            else if(seat.Count > 0)
            {
                foreach (var item in seat)
                {
                    seatRowColumn.Add(item.seat_column + item.seat_row);
                }

                errMsg = "Bangku " + string.Join(",", seatRowColumn) + " sudah dipesan";
            }

            res.result = !string.IsNullOrEmpty(errMsg) ? false : true;
            res.message = errMsg;

            return res;
        }

        public static BusinessLogicResult Login(MSUsers obj)
        {
            BusinessLogicResult res = new BusinessLogicResult();

            string errMsg = string.Empty;

            if (string.IsNullOrWhiteSpace(obj.username))
            {
                errMsg = "Username cannot be empty";
            }
            else if (string.IsNullOrWhiteSpace(obj.password))
            {
                errMsg = "Password cannot be empty";
            }

            res.result = !string.IsNullOrEmpty(errMsg) ? false : true;
            res.message = errMsg;

            return res;
        }
    }
}
