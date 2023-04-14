using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using tufol.Interfaces;
using tufol.Models;
using tufol.Helpers;
using Microsoft.Data.SqlClient;
using Dapper;
using DapperQueryBuilder;
using System.Text.Json;

namespace tufol.Services
{
    public class DashboardService : IDashboard
    {
        private readonly Helpers.AppSettings _appSettings;
        public DashboardService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
            //for date date.ToString("dd MMMM yyyy", new System.Globalization.CultureInfo("id-ID"));
        }

        public int CountVendor(string vendor_number = null) {
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

                    var sql = "SELECT COUNT(vendor_number) AS total " +
                              "FROM tr_vendor A WHERE 1=1 " + string_condition;

                    SqlCommand command = new SqlCommand(sql, cs);

                    if (!string.IsNullOrEmpty(vendor_number))
                    {
                        command.Parameters.AddWithValue("@vendor_number", vendor_number);
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
                new Helpers.GlobalFunction().LogError("CountVendor", ex.Message);
            }
            return result;
        }

        public int CountTempVendor(int position_data, string type, int verification_status_id = 0, string vendor_number = null)
        {
            int result = 0;
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    string string_condition = "";
                    if (!string.IsNullOrEmpty(vendor_number))
                    {
                        string_condition += " AND vendor_number = @vendor_number";
                    }
                    if (!string.IsNullOrEmpty(verification_status_id.ToString()) && verification_status_id != 0)
                    {
                        string_condition += " AND verification_status_id = @verification_status_id";
                    }

                    var sql = "SELECT COUNT(vendor_id) AS total " +
                              "FROM temp_vendor WHERE position_data = @position_data AND type = @type " + string_condition;
                    
                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@position_data", position_data);
                    command.Parameters.AddWithValue("@type", type);

                    if (!string.IsNullOrEmpty(verification_status_id.ToString()) && verification_status_id != 0)
                    {
                        command.Parameters.AddWithValue("@verification_status_id", verification_status_id);
                    }
                    if (!string.IsNullOrEmpty(vendor_number))
                    {
                        command.Parameters.AddWithValue("@vendor_number", vendor_number);
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
                new Helpers.GlobalFunction().LogError("CountTempVendor", ex.Message);
            }
            return result;
        }
        public int CountTicket(int position_data, Dictionary<string, dynamic> condition = null, string vendor_number = null) {
            int result = 0;
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    string string_condition = "";

                    if (vendor_number != null) {
                        condition.Add("vendor_number", vendor_number);
                    }

                    if ( position_data > 0 )
                    {
                        string_condition = " AND A.position_data = @position_data";
                    }

                    foreach (KeyValuePair<string, dynamic> con in condition)
                    {
                        if (!string.IsNullOrEmpty(con.Value.ToString()))
                        {
                            if (System.Text.RegularExpressions.Regex.IsMatch(con.Key, @"^.*(\<|\>|\!\=|\>\=|\<\=).*$"))
                            {
                                string val_scalar = System.Text.RegularExpressions.Regex.Replace(con.Key, @"\<|\>|\!\=|\>\=|\<\=", "").Trim();
                                if(val_scalar == "verification_status") {
                                    string key = val_scalar == "verification_status" ? "A.verification_status_id" : "verification_status_id";
                                    string_condition += " OR " + key + " @verification_status";
                                } else if(con.Key == "label_for_vendor") {
                                    string_condition += " AND " + con.Key + " LIKE @" + con.Key;
                                } else {
                                    string key_name = 
                                        val_scalar == "verification_status_id" || con.Key == "vendor_number" || con.Key == "is_paid" || con.Key == "remmitance_number !=" || con.Key == "miro_number !=" || con.Key == "remmitance_date"
                                        ? "A." + con.Key : con.Key;
                                    string_condition += " AND " + key_name + " @" + val_scalar;
                                }
                            }
                            else
                            {
                                if(con.Key == "verification_status") {
                                    string key = con.Key == "verification_status" ? "A.verification_status_id" : "verification_status_id";
                                    string_condition += " OR " + key + " = @verification_status";
                                } else if(con.Key == "label_for_vendor") {
                                    string_condition += " AND " + con.Key + " LIKE @" + con.Key;
                                } else {
                                    string key_name = 
                                        con.Key == "verification_status_id" || con.Key == "vendor_number" || con.Key == "is_paid" || con.Key == "remmitance_number !=" || con.Key == "miro_number !=" || con.Key == "remmitance_date"
                                        ? "A." + con.Key : con.Key;
                                    string_condition += " AND " + key_name + " = @" + con.Key;
                                }
                            }
                        }
                    }

                    var sql = "SELECT COUNT(A.ticket_number) AS total " +
                              "FROM tr_ticket A " +
                              "JOIN m_verification_status B ON A.verification_status_id = B.verification_status_id " +
                              "JOIN tr_vendor C ON A.vendor_number = C.vendor_number " +
                              "LEFT JOIN tb_role D ON A.position_data = D.role_id " +
                              "WHERE 1=1" + string_condition;
                    SqlCommand command = new SqlCommand(sql, cs);
                    if (position_data > 0)
                    {
                        command.Parameters.AddWithValue("@position_data", position_data);
                    }
                    foreach (KeyValuePair<string, dynamic> con in condition)
                    {
                        if (!string.IsNullOrEmpty(con.Value.ToString()))
                        {
                            if (System.Text.RegularExpressions.Regex.IsMatch(con.Key, @"^.*(\<|\>|\!\=|\>\=|\<\=).*$"))
                            {
                                string val_scalar = System.Text.RegularExpressions.Regex.Replace(con.Key, @"\<|\>|\!\=|\>\=|\<\=", "").Trim();
                                if(val_scalar == "verification_status") {
                                    command.Parameters.AddWithValue("@verification_status", con.Value);
                                } else if(val_scalar == "label_for_vendor") {
                                    command.Parameters.AddWithValue("@" + val_scalar, "%" + con.Value + "%");
                                } else {
                                    command.Parameters.AddWithValue("@" + val_scalar, con.Value);
                                }
                            }
                            else
                            {
                                if(con.Key == "verification_status") {
                                    command.Parameters.AddWithValue("@verification_status", con.Value);
                                } else if(con.Key == "label_for_vendor") {
                                    command.Parameters.AddWithValue("@" + con.Key, "%" + con.Value + "%");
                                } else {
                                    command.Parameters.AddWithValue("@" + con.Key, con.Value);
                                }
                            }
                        }
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
                new Helpers.GlobalFunction().LogError("CountTicket", ex.Message);
                Console.WriteLine(ex.Message);
            }
            return result;
        }
        public int CountTicketConfirmToProcurement(string vendor_number = null) {
            int result = 0;
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    string string_condition = "";

                    if(vendor_number != null)
                        string_condition += " AND C.vendor_number = @vendor_number";

                    var sql = "SELECT COUNT(DISTINCT(A.ticket_number)) AS total " +
                              "FROM tr_po_ticket A " +
                              "JOIN tr_ticket B ON A.ticket_number = B.ticket_number " +
                              "JOIN tr_vendor C ON B.vendor_number = C.vendor_number " +
                              "WHERE 1=1 AND B.verification_status_id = @verification_status_id " +
                              "AND A.qty_billed > A.gr_qty" + string_condition;
                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("verification_status_id", 9);

                    if(vendor_number != null)
                        command.Parameters.AddWithValue("vendor_number", vendor_number);

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
                new Helpers.GlobalFunction().LogError("CountTicketConfirmToProcurement", ex.Message);
                Console.WriteLine(ex.Message);
            }
            return result;
        }
    }
}