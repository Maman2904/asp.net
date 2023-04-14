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
    public class MasterService : IMaster
    {
        private readonly Helpers.AppSettings _appSettings;
        public MasterService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public IEnumerable<PurchaseOrganizationModel> GetPurchaseOrganizationGroup(string company_id, int vendor_type_id)
        {
            List<PurchaseOrganizationModel> result = new List<PurchaseOrganizationModel>();
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "SELECT B.purchase_organization_id, B.name FROM m_purchase_organization_group A " +
                              "JOIN m_purchase_organization B ON A.purchase_organization_id = B.purchase_organization_id " +
                              "WHERE A.company_id = @company_id AND A.vendor_type_id = @vendor_type_id";

                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@company_id", company_id);
                    command.Parameters.AddWithValue("@vendor_type_id", vendor_type_id);
                    cs.Open();
                    var reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result.Add(new PurchaseOrganizationModel()
                            {
                                purchase_organization_id = reader["purchase_organization_id"].ToString(),
                                name = reader["name"].ToString()
                            });
                        }
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("GetPurchaseOrganizationGroup", ex.Message);
            }
            return result;
        }

        public Dictionary<string, dynamic> GetAnnouncementList(string culture)
        {
            List<Dictionary<string, dynamic>> result = new List<Dictionary<string, dynamic>>();
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "SELECT * FROM m_announcement";

                    SqlCommand command = new SqlCommand(sql, cs);
                    cs.Open();
                    var reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Dictionary<string, dynamic> data_row = new Dictionary<string, dynamic>();
                            data_row.Add("key_flag",reader["key_flag"].ToString());
                            if(culture == "id") {
                                data_row.Add("title", reader["title"].ToString());
                                data_row.Add("content",reader["content"].ToString());
                            } else {
                                data_row.Add("title", reader["title_en"] == DBNull.Value ? null : reader["title_en"].ToString());
                                data_row.Add("content", reader["content_en"] == DBNull.Value ? null : reader["content_en"].ToString());
                            }
                            result.Add(data_row); 
                        }
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("GetAnnouncementList", ex.Message);
            }
            Dictionary<string, dynamic> ret = new Dictionary<string, dynamic>();
            var meta = new Dictionary<string, dynamic>();
            meta.Add("page", 1);
            ret.Add("meta", meta);
            ret.Add("data", result);
            return ret;
        }


        public AnnouncementModel GetAnnouncementDetail(string key_flag)
        {
            AnnouncementModel result = new AnnouncementModel();
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "SELECT * FROM m_announcement WHERE key_flag = @key_flag";

                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@key_flag", key_flag);
                    cs.Open();
                    var reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                                result.key_flag = reader["key_flag"].ToString();
                                result.title = reader["title"].ToString();
                                result.content = reader["content"].ToString();
                                result.title_en = reader["title_en"].ToString();
                                result.content_en = reader["content_en"].ToString();
                        }
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("errror bund: "+ex);
                new Helpers.GlobalFunction().LogError("Get announcement detail", ex.Message);
                
            }
            return result;
        }

        public bool UpdateAnnouncement(AnnouncementModel model, string key_flag) {
            var result = false;
            try {
                using(SqlConnection cs =  new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "UPDATE m_announcement SET ";
                    if(!String.IsNullOrEmpty(model.title) || !String.IsNullOrEmpty(model.content))
                        sql += "title=@title, content=@content ";
                    if(!String.IsNullOrEmpty(model.title_en) || !String.IsNullOrEmpty(model.content_en))
                        sql +="title_en=@title_en, content_en=@content_en ";
                    sql += "WHERE key_flag=@key_flag"; 
                    Console.WriteLine(sql);
                    SqlCommand command = new SqlCommand(sql, cs);
                    if(!String.IsNullOrEmpty(model.title) || !String.IsNullOrEmpty(model.content)) {
                        command.Parameters.AddWithValue("@title", model.title);
                        command.Parameters.AddWithValue("@content", model.content);
                    }
                    if(!String.IsNullOrEmpty(model.title_en) || !String.IsNullOrEmpty(model.content_en)) {
                        command.Parameters.AddWithValue("@title_en", model.title_en);
                        command.Parameters.AddWithValue("@content_en", model.content_en);
                    }
                    command.Parameters.AddWithValue("@key_flag", key_flag);
                    cs.Open();

                    result = command.ExecuteNonQuery() == 1;

                    if(result)
                    {
                        result = true;
                    } else {
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("error: "+ex);
                new Helpers.GlobalFunction().LogError("Update announcement", ex.Message);
            }
            return result;
        }


        public ProcurementGroupModel GetProcurementGroupByID(string initial_area)
        {
            ProcurementGroupModel result = new ProcurementGroupModel();
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "SELECT email FROM tb_procurement_group WHERE initial_area = @initial_area";

                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@initial_area", initial_area);
                    cs.Open();
                    var reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result.email = reader["email"].ToString();
                        }
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("GetProcurementGroupByID", ex.Message);
            }
            return result;
        }
        public CountryTaxModel GetCountryTax(string country_id, string? area_code)
        {
            CountryTaxModel result = new CountryTaxModel();
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "SELECT tax FROM m_country_tax WHERE country_id = @country_id";
                    if(!String.IsNullOrEmpty(area_code))
                        sql += " AND area = @area_code";

                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@country_id", country_id);
                    if(!String.IsNullOrEmpty(area_code))
                        command.Parameters.AddWithValue("@area_code", area_code);
                    cs.Open();
                    var reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result.tax = Convert.ToInt32(reader["tax"].ToString());
                        }
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("GetCountryTax", ex.Message);
            }
            return result;
        }
        public IEnumerable<CompanyAreaModel> GetCompanyArea(string company_id)
        {
            List<CompanyAreaModel> result = new List<CompanyAreaModel>();
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "SELECT B.code, B.company_id, B.name, B.address "+
                              "FROM m_company A "+
                              "JOIN m_company_area B ON A.company_id = B.company_id " +
                              "AND A.company_id = @company_id";
                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@company_id", company_id);
                    cs.Open();
                    var reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result.Add(new CompanyAreaModel() {
                                code = reader["code"].ToString(),
                                company_id = reader["company_id"].ToString(),
                                name = reader["name"].ToString(),
                                address = reader["address"] == DBNull.Value ? null : reader["address"].ToString(),
                            });
                        }
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("GetCompanyArea", ex.Message);
            }
            return result;
        }
        public IEnumerable<CompanyAreaModel> GetCompanyAreaCompany() {
            List<CompanyAreaModel> result = new List<CompanyAreaModel>();
            try {
                using(SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "select CONCAT(UPPER(b.name),' ', UPPER(a.name)) AS company_area_name, code, address from m_company_area a join m_company b on a.company_id = b.company_id";
                    var command = new ConnectionService().GetCommand();
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandText = sql;
                    command.Connection = cs;
                    cs.Open();

                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while(reader.Read())
                        {
                            result.Add(new CompanyAreaModel() 
                            {
                                code = reader["code"].ToString(),
                                company_area_name = reader["company_area_name"].ToString(),
                                address = reader["address"].ToString()
                            });
                        }
                    }
                    cs.Close();
                }
            }             
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("GetCompanyAreaCompany", ex.Message);
                Console.WriteLine(ex);
            }
            return result;
        }
        public string GetBussinessArea(string company_id, string area_code = null)
        {
            string result = "";
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "SELECT buss_area_code "+
                              "FROM m_company_area " +
                              "WHERE company_id = @company_id";
                    if(!String.IsNullOrEmpty(area_code))
                        sql += " AND code = @area_code";
                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@company_id", company_id);
                    if(!String.IsNullOrEmpty(area_code))
                        command.Parameters.AddWithValue("@area_code", area_code);
                    cs.Open();
                    var reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result = reader["buss_area_code"].ToString();
                        }
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("GetBussinessArea", ex.Message);
            }
            return result;
        }
        
        // public IEnumerable<RejectReasonModel> GetRejectReasons() {
        //     List<RejectReasonModel> result = new List<RejectReasonModel>();
        //     try {
        //         using(SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
        //         {
        //             var sql = "SELECT * FROM m_reject_reason";
        //             var command = new ConnectionService().GetCommand();
        //             command.CommandType = System.Data.CommandType.Text;
        //             command.CommandText = sql;
        //             command.Connection = cs;
        //             cs.Open();

        //             var reader = command.ExecuteReader();
        //             if (reader.HasRows)
        //             {
        //                 while(reader.Read())
        //                 {
        //                     result.Add(new RejectReasonModel() 
        //                     {
        //                         reason_code= reader["reason_code"].ToString(),
        //                         text_en = reader["text_en"].ToString(),
        //                         text_id = reader["text_id"].ToString()
        //                     });
        //                 }
        //             }
        //             cs.Close();
        //         }
        //     }             
        //     catch (Exception ex)
        //     {
        //         new Helpers.GlobalFunction().LogError("RoleList", ex.Message);
        //         Console.WriteLine(ex);
        //     }
        //     return result;
        // }
        public IEnumerable<PoTypeGroupModel> GetPoTypeGroup() {
            List<PoTypeGroupModel> result = new List<PoTypeGroupModel>();
            try {
                using(SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "SELECT * FROM m_po_type_group";
                    var command = new ConnectionService().GetCommand();
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandText = sql;
                    command.Connection = cs;
                    cs.Open();

                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while(reader.Read())
                        {
                            result.Add(new PoTypeGroupModel() 
                            {
                                po_type_group_id = Convert.ToInt32(reader["po_type_group_id"].ToString()),
                                name = reader["name"].ToString()
                            });
                        }
                    }
                    cs.Close();
                }
            }             
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("GetPoTypeGroup", ex.Message);
                Console.WriteLine(ex);
            }
            return result;
        }
    }
}
