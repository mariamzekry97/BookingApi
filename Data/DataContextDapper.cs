using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Dapper;

namespace HotelBooking.Data
{
    class DataContextDapper
    {
        private readonly IConfiguration _config;

        public DataContextDapper(IConfiguration config)
        {
            _config = config;
        }

        public IEnumerable<T> LoadData<T>(string sql)
        {
            IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("Default"));
            return dbConnection.Query<T>(sql);
        }

        public T? LoadDataSingle<T>(string sql)
        {
            IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("Default"));
            return dbConnection.QuerySingleOrDefault<T>(sql);
        }

        public bool ExecuteSql(string sql)
        {
            IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("Default"));
            return dbConnection.Execute(sql) > 0;
        }

        
        public int ExecuteSqlWithRowCount(string sql)
        {
            IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("Default"));
            return dbConnection.Execute(sql);
        }


        public bool ExecuteSqlWithParameters(string sql, List<SqlParameter> parameters)
        {
            SqlConnection dbConnection = new SqlConnection(_config.GetConnectionString("Default"));
            dbConnection.Open();
            SqlCommand command = new SqlCommand(sql, dbConnection);

            foreach (var parameter in parameters)
            {
                command.Parameters.Add(parameter);
            }   
           
            int rowsAffected = command.ExecuteNonQuery();
            
            dbConnection.Close();

            return rowsAffected > 0;
        }
    }
}