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
        [Route("GetMasterBus")]
        public async Task<IActionResult> GetMasterBus()
        {
            var response = new APIResult<List<MSBus>>();

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

        [HttpGet]
        [Route("GetMasterKelasBus")]
        public async Task<IActionResult> GetMasterKelasBus()
        {
            var response = new APIResult<List<MSKelasBus>>();

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
