using System;
using System.Data;
using System.Data.SqlClient;


namespace BookingSystem.Models
{
	public class Database
	{
		private readonly string _connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING"); 
		public SqlConnection dbConnection;

        public Database()
		{
            dbConnection = new SqlConnection(_connectionString);
        }

    }
}

