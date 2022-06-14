using Microsoft.AspNetCore.Mvc;
using SeatBookingService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using MySqlConnector;
using System.Net;
using SeatBookingService.Models.DAO;
using SeatBookingService.Models.DTO;
using Microsoft.AspNetCore.Authorization;

namespace SeatBookingService.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MasterDataController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IMasterDataDao _masterDataDao;

        public MasterDataController(IConfiguration configuration, IMasterDataDao masterDataDao)
        {
            _configuration = configuration;
            _masterDataDao = masterDataDao;
        }

        /// <summary>
        /// Digunakan untuk menampilkan seluruh bus. Dimana akan digunakan untuk set status bus apakah idle atau standby berdasarkan tanggal.
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        [HttpGet]
        [Route("GetAllMasterBus")]
        public async Task<IActionResult> GetAllMasterBus()
        {
            var response = new APIResult<List<MSBus>>();

            try
            {
                response.is_ok = true;
                response.data = _masterDataDao.GetAllMasterBus();
                response.data_records = response.data.Count;
                response.httpCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                response.is_ok = false;
                response.message = ex.Message;
            }

            return Ok(response);
        }

        /// <summary>
        /// Digunakan untuk menampilkan dropdown list origin city
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        [HttpGet]
        [Route("GetAllOriginCity")]
        public async Task<IActionResult> GetAllOriginCity()
        {
            var response = new APIResult<List<MSStationsRoutes>>();

            try
            {
                response.is_ok = true;
                response.data = _masterDataDao.GetAllOriginCity();
                response.data_records = response.data.Count;
                response.httpCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                response.is_ok = false;
                response.message = ex.Message;
            }

            return Ok(response);
        }

        /// <summary>
        /// Digunakan untuk menampilkan dropdown list destination city berdasarkan origin city
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        [HttpGet]
        [Route("GetAllDestinationCity")]
        public async Task<IActionResult> GetAllDestinationCity([FromQuery] MSStationsRoutes obj)
        {
            var response = new APIResult<List<MSStationsRoutes>>();

            try
            {
                response.is_ok = true;
                response.data = _masterDataDao.GetAllDestinationCity(obj);
                response.data_records = response.data.Count;
                response.httpCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                response.is_ok = false;
                response.message = ex.Message;
            }

            return Ok(response);
        }

        /// <summary>
        /// Digunakan untuk menampilkan seluruh list User selain User Admin
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        [HttpGet]
        [Route("GetAllListUsers")]
        public async Task<IActionResult> GetAllListUsers()
        {
            var response = new APIResult<List<MSUsersDto>>();

            try
            {
                response.is_ok = true;
                response.data = _masterDataDao.GetAllListUsers();
                response.data_records = response.data.Count;
                response.httpCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                response.is_ok = false;
                response.message = ex.Message;
            }

            return Ok(response);
        }

        /// <summary>
        /// Digunakan untuk menampilkan seluruh bus. Dimana akan digunakan untuk set status bus apakah idle atau standby berdasarkan tanggal.
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        [HttpGet]
        [Route("GetAllMasterBus")]
        public async Task<IActionResult> GetAllMasterClassBus()
        {
            var response = new APIResult<List<MSBus>>();

            try
            {
                response.is_ok = true;
                response.data = _masterDataDao.GetAllMasterBus();
                response.data_records = response.data.Count;
                response.httpCode = HttpStatusCode.OK;
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
