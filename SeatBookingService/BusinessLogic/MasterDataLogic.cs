using SeatBookingService.Models;
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
                    seat = seat + "," + item;
                    listSeat.Add(seat);
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
                foreach (string word in words)
                {
                    seat.class_bus_id = obj.class_bus_id;
                    if (counter == 1)
                        seat.seat_row = word.ToString();
                    else if (counter == 2)
                        seat.seat_column = word.ToString();

                    counter++;
                }
                result.Add(seat);
                counter = 1;
            }

            return result;
        }
    }
}
