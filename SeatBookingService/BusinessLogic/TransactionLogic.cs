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

            if (obj.schedule_date == DateTime.MinValue || obj.schedule_date == null)
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
                errMsg = "Users Id tidak boleh kosong";
            }
            else if (string.IsNullOrWhiteSpace(obj.trip_id.ToString()))
            {
                errMsg = "Trip Id tidak boleh kosong";
            }
            else if (obj.price <= 0 || string.IsNullOrWhiteSpace(obj.price.ToString()))
            {
                errMsg = "Harga Tiket tidak boleh kosong";
            }
            else if (obj.seat_detail.Count <= 0)
            {
                errMsg = "Seat Id tidak boleh kosong";
            }
            else if (seat.Count > 0)
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

        public static BusinessLogicResult SubmitExpedisi(TRExpedition obj)
        {
            BusinessLogicResult res = new BusinessLogicResult();

            string errMsg = string.Empty;

            if (obj.price <= 0 || string.IsNullOrEmpty(obj.price.ToString()))
            {
                errMsg = "Harga tidak boleh kosong";
            }
            else if (string.IsNullOrWhiteSpace(obj.goods_type))
            {
                errMsg = "Tipe Barang tidak boleh kosong";
            }
            else if (string.IsNullOrWhiteSpace(obj.volume))
            {
                errMsg = "Volume Barang tidak boleh kosong";
            }

            res.result = !string.IsNullOrEmpty(errMsg) ? false : true;
            res.message = errMsg;

            return res;
        }

        public static BusinessLogicResult CancelSeat(TRCancellation obj)
        {
            BusinessLogicResult res = new BusinessLogicResult();

            string errMsg = string.Empty;

            if (string.IsNullOrEmpty(obj.reserved_seat_id.ToString()))
            {
                errMsg = "Reserved Seat ID cannot be empty";
            }

            res.result = !string.IsNullOrEmpty(errMsg) ? false : true;
            res.message = errMsg;

            return res;
        }

        public static BusinessLogicResult ApproveCancelSeat(TRCancellation obj)
        {
            BusinessLogicResult res = new BusinessLogicResult();

            string errMsg = string.Empty;

            if (string.IsNullOrEmpty(obj.id.ToString()))
            {
                errMsg = "ID cannot be empty";
            }

            res.result = !string.IsNullOrEmpty(errMsg) ? false : true;
            res.message = errMsg;

            return res;
        }

        public static BusinessLogicResult RejectCancelSeat(TRCancellation obj)
        {
            BusinessLogicResult res = new BusinessLogicResult();

            string errMsg = string.Empty;

            if (string.IsNullOrEmpty(obj.id.ToString()))
            {
                errMsg = "ID cannot be empty";
            }

            res.result = !string.IsNullOrEmpty(errMsg) ? false : true;
            res.message = errMsg;

            return res;
        }

        public static BusinessLogicResult CreateTripScheduleNonRegular(TRTripSchedule obj)
        {
            BusinessLogicResult res = new BusinessLogicResult();

            string errMsg = string.Empty;

            if (obj.schedule_date == DateTime.MinValue || obj.schedule_date == null)
            {
                errMsg = "Tanggal Keberangkatan tidak boleh kosong";
            }
            else if (string.IsNullOrWhiteSpace(obj.origin))
            {
                errMsg = "Kota Asal tidak boleh kosong";
            }
            else if (string.IsNullOrWhiteSpace(obj.destination))
            {
                errMsg = "Kota Tujuan tidak boleh kosong";
            }
            else if (obj.schedule_date.Value.Date < DateTime.Now.Date)
            {
                errMsg = "Tanggal Keberangkatan harus lebih besar atau sama dengan hari ini";
            }
            else if (obj.origin == obj.destination)
            {
                errMsg = "Kota Asal dan Kota Tujuan tidak boleh sama";
            }
            else if (string.IsNullOrWhiteSpace(obj.class_bus_id.ToString()) || string.IsNullOrEmpty(obj.class_bus_id.ToString()))
            {
                errMsg = "Kelas Bus tidak boleh kosong";
            }

            res.result = !string.IsNullOrEmpty(errMsg) ? false : true;
            res.message = errMsg;

            return res;
        }

        public static BusinessLogicResult AssignBusStatus(List<TRBusAssignStatus> obj)
        {
            BusinessLogicResult res = new BusinessLogicResult();

            string errMsg = string.Empty;

            if(obj.Count <= 0)
            {
                errMsg = "Tidak ada bus yang dipilih, harap cek kembali data anda";
            }

            res.result = !string.IsNullOrEmpty(errMsg) ? false : true;
            res.message = errMsg;
            return res;
        }

        public static BusinessLogicResult ChangePassword(ChangePasswordDto obj)
        {
            BusinessLogicResult res = new BusinessLogicResult();

            string errMsg = string.Empty;

            if (string.IsNullOrEmpty(obj.user_id.ToString()))
            {
                errMsg = "User ID tidak boleh kosong";
            }
            else if(string.IsNullOrEmpty(obj.password))
            {
                errMsg = "Password 1 tidak boleh kosong";
            }
            else if (string.IsNullOrEmpty(obj.verify_password))
            {
                errMsg = "Password 2 tidak boleh kosong";
            }
            else if (obj.password != obj.verify_password)
            {
                errMsg = "Password 1 dan 2 tidak sesuai";
            }

            res.result = !string.IsNullOrEmpty(errMsg) ? false : true;
            res.message = errMsg;

            return res;
        }
    }
}
