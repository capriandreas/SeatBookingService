using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MySqlConnector;
using SeatBookingService.BusinessLogic;
using SeatBookingService.Helper;
using SeatBookingService.Models;
using SeatBookingService.Models.DAO;
using SeatBookingService.Models.DTO;
using SeatBookingService.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
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
            var response = new APIResult<LoginResultDto>();
            string errMsg = string.Empty;
            BusinessLogicResult res = new BusinessLogicResult();

            try
            {
                res = TransactionLogic.Login(user);

                if (res.result)
                {
                    user.password = EncryptionHelper.sha256(user.password);

                    LoginResultDto userDto = _transactionDao.GetDataLogin(user);

                    if (userDto != null)
                    {
                        #region Generate JWT Token
                        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                        var claims = new[]{
                            new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                            new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToString()),
                            new Claim("username", userDto.username),
                            new Claim("nickname", userDto.nickname),
                            new Claim("rolename", userDto.rolename)
                        };

                        var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
                          _configuration["Jwt:Audience"],
                          claims,
                          expires: DateTime.Now.AddMinutes(15),
                          signingCredentials: credentials);

                        userDto.token = new JwtSecurityTokenHandler().WriteToken(token);

                        response.data = userDto;
                        #endregion
                    }
                    else
                    {
                        response.message = "Login Failed";
                        response.httpCode = HttpStatusCode.OK;
                        response.is_ok = true;
                        return Ok(response);
                    }
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
        /// Digunakan untuk create user agen dan pengemudi oleh user admin
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
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
        /// Digunakan untuk assign bus status per tanggal
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
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
        /// Digunakan untuk menampilkan seluruh trip yang sudah di booking oleh agent
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        [HttpGet]
        [Route("GetSeatDetail")]
        public async Task<IActionResult> GetSeatDetail([FromQuery] int reserved_seat_header_id)
        {
            var response = new APIResult<List<MSSeatDetailDto>>();
            BusinessLogicResult res = new BusinessLogicResult();

            try
            {
                response.data = _transactionDao.GetSeatDetail(reserved_seat_header_id);
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
        /// Digunakan untuk print ticket
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        [HttpGet]
        [Route("PrintTicket")]
        public async Task<IActionResult> PrintTicket([FromQuery] int reserved_seat_header_id)
        {
            var response = new APIResult<List<TicketDto>>();
            BusinessLogicResult res = new BusinessLogicResult();
            TicketDto ticket = new TicketDto();
            try
            {
                Byte[] b;

                //Get Header
                ticket = _transactionDao.GetTicketDataHeader(reserved_seat_header_id);

                //Get Detail Ticket
                List<MSSeatDetailDto> detail = _transactionDao.GetSeatDetail(reserved_seat_header_id);
                ticket.ticket_seat_detail = new List<TicketSeatDetail>();

                foreach (var item in detail)
                {
                    ticket.ticket_seat_detail.Add(new TicketSeatDetail
                    {
                        seat_id = item.seat_id,
                        seat_column = item.seat_column,
                        seat_row = item.seat_row
                    });
                }

                ticket.total_price = ticket.price * detail.Count;

                Image image = GenerateTicket.DrawTicket(ticket, Color.Black, Color.White);
                b = GenerateTicket.ImageToByteArray(image);

                return File(b, "image/jpeg");
            }
            catch(Exception ex)
            {

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
