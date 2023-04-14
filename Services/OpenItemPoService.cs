using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using tufol.Interfaces;
using tufol.Models;
using tufol.Helpers;
using Microsoft.Data.SqlClient;

namespace tufol.Services
{
    public class OpenItemPoService : IOpenItemPo
    {
        private readonly Helpers.AppSettings _appSettings;
        public OpenItemPoService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public int CountTicketData(PostDatatableNetModel param, Dictionary<string, dynamic> condition = null, string vendor_number = null) {
            int result = 0;
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    string string_condition = "";

                    foreach (KeyValuePair<string, dynamic> con in condition)
                    {
                        if (!string.IsNullOrEmpty(con.Value.ToString()))
                        {
                            string key_name = con.Key;
                            string_condition += " AND " + key_name + " = @" + con.Key;
                        }
                    }

                    foreach ( columnDatatable col in param.columns )
                    {
                        bool searchable = col.searchable == "true" ? true : false;
                        bool orderable = col.orderable == "true" ? true : false;
                        if (!string.IsNullOrEmpty(col.search.value) && searchable)
                        {
                            if(col.data == "name_vendor") {
                                string_condition += " AND vendor_name LIKE @" + col.data;
                                string_condition += " OR vendor_name_2 LIKE @" + col.data;
                            }
                            else if(col.data == "po_name")
                                string_condition += " AND " + col.data + " LIKE @" + col.data;
                            else
                                string_condition += " AND " + col.data + " = @" + col.data;
                        }    
                    }

                    var sql = "SELECT COUNT(po_open_id) AS total " +
                              "FROM m_po_open A " +
                              "WHERE 1=1" + string_condition;
                    SqlCommand command = new SqlCommand(sql, cs);
                    foreach (KeyValuePair<string, dynamic> con in condition)
                    {
                        if (!string.IsNullOrEmpty(con.Value.ToString()))
                        {
                            if (System.Text.RegularExpressions.Regex.IsMatch(con.Key, @"^.*(\<|\>|\!\=|\>\=|\<\=).*$"))
                            {
                                string val_scalar = System.Text.RegularExpressions.Regex.Replace(con.Key, @"\<|\>|\!\=|\>\=|\<\=", "").Trim();
                                command.Parameters.AddWithValue("@" + val_scalar, con.Value);
                            } else
                            {
                                command.Parameters.AddWithValue("@" + con.Key, con.Value);
                            }
                        }
                    }
                    foreach (columnDatatable col in param.columns)
                    {
                        bool searchable = col.searchable == "true" ? true : false;
                        bool orderable = col.orderable == "true" ? true : false;
                        if (!string.IsNullOrEmpty(col.search.value) && searchable)
                        {
                            if(col.data == "name_vendor" || col.data == "po_name") 
                                command.Parameters.AddWithValue("@" + col.data, "%" + col.search.value + "%");
                            else
                                command.Parameters.AddWithValue("@" + col.data, col.search.value);
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
                new Helpers.GlobalFunction().LogError("CountTicketData", ex.Message);
                Console.WriteLine(ex.Message);
            }
            return result;
        }

        public Dictionary<string, dynamic> GetOpenItemPoDatatable(PostDatatableNetModel param, List<string> selected_column, Dictionary<string, dynamic> condition = null, string vendor_number = null)
        {
            List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();
            int total_data = 0;
            int total_page = 0;
            string sorttype = null;
            string sortfield = null;
            // List<string> date_columns = new List<string>() { "doc_date" };
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    string string_condition = "";

                    if (vendor_number != null) {
                        condition.Add("vendor_number", vendor_number);
                        foreach (KeyValuePair<string, dynamic> con in condition)
                        {
                            if (!string.IsNullOrEmpty(con.Value.ToString()))
                            {
                                string key_name = con.Key;
                                string_condition += " AND " + key_name + " = @" + con.Key;
                            }
                        }
                    }

                    foreach ( columnDatatable col in param.columns )
                    {
                        bool searchable = col.searchable == "true" ? true : false;
                        bool orderable = col.orderable == "true" ? true : false;
                        if (!string.IsNullOrEmpty(col.search.value) && searchable)
                        {
                            if(col.data == "name_vendor") {
                                string_condition += " AND vendor_name LIKE @" + col.data;
                                string_condition += " OR vendor_name_2 LIKE @" + col.data;
                            }
                            else if(col.data == "po_name")
                                string_condition += " AND " + col.data + " LIKE @" + col.data;
                            else
                                string_condition += " AND " + col.data + " = @" + col.data;
                        }    
                    }

                    string string_order = "";
                    string temp_string_order = "";
                    foreach ( orderDatatable item_order in param.order )
                    {
                        bool orderable = param.columns[Convert.ToInt32(item_order.column)].orderable == "true" ? true : false;
                        if (orderable)
                        {
                            sortfield = param.columns[Convert.ToInt32(item_order.column)].data;
                            sorttype = item_order.dir;
                            if (!string.IsNullOrEmpty(temp_string_order))
                                temp_string_order += ",";

                            temp_string_order += " " + sortfield + " " + sorttype;
                        }
                    }

                    if (!string.IsNullOrEmpty(temp_string_order))
                        string_order = " ORDER BY" + temp_string_order;

                    var sql = "SELECT po_open_id, vendor_name, vendor_name_2, po_item_number, doc_date, po_number, " +
                                "po_name, po_quantity, uom, currency, net_price, "+
                                "CASE WHEN vendor_name IS NULL THEN vendor_name_2 ELSE vendor_name END AS name_vendor " +
                                "FROM m_po_open " +
                                "WHERE 1=1" + string_condition + string_order + " " +
                                "OFFSET @start ROWS FETCH NEXT @length ROWS ONLY";
                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@start", Convert.ToInt32(param.start));
                    command.Parameters.AddWithValue("@length", Convert.ToInt32(param.length));

                    foreach (KeyValuePair<string, dynamic> con in condition)
                    {
                        if (!string.IsNullOrEmpty(con.Value.ToString()))
                        {
                            if (System.Text.RegularExpressions.Regex.IsMatch(con.Key, @"^.*(\<|\>|\!\=|\>\=|\<\=).*$"))
                            {
                                string val_scalar = System.Text.RegularExpressions.Regex.Replace(con.Key, @"\<|\>|\!\=|\>\=|\<\=", "").Trim();
                                command.Parameters.AddWithValue("@" + val_scalar, con.Value);
                            } else
                            {
                                command.Parameters.AddWithValue("@" + con.Key, con.Value);
                            }
                        }
                    }

                    foreach (columnDatatable col in param.columns)
                    {
                        bool searchable = col.searchable == "true" ? true : false;
                        bool orderable = col.orderable == "true" ? true : false;
                        if (!string.IsNullOrEmpty(col.search.value) && searchable)
                        {
                            if(col.data == "name_vendor" || col.data == "po_name") 
                                command.Parameters.AddWithValue("@" + col.data, "%" + col.search.value + "%");
                            else
                                command.Parameters.AddWithValue("@" + col.data, col.search.value);
                        }
                    }

                    cs.Open();
                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Dictionary<string, string> data_row = new Dictionary<string, string>();

                            foreach ( string select in selected_column )
                            {
                                data_row.Add(select, reader[select].ToString());
                            }
                            result.Add(data_row);
                        }
                    }
                    cs.Close();
                }

                total_data = this.CountTicketData(param, condition, vendor_number);
                total_page = (total_data + param.length - 1) / param.length;
            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("GetOpenItemPoDatatable", ex.Message);
                Console.WriteLine("Error:  " + ex.Message);
            }
            int page = param.start > 0 ? (param.length/param.start)+1 : 1;
            Dictionary<string, dynamic> ret = new Dictionary<string, dynamic>();
            var meta = new Dictionary<string, dynamic>();

            ret.Add("draw", param.draw);
            ret.Add("data", result);
            ret.Add("recordsTotal", total_data);
            ret.Add("recordsFiltered", total_data);
            return ret;
        }
    }
}