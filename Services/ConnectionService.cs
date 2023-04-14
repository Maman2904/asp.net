using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using tufol.Helpers;

namespace tufol.Services
{
    public class ConnectionService
    {
        private readonly Helpers.AppSettings _appSettings;
        public ConnectionService() {}
        public ConnectionService(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public SqlConnection GetConnection()
        {
            return new SqlConnection(_appSettings.ConnectionString);
        }

        public DbCommand GetCommand()
        {
            return new SqlCommand();
        }

        public SqlParameter GetParameter(string parameter, object value)
        {
            SqlParameter parameterObject = new SqlParameter(parameter, value != null ? value : DBNull.Value);
            parameterObject.Direction = ParameterDirection.Input;
            return parameterObject;
        }
    }
}