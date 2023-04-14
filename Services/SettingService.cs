using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tufol.Interfaces;
using tufol.Models;
using tufol.Helpers;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace tufol.Services
{
    public class SettingService : ISetting
    {
        private readonly Helpers.AppSettings _appSettings;
        public SettingService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public string GetSettingValue(string kode)
        {
            string result = null;
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "SELECT value FROM tb_setting WHERE kode = @kode";

                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@kode", kode);
                    cs.Open();
                    var reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result = reader["value"].ToString();
                        }
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                new Helpers.GlobalFunction().LogError("GetSettingValue", ex.Message);
            }
            return result;
        }
    }
}
