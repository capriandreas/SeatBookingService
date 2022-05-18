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
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ITransactionDao _transactionDao;

        public AccountController(IConfiguration configuration, ITransactionDao transactionDao)
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
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] MSUsers user)
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

                        var token = new JwtSecurityToken(
                          _configuration["Jwt:Issuer"],
                          _configuration["Jwt:Audience"],
                          claims,
                          expires: null,
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
    }
}
