using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeatBookingService.Helper
{
    public interface ISQLHelper
    {
        public Task<List<T>> queryList<T>(string query, Dictionary<string, object> param) where T : new();
        public Task<T> querySingle<T>(string query, Dictionary<string, object> param) where T : new();
        public Task<int> queryInsert(string query, Dictionary<string, object> param);
        public Task<int> queryInsertWithReturningId(string query, Dictionary<string, object> param);
        public Task<int> queryInsert(MySqlConnection transaction, string query, Dictionary<string, object> param);
        public Task<int> queryUpdate(string query, Dictionary<string, object> param);
        public Task<int> queryUpdate(MySqlConnection transaction, string query, Dictionary<string, object> param);
        public Task<int> queryDelete(string query, Dictionary<string, object> param);
        public Task<int> queryDelete(MySqlConnection transaction, string query, Dictionary<string, object> param);
    }
}
