using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using SeatBookingService.Helper;
using SeatBookingService.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SeatBookingService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public TransactionController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Digunakan untuk create user agent oleh user admin
        /// </summary>
        /// <returns>
        /// User's Object
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
                if(string.IsNullOrWhiteSpace(user.username))
                {
                    errMsg = "Username cannot be empty";
                }
                else if(string.IsNullOrWhiteSpace(user.password))
                {
                    errMsg = "Password cannot be empty";
                }
                else if(string.IsNullOrWhiteSpace(user.nickname))
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

                if(table.Rows.Count >= 1)
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
                                                    (@username, @password, @nickname, 2);
                ";

                using (MySqlConnection mycon = new MySqlConnection(sqlDataSource))
                {
                    mycon.Open();
                    using (MySqlCommand myCommand = new MySqlCommand(query, mycon))
                    {
                        myCommand.Parameters.AddWithValue("@username", user.username);
                        myCommand.Parameters.AddWithValue("@password", encryptPassword);
                        myCommand.Parameters.AddWithValue("@nickname", user.nickname);

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
                response.message = "User Created Successfully";
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
        /// User's Object
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
                        select username, nickname, role_id, rolename 
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
    }
}
