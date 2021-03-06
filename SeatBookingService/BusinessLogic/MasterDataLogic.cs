using SeatBookingService.Models;
using SeatBookingService.Models.DTO;
using System;
using System.Collections.Generic;

namespace SeatBookingService.BusinessLogic
{
    public class MasterDataLogic
    {
        public static BusinessLogicResult<List<string>> GenerateMasterSeat(GenerateSeat obj)
        {
            var result = new BusinessLogicResult<List<string>>();

            List<string> listSeat = new List<string>();
            List<char> listSeatType = new List<char>();

            if (obj.type == "A")
            {
                listSeatType = new List<char>
                {
                    'A','B','C'
                };

                result.data = generateSeat(obj.totalSeat, listSeatType); 
            }
            else if (obj.type == "B")
            {
                listSeatType = new List<char>
                {
                    'A','B','C', 'D'
                };

                result.data = generateSeat(obj.totalSeat, listSeatType);
            }   

            return result;
        }

        public static List<string> generateSeat(int totalSeat, List<char> listSeatType)
        {
            int loopTotal = 0;
            int counter = 0;
            int counter_seat_order = 1;

            List<string> listSeat = new List<string>();

            loopTotal = totalSeat / listSeatType.Count;

            if (totalSeat % listSeatType.Count != 0)
            {
                loopTotal += 1;
            }

            for (int i = 1; i <= loopTotal; i++)
            {
                string seat = string.Empty;
                foreach (var item in listSeatType)
                {
                    if (totalSeat == listSeat.Count)
                        break;
                    seat = i.ToString();
                    seat = seat + "," + item + "," + counter_seat_order;
                    listSeat.Add(seat);
                    counter_seat_order++;
                }
                counter++;
            }

            return listSeat;
        }

        public static List<MSSeat> MappingSeatToModel(List<string> listStrings, GenerateSeat obj)
        {
            List<MSSeat> result = new List<MSSeat>();
            int counter = 1;
            
            foreach (var item in listStrings)
            {
                MSSeat seat = new MSSeat();
                string[] words = item.Split(',');
                seat.class_bus_id = obj.class_bus_id;
                seat.seat_row = words[0].ToString();
                seat.seat_column = words[1].ToString();
                seat.seat_order = Convert.ToInt32(words[2]);
                //foreach (string word in words)
                //{
                //    seat.class_bus_id = obj.class_bus_id;
                //    if (counter == 1)
                //        seat.seat_row = word.ToString();
                //    else if (counter == 2)
                //        seat.seat_column = word.ToString();

                //    counter++;
                //}
                result.Add(seat);
                counter = 1;
            }

            return result;
        }

        public static BusinessLogicResult UpdateRoutesReguler(TRStationRoutesDto obj)
        {
            BusinessLogicResult res = new BusinessLogicResult();

            string errMsg = string.Empty;

            if(string.IsNullOrEmpty(obj.id.ToString()))
            {
                errMsg = "ID tidak boleh kosong";
            }
            else if (string.IsNullOrEmpty(obj.class_bus_id.ToString()))
            {
                errMsg = "Kelas Bus tidak boleh kosong";
            }
            else if(obj.stationRoutes.Count < 0)
            {
                errMsg = "Daftar kota tidak boleh kosong";
            }
            else if(obj.stationRoutes.Count == 1)
            {
                errMsg = "Daftar kota harus lebih dari 1";
            }
            else if(string.IsNullOrEmpty(obj.created_by))
            {
                errMsg = "Created By tidak boleh kosong";
            }

            res.result = !string.IsNullOrEmpty(errMsg) ? false : true;
            res.message = errMsg;

            return res;
        }
    }
}
