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
        public async Task<IActionResult> GetHistoryDetail([FromQuery] int users_id, int trip_schedule_id)
        {
            var response = new APIResult<List<HistoryDetailDto>>();
            var res = new BusinessLogicResult();

            try
            {
                response.data = _transactionDao.GetHistoryDetail(trip_schedule_id, users_id);
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
        public async Task<IActionResult> GetHistoryExpeditionDetail([FromQuery] int trip_schedule_id, int users_id)
        {
            var response = new APIResult<List<HistoryExpeditionDetailDto>>();
            var res = new BusinessLogicResult();

            try
            {
                response.data = _transactionDao.GetHistoryExpeditionDetail(trip_schedule_id, users_id);
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
        public async Task<IActionResult> CreateTripScheduleNonRegular(TRTripSchedule obj)
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
        public async Task<IActionResult> GetAllTrip([FromQuery] DateTime? schedule_date)
        {
            var response = new APIResult<List<MSTripDto>>();

            try
            {
                response.is_ok = true;
                response.data = _transactionDao.GetAllTrip(schedule_date);
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
                busSeat.SeatsDetail = _transactionDao.GetListAllSeat(obj);

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
