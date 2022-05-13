﻿using SeatBookingService.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeatBookingService.Models.DAO
{
    public interface ITransactionDao
    {
        public List<TRBusAssignStatusDto> GetListAssignedBus(TRBusAssignStatus obj);
    }
}