using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using tufol.Interfaces;
using tufol.Models;
using tufol.Helpers;
using Microsoft.Data.SqlClient;
using DapperQueryBuilder;

namespace tufol.Services
{
    public class RegistrationService : IRegistration
    {
        private readonly Helpers.AppSettings _appSettings;
        public RegistrationService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;

            
        }

         public IEnumerable<TaxNumberTypeModel> GetApiTaxNumberType(string account_group_id)
        {
            List<TaxNumberTypeModel> result = new List<TaxNumberTypeModel>();
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "SELECT B.tax_number_type_id, B.sap_code, B.name "+
                              "FROM m_tax_number_type_group A " +
                              "JOIN m_tax_number_type B ON A.tax_number_type_id = B.tax_number_type_id "+
                              "WHERE A.account_group_id = @account_group_id";
                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@account_group_id", account_group_id);
                    cs.Open();

                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result.Add(new TaxNumberTypeModel()
                            {
                                // tax_number_type_id = Convert.ToInt32(reader["tax_number_type_id"].ToString()),
                                tax_number_type_id = Convert.ToInt32(reader["tax_number_type_id"]),
                                sap_code = reader["sap_code"].ToString() == "0" ? Convert.ToInt32(reader["sap_code"].ToString()) : -1,
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
                new Helpers.GlobalFunction().LogError("GetApiTaxNumberType", ex.Message);
            }
            return result;
        }

        public IEnumerable<TaxTypeModel> GetApiTaxType(string account_group_id)
        {
            List<TaxTypeModel> result = new List<TaxTypeModel>();
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "SELECT B.tax_type_id, B.name, B.wht_code, B.rates " +
                              "FROM m_tax_account_type A " +
                              "JOIN m_tax_type B ON A.tax_type_id = B.tax_type_id " +
                              "WHERE A.account_group_id = @account_group_id";
                    
                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@account_group_id", account_group_id);
                    cs.Open();
                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result.Add(new TaxTypeModel()
                            {
                                tax_type_id = Convert.ToInt32(reader["tax_type_id"].ToString()),
                                name = reader["name"].ToString(),
                                wht_code = reader["wht_code"].ToString(),
                                rates = reader["rates"].ToString()                               
                            });                            
                        }
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("GetApiTaxType", ex.Message);
            }
            return result;
        }
        

        public IEnumerable<CompanyModel> GetApiCompany(int vendor_type_id)
        {
            List<CompanyModel> result = new List<CompanyModel>();
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "SELECT B.company_id, B.name FROM m_company_group A JOIN m_company B ON A.company_id = B.company_id where A.vendor_type_id = @vendor_type_id";

                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@vendor_type_id", vendor_type_id);
                    cs.Open();
                    var reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result.Add(new CompanyModel()
                            {
                                company_id = reader["company_id"].ToString(),
                                name = reader["name"].ToString()                               
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
        public IEnumerable<SgCategoryModel> GetApiSgCategory(int vendor_type_id)
        {
            List<SgCategoryModel> result = new List<SgCategoryModel>();
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "SELECT B.sg_category_id, B.name " +
                              "FROM m_sg_category_group A " +
                              "JOIN m_sg_categories B ON A.sg_category_id = B.sg_category_id " +
                              "WHERE A.vendor_type_id = @vendor_type_id";

                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@vendor_type_id", vendor_type_id);
                    cs.Open();
                    var reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result.Add(new SgCategoryModel()
                            {
                                sg_category_id = reader["sg_category_id"].ToString(),
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
                new Helpers.GlobalFunction().LogError("GetApiSgCategory", ex.Message);
            }
            return result;
        }

        public TaxTypeModel GetApiRowTaxType(int tax_type_id)
        {
            TaxTypeModel result = new TaxTypeModel();
            try
            {
                using(SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "SELECT wht_code, rates FROM m_tax_type WHERE tax_type_id = @tax_type_id";

                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@tax_type_id", tax_type_id);
                    cs.Open();
                    var reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result.wht_code = reader["wht_code"].ToString();
                            result.rates = reader["rates"].ToString();
                        }
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                result = null;
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("GetApiRowTaxType", ex.Message);
            }
            return result;
        }
        
        public BankModel GetApiRowBank(string bank_id)
        {
            BankModel result = new BankModel();
            try
            {
                using(SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "SELECT bank_id, swift_code FROM m_bank WHERE bank_id = @bank_id";

                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@bank_id", bank_id);
                    cs.Open();
                    var reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result.bank_id = reader["bank_id"].ToString();
                            result.swift_code = reader["swift_code"].ToString();
                        }
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                result = null;
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("GetApiRowBank", ex.Message);
            }
            return result;
        }

        public IEnumerable<BankModel> GetBank(string country_of_origin = null)
        {
            List<BankModel> result = new List<BankModel>();
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "SELECT bank_id, name FROM m_bank" + (string.IsNullOrEmpty(country_of_origin) ? "": "WHERE country_of_origin = @country_of_origin");
                    
                    SqlCommand command = new SqlCommand(sql, cs);
                    if (!string.IsNullOrEmpty(country_of_origin))
                    {
                        command.Parameters.AddWithValue("@country_of_origin", country_of_origin);
                    }
                    cs.Open();
                    var reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result.Add(new BankModel()
                            {
                                bank_id = reader["bank_id"].ToString(),
                                name = reader["name"].ToString()                            
                            });
                        }
                        cs.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("GetBank", ex.Message);
            }
            return result;
        }
        public IEnumerable<VendorTypeModel> GetVendorType()
        {
            List<VendorTypeModel> result = new List<VendorTypeModel>();
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "SELECT vendor_type_id, name FROM m_vendor_type WHERE is_delete = 0";
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
                            result.Add(new VendorTypeModel()
                            {
                                vendor_type_id = Convert.ToInt32(reader["vendor_type_id"].ToString()),
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
                new Helpers.GlobalFunction().LogError("GetVendorType", ex.Message);
            }
            return result;
        }
        
        public VendorTypeModel GetVendorTypeById(int vendor_type_id)
        {
            VendorTypeModel result = new VendorTypeModel();
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "SELECT name, sap_code FROM m_vendor_type WHERE vendor_type_id = @vendor_type_id";

                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@vendor_type_id", vendor_type_id);
                    cs.Open();
                    var reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result.name = reader["name"].ToString();
                            result.sap_code = reader["sap_code"].ToString();
                        }
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("GetVendorTypeById", ex.Message);
            }
            return result;
        }
        
        public SchemeGroupModel GetSchemeGroup(int vendor_type_id)
        {
            SchemeGroupModel result = new SchemeGroupModel();
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "SELECT A.scheme_group_id, B.name " +
                              "FROM m_vendor_type A JOIN m_scheme_group B ON A.scheme_group_id = B.scheme_group_id " +
                              "WHERE A.vendor_type_id = @vendor_type_id";

                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@vendor_type_id", vendor_type_id);
                    cs.Open();
                    var reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result.scheme_group_id = reader["scheme_group_id"].ToString();
                            result.name = reader["name"].ToString();
                        }
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("GetSchemeGroup", ex.Message);
            }
            return result;
        }
        public GlModel GetGl(int vendor_type_id)
        {
            GlModel result = new GlModel();
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "SELECT A.gl_id, B.name FROM m_vendor_type A JOIN m_gl B ON A.gl_id = B.gl_id WHERE A.vendor_type_id = @vendor_type_id";
                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@vendor_type_id", vendor_type_id);
                    cs.Open();
                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result.gl_id = reader["gl_id"].ToString();
                            result.name = reader["name"].ToString();
                        }
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("GetGl", ex.Message);
            }
            return result;
        }

        public IEnumerable<CompanyModel> GetCompany()
        {
            List<CompanyModel> result = new List<CompanyModel>();
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "SELECT company_id, name FROM m_company WHERE is_delete = 0";
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
                                company_id = reader["company_id"].ToString(),
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
                new Helpers.GlobalFunction().LogError("GetCompany", ex.Message);
            }
            return result;
        }

        public IEnumerable<CountryModel> GetCountry()
        {
           List<CountryModel> result = new List<CountryModel>();
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "SELECT country_id, name FROM m_country";
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
                            result.Add(new CountryModel()
                            {
                                country_id = reader["country_id"].ToString(),
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
                new Helpers.GlobalFunction().LogError("GetCountry", ex.Message);
            }
            return result;
        }

        public IEnumerable<CurrencyModel> GetCurrency()
        {
            List<CurrencyModel> result = new List<CurrencyModel>();
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "SELECT currency_id, name FROM m_currency";
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
                            result.Add(new CurrencyModel()
                            {
                                currency_id = reader["currency_id"].ToString(),
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
                new Helpers.GlobalFunction().LogError("GetCurrency", ex.Message);
            }
            return result;
        }

        public IEnumerable<TaxNumberTypeModel> GetTaxNumberType()
        {
            List<TaxNumberTypeModel> result = new List<TaxNumberTypeModel>();
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "SELECT tax_number_type_id, sap_code, name FROM m_tax_number_type";
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
                            result.Add(new TaxNumberTypeModel()
                            {
                                // tax_number_type_id = Convert.ToInt32(reader["tax_number_type_id"].ToString()),
                                tax_number_type_id = Convert.ToInt32(reader["tax_number_type_id"]),
                                sap_code = reader["sap_code"].ToString() == "0" ? Convert.ToInt32(reader["sap_code"].ToString()) : -1,
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
                new Helpers.GlobalFunction().LogError("GetTaxNumberType", ex.Message);
            }
            return result;
        }

        // public IEnumerable<TaxTypeModel> GetTaxType()
        // {
        //     List<TaxTypeModel> result = new List<TaxTypeModel>();
        //     try
        //     {
        //         using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
        //         {
        //             var sql = "SELECT * FROM m_tax_type";
        //             var command = new ConnectionService().GetCommand();
        //             command.CommandType = System.Data.CommandType.Text;
        //             command.CommandText = sql;
        //             command.Connection = cs;
        //             cs.Open();

        //             var reader = command.ExecuteReader();
        //             if (reader.HasRows)
        //             {
        //                 while (reader.Read())
        //                 {
        //                     result.Add(new TaxTypeModel()
        //                     {
        //                         tax_type_id = Convert.ToInt32(reader["tax_type_id"].ToString()),
        //                         name = String.IsNullOrEmpty(reader["name"].ToString()) ? null : reader["name"].ToString(),
        //                         wht_code = String.IsNullOrEmpty(reader["wht_code"].ToString()) ? null : reader["wht_code"].ToString(),
        //                         rates = String.IsNullOrEmpty(reader["rates"].ToString()) ? null : reader["rates"].ToString()                               
        //                     });
        //                 }
        //             }
        //             cs.Close();
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine(ex);
        //     }
        //     return result;
        // }

        public IEnumerable<TitleModel> GetTitle()
        {
            List<TitleModel> result = new List<TitleModel>();
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "SELECT title_id, category FROM m_title";
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
                            result.Add(new TitleModel()
                            {
                                title_id = reader["title_id"].ToString(),
                                category = reader["category"].ToString()                          
                            });
                        }
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("GetTitle", ex.Message);
            }
            return result;
        }
        public IEnumerable<TopModel> GetTop()
        {
            List<TopModel> result = new List<TopModel>();
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "SELECT top_id, name, interval FROM m_top";
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
                            result.Add(new TopModel()
                            {
                                top_id = reader["top_id"].ToString(),
                                name = reader["name"].ToString(),                        
                                interval = Convert.ToInt32(reader["interval"].ToString())
                            });
                        }
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("GetTop", ex.Message);
            }
            return result;
        }
        public IEnumerable<PicModel> GetPic()
        {
            List<PicModel> result = new List<PicModel>();
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "SELECT initial_area FROM tb_procurement_group";
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
                            result.Add(new PicModel()
                            {
                                initial_area = reader["initial_area"].ToString()
                            });
                        }
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("GetPic", ex.Message);
            }
            return result;
        }
        public IEnumerable<ProvinceModel> GetProvince()
        {
            List<ProvinceModel> result = new List<ProvinceModel>();
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "SELECT province_id, name FROM m_province";
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
                            result.Add(new ProvinceModel()
                            {
                                province_id = reader["province_id"].ToString(),
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
                new Helpers.GlobalFunction().LogError("GetProvince", ex.Message);
            }
            return result;
        }

        public int InsertRegistration(RegistrationModel model, String file_id_card_name, String file_npwp_name, String file_sppkp_name, String file_vendor_statement_name, String token, String type = "Registration")
        {

            var result = false;
            int returnValue = 0;
            var localforex_bank_id = string.IsNullOrEmpty(model.localforex_bank_id) ?null: "'"+model.localforex_bank_id+"'";
            var verification_status_id = string.IsNullOrEmpty(model.verification_status_id.ToString()) ? 1 : model.verification_status_id;
            var position_data = string.IsNullOrEmpty(model.position_data.ToString()) ? 1 : model.position_data;
            

            try
            {
                  using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                  {
                      var sql = "INSERT INTO temp_vendor (" +
                                "type, vendor_type_id, sg_category_id, currency_id, " +
                                "title_id, name, contact_person, search_term,street_address, " + 
                                "additional_street_address, city,postal_code, country_id, "+ 
                                "telephone, fax, email, tax_type_id, tax_number_type_id, " + 
                                "npwp, id_card_number, sppkp_number, localidr_bank_id, localidr_bank_account, " +
                                "localidr_account_holder, localforex_bank_id, localforex_bank_account, "+
                                "localforex_account_holder, localforex_currency_id, " +
                                "foreign_bank_country_id, foreign_bank_id,foreign_bank_name, " +
                                "foreign_bank_swift_code, foreign_bank_account, foreign_account_holder, foreign_currency_id, " + 
                                "reference_correspondent, reference_country_origin, pic_id, file_id_card, " + 
                                "file_npwp, file_sppkp, file_vendor_statement, " +
                                "verification_status_id, position_data, token, partner_function, updated_data, top_id) " +
                                "OUTPUT INSERTED.vendor_id " +
                                "VALUES (@type, @vendor_type_id, @sg_category_id, @currency_id, " +
                                "@title_id, @name, @contact_person, @search_term, @street_address, " +
                                "@additional_street_address, @city,@postal_code, @country_id, " +
                                "@telephone, @fax, @email, @tax_type_id, @tax_number_type_id, " +
                                "@npwp, @id_card_number, @sppkp_number, @localidr_bank_id, @localidr_bank_account, " +
                                "@localidr_account_holder, @localforex_bank_id, @localforex_bank_account, " +
                                "@localforex_account_holder, @localforex_currency_id, " +
                                "@foreign_bank_country_id, @foreign_bank_id, @foreign_bank_name, " +
                                "@foreign_bank_swift_code, @foreign_bank_account, @foreign_account_holder, @foreign_currency_id, " +
                                "@reference_correspondent, @reference_country_origin, @pic_id, @file_id_card, " +
                                "@file_npwp, @file_sppkp, @file_vendor_statement, " +
                                "@verification_status_id, @position_data, @token, @partner_function, @updated_data, @top_id)";

                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@type", model.type);
                    command.Parameters.AddWithValue("@vendor_type_id", model.vendor_type_id);
                    command.Parameters.AddWithValue("@sg_category_id", model.sg_category_id);
                    command.Parameters.AddWithValue("@currency_id", model.currency_id);
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
                    command.Parameters.AddWithValue("@file_id_card", (string.IsNullOrEmpty(file_id_card_name) ? DBNull.Value : file_id_card_name));
                    command.Parameters.AddWithValue("@file_npwp", (string.IsNullOrEmpty(file_npwp_name) ? DBNull.Value : file_npwp_name));
                    command.Parameters.AddWithValue("@file_sppkp", (string.IsNullOrEmpty(file_sppkp_name) ? DBNull.Value : file_sppkp_name));
                    command.Parameters.AddWithValue("@file_vendor_statement", (string.IsNullOrEmpty(file_vendor_statement_name) ? DBNull.Value : file_vendor_statement_name));
                    command.Parameters.AddWithValue("@verification_status_id", string.IsNullOrEmpty(model.verification_status_id.ToString()) ? 1 : model.verification_status_id);
                    command.Parameters.AddWithValue("@position_data", string.IsNullOrEmpty(model.position_data.ToString()) ? 1 : model.position_data);
                    command.Parameters.AddWithValue("@token", token);
                    command.Parameters.AddWithValue("@partner_function", (string.IsNullOrEmpty(model.partner_function) ? DBNull.Value : model.partner_function));
                    command.Parameters.AddWithValue("@updated_data", (string.IsNullOrEmpty(model.updated_data) ? DBNull.Value : model.updated_data));
                    command.Parameters.AddWithValue("@top_id", (string.IsNullOrEmpty(model.top_id) ? DBNull.Value : model.top_id));
                    cs.Open();
                    // returnValue = command.ExecuteNonQuery() == 1 ? Convert.ToInt32(command.ExecuteScalar()) : 0;
                    returnValue = Convert.ToInt32(command.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("InsertRegistration", ex.Message);
            }
            return returnValue;
        }

         public bool InsertCompany(RegistrationModel company, int vendor_id)
         {
            var result = false;
            try
            {
                var sql = "INSERT INTO  temp_company_relations(company_id, vendor_id) values ";
                var companyList = company.company_id;
                for(var i = 0; i < companyList.Length; i ++) {
                    if (i != companyList.Length - 1 ) sql += "('"+companyList[i].ToString()+"','"+vendor_id+"'),";
                    else sql += "('"+companyList[i].ToString()+"','"+vendor_id+"')";
                }
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var command = new ConnectionService().GetCommand();
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandText = sql;
                    command.Connection =  cs;
                    cs.Open();
                        
                    result = command.ExecuteNonQuery()== 1;

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("InsertCompany", ex.Message);
            }
            return result;
        }

        public bool InsertLog(TempLogVendorStatusModel model)
        {
            var result = false;
            try
            {
                var sql = "INSERT INTO  temp_log_vendor_status " +
                          "(vendor_id, verification_status_id, verification_note, created_by) " +
                          "values (@vendor_id, @verification_status_id, @verification_note, @created_by)";
                
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@vendor_id", model.vendor_id);
                    command.Parameters.AddWithValue("@verification_status_id", model.verification_status_id);
                    command.Parameters.AddWithValue("@verification_note", (string.IsNullOrEmpty(model.verification_note) ? DBNull.Value : model.verification_note));
                    command.Parameters.AddWithValue("@created_by", (string.IsNullOrEmpty(model.created_by.ToString()) ? DBNull.Value: model.created_by));
                    cs.Open();
                    result = command.ExecuteNonQuery() == 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("InsertLog", ex.Message);
            }
            return result;
        }

        public string GetToken(int vendor_id)
        {
            string token ="";
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "SELECT token FROM temp_vendor where vendor_id = @vendor_id and verification_status_id = 1";
                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@vendor_id", vendor_id);
                    cs.Open();
                    token = (string)command.ExecuteScalar();
                }
                   
        
        } catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("GetToken", ex.Message);
            }
             return token;
    }

        public bool updateVerifyEmail(int vendor_id, string verified_email, int verification_status, int position_data)
        {
            var result = false;
            try
            {
                  using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                  {
                      var sql = "Update temp_vendor set " +
                                "verified_email = @verified_email, " +
                                "verification_status_id = @verification_status, " +
                                "position_data = @position_data " +
                                "where vendor_id = @vendor_id";
                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@verified_email", verified_email);
                    command.Parameters.AddWithValue("@verification_status", verification_status);
                    command.Parameters.AddWithValue("@position_data", position_data);
                    command.Parameters.AddWithValue("@vendor_id", vendor_id);
                    cs.Open();

                    result = command.ExecuteNonQuery() == 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("updateVerifyEmail", ex.Message);
            }
            return result;
        }


        public int GetVerifiedEmail(int vendor_id)
        {
            int vem=0;
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "SELECT verified_email FROM temp_vendor where vendor_id=@vendor_id";
                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@vendor_id", vendor_id);
                    cs.Open();
                    vem = (int)command.ExecuteScalar();
                }
                   
            
            } catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    new Helpers.GlobalFunction().LogError("GetVerifiedEmail", ex.Message);
                }
             return vem;
        }

        public int GetVerificationStatus(int vendor_id)
        {
           int vem=0;
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "SELECT verification_status_id FROM temp_vendor where vendor_id=@vendor_id";
                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@vendor_id", vendor_id);
                    cs.Open();

                    vem = (int)command.ExecuteScalar();
                }
                   
            
            } catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    new Helpers.GlobalFunction().LogError("GetVerificationStatus", ex.Message);
                }
             return vem;

        }

        public IEnumerable<BankModel> GetBankList(){
            IEnumerable<BankModel> result = new List<BankModel>();

            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var q = cs.FluentQueryBuilder()
                    .Select($"*")
                    .From($"m_bank");

                    result = q.Query<BankModel>();

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("GetBankList", ex.Message);
            }
            return result;
        }
        public BankModel GetBankDetail(string bank_id)
        {
            BankModel result = new BankModel();
            try{
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "SELECT * FROM m_bank WHERE bank_id = @bank_id";
                    
                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@bank_id", bank_id);
                    
                    cs.Open();
                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result.bank_id = reader["bank_id"].ToString();
                            result.country_of_origin = reader["country_of_origin"].ToString();
                            result.name = reader["name"].ToString();
                            result.swift_code = reader["swift_code"].ToString();
                           
                        }
                    }
                    cs.Close();
                }

            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("GetBankDetail", ex.Message);
            }
            Console.WriteLine("result " + result);
            return result;
        }

        public IEnumerable<SchemeGroupModel> GetMSchemeGroup()
        {
            List<SchemeGroupModel> result = new List<SchemeGroupModel>();
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "SELECT scheme_group_id, name FROM m_scheme_group";
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
                            result.Add(new SchemeGroupModel()
                            {
                                scheme_group_id = reader["scheme_group_id"].ToString(),                               
                                name = reader["name"].ToString(),                                
                            });
                        }
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("GetMSchemeGroup", ex.Message);
            }
            return result;
        }

        public IEnumerable<GlModel> GetMGl()
        {
            List<GlModel> result = new List<GlModel>();
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "SELECT gl_id, name FROM m_gl";
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
                            result.Add(new GlModel()
                            {
                                gl_id = reader["gl_id"].ToString(),                               
                                name = reader["name"].ToString(),                                
                            });
                        }
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("GetMGl", ex.Message);
            }
            return result;
        }
    }   
}