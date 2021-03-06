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
using SeatBookingService.BusinessLogic;

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

            response.is_ok = true;
            response.data = _masterDataDao.GetAllMasterBus();
            response.data_records = response.data.Count;
            response.httpCode = HttpStatusCode.OK;

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

            response.is_ok = true;
            response.data = _masterDataDao.GetAllOriginCity();
            response.data_records = response.data.Count;
            response.httpCode = HttpStatusCode.OK;

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

            response.is_ok = true;
            response.data = _masterDataDao.GetAllDestinationCity(obj);
            response.data_records = response.data.Count;
            response.httpCode = HttpStatusCode.OK;

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

            response.is_ok = true;
            response.data = _masterDataDao.GetAllListUsers();
            response.data_records = response.data.Count;
            response.httpCode = HttpStatusCode.OK;

            return Ok(response);
        }

        /// <summary>
        /// Digunakan untuk menampilkan seluruh bus. Dimana akan digunakan untuk set status bus apakah idle atau standby berdasarkan tanggal.
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        [HttpGet]
        [Route("GetAllMasterClassBus")]
        public async Task<IActionResult> GetAllMasterClassBus()
        {
            var response = new APIResult<List<MSClassBusDto>>();

            response.is_ok = true;
            response.data = _masterDataDao.GetAllMasterClassBus();
            response.data_records = response.data.Count;
            response.httpCode = HttpStatusCode.OK;

            return Ok(response);
        }

        /// <summary>
        /// Digunakan untuk menampilkan seluruh bus. Dimana akan digunakan untuk set status bus apakah idle atau standby berdasarkan tanggal.
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        [HttpGet]
        [Route("GetAllMasterRoles")]
        public async Task<IActionResult> GetAllMasterRoles()
        {
            var response = new APIResult<List<MSRolesDto>>();

            response.is_ok = true;
            response.data = _masterDataDao.GetAllMasterRoles();
            response.data_records = response.data.Count;
            response.httpCode = HttpStatusCode.OK;

            return Ok(response);
        }

        /// <summary>
        /// Digunakan untuk menampilkan seluruh station routes
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        [HttpGet]
        [Route("GetAllStationRoutes")]
        public async Task<IActionResult> GetAllStationRoutes()
        {
            var response = new APIResult<List<MSStationsRoutesDto>>();

            response.is_ok = true;
            response.data = _masterDataDao.GetAllStationRoutes();
            response.data_records = response.data.Count;
            response.httpCode = HttpStatusCode.OK;

            return Ok(response);
        }

        /// <summary>
        /// Digunakan untuk edit routes dan station routes
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        [HttpPost]
        [Route("UpdateRoutesReguler")]
        public async Task<IActionResult> UpdateRoutesReguler(TRStationRoutesDto obj)
        {
            var response = new APIResult<List<string>>();
            BusinessLogicResult res = new BusinessLogicResult();

            res = MasterDataLogic.UpdateRoutesReguler(obj);

            if (res.result)
            {
                response.is_ok = _masterDataDao.UpdateRoutesReguler(obj);
            }

            response.is_ok = true;
            response.httpCode = HttpStatusCode.OK;
            response.message = res.message;

            return Ok(response);
        }

        /// <summary>
        /// Digunakan untuk menampilkan seluruh bus. Dimana akan digunakan untuk set status bus apakah idle atau standby berdasarkan tanggal.
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        [HttpGet]
        [Route("GetMasterRoutesDetails")]
        public async Task<IActionResult> GetMasterRoutesDetails([FromQuery] int id)
        {
            var response = new APIResult<TRStationRoutesDto>();

            TRStationRoutesDto result = new TRStationRoutesDto();
            MSRoutes routes = _masterDataDao.GetMSRoutes(id);
            List<MSStationRoutes> stationRoutes = _masterDataDao.GetMSStationRoutes(id);

            if (routes != null)
            {
                result.id = routes.id;
                result.class_bus_id = routes.class_bus_id;
                result.departure_hours = routes.departure_hours;
                result.description = routes.description;
                result.created_by = routes.created_by;
            }

            if (stationRoutes != null && stationRoutes.Count > 0)
            {
                result.stationRoutes = new List<MSStationRoutes>();
                foreach (var item in stationRoutes)
                {
                    result.stationRoutes.Add(new MSStationRoutes
                    {
                        route_order = item.route_order,
                        city = item.city
                    });
                }
            }

            response.is_ok = true;
            response.data = result;
            response.data_records = 1;
            response.httpCode = HttpStatusCode.OK;

            return Ok(response);
        }
    }
}
