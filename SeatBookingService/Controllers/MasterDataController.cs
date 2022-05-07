using Microsoft.AspNetCore.Mvc;
using SeatBookingService.Models;
using SeatBookingService.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeatBookingService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MasterDataController : ControllerBase
    {
        [HttpGet]
        [Route("GetMasterBusCategory")]
        public async Task<IActionResult> GetMasterBusCategory()
        {
            var response = new APIResult<List<MSBusCategory>>();

            try
            {
                response.is_ok = true;
                //response.data = _masterDataDao.GetMasterTransportType();
            }
            catch (Exception ex)
            {
                response.is_ok = false;
                response.message = ex.Message;
            }

            return Ok(response);
        }
    }
}
