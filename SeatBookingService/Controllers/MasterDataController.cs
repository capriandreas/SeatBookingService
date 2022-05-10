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

namespace SeatBookingService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MasterDataController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public MasterDataController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        [Route("GetAllMasterBus")]
        public async Task<IActionResult> GetAllMasterBus()
        {
            var response = new APIResult<DataTable>();

            try
            {
                string query = @"
                        select a.id, a.no_bus, a.no_polisi, a.jumlah_seat, a.kelas_id, b.kelas_bus 
                        from ms_bus a
                        left join ms_kelas_bus b on b.id = a.kelas_id
                ";

                DataTable table = new DataTable();
                string sqlDataSource = _configuration.GetConnectionString("DefaultConnection");
                MySqlDataReader myReader;
                using (MySqlConnection mycon = new MySqlConnection(sqlDataSource))
                {
                    mycon.Open();
                    using (MySqlCommand myCommand = new MySqlCommand(query, mycon))
                    {
                        myReader = myCommand.ExecuteReader();
                        table.Load(myReader);

                        myReader.Close();
                        mycon.Close();
                    }
                }

                response.httpCode = HttpStatusCode.OK;
                response.is_ok = true;
                response.data = table;
                response.data_records = table.Rows.Count;
                response.message = "Data Retrieved Successfully";
            }
            catch(Exception ex)
            {
                response.is_ok = false;
                response.message = ex.Message;
            }
            
            return Ok(response);
        }
    }
}
