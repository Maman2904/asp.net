using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using tufol.Interfaces;
using tufol.Models;
using tufol.Helpers;
using Microsoft.Data.SqlClient;
using System.Text.Json;
using System.Reflection;
using System.Collections;
using Dapper;

namespace tufol.Services
{
    public class VendorService : IVendor
    {
        private readonly Helpers.AppSettings _appSettings;
        private ITempCompanyRelation _tempCompanyRelation;
        public VendorService(IOptions<AppSettings> appSettings, ITempCompanyRelation tempCompanyRelation)
        {
          _appSettings = appSettings.Value;
          _tempCompanyRelation = tempCompanyRelation;
        }

        public IEnumerable<CompanyModel> GetCompany(string vendor_number)
        {
            List<CompanyModel> result = new List<CompanyModel>();
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "SELECT A.purchase_organization_id, C.name AS purchase_organization_name, B.* " +
                              "FROM tr_company_relations A " +
                              "JOIN m_company B ON A.company_id = B.company_id " +
                              "LEFT JOIN m_purchase_organization C ON A.purchase_organization_id = C.purchase_organization_id " +
                              "WHERE A.vendor_number = '" + vendor_number + "'";
                    var command = new ConnectionService().GetCommand();
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandText = sql;
                    command.Connection = cs;
                    cs.Open();

                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result.Add(new CompanyModel()
                            {
                                purchase_organization_id = reader["purchase_organization_id"] == DBNull.Value ? null : reader["purchase_organization_id"].ToString(),
                                purchase_organization_name = reader["purchase_organization_name"] == DBNull.Value ? null : reader["purchase_organization_name"].ToString(),
                                company_id = reader["company_id"].ToString(),
                                name = reader["name"].ToString(),
                                created_at = Convert.ToDateTime(reader["created_at"].ToString()),
                                updated_at = Convert.ToDateTime(reader["updated_at"].ToString())
                            });
                        }
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("GetCompany", ex.Message);
            }
            return result;
        }

        public IEnumerable<CompanyModel> GetTempCompany(int vendor_id)
        {
            List<CompanyModel> result = new List<CompanyModel>();
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "SELECT A.purchase_organization_id, C.name AS purchase_organization_name, B.* " +
                              "FROM temp_company_relations A " +
                              "JOIN m_company B ON A.company_id = B.company_id " +
                              "LEFT JOIN m_purchase_organization C ON A.purchase_organization_id = C.purchase_organization_id " +
                              "WHERE A.vendor_id = @vendor_id";
                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@vendor_id", vendor_id);
                    cs.Open();

                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result.Add(new CompanyModel()
                            {
                                purchase_organization_id = reader["purchase_organization_id"] == DBNull.Value ? null : reader["purchase_organization_id"].ToString(),
                                purchase_organization_name = reader["purchase_organization_name"] == DBNull.Value ? null : reader["purchase_organization_name"].ToString(),
                                company_id = reader["company_id"].ToString(),
                                name = reader["name"].ToString(),
                                created_at = Convert.ToDateTime(reader["created_at"].ToString()),
                                updated_at = Convert.ToDateTime(reader["updated_at"].ToString())
                            });
                        }
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("GetTempCompany", ex.Message);
            }
            return result;
        }

        public IEnumerable<LogVendorModel> GetLogVendor(string vendor_number)
        {
            List<LogVendorModel> result = new List<LogVendorModel>();
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "SELECT A.*, B.username AS created_by_name " +
                              "FROM log_vendor A " +
                              "LEFT JOIN tb_user B ON A.created_by = B.user_id " +
                              "WHERE A.vendor_number = '" + vendor_number +"'";
                    var command = new ConnectionService().GetCommand();
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandText = sql;
                    command.Connection = cs;
                    cs.Open();

                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result.Add(new LogVendorModel()
                            {
                                log_id = reader["log_id"].ToString(),
                                vendor_number = reader["vendor_number"].ToString(),
                                process = reader["process"].ToString(),
                                note = reader["note"] == DBNull.Value ? "-" : reader["note"].ToString(),
                                created_by = Convert.ToInt32(reader["created_by"].ToString()),  
                                created_by_name = reader["created_by_name"].ToString(),  
                                created_at = Convert.ToDateTime(reader["created_at"].ToString()),  
                                updated_at = Convert.ToDateTime(reader["updated_at"].ToString())                            
                            });                            
                        }
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("GetLogVendor", ex.Message);
            }
            return result;
        }

        public IEnumerable<TempLogVendorStatusModel> GetTempLogVendor(int vendor_id)
        {
            List<TempLogVendorStatusModel> result = new List<TempLogVendorStatusModel>();
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "SELECT A.*, B.username AS created_by_name, C.label_for_vendor AS verification_status " +
                              "FROM temp_log_vendor_status A " +
                              "LEFT JOIN tb_user B ON A.created_by = B.user_id " +
                              "LEFT JOIN m_verification_status C ON A.verification_status_id = C.verification_status_id " +
                              "WHERE A.vendor_id = '" + vendor_id + "'";
                    var command = new ConnectionService().GetCommand();
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandText = sql;
                    command.Connection = cs;
                    cs.Open();

                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result.Add(new TempLogVendorStatusModel()
                            {
                                log_id = reader["log_id"].ToString(),
                                vendor_id = Convert.ToInt32(reader["vendor_id"].ToString()),
                                verification_status_id = Convert.ToInt32(reader["verification_status_id"].ToString()),
                                verification_status = reader["verification_status"].ToString(),
                                verification_note = reader["verification_note"] == DBNull.Value ? "-" : reader["verification_note"].ToString(),
                                created_by = reader["created_by"] == DBNull.Value ? null : Convert.ToInt32(reader["created_by"].ToString()),
                                created_by_name = reader["created_by_name"] == DBNull.Value ? "-" : reader["created_by_name"].ToString(),
                                created_at = Convert.ToDateTime(reader["created_at"].ToString()),
                                updated_at = Convert.ToDateTime(reader["updated_at"].ToString())
                            });
                        }
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("GetTempLogVendor", ex.Message);
            }
            return result;
        }

        public int CountVendorData(string vendor_number = null, string name = null, int vendor_type_id = 0, string generalSearch = null) {
            int result = 0;
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    string string_condition = "";
                    if (!string.IsNullOrEmpty(vendor_number))
                    {
                        string_condition += " AND A.vendor_number = @vendor_number";
                    }
                    if (!string.IsNullOrEmpty(name))
                    {
                        string_condition += " AND A.name LIKE @name";
                    }
                    if (!string.IsNullOrEmpty(vendor_type_id.ToString()) && vendor_type_id != 0)
                    {
                        string_condition += " AND A.vendor_type_id = @vendor_type_id";
                    }
                    if (!string.IsNullOrEmpty(generalSearch)) {
                        string_condition += " AND A.name LIKE @generalSearch OR A.vendor_type LIKE @generalSearch";
                    }

                    var sql = "SELECT COUNT(*) AS total " +
                              "FROM tr_vendor A WHERE 1=1 " + string_condition;

                    SqlCommand command = new SqlCommand(sql, cs);

                    if (!string.IsNullOrEmpty(vendor_type_id.ToString()) && vendor_type_id != 0)
                    {
                        command.Parameters.AddWithValue("@vendor_type_id", vendor_type_id);
                    }
                    if (!string.IsNullOrEmpty(vendor_number))
                    {
                        command.Parameters.AddWithValue("@vendor_number", vendor_number);
                    }
                    if (!string.IsNullOrEmpty(name))
                    {
                        command.Parameters.AddWithValue("@name", "%" + name + "%");
                    }
                    if (!string.IsNullOrEmpty(generalSearch))
                    {
                        command.Parameters.AddWithValue("@generalSearch", "%" + generalSearch + "%");
                    }

                    cs.Open();
                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result = Convert.ToInt32(reader["total"].ToString());
                        }
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("CountVendorData", ex.Message);
            }
            return result;
        }

        public Dictionary<string, dynamic> GetVendorListDatatable(
            Dictionary<string, string> pagination,
            Dictionary<string, string> sort,
            Dictionary<string, string> query,
            string vendor_number = null, string name = null, int vendor_type_id = 0
        )
        {
            int page = pagination.ContainsKey("page") ? Convert.ToInt32(pagination["page"]): 1;
            int perpage = pagination.ContainsKey("perpage") ? Convert.ToInt32(pagination["perpage"]): 10;
            string sorttype = sort.ContainsKey("sort") ? string.IsNullOrEmpty(sort["sort"]) ? "desc" : sort["sort"]: "desc"; 
            string orderby = sort.ContainsKey("field") ? (string.IsNullOrEmpty(sort["field"]) ? "A.created_at" : sort["field"] == "vendor_type_name" ? "B.name" : "A." + sort["field"]) : "A.created_at";
            string generalSearch = query.ContainsKey("generalSearch") ? string.IsNullOrEmpty(query["generalSearch"]) ? null : query["generalSearch"] : null;

            Console.WriteLine("gen"+generalSearch);
            List<Dictionary<string, dynamic>> result = new List<Dictionary<string, dynamic>>();
            int total_data = 0;
            int total_page = 0;
            int offset = (page - 1)*perpage;
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    string string_condition = "";
                    if (!string.IsNullOrEmpty(vendor_number)) {
                        string_condition += " AND A.vendor_number = @vendor_number";
                    }
                    if (!string.IsNullOrEmpty(name)) {
                        string_condition += " AND A.name LIKE @name";
                    }
                    if (!string.IsNullOrEmpty(vendor_type_id.ToString()) && vendor_type_id != 0) {
                        string_condition += " AND A.vendor_type_id = @vendor_type_id";
                    }
                    if (!string.IsNullOrEmpty(generalSearch)) {

                        string_condition += " AND A.name LIKE @generalSearch";
                    }
                    string string_order = " ORDER BY " + orderby + " " + sorttype;
                    var sql = "SELECT " +
                              "A.vendor_number, " +
                              "A.vendor_type_id, B.name AS vendor_type_name, B.sap_code, B.scheme_group_id, B.gl_id, " +
                              "A.name, " +
                              "A.street_address, A.additional_street_address, A.city, A.postal_code, " +
                              "C.name AS country_name, " +
                              "A.telephone, A.pic_id " +
                              "FROM tr_vendor A " +
                              "JOIN m_vendor_type B ON A.vendor_type_id = B.vendor_type_id " +
                              "JOIN m_country C ON A.country_id = C.country_id " +
                              "WHERE 1=1 " + string_condition + " " + string_order + " " +
                              "OFFSET @offset ROWS FETCH NEXT @perpage ROWS ONLY";
                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@offset", offset);
                    command.Parameters.AddWithValue("@perpage", perpage);
                    if (!string.IsNullOrEmpty(vendor_type_id.ToString()) && vendor_type_id != 0)
                    {
                        command.Parameters.AddWithValue("@vendor_type_id", vendor_type_id);
                    }
                    if (!string.IsNullOrEmpty(vendor_number))
                    {
                        command.Parameters.AddWithValue("@vendor_number", vendor_number);
                    }
                    if (!string.IsNullOrEmpty(name))
                    {
                        command.Parameters.AddWithValue("@name", "%" + name + "%");
                    }
                    if (!string.IsNullOrEmpty(generalSearch))
                    {
                        command.Parameters.AddWithValue("@generalSearch", "%" + generalSearch + "%");
                    }
                    
                    cs.Open();
                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Dictionary<string, dynamic> data_row = new Dictionary<string, dynamic>();
                            data_row.Add("vendor_number", reader["vendor_number"].ToString());
                            data_row.Add("pic_id", reader["pic_id"].ToString());
                            data_row.Add("vendor_type_name", reader["vendor_type_name"].ToString());
                            data_row.Add("name", reader["name"].ToString());
                            data_row.Add("street_address", reader["street_address"].ToString());
                            data_row.Add("additional_street_address", reader["additional_street_address"].ToString());
                            data_row.Add("city", reader["city"].ToString());
                            data_row.Add("postal_code", reader["postal_code"].ToString());
                            data_row.Add("country_name", reader["country_name"].ToString());
                            data_row.Add("telephone", reader["telephone"].ToString());
                            result.Add(data_row);                            
                        }
                    }
                    cs.Close();
                }
                total_data = this.CountVendorData(vendor_number, name, vendor_type_id, generalSearch);
                total_page = (total_data + perpage - 1) / perpage;
                
            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("GetVendorListDatatable", ex.Message);
                Console.WriteLine(ex.Message);
            }
            Dictionary<string, dynamic> ret = new Dictionary<string, dynamic>();
            var meta = new Dictionary<string, dynamic>();
            meta.Add("page", page);
            meta.Add("pages", total_page);
            meta.Add("perpage", perpage);
            meta.Add("total", total_data);
            meta.Add("sort", sorttype);
            meta.Add("generalSearch", generalSearch);
            meta.Add("field", sort.ContainsKey("field") ? sort["field"]: null);

            ret.Add("meta", meta);
            ret.Add("data", result);
            return ret;
        }

        public int CountPendingVendorData(int position_data, string type, string vendor_number = null, string name = null, int vendor_type_id = 0, string generalSearch = null)
        {
            int result = 0;
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    string string_condition = "";
                    if (!string.IsNullOrEmpty(vendor_number))
                    {
                        string_condition += " AND A.vendor_number = @vendor_number";
                    }
                    if (!string.IsNullOrEmpty(name))
                    {
                        string_condition += " AND A.name LIKE @name";
                    }
                    if (!string.IsNullOrEmpty(vendor_type_id.ToString()) && vendor_type_id != 0)
                    {
                        string_condition += " AND A.vendor_type_id = @vendor_type_id";
                    }
                    if (!string.IsNullOrEmpty(generalSearch)) {
                        string_condition += " AND A.name LIKE @generalSearch";
                    }

                    var sql = "SELECT COUNT(*) AS total " +
                              "FROM temp_vendor A WHERE A.position_data = @position_data AND A.type = @type " + string_condition;
                    
                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@position_data", position_data);
                    command.Parameters.AddWithValue("@type", type);

                    if (!string.IsNullOrEmpty(vendor_type_id.ToString()) && vendor_type_id != 0)
                    {
                        command.Parameters.AddWithValue("@vendor_type_id", vendor_type_id);
                    }
                    if (!string.IsNullOrEmpty(vendor_number))
                    {
                        command.Parameters.AddWithValue("@vendor_number", vendor_number);
                    }
                    if (!string.IsNullOrEmpty(name))
                    {
                        command.Parameters.AddWithValue("@name", "%" + name + "%");
                    }
                     if (!string.IsNullOrEmpty(generalSearch))
                    {
                        command.Parameters.AddWithValue("@generalSearch", "%" + generalSearch + "%");
                    }

                    cs.Open();

                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result = Convert.ToInt32(reader["total"].ToString());
                        }
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("CountPendingVendorData", ex.Message);
            }
            return result;
        }

        public Dictionary<string, dynamic> GetPendingVendorDatatable(
            int position_data, string type,
            Dictionary<string, string> pagination,
            Dictionary<string, string> sort,
            Dictionary<string, string> query,
            string vendor_number = null, string name = null, int vendor_type_id = 0
        )
        {
            int page = pagination.ContainsKey("page") ? Convert.ToInt32(pagination["page"]) : 1;
            int perpage = pagination.ContainsKey("perpage") ? Convert.ToInt32(pagination["perpage"]) : 10;
            string sorttype = sort.ContainsKey("sort") ? string.IsNullOrEmpty(sort["sort"]) ? "asc" : sort["sort"] : "asc";
            string orderby = sort.ContainsKey("field") ? (string.IsNullOrEmpty(sort["field"]) ? "A.created_at" : sort["field"] == "vendor_type_name" ? "B.name" : "A." + sort["field"]) : "A.created_at";
            string generalSearch = query.ContainsKey("generalSearch") ? string.IsNullOrEmpty(query["generalSearch"]) ? null : query["generalSearch"] : null;
             
            List<Dictionary<string, dynamic>> result = new List<Dictionary<string, dynamic>>();
            int total_data = 0;
            int total_page = 0;
            int offset = (page - 1) * perpage;
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    string string_condition = "";
                    if (!string.IsNullOrEmpty(vendor_number))
                    {
                        string_condition += " AND A.vendor_number = @vendor_number";
                    }
                    if (!string.IsNullOrEmpty(name))
                    {
                        string_condition += " AND A.name LIKE @name";
                    }
                    if (!string.IsNullOrEmpty(vendor_type_id.ToString()) && vendor_type_id != 0)
                    {
                        string_condition += " AND A.vendor_type_id = @vendor_type_id";
                    }
                    if (!string.IsNullOrEmpty(generalSearch)) {

                        string_condition += " AND A.name LIKE @generalSearch";
                    }
                    string string_order = " ORDER BY " + orderby + " " + sorttype;
                    var sql = "SELECT " +
                              "A.vendor_id, A.vendor_number, " +
                              "A.vendor_type_id, B.name AS vendor_type_name, B.sap_code, B.scheme_group_id, B.gl_id, " +
                              "A.name, " +
                              "A.street_address, A.additional_street_address, A.city, A.postal_code, " +
                              "C.name AS country_name, " +
                              "A.telephone, A.pic_id, A.is_extension " +
                              "FROM temp_vendor A " +
                              "JOIN m_vendor_type B ON A.vendor_type_id = B.vendor_type_id " +
                              "JOIN m_country C ON A.country_id = C.country_id " +
                              "WHERE A.position_data = @position_data AND A.type = @type " + string_condition + " " + string_order + " " +
                              "OFFSET @offset ROWS FETCH NEXT @perpage ROWS ONLY";

                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@offset", offset);
                    command.Parameters.AddWithValue("@perpage", perpage);
                    command.Parameters.AddWithValue("@position_data", position_data);
                    command.Parameters.AddWithValue("@type", type);
                    if (!string.IsNullOrEmpty(vendor_type_id.ToString()) && vendor_type_id != 0)
                    {
                        command.Parameters.AddWithValue("@vendor_type_id", vendor_type_id);
                    }
                    if (!string.IsNullOrEmpty(vendor_number))
                    {
                        command.Parameters.AddWithValue("@vendor_number", vendor_number);
                    }
                    if (!string.IsNullOrEmpty(name))
                    {
                        command.Parameters.AddWithValue("@name", "%" + name + "%");
                    }
                    if (!string.IsNullOrEmpty(generalSearch))
                    {
                        command.Parameters.AddWithValue("@generalSearch", "%" + generalSearch + "%");
                    }

                    cs.Open();
                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Dictionary<string, dynamic> data_row = new Dictionary<string, dynamic>();
                            data_row.Add("vendor_id", reader["vendor_id"].ToString());
                            data_row.Add("vendor_number", reader["vendor_number"].ToString());
                            data_row.Add("pic_id", reader["pic_id"].ToString());
                            data_row.Add("vendor_type_name", reader["vendor_type_name"].ToString());
                            data_row.Add("name", reader["name"].ToString());
                            data_row.Add("street_address", reader["street_address"].ToString());
                            data_row.Add("additional_street_address", reader["additional_street_address"].ToString());
                            data_row.Add("city", reader["city"].ToString());
                            data_row.Add("postal_code", reader["postal_code"].ToString());
                            data_row.Add("country_name", reader["country_name"].ToString());
                            data_row.Add("telephone", reader["telephone"].ToString());
                            data_row.Add("is_extension", reader["is_extension"].ToString() == "0" ? false: true);
                            result.Add(data_row);
                        }
                    }
                    cs.Close();
                }
                total_data = this.CountPendingVendorData(position_data, type, vendor_number, name, vendor_type_id, generalSearch);
                total_page = (total_data + perpage - 1) / perpage;

            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("GetPendingVendorDatatable", ex.Message);
                Console.WriteLine(ex.Message);
            }
            Dictionary<string, dynamic> ret = new Dictionary<string, dynamic>();
            var meta = new Dictionary<string, dynamic>();
            meta.Add("page", page);
            meta.Add("pages", total_page);
            meta.Add("perpage", perpage);
            meta.Add("total", total_data);
            meta.Add("sort", sorttype);
            meta.Add("generalSearch", generalSearch);
            meta.Add("field", sort.ContainsKey("field") ? sort["field"] : null);

            ret.Add("meta", meta);
            ret.Add("data", result);
            return ret;
        }

        public VendorModel GetVendorDetail(string vendor_number)
        {
            VendorModel result = new VendorModel();
            List<LogVendorModel> LogVendor = new List<LogVendorModel>();
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "SELECT " +
                              "A.vendor_number, " +
                              "A.vendor_type_id, B.name AS vendor_type_name, B.sap_code AS vendor_type_code, B.scheme_group_id, L.name AS scheme_group_name, B.gl_id, O.name AS gl_name, " +
                              "A.sg_category_id, C.name AS sg_category_name, " +
                              "A.currency_id, A.top_id, " +
                              "D.name AS top_name, D.interval AS top_interval, A.title_id, " +
                              "A.name, A.contact_person, A.search_term, " +
                              "A.street_address, A.additional_street_address, A.city, A.postal_code, " + 
                              "A.country_id, E.name AS country_name, " +
                              "A.telephone, A.fax, A.email, " +
                              "A.tax_type_id, F.name AS tax_type_name, F.wht_code, F.rates AS tax_rates, " +
                              "A.tax_number_type_id, M.sap_code AS tax_number_type_code, M.name AS tax_number_type_name, " +
                              "A.npwp, A.id_card_number, A.sppkp_number, " +
                              "A.localidr_bank_id, G.name AS localidr_bank_name, G.swift_code AS localidr_swift_code, " +
                              "A.localidr_bank_account, "+
                              "A.localidr_account_holder, " +
                              "A.localforex_bank_id, H.name AS localforex_bank_name, H.swift_code AS localforex_swift_code, " +
                              "A.localforex_bank_account, "+
                              "A.localforex_account_holder, "+
                              "A.localforex_currency_id, " +
                              "A.foreign_bank_country_id, J.name AS foreign_bank_country_name, " +
                              "A.foreign_bank_id, I.name AS foreign_bank_name, I.swift_code AS foreign_swift_code, " +
                              "A.foreign_bank_account, A.foreign_account_holder, A.foreign_currency_id, " +
                              "A.reference_correspondent, A.reference_country_origin, " +
                              "A.pic_id, K.person_number, K.email AS email_procurement_group, " +
                              "A.file_id_card, A.file_npwp, A.file_sppkp, A.file_vendor_statement, " +
                              "A.is_locked, A.change_request_status, A.change_request_note, A.partner_function, N.name AS partner_function_name, "+
                              "A.created_at, A.updated_at "+
                              ",1 as status_rpa, null as rpa_status_description " +
                              "FROM tr_vendor A " +
                              "LEFT JOIN m_vendor_type B ON A.vendor_type_id = B.vendor_type_id " +
                              "LEFT JOIN m_sg_categories C ON A.sg_category_id = C.sg_category_id " +
                              "LEFT JOIN m_top D ON A.top_id = D.top_id " +
                              "LEFT JOIN m_country E ON A.country_id = E.country_id " +
                              "LEFT JOIN m_tax_type F ON A.tax_type_id = F.tax_type_id " +
                              "LEFT JOIN m_tax_number_type M ON A.tax_number_type_id = M.tax_number_type_id " +
                              "LEFT JOIN m_bank G ON A.localidr_bank_id = G.bank_id " +
                              "LEFT JOIN m_bank H ON A.localforex_bank_id = H.bank_id " +
                              "LEFT JOIN m_bank I ON A.foreign_bank_id = I.bank_id " +
                              "LEFT JOIN m_country J ON A.foreign_bank_country_id = J.country_id " +
                              "LEFT JOIN tb_procurement_group K ON A.pic_id = K.initial_area " +
                              "LEFT JOIN m_scheme_group L ON B.scheme_group_id = L.scheme_group_id " +
                              "LEFT JOIN m_province N ON A.partner_function = N.province_id " +
                              "LEFT JOIN m_gl O ON B.gl_id = O.gl_id " +
                              "WHERE A.vendor_number = @vendor_number";
                    Console.WriteLine("sql get vendor detail:" + sql);
                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@vendor_number", vendor_number);
                    cs.Open();
                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result.pic_id = reader["pic_id"] == DBNull.Value ? null : reader["pic_id"].ToString();
                            result.company = this.GetCompany(reader["vendor_number"].ToString());
                            result.vendor_number = reader["vendor_number"].ToString();
                            result.vendor_type_id = Convert.ToInt32(reader["vendor_type_id"].ToString());
                            result.vendor_type_name = reader["vendor_type_name"].ToString();
                            result.vendor_type_code = reader["vendor_type_code"].ToString();
                            result.scheme_group = reader["scheme_group_id"].ToString();
                            result.scheme_group_name = reader["scheme_group_name"].ToString();
                            result.gl = reader["gl_id"].ToString();
                            result.gl_name = reader["gl_name"].ToString();
                            result.sg_category_id = reader["sg_category_id"] == DBNull.Value ? 0 : Convert.ToInt32(reader["sg_category_id"].ToString());
                            result.sg_category_name = reader["sg_category_name"].ToString();
                            result.currency_id = reader["currency_id"].ToString();
                            result.top_id = reader["top_id"].ToString();
                            result.top_name = reader["top_name"].ToString();
                            result.top_interval = Convert.ToInt32(reader["top_interval"].ToString());
                            result.title_id = reader["title_id"].ToString();
                            result.name = reader["name"].ToString();
                            result.contact_person = reader["contact_person"].ToString();
                            result.search_term = reader["search_term"].ToString();
                            result.street_address = reader["street_address"].ToString();
                            result.additional_street_address = reader["additional_street_address"] == DBNull.Value ? null : reader["additional_street_address"].ToString();
                            result.city = reader["city"].ToString();
                            result.postal_code = reader["postal_code"].ToString();
                            result.country_id = reader["country_id"].ToString();
                            result.account_group_id = new Helpers.GlobalFunction().GetAccountGroupId(result.vendor_type_id, result.country_id);
                            result.country_name = reader["country_name"].ToString();
                            result.telephone = reader["telephone"].ToString();
                            result.fax = reader["fax"].ToString();
                            result.email = reader["email"].ToString();
                            result.email_list = reader["email"] == DBNull.Value ? new List<String>() : new Helpers.EmailFunction().GetEmailList(reader["email"].ToString(), ";");
                            result.tax_type_id = Convert.ToInt32(reader["tax_type_id"].ToString());
                            result.tax_type_name = reader["tax_type_name"].ToString();
                            result.wht_code = reader["wht_code"].ToString();
                            result.tax_rates = reader["tax_rates"].ToString();
                            result.tax_number_type_id = Convert.ToInt32(reader["tax_number_type_id"].ToString());
                            result.tax_number_type_code = reader["tax_number_type_code"].ToString();
                            result.tax_number_type_name = reader["tax_number_type_name"].ToString();
                            result.npwp = reader["npwp"] == DBNull.Value ? null : reader["npwp"].ToString();
                            result.id_card_number = reader["id_card_number"] == DBNull.Value ? null : reader["id_card_number"].ToString();
                            result.sppkp_number = reader["sppkp_number"] == DBNull.Value ? null : reader["sppkp_number"].ToString();
                            result.localidr_bank_id = reader["localidr_bank_id"] == DBNull.Value ? null : reader["localidr_bank_id"].ToString();
                            result.localidr_bank_name = reader["localidr_bank_name"] == DBNull.Value ? null : reader["localidr_bank_name"].ToString();
                            result.localidr_swift_code = reader["localidr_swift_code"] == DBNull.Value ? null : reader["localidr_swift_code"].ToString();
                            result.localidr_bank_account = reader["localidr_bank_account"] == DBNull.Value ? null : Convert.ToDecimal(reader["localidr_bank_account"].ToString());
                            result.localidr_account_holder = reader["localidr_account_holder"] == DBNull.Value ? null : reader["localidr_account_holder"].ToString();
                            result.localforex_bank_id = reader["localforex_bank_id"] == DBNull.Value ? null : reader["localforex_bank_id"].ToString();
                            result.localforex_bank_name = reader["localforex_bank_name"] == DBNull.Value ? null : reader["localforex_bank_name"].ToString();
                            result.localforex_swift_code = reader["localforex_swift_code"] == DBNull.Value ? null : reader["localforex_swift_code"].ToString();
                            result.localforex_bank_account = reader["localforex_bank_account"] == DBNull.Value ? null : Convert.ToDecimal(reader["localforex_bank_account"]);
                            result.localforex_account_holder = reader["localforex_account_holder"] == DBNull.Value ? null : reader["localforex_account_holder"].ToString();
                            result.localforex_currency_id = reader["localforex_currency_id"] == DBNull.Value ? null : reader["localforex_currency_id"].ToString();
                            result.foreign_bank_country_id = reader["foreign_bank_country_id"] == DBNull.Value ? null : reader["foreign_bank_country_id"].ToString();
                            result.foreign_bank_country_name = reader["foreign_bank_country_name"] == DBNull.Value ? null : reader["foreign_bank_country_name"].ToString();
                            result.foreign_bank_id = reader["foreign_bank_id"] == DBNull.Value ? null : reader["foreign_bank_id"].ToString();
                            result.foreign_bank_name = reader["foreign_bank_name"] == DBNull.Value ? null : reader["foreign_bank_name"].ToString();
                            result.foreign_bank_swift_code = reader["foreign_swift_code"] == DBNull.Value ? null : reader["foreign_swift_code"].ToString();
                            result.foreign_bank_account = reader["foreign_bank_account"] == DBNull.Value ? null : reader["foreign_bank_account"].ToString();
                            result.foreign_account_holder = reader["foreign_account_holder"] == DBNull.Value ? null : reader["foreign_account_holder"].ToString();
                            result.foreign_currency_id = reader["foreign_currency_id"] == DBNull.Value ? null : reader["foreign_currency_id"].ToString();
                            result.reference_correspondent = reader["reference_correspondent"] == DBNull.Value ? null : reader["reference_correspondent"].ToString();
                            result.reference_country_origin = reader["reference_country_origin"] == DBNull.Value ? null : reader["reference_country_origin"].ToString();
                            result.person_number = reader["person_number"] == DBNull.Value ? null : reader["person_number"].ToString();
                            result.email_procurement_group = reader["email_procurement_group"] == DBNull.Value ? null : reader["email_procurement_group"].ToString();
                            result.file_id_card = reader["file_id_card"].ToString();
                            result.file_npwp = reader["file_npwp"] == DBNull.Value ? null : reader["file_npwp"].ToString();
                            result.file_sppkp = reader["file_sppkp"] == DBNull.Value ? null : reader["file_sppkp"].ToString();
                            result.file_vendor_statement = reader["file_vendor_statement"].ToString();
                            result.change_request_status = Convert.ToInt32(reader["change_request_status"].ToString());
                            result.change_request_note = reader["change_request_note"].ToString();
                            result.partner_function = reader["partner_function"] == DBNull.Value ? null : reader["partner_function"].ToString();
                            result.is_locked = reader["is_locked"].ToString() == "False" ? false : true;
                            result.created_at = Convert.ToDateTime(reader["created_at"].ToString());
                            result.updated_at = Convert.ToDateTime(reader["updated_at"].ToString());
                            result.log_vendor = this.GetLogVendor(reader["vendor_number"].ToString());
                            result.partner_function_name = reader["partner_function_name"] == DBNull.Value ? null : reader["partner_function_name"].ToString();
                            result.status_rpa = reader["status_rpa"].ToString() == "False" ? false : true;
                            result.rpa_status_description = reader["rpa_status_description"] == DBNull.Value ? null : reader["rpa_status_description"].ToString();
                        }
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("GetVendorDetail", ex.Message);
                Console.WriteLine("hallooo"+ ex);
            }
            // Console.WriteLine("Name"+result.foreign_bank_name);
            // Console.WriteLine("Tax type"+result.tax_type_id);
            // Console.WriteLine("idr bank name "+result.localidr_bank_name);
            return result;
        }

        public TempVendorModel GetLastDataChange(string vendor_number)
        {
            TempVendorModel result = new TempVendorModel();
            List<EmailModel> emailList = new List<EmailModel>();
            List<TempLogVendorStatusModel> LogVendor = new List<TempLogVendorStatusModel>();
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "SELECT " +
                              "A.vendor_id, A.vendor_number, " +
                              "A.vendor_type_id, B.name AS vendor_type_name, B.sap_code AS vendor_type_code, "+
                              "B.scheme_group_id, N.name AS scheme_group_name, B.gl_id, P.name AS gl_name, " +
                              "A.sg_category_id, C.name AS sg_category_name, " +
                              "A.currency_id, A.top_id, " +
                              "D.name AS top_name, A.title_id, " +
                              "A.name, A.contact_person, A.search_term, " +
                              "A.street_address, A.additional_street_address, A.city, A.postal_code, " +
                              "A.country_id, E.name AS country_name, " +
                              "A.telephone, A.fax, A.email, " +
                              "A.tax_type_id, F.name AS tax_type_name, F.wht_code, F.rates AS tax_rates, " +
                              "A.tax_number_type_id, M.sap_code AS tax_number_type_code, M.name AS tax_number_type_name, " +
                              "A.npwp, A.id_card_number, A.sppkp_number, " +
                              "A.localidr_bank_id, G.name AS localidr_bank_name, G.swift_code AS localidr_swift_code, " +
                              "A.localidr_bank_account, A.localidr_account_holder, " +
                              "A.localforex_bank_id, H.name AS localforex_bank_name, H.swift_code AS localforex_swift_code, " +
                              "A.localforex_bank_account, A.localforex_account_holder, A.localforex_currency_id, " +
                              "A.foreign_bank_country_id, J.name AS foreign_bank_country_name, " +
                              "A.foreign_bank_id, A.foreign_bank_name, A.foreign_bank_swift_code, " +
                              "A.foreign_bank_account, A.foreign_account_holder, A.foreign_currency_id, " +
                              "A.reference_correspondent, A.reference_country_origin, " +
                              "A.pic_id, K.person_number, K.email AS email_procurement_group, " +
                              "A.file_id_card, A.file_npwp, A.file_sppkp, A.file_vendor_statement, " +
                              "A.verification_status_id, A.verification_note, L.label_for_vendor AS status_for_vendor, L.label_for_intern AS status_for_intern, " +
                              "A.token, A.verified_email, A.position_data, A.partner_function, O.name AS partner_function_name, A.created_at, A.updated_at, A.status_rpa, A.rpa_status_description " +
                              "FROM temp_vendor A " +
                              "LEFT JOIN m_vendor_type B ON A.vendor_type_id = B.vendor_type_id " +
                              "LEFT JOIN m_sg_categories C ON A.sg_category_id = C.sg_category_id " +
                              "LEFT JOIN m_top D ON A.top_id = D.top_id " +
                              "LEFT JOIN m_country E ON A.country_id = E.country_id " +
                              "LEFT JOIN m_tax_type F ON A.tax_type_id = F.tax_type_id " +
                              "LEFT JOIN m_tax_number_type M ON A.tax_number_type_id = M.tax_number_type_id " +
                              "LEFT JOIN m_bank G ON A.localidr_bank_id = G.bank_id " +
                              "LEFT JOIN m_bank H ON A.localforex_bank_id = H.bank_id " +
                              "LEFT JOIN m_country J ON A.foreign_bank_country_id = J.country_id " +
                              "LEFT JOIN tb_procurement_group K ON A.pic_id = K.initial_area " +
                              "LEFT JOIN m_verification_status L ON A.verification_status_id = L.verification_status_id " +
                              "LEFT JOIN m_scheme_group N ON B.scheme_group_id = N.scheme_group_id " +
                              "LEFT JOIN m_province O ON A.partner_function = O.province_id " +
                              "LEFT JOIN m_gl P ON B.gl_id = P.gl_id " +
                              "WHERE A.vendor_number = @vendor_number " +
                              "AND A.type = 'Vendor Data Change'";

                    SqlCommand command = new SqlCommand(sql, cs);
                    // Console.WriteLine("sql last data: "+sql);
                    command.Parameters.AddWithValue("@vendor_number", vendor_number);

                    cs.Open();
                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result.pic_id = reader["pic_id"] == DBNull.Value ? null : reader["pic_id"].ToString();
                            result.vendor_id = Convert.ToInt32(reader["vendor_id"].ToString());
                            result.vendor_number = reader["vendor_number"].ToString();
                            result.vendor_type_id = reader["vendor_type_id"] == DBNull.Value ? 0 : Convert.ToInt32(reader["vendor_type_id"].ToString());
                            result.vendor_type_name = reader["vendor_type_name"].ToString();
                            result.vendor_type_code = reader["vendor_type_code"].ToString();
                            result.scheme_group = reader["scheme_group_id"].ToString();
                            result.scheme_group_name = reader["scheme_group_name"].ToString();
                            result.gl = reader["gl_id"].ToString();
                            result.gl_name = reader["gl_name"].ToString();
                            result.sg_category_id = Convert.ToInt32(reader["sg_category_id"].ToString());
                            result.sg_category_name = reader["sg_category_name"].ToString();
                            result.currency_id = reader["currency_id"].ToString();
                            result.top_id = reader["top_id"].ToString();
                            result.top_name = reader["top_name"].ToString();
                            result.title_id = reader["title_id"].ToString();
                            result.name = reader["name"].ToString();
                            result.contact_person = reader["contact_person"].ToString();
                            result.search_term = reader["search_term"].ToString();
                            result.street_address = reader["street_address"].ToString();
                            result.additional_street_address = reader["additional_street_address"] == DBNull.Value ? null : reader["additional_street_address"].ToString();
                            result.city = reader["city"].ToString();
                            result.postal_code = reader["postal_code"].ToString();
                            result.country_id = reader["country_id"].ToString();
                            result.account_group_id = new Helpers.GlobalFunction().GetAccountGroupId(result.vendor_type_id, result.country_id);
                            result.country_name = reader["country_name"].ToString();
                            result.telephone = reader["telephone"].ToString();
                            result.fax = reader["fax"].ToString();
                            result.email = reader["email"].ToString();
                            result.email_list = reader["email"] == DBNull.Value ? new List<string>() : new Helpers.EmailFunction().GetEmailList(reader["email"].ToString(), ";");
                            result.tax_type_id = Convert.ToInt32(reader["tax_type_id"].ToString());
                            result.tax_type_name = reader["tax_type_name"].ToString();
                            result.wht_code = reader["wht_code"].ToString();
                            result.tax_rates = reader["tax_rates"].ToString();
                            result.tax_number_type_id = Convert.ToInt32(reader["tax_number_type_id"].ToString());
                            result.tax_number_type_code = reader["tax_number_type_code"].ToString();
                            result.tax_number_type_name = reader["tax_number_type_name"].ToString();
                            result.npwp = reader["npwp"] == DBNull.Value ? null : reader["npwp"].ToString();
                            result.id_card_number = reader["id_card_number"] == DBNull.Value ? null : reader["id_card_number"].ToString();
                            result.sppkp_number = reader["sppkp_number"] == DBNull.Value ? null : reader["sppkp_number"].ToString();
                            result.localidr_bank_id = reader["localidr_bank_id"] == DBNull.Value ? null : reader["localidr_bank_id"].ToString();
                            result.localidr_bank_name = reader["localidr_bank_name"] == DBNull.Value ? null : reader["localidr_bank_name"].ToString();
                            result.localidr_swift_code = reader["localidr_swift_code"] == DBNull.Value ? null : reader["localidr_swift_code"].ToString();
                            result.localidr_bank_account = reader["localidr_bank_account"] == DBNull.Value ? null : Convert.ToDecimal(reader["localidr_bank_account"]);
                            result.localidr_account_holder = reader["localidr_account_holder"] == DBNull.Value ? null : reader["localidr_account_holder"].ToString();
                            result.localforex_bank_id = reader["localforex_bank_id"] == DBNull.Value ? null : reader["localforex_bank_id"].ToString();
                            result.localforex_bank_name = reader["localforex_bank_name"] == DBNull.Value ? null : reader["localforex_bank_name"].ToString();
                            result.localforex_swift_code = reader["localforex_swift_code"] == DBNull.Value ? null : reader["localforex_swift_code"].ToString();
                            result.localforex_bank_account = reader["localforex_bank_account"] == DBNull.Value ? null : Convert.ToDecimal(reader["localforex_bank_account"]);
                            result.localforex_account_holder = reader["localforex_account_holder"] == DBNull.Value ? null : reader["localforex_account_holder"].ToString();
                            result.localforex_currency_id = reader["localforex_currency_id"] == DBNull.Value ? null : reader["localforex_currency_id"].ToString();
                            result.foreign_bank_country_id = reader["foreign_bank_country_id"] == DBNull.Value ? null : reader["foreign_bank_country_id"].ToString();
                            result.foreign_bank_country_name = reader["foreign_bank_country_name"] == DBNull.Value ? null : reader["foreign_bank_country_name"].ToString();
                            result.foreign_bank_id = reader["foreign_bank_id"] == DBNull.Value ? null : reader["foreign_bank_id"].ToString();
                            result.foreign_bank_name = reader["foreign_bank_name"] == DBNull.Value ? null : reader["foreign_bank_name"].ToString();
                            result.foreign_bank_swift_code = reader["foreign_bank_swift_code"] == DBNull.Value ? null : reader["foreign_bank_swift_code"].ToString();
                            result.foreign_bank_account = reader["foreign_bank_account"] == DBNull.Value ? null : reader["foreign_bank_account"].ToString();
                            result.foreign_account_holder = reader["foreign_account_holder"] == DBNull.Value ? null : reader["foreign_account_holder"].ToString();
                            result.foreign_currency_id = reader["foreign_currency_id"] == DBNull.Value ? null : reader["foreign_currency_id"].ToString();
                            result.reference_correspondent = reader["reference_correspondent"] == DBNull.Value ? null : reader["reference_correspondent"].ToString();
                            result.reference_country_origin = reader["reference_country_origin"] == DBNull.Value ? null : reader["reference_country_origin"].ToString();
                            result.person_number = reader["person_number"] == DBNull.Value ? null : reader["person_number"].ToString();
                            result.email_procurement_group = reader["email_procurement_group"] == DBNull.Value ? null : reader["email_procurement_group"].ToString();
                            result.file_id_card = reader["file_id_card"].ToString();
                            result.file_npwp = reader["file_npwp"] == DBNull.Value ? null : reader["file_npwp"].ToString();
                            result.file_sppkp = reader["file_sppkp"] == DBNull.Value ? null : reader["file_sppkp"].ToString();
                            result.file_vendor_statement = reader["file_vendor_statement"].ToString();
                            result.position_data = reader["position_data"] == DBNull.Value ? null : Convert.ToInt32(reader["position_data"].ToString());
                            result.created_at = Convert.ToDateTime(reader["created_at"].ToString());
                            result.updated_at = Convert.ToDateTime(reader["updated_at"].ToString());
                            result.company = this.GetTempCompany(Convert.ToInt32(reader["vendor_id"].ToString()));
                            result.temp_log_vendor_status = this.GetTempLogVendor(Convert.ToInt32(reader["vendor_id"].ToString()));
                            result.verification_status_id = Convert.ToInt32(reader["verification_status_id"].ToString());
                            result.status_for_vendor = reader["status_for_vendor"].ToString();
                            result.status_for_intern = reader["status_for_intern"].ToString();
                            result.verification_note = reader["verification_note"] == DBNull.Value ? null : reader["verification_note"].ToString();
                            result.verified_email = reader["verified_email"].ToString();
                            result.verified_email_list = reader["verified_email"] == DBNull.Value ? new List<string>() : new Helpers.EmailFunction().GetEmailList(reader["verified_email"].ToString(), ";");
                            result.account_group_id = new Helpers.GlobalFunction().GetAccountGroupId(result.vendor_type_id, result.country_id);
                            result.partner_function = reader["partner_function"] == DBNull.Value ? null : reader["partner_function"].ToString();
                            result.partner_function_name = reader["partner_function_name"] == DBNull.Value ? null : reader["partner_function_name"].ToString();
                            result.status_rpa = reader["status_rpa"].ToString() == "False" ? false : true;
                            result.rpa_status_description = reader["rpa_status_description"] == DBNull.Value ? null : reader["rpa_status_description"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("GetLastDataChange", ex.Message);
                Console.WriteLine("disiini", ex);
            }
            return result;
        }

        public TempVendorModel GetPendingVendorDetail(int vendor_id, string token = null)
        {
            TempVendorModel result = new TempVendorModel();
            List<EmailModel> emailList = new List<EmailModel>();
            List<TempLogVendorStatusModel> LogVendor = new List<TempLogVendorStatusModel>();
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "SELECT " +
                              "A.vendor_id, A.vendor_number, " +
                              "A.vendor_type_id, B.name AS vendor_type_name, B.sap_code AS vendor_type_code, "+
                              "B.scheme_group_id, N.name AS scheme_group_name, B.gl_id, P.name AS gl_name, " +
                              "A.sg_category_id, C.name AS sg_category_name, " +
                              "A.currency_id, A.top_id, " +
                              "D.name AS top_name, A.title_id, " +
                              "A.name, A.contact_person, A.search_term, " +
                              "A.street_address, A.additional_street_address, A.city, A.postal_code, " +
                              "A.country_id, E.name AS country_name, " +
                              "A.telephone, A.fax, A.email, " +
                              "A.tax_type_id, F.name AS tax_type_name, F.wht_code, F.rates AS tax_rates, " +
                              "A.tax_number_type_id, M.sap_code AS tax_number_type_code, M.name AS tax_number_type_name, " +
                              "A.npwp, A.id_card_number, A.sppkp_number, " +
                              "A.localidr_bank_id, G.name AS localidr_bank_name, G.swift_code AS localidr_swift_code, " +
                              "A.localidr_bank_account, A.localidr_account_holder, " +
                              "A.localforex_bank_id, H.name AS localforex_bank_name, H.swift_code AS localforex_swift_code, " +
                              "A.localforex_bank_account, A.localforex_account_holder, A.localforex_currency_id, " +
                              "A.foreign_bank_country_id, J.name AS foreign_bank_country_name, " +
                              "A.foreign_bank_id, A.foreign_bank_name, A.foreign_bank_swift_code, " +
                              "A.foreign_bank_account, A.foreign_account_holder, A.foreign_currency_id, " +
                              "A.reference_correspondent, A.reference_country_origin, " +
                              "A.pic_id, K.person_number, K.email AS email_procurement_group, " +
                              "A.file_id_card, A.file_npwp, A.file_sppkp, A.file_vendor_statement, " +
                              "A.updated_data, A.is_extension," +
                              "A.verification_status_id, A.verification_note, L.label_for_vendor AS status_for_vendor, L.label_for_intern AS status_for_intern, " +
                              "A.token, A.verified_email, A.position_data, A.partner_function, O.name AS partner_function_name, A.created_at, A.updated_at, A.status_rpa, A.rpa_status_description " +
                              "FROM temp_vendor A " +
                              "JOIN m_vendor_type B ON A.vendor_type_id = B.vendor_type_id " +
                              "LEFT JOIN m_sg_categories C ON A.sg_category_id = C.sg_category_id " +
                              "LEFT JOIN m_top D ON A.top_id = D.top_id " +
                              "JOIN m_country E ON A.country_id = E.country_id " +
                              "JOIN m_tax_type F ON A.tax_type_id = F.tax_type_id " +
                              "JOIN m_tax_number_type M ON A.tax_number_type_id = M.tax_number_type_id " +
                              "LEFT JOIN m_bank G ON A.localidr_bank_id = G.bank_id " +
                              "LEFT JOIN m_bank H ON A.localforex_bank_id = H.bank_id " +
                              "LEFT JOIN m_country J ON A.foreign_bank_country_id = J.country_id " +
                              "JOIN tb_procurement_group K ON A.pic_id = K.initial_area " +
                              "LEFT JOIN m_verification_status L ON A.verification_status_id = L.verification_status_id " +
                              "LEFT JOIN m_scheme_group N ON B.scheme_group_id = N.scheme_group_id " +
                              "LEFT JOIN m_province O ON A.partner_function = O.province_id " +
                              "LEFT JOIN m_gl P ON B.gl_id = P.gl_id " +
                              "WHERE A.vendor_id = @vendor_id " + (string.IsNullOrEmpty(token) ? null: " AND A.token = @token");

                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@vendor_id", vendor_id);
                    if ( !string.IsNullOrEmpty(token) )
                    {
                        command.Parameters.AddWithValue("@token", token);
                    }

                    cs.Open();
                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result.vendor_id = Convert.ToInt32(reader["vendor_id"].ToString());
                            result.vendor_number = reader["vendor_number"].ToString();
                            result.vendor_type_id = reader["vendor_type_id"] == DBNull.Value ? 0: Convert.ToInt32(reader["vendor_type_id"].ToString());
                            result.vendor_type_name = reader["vendor_type_name"].ToString();
                            result.vendor_type_code = reader["vendor_type_code"].ToString();
                            result.scheme_group = reader["scheme_group_id"].ToString();
                            result.scheme_group_name = reader["scheme_group_name"].ToString();
                            result.gl = reader["gl_id"].ToString();
                            result.gl_name = reader["gl_name"].ToString();
                            result.sg_category_id = Convert.ToInt32(reader["sg_category_id"].ToString());
                            result.sg_category_name = reader["sg_category_name"].ToString();
                            result.currency_id = reader["currency_id"].ToString();
                            result.top_id = reader["top_id"].ToString();
                            result.top_name = reader["top_name"].ToString();
                            result.title_id = reader["title_id"].ToString();
                            result.name = reader["name"].ToString();
                            result.contact_person = reader["contact_person"].ToString();
                            result.search_term = reader["search_term"].ToString();
                            result.street_address = reader["street_address"].ToString();
                            result.additional_street_address = reader["additional_street_address"] == DBNull.Value ? "-" : reader["additional_street_address"].ToString();
                            result.city = reader["city"].ToString();
                            result.postal_code = reader["postal_code"].ToString();
                            result.country_id = reader["country_id"].ToString();
                            result.account_group_id = new Helpers.GlobalFunction().GetAccountGroupId(result.vendor_type_id, result.country_id);
                            result.country_name = reader["country_name"].ToString();
                            result.telephone = reader["telephone"].ToString();
                            result.fax = reader["fax"].ToString();
                            result.email = reader["email"].ToString();
                            result.email_list = reader["email"] == DBNull.Value ? new List<string>() : new Helpers.EmailFunction().GetEmailList(reader["email"].ToString(), ";");
                            result.tax_type_id = Convert.ToInt32(reader["tax_type_id"].ToString());
                            result.tax_type_name = reader["tax_type_name"].ToString();
                            result.wht_code = reader["wht_code"].ToString();
                            result.tax_rates = reader["tax_rates"].ToString();
                            result.tax_number_type_id = Convert.ToInt32(reader["tax_number_type_id"].ToString());
                            result.tax_number_type_code = reader["tax_number_type_code"].ToString();
                            result.tax_number_type_name = reader["tax_number_type_name"].ToString();
                            result.npwp = reader["npwp"].ToString();
                            result.id_card_number = reader["id_card_number"].ToString();
                            result.sppkp_number = reader["sppkp_number"].ToString();
                            result.localidr_bank_id = reader["localidr_bank_id"].ToString();
                            result.localidr_bank_name = reader["localidr_bank_name"].ToString();
                            result.localidr_swift_code = reader["localidr_swift_code"].ToString();
                            result.localidr_bank_account = reader["localidr_bank_account"] == DBNull.Value ? null : Convert.ToDecimal(reader["localidr_bank_account"]);
                            result.localidr_account_holder = reader["localidr_account_holder"].ToString();
                            result.localforex_bank_id = reader["localforex_bank_id"].ToString();
                            result.localforex_bank_name = reader["localforex_bank_name"].ToString();
                            result.localforex_swift_code = reader["localforex_swift_code"].ToString();
                            result.localforex_bank_account = reader["localforex_bank_account"] == DBNull.Value ? null : Convert.ToDecimal(reader["localforex_bank_account"]);
                            result.localforex_account_holder = reader["localforex_account_holder"].ToString();
                            result.localforex_currency_id = reader["localforex_currency_id"].ToString();
                            result.foreign_bank_country_id = reader["foreign_bank_country_id"].ToString();
                            result.foreign_bank_country_name = reader["foreign_bank_country_name"].ToString();
                            result.foreign_bank_id = reader["foreign_bank_id"] == DBNull.Value ? null : reader["foreign_bank_id"].ToString();
                            result.foreign_bank_name = reader["foreign_bank_name"].ToString();
                            result.foreign_bank_swift_code = reader["foreign_bank_swift_code"].ToString();
                            result.foreign_bank_account = reader["foreign_bank_account"] == DBNull.Value ? null : reader["foreign_bank_account"].ToString();
                            result.foreign_account_holder = reader["foreign_account_holder"].ToString();
                            result.foreign_currency_id = reader["foreign_currency_id"].ToString();
                            result.reference_correspondent = reader["reference_correspondent"].ToString();
                            result.reference_country_origin = reader["reference_country_origin"].ToString();
                            result.pic_id = reader["pic_id"] == DBNull.Value ? null : reader["pic_id"].ToString();
                            result.person_number = reader["person_number"] == DBNull.Value ? null : reader["person_number"].ToString();
                            result.email_procurement_group = reader["email_procurement_group"].ToString();
                            result.file_id_card = reader["file_id_card"].ToString();
                            result.file_npwp = reader["file_npwp"].ToString();
                            result.file_sppkp = reader["file_sppkp"].ToString();
                            result.file_vendor_statement = reader["file_vendor_statement"].ToString();
                            result.position_data = reader["position_data"] == DBNull.Value ? null : Convert.ToInt32(reader["position_data"].ToString());
                            result.created_at = Convert.ToDateTime(reader["created_at"].ToString());
                            result.updated_at = Convert.ToDateTime(reader["updated_at"].ToString());
                            result.company = this.GetTempCompany(Convert.ToInt32(reader["vendor_id"].ToString()));
                            result.temp_log_vendor_status = this.GetTempLogVendor(Convert.ToInt32(reader["vendor_id"].ToString()));
                            result.verification_status_id = Convert.ToInt32(reader["verification_status_id"].ToString());
                            result.status_for_vendor =reader["status_for_vendor"].ToString();
                            result.status_for_intern = reader["status_for_intern"].ToString();
                            result.verification_note = reader["verification_note"].ToString();
                            result.verified_email = reader["verified_email"].ToString();
                            result.verified_email_list = reader["verified_email"] == DBNull.Value ? new List<string>() : new Helpers.EmailFunction().GetEmailList(reader["verified_email"].ToString(), ";");
                            result.account_group_id = new Helpers.GlobalFunction().GetAccountGroupId(result.vendor_type_id, result.country_id);
                            result.partner_function = reader["partner_function"] == DBNull.Value ? null : reader["partner_function"].ToString();
                            result.partner_function_name = reader["partner_function_name"] == DBNull.Value ? null : reader["partner_function_name"].ToString();
                            result.updated_data = reader["updated_data"] == DBNull.Value ? null : reader["updated_data"].ToString();
                            result.is_extension = reader["is_extension"] == DBNull.Value ? null : Convert.ToInt32(reader["is_extension"].ToString());
                            result.status_rpa = reader["status_rpa"].ToString() == "False" ? false : true;
                            result.rpa_status_description = reader["rpa_status_description"] == DBNull.Value ? null : reader["rpa_status_description"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("GetPendingVendorDetail", ex.Message);
                Console.WriteLine(ex.Message);
            }
            return result;
        }
        public bool InsertVendor(VendorModel model)
        {
            var result = false;
            var returnValue = "";
            try
            {
                  using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                  {
                      var sql = "INSERT INTO tr_vendor (" +
                                "vendor_number, vendor_type_id, sg_category_id, currency_id, top_id, " +
                                "title_id, name, contact_person, search_term,street_address, " + 
                                "additional_street_address, city,postal_code, country_id, "+ 
                                "telephone, fax, email, tax_type_id, tax_number_type_id, " + 
                                "npwp, id_card_number, sppkp_number, localidr_bank_id, localidr_bank_account, " +
                                "localidr_account_holder,localforex_bank_id, localforex_bank_account, "+
                                "localforex_account_holder,localforex_currency_id, " +
                                "foreign_bank_country_id, foreign_bank_id, " +
                                "foreign_bank_account, foreign_account_holder, foreign_currency_id, " + 
                                "reference_correspondent, reference_country_origin, pic_id, file_id_card, " + 
                                "file_npwp, file_sppkp, file_vendor_statement)" +
                                // "OUTPUT INSERTED.vendor_number " +
                                "VALUES " +
                                "(@vendor_number, @vendor_type_id, @sg_category_id, @currency_id, @top_id, " +
                                "@title_id, @name, @contact_person, @search_term, @street_address, " + 
                                "@additional_street_address, @city, @postal_code, @country_id, "+ 
                                "@telephone, @fax, @email, @tax_type_id, @tax_number_type_id, " + 
                                "@npwp, @id_card_number, @sppkp_number, @localidr_bank_id, @localidr_bank_account, " +
                                "@localidr_account_holder, @localforex_bank_id, @localforex_bank_account, "+
                                "@localforex_account_holder, @localforex_currency_id, " +
                                "@foreign_bank_country_id, @foreign_bank_id, " +
                                "@foreign_bank_account, @foreign_account_holder, @foreign_currency_id, " + 
                                "@reference_correspondent, @reference_country_origin, @pic_id, @file_id_card, " + 
                                "@file_npwp, @file_sppkp, @file_vendor_statement)";

                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@vendor_number", model.vendor_number);
                    command.Parameters.AddWithValue("@vendor_type_id", model.vendor_type_id);
                    command.Parameters.AddWithValue("@sg_category_id", model.sg_category_id);
                    command.Parameters.AddWithValue("@currency_id", model.currency_id);
                    command.Parameters.AddWithValue("@title_id", model.title_id);
                    command.Parameters.AddWithValue("@top_id", (string.IsNullOrEmpty(model.top_id) ? DBNull.Value : model.top_id));
                    command.Parameters.AddWithValue("@name", model.name);
                    command.Parameters.AddWithValue("@contact_person", model.contact_person);
                    command.Parameters.AddWithValue("@search_term", model.search_term);
                    command.Parameters.AddWithValue("@street_address", model.street_address);
                    command.Parameters.AddWithValue("@additional_street_address", (string.IsNullOrEmpty(model.additional_street_address) ? DBNull.Value : model.additional_street_address));
                    command.Parameters.AddWithValue("@city", model.city);
                    command.Parameters.AddWithValue("@postal_code", model.postal_code);
                    command.Parameters.AddWithValue("@country_id", model.country_id);
                    command.Parameters.AddWithValue("@telephone", model.telephone);
                    command.Parameters.AddWithValue("@fax", (string.IsNullOrEmpty(model.fax) ? DBNull.Value : model.fax));
                    command.Parameters.AddWithValue("@email", model.email);
                    command.Parameters.AddWithValue("@tax_type_id", model.tax_type_id);
                    command.Parameters.AddWithValue("@tax_number_type_id", model.tax_number_type_id);
                    command.Parameters.AddWithValue("@npwp", (string.IsNullOrEmpty(model.npwp.ToString()) ? DBNull.Value : model.npwp));
                    command.Parameters.AddWithValue("@id_card_number", (string.IsNullOrEmpty(model.id_card_number.ToString()) ? DBNull.Value : model.id_card_number));
                    command.Parameters.AddWithValue("@sppkp_number", (string.IsNullOrEmpty(model.sppkp_number) ? DBNull.Value : model.sppkp_number));
                    command.Parameters.AddWithValue("@localidr_bank_id", (string.IsNullOrEmpty(model.localidr_bank_id) ? DBNull.Value : model.localidr_bank_id));
                    command.Parameters.AddWithValue("@localidr_bank_account", (string.IsNullOrEmpty(model.localidr_bank_account.ToString()) ? DBNull.Value : model.localidr_bank_account));
                    command.Parameters.AddWithValue("@localidr_account_holder", (string.IsNullOrEmpty(model.localidr_account_holder.ToString()) ? DBNull.Value : model.localidr_account_holder));
                    command.Parameters.AddWithValue("@localforex_bank_id", (String.IsNullOrEmpty(model.localforex_bank_id) ? DBNull.Value : model.localforex_bank_id));
                    command.Parameters.AddWithValue("@localforex_bank_account", (String.IsNullOrEmpty(model.localforex_bank_account.ToString()) ? DBNull.Value : model.localforex_bank_account));
                    command.Parameters.AddWithValue("@localforex_account_holder", (string.IsNullOrEmpty(model.localforex_account_holder.ToString()) ? DBNull.Value : model.localforex_account_holder));
                    command.Parameters.AddWithValue("@localforex_currency_id", (string.IsNullOrEmpty(model.localforex_currency_id) ? DBNull.Value : model.localforex_currency_id));
                    command.Parameters.AddWithValue("@foreign_bank_country_id", (string.IsNullOrEmpty(model.foreign_bank_country_id) ? DBNull.Value : model.foreign_bank_country_id));
                    command.Parameters.AddWithValue("@foreign_bank_id", (String.IsNullOrEmpty(model.foreign_bank_id) ? DBNull.Value : model.foreign_bank_id));
                    // command.Parameters.AddWithValue("@foreign_bank_name", (string.IsNullOrEmpty(model.foreign_bank_name) ? DBNull.Value : model.foreign_bank_name));
                    // command.Parameters.AddWithValue("@foreign_bank_swift_code", (string.IsNullOrEmpty(model.foreign_bank_swift_code) ? DBNull.Value : model.foreign_bank_swift_code));
                    command.Parameters.AddWithValue("@foreign_bank_account", (string.IsNullOrEmpty(model.foreign_bank_account) ? DBNull.Value : model.foreign_bank_account));
                    command.Parameters.AddWithValue("@foreign_account_holder", (String.IsNullOrEmpty(model.foreign_account_holder) ? DBNull.Value : model.foreign_account_holder));
                    command.Parameters.AddWithValue("@foreign_currency_id", (string.IsNullOrEmpty(model.foreign_currency_id) ? DBNull.Value : model.foreign_currency_id));
                    command.Parameters.AddWithValue("@reference_correspondent", (string.IsNullOrEmpty(model.reference_correspondent) ? DBNull.Value : model.reference_correspondent));
                    command.Parameters.AddWithValue("@reference_country_origin", (string.IsNullOrEmpty(model.reference_country_origin) ? DBNull.Value : model.reference_country_origin));
                    command.Parameters.AddWithValue("@pic_id", model.pic_id);
                    command.Parameters.AddWithValue("@file_id_card", model.file_id_card);
                    command.Parameters.AddWithValue("@file_npwp", (string.IsNullOrEmpty(model.file_npwp) ? DBNull.Value : model.file_npwp));
                    command.Parameters.AddWithValue("@file_sppkp", (string.IsNullOrEmpty(model.file_sppkp) ? DBNull.Value : model.file_sppkp));
                    command.Parameters.AddWithValue("@file_vendor_statement", (string.IsNullOrEmpty(model.file_vendor_statement) ? DBNull.Value : model.file_vendor_statement));
                    cs.Open();
                    result = command.ExecuteNonQuery() == 1 ;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                new Helpers.GlobalFunction().LogError("InsertVendor", ex.Message);
            }
            return result;
        }

        public bool updateVendorNumber(string vendor_number, int vendor_id)
        {
            var result = false;
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "Update temp_vendor set vendor_number = @vendor_number " +
                              "where vendor_id = @vendor_id";
                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@vendor_number", vendor_number);
                    command.Parameters.AddWithValue("@vendor_id", vendor_id);
                    cs.Open();

                    result = command.ExecuteNonQuery() == 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("updateVendorNumber", ex.Message);
            }
            return result;
        }

        public bool updateIsLockedAndChangeRequestStatus(string vendor_number, int is_locked, int change_request_status, string? change_request_note)
        {
            var result = false;
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "Update tr_vendor set " +
                              "is_locked = @is_locked, change_request_status = @change_request_status, change_request_note = @change_request_note " +
                              "where vendor_number = @vendor_number";
                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@is_locked", is_locked);
                    command.Parameters.AddWithValue("@change_request_status", change_request_status);
                    command.Parameters.AddWithValue("@change_request_note", (String.IsNullOrEmpty(change_request_note) ? DBNull.Value : change_request_note));
                    command.Parameters.AddWithValue("@vendor_number", vendor_number);
                    cs.Open();

                    result = command.ExecuteNonQuery() == 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("updateIsLockedAndChangeRequestStatus", ex.Message);
            }
            return result;
        }

        public bool updateMailingData(string vendor_number, string mailing_address, string mailing_district, string mailing_village, string mailing_province)
        {
            var result = false;
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "Update tr_vendor set " +
                              "mailing_address = @mailing_address, mailing_district = @mailing_district, mailing_village = @mailing_village, mailing_province = @mailing_province " +
                              "where vendor_number = @vendor_number";
                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@mailing_address", mailing_address);
                    command.Parameters.AddWithValue("@mailing_district", mailing_district);
                    command.Parameters.AddWithValue("@mailing_village", mailing_village);
                    command.Parameters.AddWithValue("@mailing_province", mailing_province);
                    command.Parameters.AddWithValue("@vendor_number", vendor_number);
                    cs.Open();

                    result = command.ExecuteNonQuery() == 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("updateMailingData", ex.Message);
            }
            return result;
        }
        public bool updateVendor(string vendor_number, RegistrationModel model, int is_locked, int change_request_status, String file_id_card_name = null, String file_npwp_name = null, String file_sppkp_name = null, String file_vendor_statement_name = null)
        {
            var result = false;
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "Update tr_vendor set " +
                              "vendor_type_id = @vendor_type_id, " +
                              "sg_category_id = @sg_category_id, " +
                              "currency_id = @currency_id, " +
                              "top_id = @top_id, " +
                              "title_id = @title_id, " +
                              "name = @name, " +
                              "contact_person = @contact_person, " +
                              "search_term = @search_term, " +
                              "street_address = @street_address, " +
                              "additional_street_address = @additional_street_address, " +
                              "city = @city, " +
                              "postal_code = @postal_code, " +
                              "country_id = @country_id, " +
                              "telephone = @telephone, " +
                              "fax = @fax, email = @email, " +
                              "tax_type_id = @tax_type_id, " +
                              "tax_number_type_id = @tax_number_type_id, " +
                              "npwp = @npwp, id_card_number = @id_card_number, " +
                              "sppkp_number = @sppkp_number, " +
                              "localidr_bank_id = @localidr_bank_id, localidr_bank_account = @localidr_bank_account, " +
                              "localidr_account_holder = @localidr_account_holder, " +
                              "localforex_bank_id = @localforex_bank_id, " +
                              "localforex_account_holder = @localforex_account_holder, " +
                            //   "localforex_bank_holder = @localforex_bank_holder, " +
                              "localforex_currency_id = @localforex_currency_id, " +
                              "foreign_bank_country_id = @foreign_bank_country_id, " +
                              "foreign_bank_id = @foreign_bank_id, " +
                            //   "foreign_bank_name = @foreign_bank_name, " +
                            //   "foreign_bank_swift_code = @foreign_bank_swift_code, " +
                              "foreign_bank_account = @foreign_bank_account, " +
                              "foreign_account_holder = @foreign_account_holder, " +
                              "foreign_currency_id = @foreign_currency_id, " +
                              "reference_correspondent = @reference_correspondent, " +
                              "reference_country_origin = @reference_country_origin, " +
                              "pic_id = @pic_id, " +
                              (string.IsNullOrEmpty(file_id_card_name) ? "": "file_id_card = @file_id_card, ") +
                              (string.IsNullOrEmpty(file_npwp_name) ? "" : "file_npwp = @file_npwp, ") +
                              (string.IsNullOrEmpty(file_sppkp_name) ? "" : "file_sppkp = @file_sppkp, ") +
                              (string.IsNullOrEmpty(file_vendor_statement_name) ? "" : "file_vendor_statement = @file_vendor_statement, ") +
                            //   "verification_status_id = @verification_status_id, " +
                            //   "verification_note = @verification_note, " +
                            //   "position_data = @position_data, token = @token " +
                              "is_locked = @is_locked, " +
                              "change_request_status = @change_request_status, " +
                              "partner_function = @partner_function " +
                              "where vendor_number = @vendor_number";
                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@vendor_type_id", model.vendor_type_id);
                    command.Parameters.AddWithValue("@sg_category_id", model.sg_category_id);
                    command.Parameters.AddWithValue("@currency_id", model.currency_id);
                    command.Parameters.AddWithValue("@top_id", model.top_id);
                    command.Parameters.AddWithValue("@title_id", model.title_id);
                    command.Parameters.AddWithValue("@name", model.name);
                    command.Parameters.AddWithValue("@contact_person", model.contact_person);
                    command.Parameters.AddWithValue("@search_term", model.search_term);
                    command.Parameters.AddWithValue("@street_address", model.street_address);
                    command.Parameters.AddWithValue("@additional_street_address", (string.IsNullOrEmpty(model.additional_street_address) ? DBNull.Value : model.additional_street_address));
                    command.Parameters.AddWithValue("@city", model.city);
                    command.Parameters.AddWithValue("@postal_code", model.postal_code);
                    command.Parameters.AddWithValue("@country_id", model.country_id);
                    command.Parameters.AddWithValue("@telephone", model.telephone);
                    command.Parameters.AddWithValue("@fax", (string.IsNullOrEmpty(model.fax) ? DBNull.Value : model.fax));
                    command.Parameters.AddWithValue("@email", model.email);
                    command.Parameters.AddWithValue("@tax_type_id", model.tax_type_id);
                    command.Parameters.AddWithValue("@tax_number_type_id", model.tax_number_type_id);
                    command.Parameters.AddWithValue("@npwp", (string.IsNullOrEmpty(model.npwp.ToString()) ? DBNull.Value : model.npwp));
                    command.Parameters.AddWithValue("@id_card_number", (string.IsNullOrEmpty(model.id_card_number.ToString()) ? DBNull.Value : model.id_card_number));
                    command.Parameters.AddWithValue("@sppkp_number", (string.IsNullOrEmpty(model.sppkp_number) ? DBNull.Value : model.sppkp_number));
                    command.Parameters.AddWithValue("@localidr_bank_id", (string.IsNullOrEmpty(model.localidr_bank_id) ? DBNull.Value : model.localidr_bank_id));
                    command.Parameters.AddWithValue("@localidr_bank_account", (string.IsNullOrEmpty(model.localidr_bank_account.ToString()) ? DBNull.Value : model.localidr_bank_account));
                    command.Parameters.AddWithValue("@localidr_account_holder", (String.IsNullOrEmpty(model.localidr_account_holder) ? DBNull.Value : model.localidr_account_holder));
                    command.Parameters.AddWithValue("@localforex_bank_id", (String.IsNullOrEmpty(model.localforex_bank_id) ? DBNull.Value : model.localforex_bank_id));
                    command.Parameters.AddWithValue("@localforex_bank_account", (String.IsNullOrEmpty(model.localforex_bank_account.ToString()) ? DBNull.Value : model.localforex_bank_account));
                    command.Parameters.AddWithValue("@localforex_account_holder", (string.IsNullOrEmpty(model.localforex_account_holder) ? DBNull.Value : model.localforex_account_holder));
                    command.Parameters.AddWithValue("@localforex_currency_id", (string.IsNullOrEmpty(model.localforex_currency_id) ? DBNull.Value : model.localforex_currency_id));
                    command.Parameters.AddWithValue("@foreign_bank_country_id", (string.IsNullOrEmpty(model.foreign_bank_country_id) ? DBNull.Value : model.foreign_bank_country_id));
                    command.Parameters.AddWithValue("@foreign_bank_id", (string.IsNullOrEmpty(model.foreign_bank_id) ? DBNull.Value : model.foreign_bank_id));
                    // command.Parameters.AddWithValue("@foreign_bank_name", (string.IsNullOrEmpty(model.foreign_bank_name) ? DBNull.Value : model.foreign_bank_name));
                    // command.Parameters.AddWithValue("@foreign_bank_swift_code", (string.IsNullOrEmpty(model.foreign_bank_swift_code) ? DBNull.Value : model.foreign_bank_swift_code));
                    command.Parameters.AddWithValue("@foreign_bank_account", (string.IsNullOrEmpty(model.foreign_bank_account) ? DBNull.Value : model.foreign_bank_account));
                    command.Parameters.AddWithValue("@foreign_account_holder", (String.IsNullOrEmpty(model.foreign_account_holder) ? DBNull.Value : model.foreign_account_holder));
                    command.Parameters.AddWithValue("@foreign_currency_id", (string.IsNullOrEmpty(model.foreign_currency_id) ? DBNull.Value : model.foreign_currency_id));
                    command.Parameters.AddWithValue("@reference_correspondent", (string.IsNullOrEmpty(model.reference_correspondent) ? DBNull.Value : model.reference_correspondent));
                    command.Parameters.AddWithValue("@reference_country_origin", (string.IsNullOrEmpty(model.reference_country_origin) ? DBNull.Value : model.reference_country_origin));
                    command.Parameters.AddWithValue("@pic_id", model.pic_id);
                    if ( !String.IsNullOrEmpty(file_id_card_name))
                        command.Parameters.AddWithValue("@file_id_card", file_id_card_name);
                    if (!String.IsNullOrEmpty(file_npwp_name))
                        command.Parameters.AddWithValue("@file_npwp", (string.IsNullOrEmpty(file_npwp_name) ? DBNull.Value : file_npwp_name));
                    if (!String.IsNullOrEmpty(file_sppkp_name))
                        command.Parameters.AddWithValue("@file_sppkp", (string.IsNullOrEmpty(file_sppkp_name) ? DBNull.Value : file_sppkp_name));
                    if (!String.IsNullOrEmpty(file_vendor_statement_name))
                        command.Parameters.AddWithValue("@file_vendor_statement", (string.IsNullOrEmpty(file_vendor_statement_name) ? DBNull.Value : file_vendor_statement_name));
                    // command.Parameters.AddWithValue("@verification_status_id", model.verification_status_id);
                    // command.Parameters.AddWithValue("@verification_note", (string.IsNullOrEmpty(model.verification_note) ? DBNull.Value : model.verification_note));
                    // command.Parameters.AddWithValue("@position_data", model.position_data);
                    command.Parameters.AddWithValue("@is_locked", is_locked);
                    command.Parameters.AddWithValue("@change_request_status", change_request_status);
                    command.Parameters.AddWithValue("@token", (string.IsNullOrEmpty(model.token) ? DBNull.Value: model.token));
                    command.Parameters.AddWithValue("@partner_function", (String.IsNullOrEmpty(model.partner_function) ? DBNull.Value : model.partner_function));
                    command.Parameters.AddWithValue("@vendor_number", vendor_number);
                    cs.Open();

                    result = command.ExecuteNonQuery() == 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("updateVendor", ex.Message);
            }
            return result;
        }
        public bool updateTempVendor(int vendor_id, RegistrationModel model, String file_id_card_name = null, String file_npwp_name = null, String file_sppkp_name = null, String file_vendor_statement_name = null)
        {
            var result = false;
            String updatedData = String.IsNullOrEmpty(model.updated_data) ?  "" : ",updated_data = @updated_data ";
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "Update temp_vendor set " +
                              "vendor_type_id = @vendor_type_id, " +
                              "sg_category_id = @sg_category_id, " +
                              "currency_id = @currency_id, " +
                              "top_id = @top_id, " +
                              "title_id = @title_id, " +
                              "name = @name, " +
                              "contact_person = @contact_person, " +
                              "search_term = @search_term, " +
                              "street_address = @street_address, " +
                              "additional_street_address = @additional_street_address, " +
                              "city = @city, " +
                              "postal_code = @postal_code, " +
                              "country_id = @country_id, " +
                              "telephone = @telephone, " +
                              "fax = @fax, email = @email, " +
                              "tax_type_id = @tax_type_id, " +
                              "tax_number_type_id = @tax_number_type_id, " +
                              "npwp = @npwp, id_card_number = @id_card_number, " +
                              "sppkp_number = @sppkp_number, " +
                              "localidr_bank_id = @localidr_bank_id, localidr_bank_account = @localidr_bank_account, " +
                              "localidr_account_holder = @localidr_account_holder, " +
                              "localforex_bank_id = @localforex_bank_id, " +
                              "localforex_bank_account = @localforex_bank_account, " +
                              "localforex_account_holder = @localforex_account_holder, " +
                              "localforex_currency_id = @localforex_currency_id, " +
                              "foreign_bank_country_id = @foreign_bank_country_id, " +
                              "foreign_bank_id = @foreign_bank_id, " +
                              "foreign_bank_name = @foreign_bank_name, " +
                              "foreign_bank_swift_code = @foreign_bank_swift_code, " +
                              "foreign_bank_account = @foreign_bank_account, " +
                              "foreign_account_holder = @foreign_account_holder, " +
                              "foreign_currency_id = @foreign_currency_id, " +
                              "reference_correspondent = @reference_correspondent, " +
                              "reference_country_origin = @reference_country_origin, " +
                              "pic_id = @pic_id, " +
                              (string.IsNullOrEmpty(file_id_card_name) ? "": "file_id_card = @file_id_card, ") +
                              (string.IsNullOrEmpty(file_npwp_name) ? "" : "file_npwp = @file_npwp, ") +
                              (string.IsNullOrEmpty(file_sppkp_name) ? "" : "file_sppkp = @file_sppkp, ") +
                              (string.IsNullOrEmpty(file_vendor_statement_name) ? "" : "file_vendor_statement = @file_vendor_statement, ") +
                              "verification_status_id = @verification_status_id, " +
                              "verification_note = @verification_note, " +
                              "position_data = @position_data, token = @token, partner_function = @partner_function " + updatedData +
                              "where vendor_id = @vendor_id";
                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@vendor_type_id", model.vendor_type_id);
                    command.Parameters.AddWithValue("@sg_category_id", model.sg_category_id);
                    command.Parameters.AddWithValue("@currency_id", model.currency_id);
                    command.Parameters.AddWithValue("@top_id", model.top_id);
                    command.Parameters.AddWithValue("@title_id", model.title_id);
                    command.Parameters.AddWithValue("@name", model.name);
                    command.Parameters.AddWithValue("@contact_person", model.contact_person);
                    command.Parameters.AddWithValue("@search_term", model.search_term);
                    command.Parameters.AddWithValue("@street_address", model.street_address);
                    command.Parameters.AddWithValue("@additional_street_address", (string.IsNullOrEmpty(model.additional_street_address) ? DBNull.Value : model.additional_street_address));
                    command.Parameters.AddWithValue("@city", model.city);
                    command.Parameters.AddWithValue("@postal_code", model.postal_code);
                    command.Parameters.AddWithValue("@country_id", model.country_id);
                    command.Parameters.AddWithValue("@telephone", model.telephone);
                    command.Parameters.AddWithValue("@fax", (string.IsNullOrEmpty(model.fax) ? DBNull.Value : model.fax));
                    command.Parameters.AddWithValue("@email", model.email);
                    command.Parameters.AddWithValue("@tax_type_id", model.tax_type_id);
                    command.Parameters.AddWithValue("@tax_number_type_id", model.tax_number_type_id);
                    command.Parameters.AddWithValue("@npwp", (string.IsNullOrEmpty(model.npwp) ? DBNull.Value : model.npwp));
                    command.Parameters.AddWithValue("@id_card_number", (string.IsNullOrEmpty(model.id_card_number) ? DBNull.Value : model.id_card_number));
                    command.Parameters.AddWithValue("@sppkp_number", (string.IsNullOrEmpty(model.sppkp_number) ? DBNull.Value : model.sppkp_number));
                    command.Parameters.AddWithValue("@localidr_bank_id", (string.IsNullOrEmpty(model.localidr_bank_id) ? DBNull.Value : model.localidr_bank_id));
                    command.Parameters.AddWithValue("@localidr_bank_account", (string.IsNullOrEmpty(model.localidr_bank_account.ToString()) ? DBNull.Value : model.localidr_bank_account));
                    command.Parameters.AddWithValue("@localidr_account_holder", (String.IsNullOrEmpty(model.localidr_account_holder) ? DBNull.Value : model.localidr_account_holder));
                    command.Parameters.AddWithValue("@localforex_bank_id", (String.IsNullOrEmpty(model.localforex_bank_id) ? DBNull.Value : model.localforex_bank_id));
                    command.Parameters.AddWithValue("@localforex_bank_account", (String.IsNullOrEmpty(model.localforex_bank_account.ToString()) ? DBNull.Value : model.localforex_bank_account));
                    command.Parameters.AddWithValue("@localforex_account_holder", (string.IsNullOrEmpty(model.localforex_account_holder) ? DBNull.Value : model.localforex_account_holder));
                    command.Parameters.AddWithValue("@localforex_currency_id", (string.IsNullOrEmpty(model.localforex_currency_id) ? DBNull.Value : model.localforex_currency_id));
                    command.Parameters.AddWithValue("@foreign_bank_country_id", (string.IsNullOrEmpty(model.foreign_bank_country_id) ? DBNull.Value : model.foreign_bank_country_id));
                    command.Parameters.AddWithValue("@foreign_bank_id", (string.IsNullOrEmpty(model.foreign_bank_id) ? DBNull.Value : model.foreign_bank_id));
                    command.Parameters.AddWithValue("@foreign_bank_name", (string.IsNullOrEmpty(model.foreign_bank_name) ? DBNull.Value : model.foreign_bank_name));
                    command.Parameters.AddWithValue("@foreign_bank_swift_code", (string.IsNullOrEmpty(model.foreign_bank_swift_code) ? DBNull.Value : model.foreign_bank_swift_code));
                    command.Parameters.AddWithValue("@foreign_bank_account", (string.IsNullOrEmpty(model.foreign_bank_account) ? DBNull.Value : model.foreign_bank_account));
                    command.Parameters.AddWithValue("@foreign_account_holder", (String.IsNullOrEmpty(model.foreign_account_holder) ? DBNull.Value : model.foreign_account_holder));
                    command.Parameters.AddWithValue("@foreign_currency_id", (string.IsNullOrEmpty(model.foreign_currency_id) ? DBNull.Value : model.foreign_currency_id));
                    command.Parameters.AddWithValue("@reference_correspondent", (string.IsNullOrEmpty(model.reference_correspondent) ? DBNull.Value : model.reference_correspondent));
                    command.Parameters.AddWithValue("@reference_country_origin", (string.IsNullOrEmpty(model.reference_country_origin) ? DBNull.Value : model.reference_country_origin));
                    command.Parameters.AddWithValue("@pic_id", model.pic_id);
                    if ( !String.IsNullOrEmpty(file_id_card_name))
                        command.Parameters.AddWithValue("@file_id_card", file_id_card_name);
                    if (!String.IsNullOrEmpty(file_npwp_name))
                        command.Parameters.AddWithValue("@file_npwp", (string.IsNullOrEmpty(file_npwp_name) ? DBNull.Value : file_npwp_name));
                    if (!String.IsNullOrEmpty(file_sppkp_name))
                        command.Parameters.AddWithValue("@file_sppkp", (string.IsNullOrEmpty(file_sppkp_name) ? DBNull.Value : file_sppkp_name));
                    if (!String.IsNullOrEmpty(file_vendor_statement_name))
                        command.Parameters.AddWithValue("@file_vendor_statement", (string.IsNullOrEmpty(file_vendor_statement_name) ? DBNull.Value : file_vendor_statement_name));
                    command.Parameters.AddWithValue("@verification_status_id", model.verification_status_id);
                    command.Parameters.AddWithValue("@verification_note", (string.IsNullOrEmpty(model.verification_note) ? DBNull.Value : model.verification_note));
                    command.Parameters.AddWithValue("@position_data", model.position_data);
                    command.Parameters.AddWithValue("@token", (string.IsNullOrEmpty(model.token) ? DBNull.Value: model.token));
                    command.Parameters.AddWithValue("@partner_function", (string.IsNullOrEmpty(model.partner_function) ? DBNull.Value : model.partner_function));
                    command.Parameters.AddWithValue("@vendor_id", vendor_id);
                    if (!String.IsNullOrEmpty(model.updated_data))
                        command.Parameters.AddWithValue("@updated_data", (string.IsNullOrEmpty(model.updated_data) ? DBNull.Value : model.updated_data));
                    cs.Open();

                    result = command.ExecuteNonQuery() == 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("updateTempVendor", ex.Message);
            }
            return result;
        }
        public bool updateVendorVerificationStatus(int vendor_id, int verification_status_id, int position_data, string verification_note = null, string token = null)
        {
            var result = false;
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "Update temp_vendor set " +
                              "verification_status_id = @verification_status_id, " +
                              "position_data = @position_data, " +
                              "verification_note = @verification_note, " +
                              "token = @token " +
                              "where vendor_id = @vendor_id";
                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@verification_status_id", verification_status_id);
                    command.Parameters.AddWithValue("@position_data", position_data);
                    command.Parameters.AddWithValue("@verification_note", (String.IsNullOrEmpty(verification_note) ? DBNull.Value: verification_note));
                    command.Parameters.AddWithValue("@token", (string.IsNullOrEmpty(token) ? DBNull.Value: token));
                    command.Parameters.AddWithValue("@vendor_id", vendor_id);
                    cs.Open();

                    result = command.ExecuteNonQuery() == 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("updateVendorVerificationStatus", ex.Message);
            }
            return result;
        }
        public bool deleteTempCompanyByVendorId(int vendor_id)
        {
            var result = false;
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "DELETE FROM temp_company_relations WHERE vendor_id = @vendor_id";
                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@vendor_id", vendor_id);
                    cs.Open();

                    result = command.ExecuteNonQuery() == 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("deleteTempCompanyByVendorId", ex.Message);
            }
            return result;
        }
        public bool deleteCompanyByVendorNumber(string vendor_number)
        {
            var result = false;
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "DELETE FROM tr_company_relations WHERE vendor_number = @vendor_number";
                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@vendor_number", vendor_number);
                    cs.Open();

                    result = command.ExecuteNonQuery() == 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("deleteCompanyByVendorNumber", ex.Message);
            }
            return result;
        }
        public bool InsertLogTempVendor(TempLogVendorStatusModel model)
        {
            var result = false;
            try
            {
                var sql = "INSERT INTO temp_log_vendor_status " +
                          "(vendor_id, verification_status_id, verification_note, created_by) " +
                          "values (@vendor_id, @verification_status_id, @verification_note, @created_by)";

                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@vendor_id", model.vendor_id);
                    command.Parameters.AddWithValue("@verification_status_id", model.verification_status_id);
                    command.Parameters.AddWithValue("@verification_note", (string.IsNullOrEmpty(model.verification_note) ? DBNull.Value : model.verification_note));
                    command.Parameters.AddWithValue("@created_by", (string.IsNullOrEmpty(model.created_by.ToString()) ? DBNull.Value : model.created_by));
                    cs.Open();
                    result = command.ExecuteNonQuery() == 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("InsertLogTempVendor", ex.Message);
            }
            return result;
        }
        public bool InsertLogVendor(LogVendorModel model)
        {
            var result = false;
            try
            {
                var sql = "INSERT INTO log_vendor " +
                          "(vendor_number, process, note, created_by) " +
                          "values (@vendor_number, @process, @note, @created_by)";

                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@vendor_number", model.vendor_number);
                    command.Parameters.AddWithValue("@process", model.process);
                    command.Parameters.AddWithValue("@note", (string.IsNullOrEmpty(model.note) ? DBNull.Value : model.note));
                    command.Parameters.AddWithValue("@created_by", (string.IsNullOrEmpty(model.created_by.ToString()) ? DBNull.Value : model.created_by));
                    cs.Open();
                    result = command.ExecuteNonQuery() == 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("InsertLogVendor", ex.Message);
            }
            return result;
        }
        public bool InsertTempCompany(RegistrationModel company, int vendor_id)
        {
            var result = false;
            try
            {
                var sql = "INSERT INTO temp_company_relations(vendor_id, company_id, purchase_organization_id) values ";
                var companyList = company.company_id;
                var purchaseOrganizationList = company.purchase_organization_id;
                for (var i = 0; i < companyList.Length; i++)
                {
                    var purchase_organization_id = String.IsNullOrEmpty(purchaseOrganizationList[i].ToString()) ? null : purchaseOrganizationList[i].ToString();
                    if (i != companyList.Length - 1) sql += "('" + vendor_id + "', '" + companyList[i].ToString() + "', '" + purchase_organization_id + "'),";
                    else sql += "('" + vendor_id + "', '" + companyList[i].ToString() + "', '" + purchase_organization_id + "')";
                }
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var command = new ConnectionService().GetCommand();
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandText = sql;
                    command.Connection = cs;
                    cs.Open();

                    result = command.ExecuteNonQuery() == 1;

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("InsertTempCompany", ex.Message);
            }
            return result;
        }
        public bool InsertCompany(IEnumerable<CompanyModel> company, string vendor_number)
        {
            var result = false;
            try
            {
                var sql = "INSERT INTO tr_company_relations(vendor_number, company_id, purchase_organization_id) values ";
                var len = company.Count();
                int i = 1;
                foreach (var c in company)
                {
                    if (i < len) sql += "('" + vendor_number + "', '" + c.company_id.ToString() + "', '" + c.purchase_organization_id.ToString() + "'),";
                    else sql += "('" + vendor_number + "', '" + c.company_id.ToString() + "', '" + c.purchase_organization_id.ToString()  + "')";
                    i++;
                }
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var command = new ConnectionService().GetCommand();
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandText = sql;
                    command.Connection = cs;
                    cs.Open();

                    result = command.ExecuteNonQuery() == 1;

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("InsertCompany", ex.Message);
            }
            return result;
        }
        public IEnumerable<string> NewVendorActivation(int status_rpa, string rpa_status_description, string vendor_number) {
            List<string> error = new List<string> {};
            return error;
        }
        
         public bool UpdateTempVendorRpa(TempVendorModel model, int? status_rpa, string? rpa_status_description) {
            var result = false;
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "Update temp_vendor set " +
                              "status_rpa = @status_rpa, " +  
                              "rpa_status_description = @rpa_status_description, " +
                              "position_data = @position_data, " +
                              "verification_status_id = @verification_status_id " +
                              "where vendor_id = @vendor_id";

                              Console.WriteLine("sql+ :"+sql);
                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@status_rpa", status_rpa);
                    command.Parameters.AddWithValue("@position_data", model.position_data);
                    command.Parameters.AddWithValue("@verification_status_id", model.verification_status_id);
                    command.Parameters.AddWithValue("@rpa_status_description ", (string.IsNullOrEmpty(rpa_status_description) ? DBNull.Value : rpa_status_description));
                    command.Parameters.AddWithValue("@vendor_id", model.vendor_id);
                    cs.Open();

                    result = command.ExecuteNonQuery() == 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("UpdateTempVendorRpa", ex.Message);
            }
            return result;
        }
      public IEnumerable<RpaTempVendorModel> ApprovedVendorRegistration(string type)
    {
      List<RpaTempVendorModel> result = new List<RpaTempVendorModel>();
      var dict = new Dictionary<dynamic, RpaTempVendorModel>();

      try
      {
        using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
        {
          var sql = "SELECT A.vendor_id, A.vendor_number, A.type, A.is_extension, " +
          "B.name AS vendor_type_name, " +
          "B.vendor_type_id AS vendor_type_id, " +
          "B.sap_code AS vendor_type_sap_code, " +
          "B.gl_id AS vendor_type_gl_id, " +
          "B.scheme_group_id AS vendor_type_scheme_group_id, " +
          "C.name AS services_goods_categories, " +
          "A.currency_id AS currency, " +
          "A.top_id, " +
          "E.name AS top_name, " +
          "E.baseline_date AS top_baseline_date, " +
          "E.interval AS top_interval, " +
          "A.title_id AS title, " +
          "A.name, " +
          "A.contact_person, " +
          "A.search_term, " +
          "A.street_address, " +
          "A.additional_street_address, " +
          "A.city, " +
          "A.postal_code, " +
          "A.country_id, " +
          "G.name AS country_name, " +
          "A.telephone, " +
          "A.fax, " +
           "A.email, " +
          "H.name AS services_categories, " +
          "H.wht_code AS withholding_tax_type, " +
          "H.rates AS withholding_tax_code, " +
          "I.name AS tax_number_type, " +
          "I.sap_code AS tax_number_code, " +
          "A.npwp, " +
          "A.id_card_number AS id_card_no, " +
          "A.sppkp_number, " +
          "J.bank_id AS localidr_bank_key, " +
          "J.name AS localidr_bank_name, " +
          "J.swift_code AS localidr_bank_swift_code, " +
          "A.localidr_bank_account, " +
          "A.localidr_account_holder, " +
          "K.bank_id AS localforex_bank_key, " +
          "K.name AS localforex_bank_name, " +
          "K.swift_code AS localforex_bank_swift_code, " +
          "A.localforex_bank_account, " +
          "A.localforex_account_holder, " +
          "A.localforex_currency_id AS localforex_currency, " +
          "A.foreign_bank_country_id, " +
          "L.bank_id AS foreign_bank_key, " +
          "A.foreign_bank_name, " +
          "A.foreign_bank_swift_code, " +
          "A.foreign_bank_account, " +
          "A.foreign_account_holder, " +
          "A.foreign_currency_id AS foreign_currency, " +
          "A.reference_correspondent, " +
          "A.reference_country_origin, " +
          "N.person_number AS pic_pers_num, " +
          "substring(A.pic_id,1,charindex('/',A.pic_id)) as pic_initial, " +
          "reverse(substring(reverse(A.pic_id),0,charindex('/',reverse(A.pic_id)))) as pic_area, " +
          "N.email AS pic_group_email, " +
          "A.file_id_card, " +
          "A.file_npwp, " +
          "A.file_sppkp, " +
          "A.file_vendor_statement, " +
          "P.scheme_group_id AS vendor_schema, " +
          "P.name AS scheme_vendor_name, " +
          "B.sap_code  AS account_group, " +
          "B.sap_code  AS cash_management, B.gl_id AS  recon_account, " +
          "O.province_id AS partner_function_province_id,  O.name AS partner_function_name, " + 
          "Q.company_relation_id, Q.company_id, Q.purchase_organization_id "+
          "FROM temp_vendor A " +
          "LEFT JOIN m_vendor_type B ON A.vendor_type_id = B.vendor_type_id " +
          "LEFT JOIN m_sg_categories C ON A.sg_category_id = C.sg_category_id " +
          "LEFT JOIN m_top E ON A.top_id = E.top_id " +
          "LEFT JOIN m_country G ON A.country_id = G.country_id " +
          "LEFT JOIN m_tax_type H ON A.tax_type_id = H.tax_type_id " +
          "LEFT JOIN m_tax_number_type I ON A.tax_number_type_id = I.tax_number_type_id " +
          "LEFT JOIN m_bank J ON A.localidr_bank_id = J.bank_id " +
          "LEFT JOIN m_bank K ON A.localforex_bank_id = K.bank_id " +
          "LEFT JOIN m_bank L ON A.foreign_bank_id = L.bank_id " +
          "LEFT JOIN m_country M ON A.foreign_bank_country_id = M.country_id " +
          "LEFT JOIN tb_procurement_group N ON A.pic_id = N.initial_area " +
          "LEFT JOIN m_province O ON A.partner_function = O.province_id " +
          "LEFT JOIN m_scheme_group P on B.scheme_group_id = P.scheme_group_id " + 
          "LEFT JOIN temp_company_relations Q ON Q.vendor_id = A.vendor_id " + 
          "WHERE verification_status_id = 5 AND type = @type";
          SqlCommand command = new SqlCommand(sql, cs);
          Console.WriteLine("SQL GET APPROVED: "+sql);
          command.Parameters.AddWithValue("@type", type);

          cs.Open();

          var reader = command.ExecuteReader();
          if (reader.HasRows)
          {
            while (reader.Read())
            {
              var vendor_id = Convert.ToInt32(reader["vendor_id"].ToString());
              var telList = reader["telephone"] == DBNull.Value ? new List<string>() : new Helpers.EmailFunction().GetEmailList(reader["telephone"].ToString(), ";");
              var emailList = reader["email"] == DBNull.Value ? new List<string>() : new Helpers.EmailFunction().GetEmailList(reader["email"].ToString(), ";");
              var primary_company_id = "";
              // Console.WriteLine(reader["vendor_id"].ToString());
              
              if(!dict.ContainsKey(vendor_id)){
                primary_company_id = reader["company_id"] == DBNull.Value ? null : reader["company_id"].ToString();
                var newRpaTempVendor = new RpaTempVendorModel()
              {
                vendor_id = vendor_id,
                vendor_number = reader["vendor_number"] == DBNull.Value ? null : reader["vendor_number"].ToString(),
                type = reader["type"] == DBNull.Value ? null : reader["type"].ToString(),
                primary_company_purchase_organization_id = reader["purchase_organization_id"] == DBNull.Value ? null : reader["purchase_organization_id"].ToString(),
                primary_company_relation_id = reader["company_relation_id"] == DBNull.Value ? null : Convert.ToInt32(reader["company_relation_id"].ToString()),
                primary_company_id = primary_company_id,
                secondary_company_list = new List<SecondaryCompanyList>(),
                is_extension = reader["is_extension"] == DBNull.Value ? null : Convert.ToInt32(reader["is_extension"].ToString()),
                partner_function = new()
                {
                  name = reader["partner_function_name"] == DBNull.Value ? null : reader["partner_function_name"].ToString(),
                  province_id = reader["partner_function_province_id"] == DBNull.Value ? null : reader["partner_function_province_id"].ToString(),
                },
                vendor_type = new()
                {
                  name = reader["vendor_type_name"] == DBNull.Value ? null : reader["vendor_type_name"].ToString(),
                  vendor_type_id = reader["vendor_type_id"] == DBNull.Value ? null : Convert.ToInt32(reader["vendor_type_id"].ToString()),
                  sap_code = reader["vendor_type_sap_code"] == DBNull.Value ? null : reader["vendor_type_sap_code"].ToString(),
                  gl_id = reader["vendor_type_gl_id"] == DBNull.Value ? null : reader["vendor_type_gl_id"].ToString(),
                  scheme_group_id = reader["vendor_type_scheme_group_id"] == DBNull.Value ? null : reader["vendor_type_scheme_group_id"].ToString(),
                },
                department = new List<string>(),
                vendor_schema = reader["vendor_schema"] == DBNull.Value ? null : reader["vendor_schema"].ToString(),
                account_group = reader["account_group"] == DBNull.Value ? null : reader["account_group"].ToString(),
                cash_management = reader["cash_management"] == DBNull.Value ? null : reader["cash_management"].ToString(),
                recon_account = reader["recon_account"] == DBNull.Value ? null : reader["recon_account"].ToString(),
                services_goods_categories = reader["services_goods_categories"] == DBNull.Value ? null : reader["services_goods_categories"].ToString(),
                currency = reader["currency"] == DBNull.Value ? null : reader["currency"].ToString(),
                term_payment = reader["top_id"] == DBNull.Value ? null : reader["top_id"].ToString(),
                top_name = reader["top_name"] == DBNull.Value ? null : reader["top_name"].ToString(),
                top_baseline_date = reader["top_baseline_date"] == DBNull.Value ? null : String.Format("{0:dd.MM.yyyy}", reader["top_baseline_date"].ToString()),
                top_interval = reader["top_interval"] == DBNull.Value ? null : reader["top_interval"].ToString(),
                title = reader["title"] == DBNull.Value ? null : reader["title"].ToString(),
                name = reader["name"] == DBNull.Value ? null : reader["name"].ToString(),
                contact_person = reader["contact_person"] == DBNull.Value ? null : reader["contact_person"].ToString(),
                search_term = reader["search_term"] == DBNull.Value ? null : reader["search_term"].ToString(),
                street_address = reader["street_address"] == DBNull.Value ? null : reader["street_address"].ToString(),
                additional_street_address = reader["additional_street_address"] == DBNull.Value ? null : reader["additional_street_address"].ToString(),
                city = reader["city"] == DBNull.Value ? null : reader["city"].ToString(),
                postal_code = reader["postal_code"] == DBNull.Value ? null : reader["postal_code"].ToString(),
                country_id = reader["country_id"] == DBNull.Value ? null : reader["country_id"].ToString(),
                country_name = reader["country_name"] == DBNull.Value ? null : reader["country_name"].ToString(),
                telephone = reader["telephone"] == DBNull.Value ? null : reader["telephone"].ToString(),
                telephone_list = reader["telephone"] == DBNull.Value ? new List<string>() : new Helpers.EmailFunction().GetEmailList(reader["telephone"].ToString(), ";"),
                rpa_telephone_list = new List<Dictionary<String, String>>(){
                  new Dictionary<String, String>{
                    {"telephone1", telList.ElementAtOrDefault(0) != null ? telList[0] : null},
                    {"telephone2", telList.ElementAtOrDefault(1) != null ? telList[1] : null}
                  }
                },
                fax = reader["fax"] == DBNull.Value ? null : reader["fax"].ToString(),
                email = reader["email"] == DBNull.Value ? null : reader["email"].ToString(),
                verified_email_list = reader["email"] == DBNull.Value ? new List<string>() : new Helpers.EmailFunction().GetEmailList(reader["email"].ToString(), ";"),
                rpa_email_list = new List<Dictionary<String, String>>(){
                  new Dictionary<String, String>{
                    {"email1", emailList.ElementAtOrDefault(0) != null ? emailList[0] : null},
                    {"email2", emailList.ElementAtOrDefault(1) != null ? emailList[1] : null},
                    {"email3", emailList.ElementAtOrDefault(2) != null ? emailList[2] : null}
                  }
                },
                services_categories = reader["services_categories"] == DBNull.Value ? null : reader["services_categories"].ToString(),
                withholding_tax_type = reader["withholding_tax_type"] == DBNull.Value ? null : reader["withholding_tax_type"].ToString(),
                withholding_tax_code = reader["withholding_tax_code"] == DBNull.Value ? null : reader["withholding_tax_code"].ToString(),
                tax_number_type = reader["tax_number_type"] == DBNull.Value ? null : reader["tax_number_type"].ToString(),
                tax_number_code = reader["tax_number_code"] == DBNull.Value ? null : reader["tax_number_code"].ToString(),
                npwp = reader["npwp"] == DBNull.Value ? null : reader["npwp"].ToString(),
                id_card_no = reader["id_card_no"] == DBNull.Value ? null : reader["id_card_no"].ToString(),
                sppkp_number = reader["sppkp_number"] == DBNull.Value ? null : reader["sppkp_number"].ToString(),
                localidr_bank_key = reader["localidr_bank_key"] == DBNull.Value ? null : reader["localidr_bank_key"].ToString(),
                localidr_bank_name = reader["localidr_bank_name"] == DBNull.Value ? null : reader["localidr_bank_name"].ToString(),
                localidr_bank_swift_code = reader["localidr_bank_swift_code"] == DBNull.Value ? null : reader["localidr_bank_swift_code"].ToString(),
                localidr_bank_account = reader["localidr_bank_account"] == DBNull.Value ? null : Convert.ToDecimal(reader["localidr_bank_account"].ToString()),
                localidr_account_holder = reader["localidr_account_holder"] == DBNull.Value ? null : reader["localidr_account_holder"].ToString(),
                localforex_bank_key = reader["localforex_bank_key"] == DBNull.Value ? null : reader["localforex_bank_key"].ToString(),
                localforex_bank_name = reader["localforex_bank_name"] == DBNull.Value ? null : reader["localforex_bank_name"].ToString(),
                localforex_bank_swift_code = reader["localforex_bank_swift_code"] == DBNull.Value ? null : reader["localforex_bank_swift_code"].ToString(),
                localforex_bank_account = reader["localforex_bank_account"] == DBNull.Value ? null : Convert.ToDecimal(reader["localforex_bank_account"].ToString()),
                localforex_account_holder = reader["localforex_account_holder"] == DBNull.Value ? null : reader["localforex_account_holder"].ToString(),
                localforex_currency = reader["localforex_currency"] == DBNull.Value ? null : reader["localforex_currency"].ToString(),
                foreign_bank_country_id = reader["foreign_bank_country_id"] == DBNull.Value ? null : reader["foreign_bank_country_id"].ToString(),
                foreign_bank_key = reader["foreign_bank_key"] == DBNull.Value ? null : reader["foreign_bank_key"].ToString(),
                foreign_bank_name = reader["foreign_bank_name"] == DBNull.Value ? null : reader["foreign_bank_name"].ToString(),
                foreign_bank_swift_code = reader["foreign_bank_swift_code"] == DBNull.Value ? null : reader["foreign_bank_swift_code"].ToString(),
                foreign_bank_account = reader["foreign_bank_account"] == DBNull.Value ? null : reader["foreign_bank_account"].ToString(),
                foreign_account_holder = reader["foreign_account_holder"] == DBNull.Value ? null : reader["foreign_account_holder"].ToString(),
                foreign_currency = reader["foreign_currency"] == DBNull.Value ? null : reader["foreign_currency"].ToString(),
                reference_correspondent = reader["reference_correspondent"] == DBNull.Value ? null : reader["reference_correspondent"].ToString(),
                reference_country_origin = reader["reference_country_origin"] == DBNull.Value ? null : reader["reference_country_origin"].ToString(),
                pic_pers_num = reader["pic_pers_num"] == DBNull.Value ? null : reader["pic_pers_num"].ToString(),
                pic_initial = reader["pic_initial"] == DBNull.Value ? null : reader["pic_initial"].ToString(),
                pic_area = reader["pic_area"] == DBNull.Value ? null : reader["pic_area"].ToString(),
                pic_group_email = reader["pic_group_email"] == DBNull.Value ? null : reader["pic_group_email"].ToString(),
                file_id_card = reader["file_id_card"] == DBNull.Value ? null : reader["file_id_card"].ToString(),
                file_npwp = reader["file_npwp"] == DBNull.Value ? null : reader["file_npwp"].ToString(),
                file_sppkp = reader["file_sppkp"] == DBNull.Value ? null : reader["file_sppkp"].ToString(),
                file_vendor_statement = reader["file_vendor_statement"] == DBNull.Value ? null : reader["file_vendor_statement"].ToString()
              };

               dict[vendor_id] = newRpaTempVendor; 
                // Console.WriteLine(JsonSerializer.Serialize(dict.Values.ToList()));
              }

              // Create Field from the reader's columns 
                var NewSecondaryCompanyList = new SecondaryCompanyList()
                {
                    company_id = reader["company_id"] == DBNull.Value ? null : reader["company_id"].ToString(),
                    company_relation_id = reader["company_relation_id"] == DBNull.Value ? null : Convert.ToInt32(reader["company_relation_id"].ToString()),
                    purchase_organization_id = reader["purchase_organization_id"] == DBNull.Value ? null : reader["purchase_organization_id"].ToString()
                };
                if (primary_company_id != NewSecondaryCompanyList.company_id) {
                    dict[vendor_id].secondary_company_list.Add(NewSecondaryCompanyList);
                }
                // dict[vendor_id].department.Add("Purchasing");
                // dict[vendor_id].department.Add("Sales");
                // Console.WriteLine(JsonSerializer.Serialize(dict.Values.ToList()));

            }
          }
          cs.Close();
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine("Error query");
        Console.WriteLine(ex);
        new Helpers.GlobalFunction().LogError("ApprovedVendorRegistration", ex.Message);
      }
      return dict.Values.ToList();
    }

        public string GetIsLocked(string vendor_number)
        {
            var result = "";
            try
            {
                using (var cs = new ConnectionService(_appSettings).GetConnection())
                {
                    var sql = "SELECT is_locked from tr_vendor WHERE vendor_number=@vendor_number";

                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@vendor_number", vendor_number);
                    cs.Open();
                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result= reader["is_locked"].ToString();
                        }
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("GetIsLocked", ex.Message);
            }
            Console.WriteLine("ress :"+ result);
            return result;
        }

        public bool DeleteTempVendor(int vendor_id)
        {
        var result = false;
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "DELETE FROM temp_vendor WHERE vendor_id = @vendor_id";
                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@vendor_id", vendor_id);
                    cs.Open();

                    result = command.ExecuteNonQuery() == 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("DeleteTempVendor", ex.Message);
            }
            return result;
        }

        public bool InsertBank(BankModel model)
        {
            
            var result = false;
            try
            {
                var sql = "INSERT INTO m_bank " +
                          "(bank_id, country_of_origin, name, swift_code) " +
                          "values (@bank_id, @country_of_origin, @name, @swift_code)";

                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@bank_id", string.IsNullOrEmpty(model.bank_id) ? DBNull.Value : model.bank_id);
                    command.Parameters.AddWithValue("@country_of_origin", model.country_of_origin);
                    command.Parameters.AddWithValue("@name", model.name);
                    command.Parameters.AddWithValue("@swift_code", (string.IsNullOrEmpty(model.swift_code) ? DBNull.Value : model.swift_code));
                    cs.Open();
                    result = command.ExecuteNonQuery() == 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("InsertBank", ex.Message);
            }
            return result;
        }

        public bool updateBank(BankModel model)
        {

            //Console.WriteLine(model.bank_id + model.name+ model.country_of_origin);
            var result = false;
            try
            {
                var sql = "Update m_bank set " +
                          "country_of_origin = @country_of_origin, name = @name, swift_code= @swift_code " +
                          "WHERE bank_id = @bank_id ";

                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@bank_id", string.IsNullOrEmpty(model.bank_id) ? DBNull.Value : model.bank_id);
                    command.Parameters.AddWithValue("@country_of_origin", model.country_of_origin);
                    command.Parameters.AddWithValue("@name", model.name);
                    command.Parameters.AddWithValue("@swift_code", (string.IsNullOrEmpty(model.swift_code) ? DBNull.Value : model.swift_code));
                    cs.Open();
                    result = command.ExecuteNonQuery() == 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("updateBank", ex.Message);
            }
            //Console.WriteLine(result);
            return result;
        }

        public string GetBank(string bank_id)
        {
             var result = "";
            try
            {
                using (var cs = new ConnectionService(_appSettings).GetConnection())
                {
                    var sql = "SELECT bank_id from m_bank WHERE bank_id=@bank_id";

                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@bank_id", bank_id);
                    cs.Open();
                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result= reader["bank_id"].ToString();
                        }
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("GetBank", ex.Message);
            }
            //Console.WriteLine("ress :"+ result);
            return result;
            }

        public bool updateVendorRpa(VendorModel model)
        {
            var result = false;
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "Update tr_vendor set " +
                              "vendor_type_id = @vendor_type_id, " +
                              "sg_category_id = @sg_category_id, " +
                              "currency_id = @currency_id, " +
                              "top_id = @top_id, " +
                              "title_id = @title_id, " +
                              "name = @name, " +
                              "contact_person = @contact_person, " +
                              "search_term = @search_term, " +
                              "street_address = @street_address, " +
                              "additional_street_address = @additional_street_address, " +
                              "city = @city, " +
                              "postal_code = @postal_code, " +
                              "country_id = @country_id, " +
                              "telephone = @telephone, " +
                              "fax = @fax, email = @email, " +
                              "tax_type_id = @tax_type_id, " +
                              "tax_number_type_id = @tax_number_type_id, " +
                              "npwp = @npwp, id_card_number = @id_card_number, " +
                              "sppkp_number = @sppkp_number, " +
                              "localidr_bank_id = @localidr_bank_id, localidr_bank_account = @localidr_bank_account, " +
                              "localidr_account_holder = @localidr_account_holder, " +
                              "localforex_bank_id = @localforex_bank_id, " +
                              "localforex_account_holder = @localforex_account_holder, " +
                            //   "localforex_bank_holder = @localforex_bank_holder, " +
                              "localforex_currency_id = @localforex_currency_id, " +
                              "foreign_bank_country_id = @foreign_bank_country_id, " +
                              "foreign_bank_id = @foreign_bank_id, " +
                            //   "foreign_bank_name = @foreign_bank_name, " +
                            //   "foreign_bank_swift_code = @foreign_bank_swift_code, " +
                              "foreign_bank_account = @foreign_bank_account, " +
                              "foreign_account_holder = @foreign_account_holder, " +
                              "foreign_currency_id = @foreign_currency_id, " +
                              "reference_correspondent = @reference_correspondent, " +
                              "reference_country_origin = @reference_country_origin, " +
                              "pic_id = @pic_id, " +
                              (string.IsNullOrEmpty(model.file_id_card) ? "": "file_id_card = @file_id_card, ") +
                              (string.IsNullOrEmpty(model.file_npwp) ? "" : "file_npwp = @file_npwp, ") +
                              (string.IsNullOrEmpty(model.file_sppkp) ? "" : "file_sppkp = @file_sppkp, ") +
                              (string.IsNullOrEmpty(model.file_vendor_statement) ? "" : "file_vendor_statement = @file_vendor_statement, ") +
                            //   "verification_status_id = @verification_status_id, " +
                            //   "verification_note = @verification_note, " +
                            //   "position_data = @position_data, token = @token " +
                              "is_locked = @is_locked, " +
                              "change_request_status = @change_request_status, " +
                              "partner_function = @partner_function " +
                              "where vendor_number = @vendor_number";
                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@vendor_type_id", model.vendor_type_id);
                    command.Parameters.AddWithValue("@sg_category_id", model.sg_category_id);
                    command.Parameters.AddWithValue("@currency_id", model.currency_id);
                    command.Parameters.AddWithValue("@top_id", (string.IsNullOrEmpty(model.top_id) ? DBNull.Value : model.top_id));
                    command.Parameters.AddWithValue("@title_id", model.title_id);
                    command.Parameters.AddWithValue("@name", model.name);
                    command.Parameters.AddWithValue("@contact_person", model.contact_person);
                    command.Parameters.AddWithValue("@search_term", model.search_term);
                    command.Parameters.AddWithValue("@street_address", model.street_address);
                    command.Parameters.AddWithValue("@additional_street_address", (string.IsNullOrEmpty(model.additional_street_address) ? DBNull.Value : model.additional_street_address));
                    command.Parameters.AddWithValue("@city", model.city);
                    command.Parameters.AddWithValue("@postal_code", model.postal_code);
                    command.Parameters.AddWithValue("@country_id", model.country_id);
                    command.Parameters.AddWithValue("@telephone", model.telephone);
                    command.Parameters.AddWithValue("@fax", (string.IsNullOrEmpty(model.fax) ? DBNull.Value : model.fax));
                    command.Parameters.AddWithValue("@email", model.email);
                    command.Parameters.AddWithValue("@tax_type_id", model.tax_type_id);
                    command.Parameters.AddWithValue("@tax_number_type_id", model.tax_number_type_id);
                    command.Parameters.AddWithValue("@npwp", (string.IsNullOrEmpty(model.npwp.ToString()) ? DBNull.Value : model.npwp));
                    command.Parameters.AddWithValue("@id_card_number", (string.IsNullOrEmpty(model.id_card_number.ToString()) ? DBNull.Value : model.id_card_number));
                    command.Parameters.AddWithValue("@sppkp_number", (string.IsNullOrEmpty(model.sppkp_number) ? DBNull.Value : model.sppkp_number));
                    command.Parameters.AddWithValue("@localidr_bank_id", (string.IsNullOrEmpty(model.localidr_bank_id) ? DBNull.Value : model.localidr_bank_id));
                    command.Parameters.AddWithValue("@localidr_bank_account", (string.IsNullOrEmpty(model.localidr_bank_account.ToString()) ? DBNull.Value : model.localidr_bank_account));
                    command.Parameters.AddWithValue("@localidr_account_holder", (String.IsNullOrEmpty(model.localidr_account_holder) ? DBNull.Value : model.localidr_account_holder));
                    command.Parameters.AddWithValue("@localforex_bank_id", (String.IsNullOrEmpty(model.localforex_bank_id) ? DBNull.Value : model.localforex_bank_id));
                    command.Parameters.AddWithValue("@localforex_bank_account", (String.IsNullOrEmpty(model.localforex_bank_account.ToString()) ? DBNull.Value : model.localforex_bank_account));
                    command.Parameters.AddWithValue("@localforex_account_holder", (string.IsNullOrEmpty(model.localforex_account_holder) ? DBNull.Value : model.localforex_account_holder));
                    command.Parameters.AddWithValue("@localforex_currency_id", (string.IsNullOrEmpty(model.localforex_currency_id) ? DBNull.Value : model.localforex_currency_id));
                    command.Parameters.AddWithValue("@foreign_bank_country_id", (string.IsNullOrEmpty(model.foreign_bank_country_id) ? DBNull.Value : model.foreign_bank_country_id));
                    command.Parameters.AddWithValue("@foreign_bank_id", (string.IsNullOrEmpty(model.foreign_bank_id) ? DBNull.Value : model.foreign_bank_id));
                    // command.Parameters.AddWithValue("@foreign_bank_name", (string.IsNullOrEmpty(model.foreign_bank_name) ? DBNull.Value : model.foreign_bank_name));
                    // command.Parameters.AddWithValue("@foreign_bank_swift_code", (string.IsNullOrEmpty(model.foreign_bank_swift_code) ? DBNull.Value : model.foreign_bank_swift_code));
                    command.Parameters.AddWithValue("@foreign_bank_account", (string.IsNullOrEmpty(model.foreign_bank_account) ? DBNull.Value : model.foreign_bank_account));
                    command.Parameters.AddWithValue("@foreign_account_holder", (String.IsNullOrEmpty(model.foreign_account_holder) ? DBNull.Value : model.foreign_account_holder));
                    command.Parameters.AddWithValue("@foreign_currency_id", (string.IsNullOrEmpty(model.foreign_currency_id) ? DBNull.Value : model.foreign_currency_id));
                    command.Parameters.AddWithValue("@reference_correspondent", (string.IsNullOrEmpty(model.reference_correspondent) ? DBNull.Value : model.reference_correspondent));
                    command.Parameters.AddWithValue("@reference_country_origin", (string.IsNullOrEmpty(model.reference_country_origin) ? DBNull.Value : model.reference_country_origin));
                    command.Parameters.AddWithValue("@pic_id", model.pic_id);
                    if ( !String.IsNullOrEmpty(model.file_id_card))
                        command.Parameters.AddWithValue("@file_id_card", model.file_id_card);
                    if (!String.IsNullOrEmpty(model.file_npwp))
                        command.Parameters.AddWithValue("@file_npwp", (string.IsNullOrEmpty(model.file_npwp) ? DBNull.Value : model.file_npwp));
                    if (!String.IsNullOrEmpty(model.file_sppkp))
                        command.Parameters.AddWithValue("@file_sppkp", (string.IsNullOrEmpty(model.file_sppkp) ? DBNull.Value : model.file_sppkp));
                    if (!String.IsNullOrEmpty(model.file_vendor_statement))
                        command.Parameters.AddWithValue("@file_vendor_statement", (string.IsNullOrEmpty(model.file_vendor_statement) ? DBNull.Value : model.file_vendor_statement));
                    // command.Parameters.AddWithValue("@verification_status_id", model.verification_status_id);
                    // command.Parameters.AddWithValue("@verification_note", (string.IsNullOrEmpty(model.verification_note) ? DBNull.Value : model.verification_note));
                    // command.Parameters.AddWithValue("@position_data", model.position_data);
                    command.Parameters.AddWithValue("@is_locked", model.is_locked);
                    command.Parameters.AddWithValue("@change_request_status", model.change_request_status);
                    command.Parameters.AddWithValue("@token", (string.IsNullOrEmpty(model.token) ? DBNull.Value: model.token));
                    command.Parameters.AddWithValue("@partner_function", (String.IsNullOrEmpty(model.partner_function) ? DBNull.Value : model.partner_function));
                    command.Parameters.AddWithValue("@vendor_number", model.vendor_number);
                    cs.Open();

                    result = command.ExecuteNonQuery() == 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("updateVendorRpa", ex.Message);
            }
            return result;
        }

        public VendorModel mappingRegisToVendor(VendorModel DataVendor, RegistrationModel model, dynamic custom ){
          VendorModel dataMapping = new VendorModel(){
              vendor_number = model.vendor_number,
              vendor_type_id = model.vendor_type_id,
              account_group_id = model.account_group_id,
              vendor_type_name  = DataVendor.vendor_type_name,
              vendor_type_code = DataVendor.vendor_type_code,
              scheme_group = DataVendor.scheme_group,
              scheme_group_name = DataVendor.scheme_group_name,
              gl = DataVendor.gl,
              gl_name = DataVendor.gl_name,
              sg_category_id = model.sg_category_id,
              sg_category_name = DataVendor.sg_category_name,
              company = model.company, 
              currency_id = model.currency_id,
              top_id = model.top_id,
              top_name = DataVendor.top_name,
              title_id = model.title_id,
              name = model.name,
              contact_person = model.contact_person,
              search_term = model.search_term,
              street_address = model.street_address,
              additional_street_address = model.additional_street_address,
              city = model.city,
              postal_code = model.postal_code,
              country_id = model.country_id,
              country_name = DataVendor.country_name,
              telephone = model.telephone,
              fax = model.fax,
              email = model.email,
              email_list = DataVendor.email_list,
              tax_type_id = model.tax_type_id,
              tax_type_name = DataVendor.tax_type_name,
              wht_code = DataVendor.wht_code,
              tax_rates = DataVendor.tax_rates,
              tax_number_type_id = model.tax_number_type_id,
              tax_number_type_code = DataVendor.tax_number_type_code, 
              tax_number_type_name = DataVendor.tax_number_type_name, 
              npwp = DataVendor.npwp,
              id_card_number = DataVendor.id_card_number,
              sppkp_number = model.sppkp_number,
              localidr_bank_id = model.localidr_bank_id,
              localidr_bank_name = DataVendor.localidr_bank_name,
              localidr_swift_code = DataVendor.localidr_swift_code,
              localidr_bank_account = model.localidr_bank_account,
              localidr_account_holder = model.localidr_account_holder,
              localforex_bank_id = model.localforex_bank_id,
              localforex_bank_name= DataVendor.localforex_bank_name, 
              localforex_swift_code = DataVendor.localidr_swift_code,
              localforex_bank_account = model.localforex_bank_account,
              localforex_account_holder = model.localforex_account_holder,
              localforex_currency_id = model.localforex_currency_id,
              foreign_bank_country_id = model.foreign_bank_country_id,
              foreign_bank_country_name = DataVendor.foreign_bank_country_name,
              foreign_bank_id = model.foreign_bank_id,
              foreign_bank_name = model.foreign_bank_name,
              foreign_bank_swift_code = model.foreign_bank_swift_code,
              foreign_bank_account = model.foreign_bank_account,
              foreign_account_holder = model.foreign_account_holder,
              foreign_currency_id = model.foreign_currency_id,
              reference_correspondent = model.reference_correspondent,
              reference_country_origin = model.reference_country_origin,
              pic_id = model.pic_id,
              person_number = DataVendor.person_number,
              email_procurement_group = DataVendor.email_procurement_group,
              file_id_card = custom.file_id_card_path,
              file_npwp = custom.file_npwp_path,
              file_sppkp = custom.file_sppkp_path,
              file_vendor_statement = custom.file_vendor_statement_path,
              is_locked = DataVendor.is_locked,
              position_data = model.position_data,
              token = model.token,
              created_at = DataVendor.created_at,
              updated_at = DataVendor.updated_at,
              log_vendor = DataVendor.log_vendor,
              change_request_status = DataVendor.change_request_status,
              change_request_note = DataVendor.change_request_note,
              partner_function = model.partner_function,
              partner_function_name = DataVendor.partner_function_name,
              status_rpa = DataVendor.status_rpa,
              rpa_status_description = DataVendor.rpa_status_description
            };
            
            return dataMapping;
        }

        public Dictionary<string, string> dictionaryField(){
          Dictionary<string, string> dictField = new Dictionary<string, string>();

            dictField.Add("vendor_number", "Vendor Number");
            dictField.Add("vendor_type_id", "Vendor Type");
            dictField.Add("account_group_id", "Account Group");
            dictField.Add("vendor_type_name ", "Vendor Type");
            dictField.Add("vendor_type_code", "Vendor Type Code");
            dictField.Add("scheme_group", "Scheme Group");
            dictField.Add("scheme_group_name", "Scheme Group Name");
            dictField.Add("gl", "GL");
            dictField.Add("gl_name", "GL Name");
            dictField.Add("sg_category_id", "SG Category");
            dictField.Add("sg_category_name", "SG Category Name");
            dictField.Add("currency_id", "Currency ID");
            dictField.Add("top_id", "Term of payment (TOP)");
            // dictField.Add("top_interval", "TOP Interval");
            dictField.Add("top_name", "TOP Name");
            dictField.Add("title_id", "Title");
            dictField.Add("name", "Vendor Name");
            dictField.Add("contact_person", "Contact Person");
            dictField.Add("search_term", "Search Term");
            dictField.Add("street_address", "Address");
            dictField.Add("additional_street_address", "Additional Address");
            dictField.Add("city", "City");
            dictField.Add("postal_code", "Postal Code");
            dictField.Add("country_id", "Country");
            dictField.Add("country_name", "Country Name");
            dictField.Add("telephone", "Telephone");
            dictField.Add("fax", "Fax");
            dictField.Add("email", "Email");
            dictField.Add("email_list", "Email List");
            dictField.Add("tax_type_id", "Tax Type");
            dictField.Add("tax_type_name", "Tax Type Name");
            dictField.Add("wht_code", "WHT Code");
            dictField.Add("tax_rates", "Tax Rates");
            dictField.Add("tax_number_type_id", "Tax Number Type");
            dictField.Add("tax_number_type_code", "Tax Number Type Code");
            dictField.Add("tax_number_type_name", "Tax Number Type Name");
            dictField.Add("npwp", "NPWP");
            dictField.Add("id_card_number", "ID Card");
            dictField.Add("sppkp_number", "SPPKP Number");
            dictField.Add("localidr_bank_id", "Local IDR Bank");
            dictField.Add("localidr_bank_name", "Local IDR Bank Name");
            dictField.Add("localidr_swift_code", "Local IDR Swift Code");
            dictField.Add("localidr_bank_account", "Local IDR Bank Account");
            dictField.Add("localidr_account_holder", "Local IDR Account Holder");
            dictField.Add("localforex_bank_id", "Local Forex Bank");
            dictField.Add("localforex_bank_name", "Local Forex Bank Name");
            // dictField.Add("localforex_swift_code", "Local Forex Swift Code");
            dictField.Add("localforex_bank_account", "Local Forex Bank Account");
            dictField.Add("localforex_account_holder", "Local Forex Account Holder");
            dictField.Add("localforex_currency_id", "Local Forex Currency");
            dictField.Add("foreign_bank_country_id", "Foreign Bank");
            dictField.Add("foreign_bank_country_name", "Foreign Bank Country Name");
            dictField.Add("foreign_bank_id", "Foreign Bank");
            dictField.Add("foreign_bank_name", "Foreign Bank Name");
            dictField.Add("foreign_bank_swift_code", "Foreign Bank Swift COde");
            dictField.Add("foreign_bank_account", "Foreign Bank Account");
            dictField.Add("foreign_account_holder", "Foreign Bank Holder");
            dictField.Add("foreign_currency_id", "Foreign Bank Currency");
            dictField.Add("reference_correspondent", "Reference Correspondent");
            dictField.Add("reference_country_origin", "Reference Country Origin");
            dictField.Add("pic_id", "PIC");
            dictField.Add("person_number", "Person Number");
            dictField.Add("email_procurement_group", "Email PIC");
            dictField.Add("file_id_card", "File ID Card");
            dictField.Add("file_npwp", "File NPWP");
            dictField.Add("file_sppkp", "File SPPKP");
            dictField.Add("file_vendor_statement", "File Vendor Statement");
            dictField.Add("is_locked", "Is Locked");
            dictField.Add("created_at", "Created At");
            dictField.Add("updated_at", "Updated At");
            dictField.Add("log_vendor", "Log Vendor");
            dictField.Add("change_request_status", "Status");
            dictField.Add("change_request_note", "Note");
            dictField.Add("partner_function", "Partner Function");
            dictField.Add("partner_function_name", "Partner Function Name");
            dictField.Add("status_rpa", "Status");
            dictField.Add("rpa_status_description", "Status Description");

            return dictField;
        }
        
        public List<String> getListUpdatedColumn(VendorModel data_existing, VendorModel data_update)
        {
            List<String> result = new List<String>();
            String[] excludeField = {"top_interval", "localforex_swift_code", "is_extension", "type", "company", "company_id" , "verification_status_id", "verification_note", "position_data", "token","updated_data"};
            PropertyInfo[] data_existing_properties = data_existing.GetType().GetProperties();
            Console.WriteLine("Count properties: " + JsonSerializer.Serialize(data_existing_properties.Count()));
            foreach (var item in data_existing_properties)
            {
                
                var key_name = item.Name;

                if(!excludeField.Contains(key_name)){
                    Console.WriteLine("Key name " + key_name);
                    var value_existing = data_existing.GetType().GetProperty(key_name).GetValue(data_existing, null);
                    var updated_value = data_update.GetType().GetProperty(key_name).GetValue(data_update, null);
                    Console.WriteLine("Value existing : " + value_existing);
                    Console.WriteLine("Updated value : " + updated_value);
                    if ( updated_value != null && !Comparer.Equals(value_existing, updated_value)) 
                    {
                        Console.WriteLine("Diff Field : " + updated_value + " " + value_existing);
                        result.Add(key_name);
                    }
                }
                
            }
            return result;
        }

        public IEnumerable<CompanyModel> GetCompanyRelation(string vendor_number)
        {
            List<CompanyModel> result = new List<CompanyModel>();
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "SELECT A.purchase_organization_id, C.name AS purchase_organization_name, B.* " +
                              "FROM tr_company_relations A " +
                              "JOIN m_company B ON A.company_id = B.company_id " +
                              "LEFT JOIN m_purchase_organization C ON A.purchase_organization_id = C.purchase_organization_id " +
                              "WHERE A.vendor_number = @vendor_number";
                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@vendor_number", vendor_number);
                    cs.Open();

                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result.Add(new CompanyModel()
                            {
                                purchase_organization_id = reader["purchase_organization_id"] == DBNull.Value ? null : reader["purchase_organization_id"].ToString(),
                                purchase_organization_name = reader["purchase_organization_name"] == DBNull.Value ? null : reader["purchase_organization_name"].ToString(),
                                company_id = reader["company_id"].ToString(),
                                name = reader["name"].ToString(),
                                created_at = Convert.ToDateTime(reader["created_at"].ToString()),
                                updated_at = Convert.ToDateTime(reader["updated_at"].ToString())
                            });
                        }
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("GetCompanyRelation", ex.Message);
            }
            return result;
        }

        public int updateIsExtension(int vendor_id, int is_extension)
        {
            var result = 0;
            try {
                using(var cs = new ConnectionService(_appSettings).GetConnection())
                {
                    var sql = "UPDATE temp_vendor SET is_extension = @is_extension WHERE vendor_id = @vendor_id"; 

                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@is_extension", is_extension);
                    command.Parameters.AddWithValue("@vendor_id", vendor_id);
                    cs.Open();

                    Console.WriteLine(sql);

                    var reader = command.ExecuteNonQuery();
                    Console.WriteLine(reader);

                    result = reader;
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("updateIsExtension", ex.Message);
            }
            return result;
        }

        public bool CekBankExist(string bank_id)
        {
            bool result = false;
            try
            {
                using (var cs = new ConnectionService(_appSettings).GetConnection())
                {
                    var sql = "SELECT COUNT(*) AS TOTAL from m_bank WHERE bank_id=@bank_id";

                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@bank_id", bank_id);
                    cs.Open();
                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result= Convert.ToInt32(reader["TOTAL"].ToString())> 0 ? true : false;
                        }
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("CekBankExist", ex.Message);
            }
            //Console.WriteLine("ress :"+ result);
            return result;
        }

        public List<String> getCompanyId()
        {
            List<String> result = new List<String>();
            try
            {
                using (var cs = new ConnectionService(_appSettings).GetConnection())
                {
                    var sql = "SELECT company_id from m_company";

                    SqlCommand command = new SqlCommand(sql, cs);
                    cs.Open();
                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result.Add(reader["company_id"].ToString());
                        }
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("getCompanyId", ex.Message);
            }
            Console.WriteLine("ress :"+ result);
            return result;
        }

        public async Task<bool> DeleteBank(string bank_id)
        {
             var result = false;
            try {
                using(var cs = new ConnectionService(_appSettings).GetConnection())
                {
                    //Console.WriteLine("masuk" + bank_id);
                    cs.Open();
                    SqlTransaction trans = cs.BeginTransaction();
                    var DeleteBank = await cs.ExecuteAsync(@"UPDATE m_bank SET is_delete = 1 WHERE bank_id = @bank_id", new {
                        bank_id = bank_id
                    }, transaction: trans);
                    if (DeleteBank > 0) {
                        result = true;
                    }
                    trans.Commit();
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine("error: "+ex);
                new Helpers.GlobalFunction().LogError("DeleteBank", ex.Message);
            }
            //Console.WriteLine("res "+result);
            return result;
        }

        public bool updateMaterial(MaterialItemModel model)
        {
            var result = false;
            try
            {
                var sql = "Update m_material_items set " +
                          "material_plnt = @material_plnt, description = @description, is_delete = 0 " +
                          "WHERE material_id = @material_id ";

                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@material_id", string.IsNullOrEmpty(model.material_id) ? DBNull.Value : model.material_id);
                    command.Parameters.AddWithValue("@material_plnt", model.material_plnt);
                    command.Parameters.AddWithValue("@description", model.description);
                    cs.Open();
                    result = command.ExecuteNonQuery() == 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("updateMaterial", ex.Message);
            }
            return result;
        }

        public bool InsertMaterial(MaterialItemModel model)
        {
            var result = false;
            try
            {
                var sql = "INSERT INTO m_material_items " +
                          "(material_id, material_plnt, description) " +
                          "values (@material_id, @material_plnt, @description)";

                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@material_id", string.IsNullOrEmpty(model.material_id) ? DBNull.Value : model.material_id);
                    command.Parameters.AddWithValue("@material_plnt", model.material_plnt);
                    command.Parameters.AddWithValue("@description", model.description);
                    cs.Open();
                    result = command.ExecuteNonQuery() == 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("InsertMaterial", ex.Message);
            }
            return result;
        }

        public async Task<bool> DeleteMaterial(string material_id)
        {
            var result = false;
            try {
                using(var cs = new ConnectionService(_appSettings).GetConnection())
                {
                    cs.Open();
                    SqlTransaction trans = cs.BeginTransaction();
                    var DeleteMaterial = await cs.ExecuteAsync(@"UPDATE m_material_items SET is_delete = 1  WHERE material_id = @material_id", new {
                        material_id = material_id
                    }, transaction: trans);
                    if (DeleteMaterial > 0) {
                        result = true;
                    }
                    trans.Commit();
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("error: "+ex);
                new Helpers.GlobalFunction().LogError("DeleteMaterial", ex.Message);
            }
            return result;
        }

        public CostCenterModel GetCcItems(string cc_id)
        {
            CostCenterModel result = new CostCenterModel();
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "SELECT * FROM m_cc_items" +
                              " WHERE cc_id = @cc_id";

                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@cc_id", cc_id);
                    cs.Open();
                    var reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result.cc_id = reader["cc_id"].ToString();
                            result.description = reader["description"].ToString();
                        }
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("GetCcItems", ex.Message);
            }
            return result;
        }

        public bool updatecostCenter(CostCenterModel model)
        {
           var result = false;
            try
            {
                var sql = "Update m_cc_items set " +
                          "description = @description, is_delete = 0 " +
                          "WHERE cc_id = @cc_id ";

                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@cc_id", string.IsNullOrEmpty(model.cc_id) ? DBNull.Value : model.cc_id);
                    command.Parameters.AddWithValue("@description", model.description);
                    cs.Open();
                    result = command.ExecuteNonQuery() == 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("updatecostCenter", ex.Message);
            }
            return result;
        }

        public bool InsertCostCenter(CostCenterModel model)
        {
            var result = false;
            try
            {
                var sql = "INSERT INTO m_cc_items " +
                          "(cc_id, description) " +
                          "values (@cc_id, @description)";

                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@cc_id", string.IsNullOrEmpty(model.cc_id) ? DBNull.Value : model.cc_id);
                    command.Parameters.AddWithValue("@description", model.description);
                    cs.Open();
                    result = command.ExecuteNonQuery() == 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("InsertCostCenter", ex.Message);
            }
            return result;
        }

        public async Task<bool> DeleteCostCenter(string cc_id)
        {
           var result = false;
            try {
                using(var cs = new ConnectionService(_appSettings).GetConnection())
                {
                    cs.Open();
                    SqlTransaction trans = cs.BeginTransaction();
                    var DeleteCostCenter = await cs.ExecuteAsync(@"UPDATE m_cc_items SET is_delete = 1 WHERE cc_id = @cc_id", new {
                        cc_id = cc_id
                    }, transaction: trans);
                    if (DeleteCostCenter > 0) {
                        result = true;
                    }
                    trans.Commit();
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("error: "+ex);
                new Helpers.GlobalFunction().LogError("DeleteCostCenter", ex.Message);
            }
            return result;
        }

        public CompanyModel GetCompanyDetail(string company_id)
        {
            CompanyModel result = new CompanyModel();
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "SELECT * FROM m_company" +
                              " WHERE company_id = @company_id";

                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@company_id", company_id);
                    cs.Open();
                    var reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result.company_id = reader["company_id"].ToString();
                            result.name = reader["name"].ToString();
                        }
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("GetCompanyDetail", ex.Message);
            }
            return result;
        }

        public bool updateCompany(CompanyModel model)
        {
            var result = false;
            try
            {
                var sql = "Update m_company set " +
                          "name = @name, is_delete = 0 " +
                          "WHERE company_id = @company_id ";

                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@company_id", string.IsNullOrEmpty(model.company_id) ? DBNull.Value : model.company_id);
                    command.Parameters.AddWithValue("@name", model.name);
                    cs.Open();
                    result = command.ExecuteNonQuery() == 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("updateCompany", ex.Message);
            }
            return result;
        }

        public bool InsertCompany(CompanyModel model)
        {
            var result = false;
            try
            {
                var sql = "INSERT INTO m_company " +
                          "(company_id, name) " +
                          "values (@company_id, @name)";

                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@company_id", string.IsNullOrEmpty(model.company_id) ? DBNull.Value : model.company_id);
                    command.Parameters.AddWithValue("@name", model.name);
                    cs.Open();
                    result = command.ExecuteNonQuery() == 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("InsertCompany", ex.Message);
            }
            return result;
        }

        public async Task<bool> DeleteCompany(string company_id)
        {
            var result = false;
            try {
                using(var cs = new ConnectionService(_appSettings).GetConnection())
                {
                    cs.Open();
                    SqlTransaction trans = cs.BeginTransaction();
                    var DeleteCompany = await cs.ExecuteAsync(@"UPDATE m_company set is_delete = 1 WHERE company_id = @company_id", new {
                        company_id = company_id
                    }, transaction: trans);
                    if (DeleteCompany > 0) {
                        result = true;
                    }
                    trans.Commit();
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("error: "+ex);
                new Helpers.GlobalFunction().LogError("DeleteCompany", ex.Message);
            }
            return result;
        }

        public VendorTypeModel GetVendorGroupDetail(int vendor_type_id)
        {
             VendorTypeModel result = new VendorTypeModel();
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "SELECT * FROM m_vendor_type" +
                              " WHERE vendor_type_id = @vendor_type_id";

                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@vendor_type_id", vendor_type_id);
                    cs.Open();
                    var reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result.vendor_type_id = Convert.ToInt32(reader["vendor_type_id"].ToString());
                            result.name = reader["name"].ToString();
                            result.sap_code = reader["sap_code"].ToString();
                            result.scheme_group_id = reader["scheme_group_id"].ToString();
                            result.gl_id = reader["gl_id"].ToString();

                        }
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("GetVendorGroupDetail", ex.Message);
            }
            return result;
        }

        public bool updateVendorGroup(VendorTypeModel model)
        {
            var result = false;
            try
            {
                var sql = "Update m_vendor_type set " +
                          "name = @name, sap_code =@sap_code, scheme_group_id = @scheme_group_id, gl_id = @gl_id, is_delete = 0 " +
                          "WHERE vendor_type_id = @vendor_type_id ";

                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@vendor_type_id", string.IsNullOrEmpty(model.vendor_type_id.ToString()) ? DBNull.Value : model.vendor_type_id.ToString());
                    command.Parameters.AddWithValue("@name", model.name);
                    command.Parameters.AddWithValue("@sap_code", model.sap_code);
                    command.Parameters.AddWithValue("@scheme_group_id", model.scheme_group_id);
                    command.Parameters.AddWithValue("@gl_id", model.gl_id);
                    cs.Open();
                    result = command.ExecuteNonQuery() == 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                new Helpers.GlobalFunction().LogError("updateVendorGroup", ex.Message);
            }
            return result;
        }

        public bool InsertVendorGroup(VendorTypeModel model)
        {
             var result = false;
            try
            {
                var sql = "INSERT INTO m_vendor_type " +
                          "(name, sap_code, scheme_group_id, gl_id) " +
                          "values (@name, @sap_code, @scheme_group_id, @gl_id)";

                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@vendor_type_id", string.IsNullOrEmpty(model.vendor_type_id.ToString()) ? DBNull.Value : model.vendor_type_id.ToString());
                    command.Parameters.AddWithValue("@name", model.name);
                    command.Parameters.AddWithValue("@sap_code", model.sap_code);
                    command.Parameters.AddWithValue("@scheme_group_id", model.scheme_group_id);
                    command.Parameters.AddWithValue("@gl_id", model.gl_id);
                    cs.Open();
                    result = command.ExecuteNonQuery() == 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("InsertVendorGroup", ex.Message);
            }
            return result;
        }

        public async Task<bool> DeleteVendorGroup(int vendor_type_id)
        {
             var result = false;
            try {
                using(var cs = new ConnectionService(_appSettings).GetConnection())
                {
                    cs.Open();
                    SqlTransaction trans = cs.BeginTransaction();
                    var DeleteVendorGroup = await cs.ExecuteAsync(@"UPDATE m_vendor_type SET is_delete = 1 WHERE vendor_type_id = @vendor_type_id", new {
                        vendor_type_id = vendor_type_id
                    }, transaction: trans);
                    if (DeleteVendorGroup > 0) {
                        result = true;
                    }
                    trans.Commit();
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("error: "+ex);
                new Helpers.GlobalFunction().LogError("DeleteVendorGroup", ex.Message);
            }
            return result;
        }

        public Dictionary<string, dynamic> GetBankListDatatable(Dictionary<string, string> pagination, Dictionary<string, string> sort, Dictionary<string, string> query)
        {
            int page = pagination.ContainsKey("page") ? Convert.ToInt32(pagination["page"]): 1;
            int perpage = pagination.ContainsKey("perpage") ? Convert.ToInt32(pagination["perpage"]): 10;
            string sorttype = sort.ContainsKey("sort") ? string.IsNullOrEmpty(sort["sort"]) ? "asc" : sort["sort"]: "asc"; 
            string orderby = sort.ContainsKey("field") ? string.IsNullOrEmpty(sort["field"]) ? "bank_id" : "" + sort["field"]: "bank_id";
            string generalSearch = query.ContainsKey("generalSearch") ? string.IsNullOrEmpty(query["generalSearch"]) ? null : query["generalSearch"] : null;
            
            List<Dictionary<string, dynamic>> result = new List<Dictionary<string, dynamic>>();
            int total_data = 0;
            int total_page = 0;
            int offset = (page - 1)*perpage;
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    string string_condition = "";
                    if (!string.IsNullOrEmpty(generalSearch))
                    {
                        string_condition += " AND name LIKE @name";
                    }
                    string string_order = " ORDER BY " + orderby + " " + sorttype;
                    var sql = "SELECT * FROM m_bank WHERE 1=1 AND is_delete = 0 " + string_condition + " "+ string_order + " " +
                              "OFFSET @offset ROWS FETCH NEXT @perpage ROWS ONLY";
                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@offset", offset);
                    command.Parameters.AddWithValue("@perpage", perpage);
                    if (!string.IsNullOrEmpty(generalSearch))
                    {
                        command.Parameters.AddWithValue("@name", "%" + generalSearch + "%");
                    }
                    
                    cs.Open();
                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Dictionary<string, dynamic> data_row = new Dictionary<string, dynamic>();
                            data_row.Add("bank_id", reader["bank_id"].ToString());
                            data_row.Add("country_of_origin", reader["country_of_origin"].ToString());
                            data_row.Add("name", reader["name"].ToString());
                            data_row.Add("swift_code", reader["swift_code"].ToString());
                            result.Add(data_row);                            
                        }
                    }
                    cs.Close();
                }
                total_data = this.CountBankData(generalSearch);
                total_page = (total_data + perpage - 1) / perpage;
                
            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("GetBankListDatatable", ex.Message);
                Console.WriteLine(ex.Message);
            }
            Dictionary<string, dynamic> ret = new Dictionary<string, dynamic>();
            var meta = new Dictionary<string, dynamic>();
            meta.Add("page", page);
            meta.Add("pages", total_page);
            meta.Add("perpage", perpage);
            meta.Add("total", total_data);
            meta.Add("sort", sorttype);
            meta.Add("generalSearch", generalSearch);
            meta.Add("field", sort.ContainsKey("field") ? sort["field"]: null);

            ret.Add("meta", meta);
            ret.Add("data", result);
            return ret;
        }

        public int CountBankData(string generalSearch)
        {
            int result = 0;
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    string string_condition = "";
                    if (!string.IsNullOrEmpty(generalSearch)) {
                        string_condition += " AND A.name LIKE @generalSearch";
                    }

                    var sql = "SELECT COUNT(*) AS total " +
                              "FROM m_bank A WHERE is_delete = 0 " + string_condition ;

                    SqlCommand command = new SqlCommand(sql, cs);
                    if (!string.IsNullOrEmpty(generalSearch))
                    {
                        command.Parameters.AddWithValue("@generalSearch", "%" + generalSearch + "%");
                    }

                    cs.Open();

                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result = Convert.ToInt32(reader["total"].ToString());
                        }
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("CountBankData", ex.Message);
            }
            return result;
        }

        public Dictionary<string, dynamic> GetMaterialListDatatable(Dictionary<string, string> pagination, Dictionary<string, string> sort, Dictionary<string, string> query)
        {
            int page = pagination.ContainsKey("page") ? Convert.ToInt32(pagination["page"]): 1;
            int perpage = pagination.ContainsKey("perpage") ? Convert.ToInt32(pagination["perpage"]): 10;
            string sorttype = sort.ContainsKey("sort") ? string.IsNullOrEmpty(sort["sort"]) ? "desc" : sort["sort"]: "desc"; 
            string orderby = sort.ContainsKey("created_at") ? string.IsNullOrEmpty(sort["created_at"]) ? "material_id" : "" + sort["created_at"]: "material_id";
            string generalSearch = query.ContainsKey("generalSearch") ? string.IsNullOrEmpty(query["generalSearch"]) ? null : query["generalSearch"] : null;

            //Console.WriteLine(";generalsearch: "+generalSearch);
            List<Dictionary<string, dynamic>> result = new List<Dictionary<string, dynamic>>();
            int total_data = 0;
            int total_page = 0;
            int offset = (page - 1)*perpage;
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                     string string_condition = "";
                    if (!string.IsNullOrEmpty(generalSearch))
                    {
                        string_condition += " AND description LIKE @description";
                    }
                    string string_order = " ORDER BY " + orderby + " " + sorttype;
                    var sql = "SELECT * FROM m_material_items WHERE 1=1 AND is_delete = 0"  + string_condition + " "+ string_order + " " +
                              "OFFSET @offset ROWS FETCH NEXT @perpage ROWS ONLY";
                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@offset", offset);
                    command.Parameters.AddWithValue("@perpage", perpage);
                    if (!string.IsNullOrEmpty(generalSearch))
                    {
                        command.Parameters.AddWithValue("@description", "%" + generalSearch + "%");
                    }
                    
                    cs.Open();
                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Dictionary<string, dynamic> data_row = new Dictionary<string, dynamic>();
                            data_row.Add("material_id", reader["material_id"].ToString());
                            data_row.Add("material_plnt", reader["material_plnt"].ToString());
                            data_row.Add("description", reader["description"].ToString());
                            result.Add(data_row);                            
                        }
                    }
                    cs.Close();
                }
                total_data = this.CountMaterialData(generalSearch);
                total_page = (total_data + perpage - 1) / perpage;
                
            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("GetMaterialListDatatable", ex.Message);
                Console.WriteLine(ex.Message);
            }
            Dictionary<string, dynamic> ret = new Dictionary<string, dynamic>();
            var meta = new Dictionary<string, dynamic>();
            meta.Add("page", page);
            meta.Add("pages", total_page);
            meta.Add("perpage", perpage);
            meta.Add("total", total_data);
            meta.Add("sort", sorttype);
            meta.Add("generalSearch", generalSearch);
            meta.Add("field", sort.ContainsKey("field") ? sort["field"]: null);

            ret.Add("meta", meta);
            ret.Add("data", result);
            return ret;
        }

        public int CountMaterialData(string generalSearch)
        {
             int result = 0;
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    string string_condition = "";
                    if (!string.IsNullOrEmpty(generalSearch)) {
                        string_condition += " AND description LIKE @generalSearch";
                    }
                    var sql = "SELECT COUNT(*) AS total " +
                              "FROM m_material_items WHERE is_delete = 0 " + string_condition ;

                    SqlCommand command = new SqlCommand(sql, cs);
                    if (!string.IsNullOrEmpty(generalSearch))
                    {
                        command.Parameters.AddWithValue("@generalSearch", "%" + generalSearch + "%");
                    }

                    cs.Open();

                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result = Convert.ToInt32(reader["total"].ToString());
                        }
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("CountMaterialData", ex.Message);
            }
            return result;
        }

        public Dictionary<string, dynamic> GetCompanyCodeListDatatable(Dictionary<string, string> pagination, Dictionary<string, string> sort, Dictionary<string, string> query)
        {
             int page = pagination.ContainsKey("page") ? Convert.ToInt32(pagination["page"]): 1;
            int perpage = pagination.ContainsKey("perpage") ? Convert.ToInt32(pagination["perpage"]): 10;
            string sorttype = sort.ContainsKey("sort") ? string.IsNullOrEmpty(sort["sort"]) ? "asc" : sort["sort"]: "asc"; 
            string orderby = sort.ContainsKey("field") ? string.IsNullOrEmpty(sort["field"]) ? "company_id" : "" + sort["field"]: "company_id";
            string generalSearch = query.ContainsKey("generalSearch") ? string.IsNullOrEmpty(query["generalSearch"]) ? null : query["generalSearch"] : null;

            List<Dictionary<string, dynamic>> result = new List<Dictionary<string, dynamic>>();
            int total_data = 0;
            int total_page = 0;
            int offset = (page - 1)*perpage;
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    string string_condition = "";
                    if (!string.IsNullOrEmpty(generalSearch))
                    {
                        string_condition += " AND name LIKE @name";
                    }
                    string string_order = " ORDER BY " + orderby + " " + sorttype;
                    var sql = "SELECT * FROM m_company WHERE 1=1 AND is_delete = 0 " + string_condition + " " + string_order + " " +
                              "OFFSET @offset ROWS FETCH NEXT @perpage ROWS ONLY";
                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@offset", offset);
                    command.Parameters.AddWithValue("@perpage", perpage);
                    if (!string.IsNullOrEmpty(generalSearch))
                    {
                        command.Parameters.AddWithValue("@name", "%" + generalSearch + "%");
                    }

                    cs.Open();
                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Dictionary<string, dynamic> data_row = new Dictionary<string, dynamic>();
                            data_row.Add("company_id", reader["company_id"].ToString());
                            data_row.Add("name", reader["name"].ToString());
                            result.Add(data_row);                            
                        }
                    }
                    cs.Close();
                }
                total_data = this.CountCompanyCodeData(generalSearch);
                total_page = (total_data + perpage - 1) / perpage;
                
            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("GetCompanyCodeListDatatable", ex.Message);
                Console.WriteLine(ex.Message);
            }
            Dictionary<string, dynamic> ret = new Dictionary<string, dynamic>();
            var meta = new Dictionary<string, dynamic>();
            meta.Add("page", page);
            meta.Add("pages", total_page);
            meta.Add("perpage", perpage);
            meta.Add("total", total_data);
            meta.Add("sort", sorttype);
            meta.Add("generalSearch", generalSearch);
            meta.Add("field", sort.ContainsKey("field") ? sort["field"]: null);

            ret.Add("meta", meta);
            ret.Add("data", result);
            return ret;
        }

        public int CountCompanyCodeData(string generalSearch)
        {
             int result = 0;
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    string string_condition = "";
                    if (!string.IsNullOrEmpty(generalSearch)) {
                        string_condition += " AND A.name LIKE @generalSearch";
                    }

                    var sql = "SELECT COUNT(*) AS total " +
                              "FROM m_company A WHERE is_delete = 0 " + string_condition ;

                    SqlCommand command = new SqlCommand(sql, cs);
                    if (!string.IsNullOrEmpty(generalSearch))
                    {
                        command.Parameters.AddWithValue("@generalSearch", "%" + generalSearch + "%");
                    }
                    cs.Open();

                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result = Convert.ToInt32(reader["total"].ToString());
                        }
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("CountCompanyCodeData", ex.Message);
            }
            return result;
        }

        public Dictionary<string, dynamic> GetCcItemsListDatatable(Dictionary<string, string> pagination, Dictionary<string, string> sort, Dictionary<string, string> query)
        {
             int page = pagination.ContainsKey("page") ? Convert.ToInt32(pagination["page"]): 1;
            int perpage = pagination.ContainsKey("perpage") ? Convert.ToInt32(pagination["perpage"]): 10;
            string sorttype = sort.ContainsKey("sort") ? string.IsNullOrEmpty(sort["sort"]) ? "asc" : sort["sort"]: "asc"; 
            string orderby = sort.ContainsKey("field") ? string.IsNullOrEmpty(sort["field"]) ? "cc_id" : "" + sort["field"]: "cc_id";
            string generalSearch = query.ContainsKey("generalSearch") ? string.IsNullOrEmpty(query["generalSearch"]) ? null : query["generalSearch"] : null;

            List<Dictionary<string, dynamic>> result = new List<Dictionary<string, dynamic>>();
            int total_data = 0;
            int total_page = 0;
            int offset = (page - 1)*perpage;
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                     string string_condition = "";
                    if (!string.IsNullOrEmpty(generalSearch))
                    {
                        string_condition += " AND description LIKE @description";
                    }
                    string string_order = " ORDER BY " + orderby + " " + sorttype;
                    var sql = "SELECT * FROM m_cc_items WHERE 1=1 AND is_delete = 0" + string_condition + " " + string_order + " " +
                              "OFFSET @offset ROWS FETCH NEXT @perpage ROWS ONLY";
                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@offset", offset);
                    command.Parameters.AddWithValue("@perpage", perpage);
                    if (!string.IsNullOrEmpty(generalSearch))
                    {
                        command.Parameters.AddWithValue("@description", "%" + generalSearch + "%");
                    }
                    
                    cs.Open();
                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Dictionary<string, dynamic> data_row = new Dictionary<string, dynamic>();
                            data_row.Add("cc_id", reader["cc_id"].ToString());
                            data_row.Add("description", reader["description"].ToString());
                            result.Add(data_row);                            
                        }
                    }
                    cs.Close();
                }
                total_data = this.CountCcItemsData(generalSearch);
                total_page = (total_data + perpage - 1) / perpage;
                
            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("GetCcItemsListDatatable", ex.Message);
                Console.WriteLine(ex.Message);
            }
            Dictionary<string, dynamic> ret = new Dictionary<string, dynamic>();
            var meta = new Dictionary<string, dynamic>();
            meta.Add("page", page);
            meta.Add("pages", total_page);
            meta.Add("perpage", perpage);
            meta.Add("total", total_data);
            meta.Add("sort", sorttype);
            meta.Add("generalSearch", generalSearch);
            meta.Add("field", sort.ContainsKey("field") ? sort["field"]: null);

            ret.Add("meta", meta);
            ret.Add("data", result);
            return ret;
        }

        public int CountCcItemsData(string generalSearch)
        {
             int result = 0;
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    string string_condition = "";
                    if (!string.IsNullOrEmpty(generalSearch)) {
                        string_condition += " AND description LIKE @generalSearch";
                    }

                    var sql = "SELECT COUNT(*) AS total " +
                              "FROM m_cc_items WHERE is_delete = 0 " + string_condition;

                    SqlCommand command = new SqlCommand(sql, cs);
                    if (!string.IsNullOrEmpty(generalSearch))
                    {
                        command.Parameters.AddWithValue("@generalSearch", "%" + generalSearch + "%");
                    }

                    cs.Open();

                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result = Convert.ToInt32(reader["total"].ToString());
                        }
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("CountCcItemsData", ex.Message);
            }
            return result;
        }

        public Dictionary<string, dynamic> GetVendorGroupListDatatable(Dictionary<string, string> pagination, Dictionary<string, string> sort, Dictionary<string, string> query)
        {
            int page = pagination.ContainsKey("page") ? Convert.ToInt32(pagination["page"]): 1;
            int perpage = pagination.ContainsKey("perpage") ? Convert.ToInt32(pagination["perpage"]): 10;
            string sorttype = sort.ContainsKey("sort") ? string.IsNullOrEmpty(sort["sort"]) ? "asc" : sort["sort"]: "asc"; 
            string orderby = sort.ContainsKey("field") ? string.IsNullOrEmpty(sort["field"]) ? "vendor_type_id" : "" + sort["field"]: "vendor_type_id";
            string generalSearch = query.ContainsKey("generalSearch") ? string.IsNullOrEmpty(query["generalSearch"]) ? null : query["generalSearch"] : null;

            List<Dictionary<string, dynamic>> result = new List<Dictionary<string, dynamic>>();
            int total_data = 0;
            int total_page = 0;
            int offset = (page - 1)*perpage;
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    string string_condition = "";
                    if (!string.IsNullOrEmpty(generalSearch))
                    {
                        string_condition += " AND name LIKE @name";
                    }
                    string string_order = " ORDER BY " + orderby + " " + sorttype;
                    var sql = "SELECT * FROM m_vendor_type WHERE 1=1 AND is_delete = 0" + string_condition + " " + string_order + " " +
                              "OFFSET @offset ROWS FETCH NEXT @perpage ROWS ONLY";
                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@offset", offset);
                    command.Parameters.AddWithValue("@perpage", perpage);
                    if (!string.IsNullOrEmpty(generalSearch))
                    {
                        command.Parameters.AddWithValue("@name", "%" + generalSearch + "%");
                    }

                    cs.Open();
                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Dictionary<string, dynamic> data_row = new Dictionary<string, dynamic>();
                            data_row.Add("vendor_type_id", reader["vendor_type_id"].ToString());
                            data_row.Add("name", reader["name"].ToString());
                            data_row.Add("sap_code", reader["sap_code"].ToString());
                            data_row.Add("scheme_group_id", reader["scheme_group_id"].ToString());
                            data_row.Add("gl_id", reader["gl_id"].ToString());
                            result.Add(data_row);                            
                        }
                    }
                    cs.Close();
                }
                total_data = this.CountVendorGroupData(generalSearch);
                total_page = (total_data + perpage - 1) / perpage;
                
            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("GetVendorGroupListDatatable", ex.Message);
                Console.WriteLine(ex.Message);
            }
            Dictionary<string, dynamic> ret = new Dictionary<string, dynamic>();
            var meta = new Dictionary<string, dynamic>();
            meta.Add("page", page);
            meta.Add("pages", total_page);
            meta.Add("perpage", perpage);
            meta.Add("total", total_data);
            meta.Add("sort", sorttype);
            meta.Add("generalSearch", generalSearch);
            meta.Add("field", sort.ContainsKey("field") ? sort["field"]: null);

            ret.Add("meta", meta);
            ret.Add("data", result);
            return ret;
        }

        public int CountVendorGroupData(string generalSearch)
        {
             int result = 0;
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    string string_condition = "";
                    if (!string.IsNullOrEmpty(generalSearch)) {
                        string_condition += " AND A.name LIKE @generalSearch";
                    }

                    var sql = "SELECT COUNT(*) AS total " +
                              "FROM m_vendor_type A WHERE is_delete = 0 " + string_condition;

                    SqlCommand command = new SqlCommand(sql, cs);
                    if (!string.IsNullOrEmpty(generalSearch))
                    {
                        command.Parameters.AddWithValue("@generalSearch", "%" + generalSearch + "%");
                    }

                    cs.Open();

                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result = Convert.ToInt32(reader["total"].ToString());
                        }
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("CountVendorGroupData", ex.Message);
            }
            return result;
        }

        public bool CekMaterialExist(string id)
        {
            bool result = false;
            try
            {
                using (var cs = new ConnectionService(_appSettings).GetConnection())
                {
                    var sql = "SELECT COUNT(*) AS TOTAL from m_material_items WHERE material_id=@material_id";

                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@material_id", id);
                    cs.Open();
                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result= Convert.ToInt32(reader["TOTAL"].ToString())> 0 ? true : false;
                        }
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("CekMaterialExist", ex.Message);
            }
            //Console.WriteLine("ress :"+ result);
            return result;
        }

        public bool CekCcExist(string id)
        {
            bool result = false;
            try
            {
                using (var cs = new ConnectionService(_appSettings).GetConnection())
                {
                    var sql = "SELECT COUNT(*) AS TOTAL from m_cc_items WHERE cc_id=@cc_id";

                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@cc_id", id);
                    cs.Open();
                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result= Convert.ToInt32(reader["TOTAL"].ToString())> 0 ? true : false;
                        }
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("CekCcExist", ex.Message);
            }
            //Console.WriteLine("ress :"+ result);
            return result;
        }

        public bool CekcompanyCodeExist(string id)
        {
             bool result = false;
            try
            {
                using (var cs = new ConnectionService(_appSettings).GetConnection())
                {
                    var sql = "SELECT COUNT(*) AS TOTAL from m_company WHERE company_id=@company_id";

                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@company_id", id);
                    cs.Open();
                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result= Convert.ToInt32(reader["TOTAL"].ToString())> 0 ? true : false;
                        }
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("CekCompanyCodeExist", ex.Message);
            }
            //Console.WriteLine("ress :"+ result);
            return result;
        }
    }    
}
