using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using SeatBookingService.BusinessLogic;
using SeatBookingService.Helper;
using SeatBookingService.Models;
using SeatBookingService.Models.DAO;
using SeatBookingService.Models.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SeatBookingService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ITransactionDao _transactionDao;

        public TransactionController(IConfiguration configuration, ITransactionDao transactionDao)
        {
            _configuration = configuration;
            _transactionDao = transactionDao;
        }

        /// <summary>
        /// Digunakan untuk create user agen dan pengemudi oleh user admin
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("CreateUser")]
        public async Task<IActionResult> CreateUser(MSUsers user)
        {
            var response = new APIResult<DataTable>();
            string errMsg = string.Empty;
            string query = string.Empty;

            try
            {
                #region Validation
                if (string.IsNullOrWhiteSpace(user.username))
                {
                    errMsg = "Username cannot be empty";
                }
                else if (string.IsNullOrWhiteSpace(user.password))
                {
                    errMsg = "Password cannot be empty";
                }
                else if (string.IsNullOrWhiteSpace(user.nickname))
                {
                    errMsg = "Nickname cannot be empty";
                }

                if (!string.IsNullOrEmpty(errMsg))
                {
                    response.httpCode = HttpStatusCode.OK;
                    response.is_ok = false;
                    response.message = errMsg;
                    return Ok(response);
                }
                #endregion

                DataTable table = new DataTable();
                string sqlDataSource = _configuration.GetConnectionString("DefaultConnection");
                MySqlDataReader myReader;

                #region Check If Data Exists
                query = @"
                        select username, password, nickname, role_id from ms_users 
                        where username = @username and is_active = 1;
                ";

                using (MySqlConnection mycon = new MySqlConnection(sqlDataSource))
                {
                    mycon.Open();
                    using (MySqlCommand myCommand = new MySqlCommand(query, mycon))
                    {
                        myCommand.Parameters.AddWithValue("@username", user.username);

                        myReader = myCommand.ExecuteReader();
                        table.Load(myReader);

                        myReader.Close();
                        mycon.Close();
                    }
                }

                if (table.Rows.Count >= 1)
                {
                    response.httpCode = HttpStatusCode.OK;
                    response.is_ok = false;
                    response.message = "Username Already Exists";
                    return Ok(response);
                }
                #endregion

                #region Insert Into
                string encryptPassword = EncryptionHelper.sha256(user.password);

                query = @"
                        insert into ms_users (username, password, nickname, role_id) values
                                                    (@username, @password, @nickname, @role_id);
                ";

                using (MySqlConnection mycon = new MySqlConnection(sqlDataSource))
                {
                    mycon.Open();
                    using (MySqlCommand myCommand = new MySqlCommand(query, mycon))
                    {
                        myCommand.Parameters.AddWithValue("@username", user.username);
                        myCommand.Parameters.AddWithValue("@password", encryptPassword);
                        myCommand.Parameters.AddWithValue("@nickname", user.nickname);
                        myCommand.Parameters.AddWithValue("@role_id", user.role_id);

                        myReader = myCommand.ExecuteReader();
                        table.Load(myReader);

                        myReader.Close();
                        mycon.Close();
                    }
                }
                #endregion

                response.httpCode = HttpStatusCode.OK;
                response.is_ok = true;
                response.data = table;
                response.data_records = table.Rows.Count;
            }
            catch (Exception ex)
            {
                response.is_ok = false;
                response.message = ex.Message;
            }

            return Ok(response);
        }

        /// <summary>
        /// Digunakan untuk Login
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(MSUsers user)
        {
            var response = new APIResult<DataTable>();
            string errMsg = string.Empty;

            try
            {
                #region Validation
                if (string.IsNullOrWhiteSpace(user.username))
                {
                    errMsg = "Username cannot be empty";
                }
                else if (string.IsNullOrWhiteSpace(user.password))
                {
                    errMsg = "Password cannot be empty";
                }

                if (!string.IsNullOrEmpty(errMsg))
                {
                    response.httpCode = HttpStatusCode.OK;
                    response.is_ok = false;
                    response.message = errMsg;
                    return Ok(response);
                }
                #endregion

                string encryptPassword = EncryptionHelper.sha256(user.password);

                string query = @"
                        select a.id, username, nickname, role_id, rolename
                        from ms_users a
                        left join ms_roles b on a.role_id = b.id
                        where username = @username and password = @password and is_active = 1;
                ";

                DataTable table = new DataTable();
                string sqlDataSource = _configuration.GetConnectionString("DefaultConnection");
                MySqlDataReader myReader;
                using (MySqlConnection mycon = new MySqlConnection(sqlDataSource))
                {
                    mycon.Open();
                    using (MySqlCommand myCommand = new MySqlCommand(query, mycon))
                    {
                        myCommand.Parameters.AddWithValue("@username", user.username);
                        myCommand.Parameters.AddWithValue("@password", encryptPassword);

                        myReader = myCommand.ExecuteReader();
                        table.Load(myReader);

                        myReader.Close();
                        mycon.Close();
                    }
                }

                response.httpCode = HttpStatusCode.OK;
                response.data = table;
                response.data_records = table.Rows.Count;

                if (table.Rows.Count > 0)
                {
                    response.is_ok = true;
                    response.message = "Login Successfully";
                }
                else
                {
                    response.is_ok = false;
                    response.message = "Login Failed";
                }
            }
            catch (Exception ex)
            {
                response.is_ok = false;
                response.message = ex.Message;
            }

            return Ok(response);
        }

        /// <summary>
        /// Digunakan untuk assign bus status per tanggal
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("AssignBusStatus")]
        public async Task<IActionResult> AssignBusStatus(List<TRBusAssignStatus> bus)
        {
            var response = new APIResult<DataTable>();
            string errMsg = string.Empty;
            string query = string.Empty;

            try
            {
                #region Validation
                if(bus.Count <= 0)
                {
                    errMsg = "No Data, Please Check Your Request!";
                }

                if (!string.IsNullOrEmpty(errMsg))
                {
                    response.httpCode = HttpStatusCode.OK;
                    response.is_ok = false;
                    response.message = errMsg;
                    return Ok(response);
                }
                #endregion

                string sqlDataSource = _configuration.GetConnectionString("DefaultConnection");

                StringBuilder sCommand = new StringBuilder("insert into tr_bus_assign_status (no_bus, status_bus_id, status_date) values ");

                using (MySqlConnection mycon = new MySqlConnection(sqlDataSource))
                {
                    List<string> Rows = new List<string>();

                    foreach(var item in bus)
                    {
                        Rows.Add(string.Format("('{0}','{1}','{2}')", item.no_bus, item.status_bus_id, item.assign_date.ToString("yyyy-MM-dd HH:mm:ss")));
                    }

                    sCommand.Append(string.Join(",", Rows));
                    sCommand.Append(";");
                    mycon.Open();

                    using (MySqlCommand myCommand = new MySqlCommand(sCommand.ToString(), mycon))
                    {
                        myCommand.CommandType = CommandType.Text;
                        myCommand.ExecuteNonQuery();
                    }
                }

                response.httpCode = HttpStatusCode.OK;
                response.is_ok = true;
            }
            catch (Exception ex)
            {
                response.is_ok = false;
                response.message = ex.Message;
            }

            return Ok(response);
        }

        /// <summary>
        /// Digunakan untuk menampilkan seluruh bus yang sudah di assign berdasarkan tanggal assign
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        [HttpGet]
        [Route("GetListAssignedBus")]
        public async Task<IActionResult> GetListAssignedBus([FromQuery] TRBusAssignStatus obj)
        {
            var response = new APIResult<List<TRBusAssignStatusDto>>();

            try
            {
                response.is_ok = true;
                response.data = _transactionDao.GetListAssignedBus(obj);
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
        /// Digunakan untuk submit trip schedule oleh admin
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        [HttpPost]
        [Route("SubmitTripSchedule")]
        public async Task<IActionResult> SubmitTripSchedule(TRTripScheduleDto obj)
        {
            var response = new APIResult<List<TRTripSchedule>>();
            BusinessLogicResult res = new BusinessLogicResult();

            try
            {
                res = TransactionLogic.SubmitTripScheduleValidation(obj);

                if(res.result)
                {
                    response.is_ok = _transactionDao.SubmitTripSchedule(obj);
                }

                response.is_ok = true;
                response.httpCode = HttpStatusCode.OK;
                response.message = res.message;
                
            }
            catch (Exception ex)
            {
                response.is_ok = false;
                response.message = ex.Message;
            }

            return Ok(response);
        }

        /// <summary>
        /// Digunakan untuk menampilkan seluruh jadwal trip yang telah dibuat oleh admin
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        [HttpGet]
        [Route("GetListTripSchedule")]
        public async Task<IActionResult> GetListTripSchedule([FromQuery] TRTripSchedule obj)
        {
            var response = new APIResult<List<TRTripScheduleDto>>();
            BusinessLogicResult res = new BusinessLogicResult();

            try
            {
                res = TransactionLogic.GetListTripScheduleValidation(obj);

                if (res.result)
                {
                    response.data = _transactionDao.GetListTripSchedule(obj);
                    response.data_records = response.data.Count;
                }

                response.is_ok = true;
                response.httpCode = HttpStatusCode.OK;
                response.message = res.message;
            }
            catch (Exception ex)
            {
                response.is_ok = false;
                response.message = ex.Message;
            }

            return Ok(response);
        }

        /// <summary>
        /// Digunakan untuk menampilkan seluruh data seat berdasarkan trip schedule id
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        [HttpGet]
        [Route("GetListAllSeat")]
        public async Task<IActionResult> GetListAllSeat([FromQuery] int trip_schedule_id)
        {
            var response = new APIResult<List<MSSeatDto>>();
            BusinessLogicResult res = new BusinessLogicResult();

            try
            {
                response.data = _transactionDao.GetListAllSeat(trip_schedule_id);
                response.data_records = response.data.Count;

                response.is_ok = true;
                response.httpCode = HttpStatusCode.OK;
                response.message = res.message;
            }
            catch (Exception ex)
            {
                response.is_ok = false;
                response.message = ex.Message;
            }

            return Ok(response);
        }

        /// <summary>
        /// Digunakan untuk submit seat booking
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        [HttpPost]
        [Route("SubmitSeatBooking")]
        public async Task<IActionResult> SubmitSeatBooking(TRReservedSeatHeaderDto obj)
        {
            var response = new APIResult<List<TRReservedSeatHeaderDto>>();
            BusinessLogicResult res = new BusinessLogicResult();

            try
            {
                res = TransactionLogic.SubmitSeatBooking(obj);

                if (res.result)
                {
                    response.is_ok = _transactionDao.SubmitSeatBooking(obj);
                }

                response.is_ok = true;
                response.httpCode = HttpStatusCode.OK;
                response.message = res.message;

            }
            catch (Exception ex)
            {
                response.is_ok = false;
                response.message = ex.Message;
            }

            return Ok(response);
        }

        /// <summary>
        /// Digunakan untuk menampilkan seluruh trip yang sudah di booking oleh agent
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        [HttpGet]
        [Route("GetListBookedTrip")]
        public async Task<IActionResult> GetListBookedTrip([FromQuery] int users_id)
        {
            var response = new APIResult<List<TRReservedSeatHeaderBookedDto>>();
            BusinessLogicResult res = new BusinessLogicResult();

            try
            {
                response.data = _transactionDao.GetListBookedTrip(users_id);
                foreach(var date in response.data)
                {
                    date.schedule_date_string = date.schedule_date.ToString("dddd, dd MMMM yyyy");
                }

                response.data_records = response.data.Count;

                response.is_ok = true;
                response.httpCode = HttpStatusCode.OK;
                response.message = res.message;
            }
            catch (Exception ex)
            {
                response.is_ok = false;
                response.message = ex.Message;
            }

            return Ok(response);
        }

        /// <summary>
        /// Digunakan untuk generate seat untuk master data
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        [HttpPost]
        [Route("GenerateSeat")]
        public async Task<IActionResult> GenerateSeat(GenerateSeat obj)
        {
            var response = new APIResult<List<string>>();
            var res = new BusinessLogicResult<List<string>>();

            try
            {
                res = MasterDataLogic.GenerateMasterSeat(obj);
                List<MSSeat> listSeat = MasterDataLogic.MappingSeatToModel(res.data, obj);

                //response.data = res.data;
                response.is_ok = _transactionDao.InsertMasterSeat(listSeat);
                response.httpCode = HttpStatusCode.OK;
                response.message = res.message;
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
