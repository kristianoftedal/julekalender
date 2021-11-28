﻿using System.Data;
using Microsoft.Data.SqlClient;

namespace ChristmasCalendar.Data.Dapper
{
    public class DbConnectionFactory
    {
        private readonly string _connectionString;

        public DbConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDbConnection Connection => new SqlConnection(_connectionString);
    }
}
