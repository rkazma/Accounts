using Dapper;
using System.Data;
using System.Data.SqlClient;
using static Dapper.SqlMapper;

namespace Accounts.Domain
{
    public class DapperUtils
    {
        private string enterpriseConnectionString;

        public DapperUtils(string enterpriseConnectionString)
        {
            this.enterpriseConnectionString = enterpriseConnectionString;
        }

        public IDbConnection GetConnection()
        {
            return new SqlConnection(enterpriseConnectionString);
        }

        public async Task<List<T>> GetListAsync<T>(string query, object param = null, CommandType commandType = CommandType.StoredProcedure) where T : class
        {
            using (IDbConnection conn = GetConnection())
            {
                var enumerable = await conn.QueryAsync<T>(query, param, commandTimeout: 300, commandType: commandType);

                return enumerable.ToList();
            }
        }

        public async Task<T> ExcecuteScalarAsync<T>(string query, object param = null, int commandTimeout = 300, CommandType commandType = CommandType.StoredProcedure)
        {
            using (IDbConnection conn = GetConnection())
            {
                var results = await conn.ExecuteScalarAsync<T>(query, param, commandTimeout: commandTimeout, commandType: commandType);
                return results;
            }
        }

        public async Task<GridReader> GetMultipleAsync(string query, object param = null, CommandType commandType = CommandType.StoredProcedure)
        {
            IDbConnection conn = GetConnection();
            var results = await conn.QueryMultipleAsync(query, param, commandTimeout: 300, commandType: commandType);
            return results;
        }

        public async Task<T> GetSingleAsync<T>(string query, object param = null, CommandType commandType = CommandType.StoredProcedure)
        {
            using (IDbConnection conn = GetConnection())
            {
                var results = await conn.QueryFirstOrDefaultAsync<T>(query, param, commandTimeout: 300, commandType: commandType);
                return results;
            }
        }
    }
}