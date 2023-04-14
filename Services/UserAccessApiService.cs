using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tufol.Interfaces;
using tufol.Helpers;
using tufol.Models;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;

namespace tufol.Services
{
  public class UserAccessApiService : IUserAccessApi
  {
    private readonly Helpers.AppSettings _appSettings;
    public UserAccessApiService(IOptions<AppSettings> appSettings)
    {
      _appSettings = appSettings.Value;
    }
    public UserAccessApiModel GetUserDetail(string access_token)
    {
      UserAccessApiModel result = new UserAccessApiModel();
      try
      {
        using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
        {
          var sql = "SELECT " +
                    "A.username, A.access_token, " +
                    "A.created_at, A.updated_at " +
                    "FROM tb_user_access_api A " +
                    "WHERE A.access_token = @access_token ";

          SqlCommand command = new SqlCommand(sql, cs);
          command.Parameters.AddWithValue("@access_token ", access_token);

          cs.Open();
          var reader = command.ExecuteReader();
          if (reader.HasRows)
          {
            while (reader.Read())
            {
              result.username = reader["username"].ToString();
              result.access_token = reader["access_token"].ToString();
              result.created_at = Convert.ToDateTime(reader["created_at"].ToString());
              result.updated_at = Convert.ToDateTime(reader["updated_at"].ToString());
            }
          }
          cs.Close();
        }

      }
      catch (Exception ex)
      {
        new Helpers.GlobalFunction().LogError("GetUserDetail", ex.Message);
      }
      return result;
    }
  }
}