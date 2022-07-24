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
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SeatBookingService.Controllers
{
    [Authorize]
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
        /// Digunakan untuk create station routes
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        [HttpPost]
        [Route("CreateStationRoutes")]
        public async Task<IActionResult> CreateStationRoutes(TRStationRoutesDto obj)
        {
            var response = new APIResult<List<string>>();
            var res = new BusinessLogicResult<List<string>>();

            try
            {
                response.is_ok = _transactionDao.InsertNewStationRoutes(obj);
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
        /// Digunakan untuk create trip schedule (non regular) oleh admin
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        [HttpPost]
        [Route("CreateTripScheduleNonRegular")]
        public async Task<IActionResult> CreateTripScheduleNonRegular(TRTripScheduleRoutesDto obj)
        {
            var response = new APIResult<List<TRTripSchedule>>();
            BusinessLogicResult res = new BusinessLogicResult();

            try
            {
                res = TransactionLogic.CreateTripScheduleNonRegular(obj);

                if (res.result)
                {
                    response.is_ok = _transactionDao.CreateTripScheduleNonRegular(obj);
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
        /// Digunakan untuk menampilkan seluruh trip baik regular maupun non regular
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        [HttpGet]
        [Route("GetAllTrip")]
        public async Task<IActionResult> GetAllTrip([FromQuery] DateTime? schedule_date, string city_from, string city_to)
        {
            var response = new APIResult<List<MSTripDto>>();
            BusinessLogicResult res = new BusinessLogicResult();

            try
            {
                res = TransactionLogic.GetAllTripValidation(schedule_date, city_from, city_to);

                if (res.result)
                {
                    response.data = _transactionDao.GetAllTrip(schedule_date, city_from, city_to);
                    response.data_records = response.data.Count;
                }

                response.message = res.message;
                response.is_ok = true;
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
        /// Digunakan untuk menampilkan seluruh trip baik regular maupun non regular
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        [HttpGet]
        [Route("GetTripSeatDetail")]
        public async Task<IActionResult> GetTripSeatDetail([FromQuery] TripDetailParamDto obj)
        {
            var response = new APIResult<BusSeatDetails>();
            BusSeatDetails busSeat = new BusSeatDetails();
            try
            {
                response.is_ok = true;

                #region check if data exists
                TRTrip trTrip = _transactionDao.GetTrTrip(obj);
                if (trTrip == null)
                {
                    bool isSuccessInsert = _transactionDao.CreateTrTrip(obj);
                }
                #endregion

                TRTrip getTrTrip = _transactionDao.GetTrTrip(obj);
                busSeat.trip_id = getTrTrip.id;
                busSeat.SeatsDetail = _transactionDao.GetListAllSeat(getTrTrip.id);

                response.data = busSeat;
                response.data_records = busSeat.SeatsDetail.Count;
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
                string joinSeatId = string.Join(",", obj.seat_detail.Select(x => x.seat_id));
                List<TRReservedSeatHeader2Dto> tRReservedSeatHeader2Dtos = _transactionDao.GetDataSeatValidation(obj.trip_id, joinSeatId);
                res = TransactionLogic.SubmitSeatBooking(obj, tRReservedSeatHeader2Dtos);

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
        /// Digunakan untuk assign bus status per tanggal
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        [HttpPost]
        [Route("AssignBusStatus")]
        public async Task<IActionResult> AssignBusStatus(List<TRBusAssignStatus> obj)
        {
            var response = new APIResult<List<TRTripSchedule>>();
            BusinessLogicResult res = new BusinessLogicResult();

            try
            {
                res = TransactionLogic.AssignBusStatus(obj);

                if (res.result)
                {
                    response.is_ok = _transactionDao.AssignBusStatus(obj);
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
        /// Digunakan untuk submit expedisi barang oleh agent
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        [HttpPost]
        [Route("SubmitExpedisi")]
        public async Task<IActionResult> SubmitExpedisi(TRExpedition obj)
        {
            var response = new APIResult<List<TRExpedition>>();
            var res = new BusinessLogicResult();

            try
            {
                res = TransactionLogic.SubmitExpedisi(obj);

                if (res.result)
                {
                    response.is_ok = _transactionDao.SubmitExpedition(obj);
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
        /// Digunakan untuk submit expedisi barang oleh agent
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        [HttpGet]
        [Route("GetExpedition")]
        public async Task<IActionResult> GetExpedition([FromQuery] TRExpedition obj)
        {
            var response = new APIResult<List<TRExpeditionDto>>();
            var res = new BusinessLogicResult();

            try
            {
                response.data = _transactionDao.GetExpedition(obj);
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
        /// Digunakan untuk cancel seat oleh agent
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        [HttpPost]
        [Route("CancelSeat")]
        public async Task<IActionResult> CancelSeat(TRCancellation obj)
        {
            var response = new APIResult<List<TRCancellation>>();
            var res = new BusinessLogicResult();

            try
            {
                res = TransactionLogic.CancelSeat(obj);

                if (res.result)
                {
                    response.is_ok = _transactionDao.CancelSeat(obj);
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
        /// Digunakan oleh admin untuk get list cancel yang dilakukan agent
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        [HttpGet]
        [Route("GetListCancelSeat")]
        public async Task<IActionResult> GetListCancelSeat()
        {
            var response = new APIResult<List<TRCancellationDto>>();
            var res = new BusinessLogicResult();

            try
            {
                response.data = _transactionDao.GetListCancelSeat();
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
        /// Digunakan untuk action approve cancel seat oleh admin
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        [HttpPost]
        [Route("ApproveCancelSeat")]
        public async Task<IActionResult> ApproveCancelSeat([FromBody] TRCancellation obj)
        {
            var response = new APIResult<List<TRCancellation>>();
            var res = new BusinessLogicResult();

            try
            {
                res = TransactionLogic.ApproveCancelSeat(obj);

                if (res.result)
                {
                    response.is_ok = _transactionDao.ApproveCancelSeat(obj);
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
        /// Digunakan untuk action reject cancel seat oleh admin
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        [HttpPost]
        [Route("RejectCancelSeat")]
        public async Task<IActionResult> RejectCancelSeat([FromBody] TRCancellation obj)
        {
            var response = new APIResult<List<TRCancellation>>();
            var res = new BusinessLogicResult();

            try
            {
                res = TransactionLogic.RejectCancelSeat(obj);

                if (res.result)
                {
                    response.is_ok = _transactionDao.RejectCancelSeat(obj);
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
        /// Digunakan oleh admin untuk get list cancel yang dilakukan agent
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        [HttpGet]
        [Route("GetListHistoryCancelSeat")]
        public async Task<IActionResult> GetListHistoryCancelSeat()
        {
            var response = new APIResult<List<TRCancellationDto>>();
            var res = new BusinessLogicResult();

            try
            {
                response.data = _transactionDao.GetListHistoryCancelSeat();
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
        /// Digunakan untuk get history header
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        [HttpGet]
        [Route("GetHistoryHeader")]
        public async Task<IActionResult> GetHistoryHeader([FromQuery] int users_id)
        {
            var response = new APIResult<List<HistoryHeaderDto>>();
            var res = new BusinessLogicResult();

            try
            {
                response.data = _transactionDao.GetHistoryHeader(users_id);
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
        /// Digunakan untuk get history detail
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        [HttpGet]
        [Route("GetHistoryDetail")]
        public async Task<IActionResult> GetHistoryDetail([FromQuery] int users_id, int trip_id)
        {
            var response = new APIResult<List<HistoryDetailDto>>();
            var res = new BusinessLogicResult();

            try
            {
                response.data = _transactionDao.GetHistoryDetail(trip_id, users_id);
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
        /// Digunakan untuk get history seat detail
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        [HttpGet]
        [Route("GetHistorySeatDetail")]
        public async Task<IActionResult> GetHistorySeatDetail([FromQuery] int reserved_seat_header_id)
        {
            var response = new APIResult<List<HistorySeatDetailDto>>();
            var res = new BusinessLogicResult();

            try
            {
                response.data = _transactionDao.GetHistorySeatDetail(reserved_seat_header_id);
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
        /// Digunakan untuk get history expedition detail
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        [HttpGet]
        [Route("GetHistoryExpeditionDetail")]
        public async Task<IActionResult> GetHistoryExpeditionDetail([FromQuery] int trip_id, int users_id)
        {
            var response = new APIResult<List<HistoryExpeditionDetailDto>>();
            var res = new BusinessLogicResult();

            try
            {
                response.data = _transactionDao.GetHistoryExpeditionDetail(trip_id, users_id);
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
        /// Digunakan untuk mengganti password oleh user
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        [HttpPost]
        [Route("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto obj)
        {
            var response = new APIResult<List<TRCancellation>>();
            var res = new BusinessLogicResult();

            try
            {
                res = TransactionLogic.ChangePassword(obj);

                if (res.result)
                {
                    string encryptPassword = EncryptionHelper.sha256(obj.password);
                    obj.encrypted_password = encryptPassword;
                    response.is_ok = _transactionDao.ChangePassword(obj);
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
        /// Digunakan untuk nge-Assign bus dengan trip tertentu
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        [HttpPost]
        [Route("AssignBusTrip")]
        public async Task<IActionResult> AssignBusTrip([FromBody] TRBusTripSchedule obj)
        {
            var response = new APIResult<List<TRBusTripSchedule>>();
            var res = new BusinessLogicResult();

            try
            {
                List<MSBus> checkIfAssigned = _transactionDao.GetAllBusAssignValidation(obj);
                res = TransactionLogic.AssignBusTrip(obj, checkIfAssigned);

                if (res.result)
                {
                    response.is_ok = _transactionDao.AssignBusTrip(obj);
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
        /// Digunakan untuk get seluruh bus yang akan di assign
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        [HttpGet]
        [Route("GetAllBusToAssign")]
        public async Task<IActionResult> GetAllBusToAssign([FromQuery] DateTime? schedule_date)
        {
            var response = new APIResult<List<MSBus>>();
            var res = new BusinessLogicResult();

            try
            {
                response.data = _transactionDao.GetAllBusToAssign(schedule_date);
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
        /// Digunakan untuk report summary
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        [HttpGet]
        [Route("GetSummaryReport")]
        public async Task<IActionResult> GetSummaryReport([FromQuery] DateTime? schedule_date)
        {
            var response = new APIResult<List<GetSummaryReportDto>>();
            var res = new BusinessLogicResult();

            try
            {
                response.data = _transactionDao.GetSummaryReport(schedule_date);
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
        /// Digunakan untuk menampilkan seluruh trip baik regular maupun non regular yang akan di assign ke bus
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        [HttpGet]
        [Route("GetTripAssignToBus")]
        public async Task<IActionResult> GetTripAssignToBus([FromQuery] TripDetailParamDto obj)
        {
            var response = new APIResult<TRTrip>();
            BusSeatDetails busSeat = new BusSeatDetails();
            try
            {
                response.is_ok = true;

                #region check if data exists
                TRTrip trTrip = _transactionDao.GetTrTrip(obj);
                if (trTrip == null)
                {
                    bool isSuccessInsert = _transactionDao.CreateTrTrip(obj);
                }
                #endregion

                TRTrip getTrTrip = _transactionDao.GetTrTrip(obj);
                busSeat.trip_id = getTrTrip.id;

                response.data = getTrTrip;
                response.data_records = 1;
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
        /// Digunakan untuk menampilkan seluruh trip schedule non reguler
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        [HttpGet]
        [Route("GetAllTripScheduleNonReguler")]
        public async Task<IActionResult> GetAllTripScheduleNonReguler([FromQuery] DateTime? schedule_date)
        {
            var response = new APIResult<List<TripNonRegulerDto>>();

            try
            {
                response.is_ok = true;
                response.data = _transactionDao.GetAllTripScheduleNonReguler(schedule_date);
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
        [Route("GetTripNonRegulerDetails")]
        public async Task<IActionResult> GetTripNonRegulerDetails([FromQuery] int id)
        {
            var response = new APIResult<TRTripScheduleRoutesDto>();

            try
            {
                TRTripScheduleRoutesDto result = new TRTripScheduleRoutesDto();
                TRTripSchedule routes = _transactionDao.GetTrTripScheduleDetail(id);
                List<TRTripScheduleRoutes> tripRoutes = _transactionDao.GetTripScheduleRoutesDetail(id);

                if (routes != null)
                {
                    result.id = routes.id;
                    result.class_bus_id = routes.class_bus_id;
                    result.schedule_date = routes.schedule_date;
                    result.departure_hours = routes.departure_hours;
                    result.description = routes.description;
                    result.created_by = routes.created_by;
                }

                if (tripRoutes != null && tripRoutes.Count > 0)
                {
                    result.tripRoutes = new List<TripScheduleRoutes>();
                    foreach (var item in tripRoutes)
                    {
                        result.tripRoutes.Add(new TripScheduleRoutes
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
            }
            catch (Exception ex)
            {
                response.is_ok = false;
                response.message = ex.Message;
            }

            return Ok(response);
        }

        /// <summary>
        /// Digunakan untuk edit routes dan station routes
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        [HttpPost]
        [Route("UpdateRoutesNonReguler")]
        public async Task<IActionResult> UpdateRoutesNonReguler(TRTripScheduleRoutesDto obj)
        {
            var response = new APIResult<List<string>>();
            BusinessLogicResult res = new BusinessLogicResult();

            try
            {
                res = TransactionLogic.UpdateRoutesNonReguler(obj);

                if (res.result)
                {
                    response.is_ok = _transactionDao.UpdateRoutesNonReguler(obj);
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
        /// Digunakan untuk menampilkan seluruh trip schedule non reguler
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        [HttpGet]
        [Route("GetAllTripReguler")]
        public async Task<IActionResult> GetAllTripReguler()
        {
            var response = new APIResult<List<TripRegulerDto>>();

            try
            {
                response.is_ok = true;
                response.data = _transactionDao.GetAllTripReguler();
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
        [Route("GetTripRegulerDetails")]
        public async Task<IActionResult> GetTripRegulerDetails([FromQuery] int id)
        {
            var response = new APIResult<TRStationRoutesDto>();

            try
            {
                TRStationRoutesDto result = new TRStationRoutesDto();
                MSRoutes routes = _transactionDao.GetMsRoutesById(id);
                List<MSStationRoutes> tripRoutes = _transactionDao.GetMasterRoutesDetail(id);

                if (routes != null)
                {
                    result.id = routes.id;
                    result.class_bus_id = routes.class_bus_id;
                    result.departure_hours = routes.departure_hours;
                    result.description = routes.description;
                    result.created_by = routes.created_by;
                }

                if (tripRoutes != null && tripRoutes.Count > 0)
                {
                    result.stationRoutes = new List<MSStationRoutes>();
                    foreach (var item in tripRoutes)
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
            }
            catch (Exception ex)
            {
                response.is_ok = false;
                response.message = ex.Message;
            }

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

            try
            {
                res = TransactionLogic.UpdateRoutesReguler(obj);

                if (res.result)
                {
                    response.is_ok = _transactionDao.UpdateRoutesReguler(obj);
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
        /// Digunakan untuk edit routes dan station routes
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        [HttpPost]
        [Route("DeleteTripReguler")]
        public async Task<IActionResult> DeleteTripReguler(List<MSRoutes> obj)
        {
            var response = new APIResult<List<string>>();
            BusinessLogicResult res = new BusinessLogicResult();

            try
            {
                res = TransactionLogic.DeleteTripReguler(obj);

                if (res.result)
                {
                    response.is_ok = _transactionDao.DeleteTripReguler(obj);
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

                response.data = res.data;
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
