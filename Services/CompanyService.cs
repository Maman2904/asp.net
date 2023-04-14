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
  public class CompanyService : ITempCompanyRelation
  {
    private readonly Helpers.AppSettings _appSettings;
    public CompanyService(IOptions<AppSettings> appSettings)
    {
      _appSettings = appSettings.Value;
    }

    public IEnumerable<TempCompanyRelationModel> GetApiCompany(int vendor_id)
    {
      List<TempCompanyRelationModel> result = new List<TempCompanyRelationModel>();
      try
      {
        using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
        {
          var sql = "SELECT company_relation_id, company_id, purchase_organization_id FROM temp_company_relations WHERE vendor_id = @vendor_id ORDER BY created_at ASC";

          SqlCommand command = new SqlCommand(sql, cs);
          command.Parameters.AddWithValue("@vendor_id", vendor_id);
          cs.Open();
          var reader = command.ExecuteReader();

          if (reader.HasRows)
          {
            while (reader.Read())
            {
              result.Add(new TempCompanyRelationModel()
              {
                company_relation_id = Convert.ToInt32(reader["company_relation_id"].ToString()),
                company_id = reader["company_id"].ToString(),
                purchase_organization_id = reader["purchase_organization_id"] == DBNull.Value ? null : reader["purchase_organization_id"].ToString()
              });
            }
          }
          cs.Close();
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        new Helpers.GlobalFunction().LogError("GetApiCompany", ex.Message);
      }
      return result;
    }



  }

}