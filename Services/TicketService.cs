using System;
using System.Text.RegularExpressions;
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
    public class TicketService : ITicket
    {
        private readonly Helpers.AppSettings _appSettings;
        public TicketService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
            //for date date.ToString("dd MMMM yyyy", new System.Globalization.CultureInfo("id-ID"));
        }

        public string InsertTicket(CreateTicketModel model, String file_invoice, String file_po, String file_lpb_gr, String file_tax_invoice, String file_delivery_note, String file_bast)
        {
            string returnValue = "";
            var verification_status_id = string.IsNullOrEmpty(model.verification_status_id.ToString()) ? 1 : model.verification_status_id;
            var position_data = string.IsNullOrEmpty(model.position_data.ToString()) ? 1 : model.position_data;
            try
            {
                  using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                  {
                      var sql = "INSERT INTO tr_ticket (" +
                                "ticket_number, vendor_number, " +
                                "country_id, mailing_address, district, village, province, " + 
                                "phone_number, company_id, area_code, top_id, top_duration, "+ 
                                "tax_status_id, tax_invoice_number, tax_invoice_date, invoice_number, invoice_date, " + 
                                "awb_mail, po_type_id, po_number, gr_number, invoice_amount, " +
                                "invoice_currency, dpp_original, dpp, ppn, "+
                                "pph, total_price, " +
                                "file_invoice, file_po, file_lpb_gr, file_tax_invoice, file_delivery_note, file_bast, " +
                                "verification_status_id, position_data, type_of_payment) " +
                                "VALUES (@ticket_number, @vendor_number, " +
                                "@country_id, @mailing_address, @district, @village, @province, " +
                                "@phone_number, @company_id, @area_code, @top_id,  @top_duration, " +
                                "@tax_status_id, @tax_invoice_number, @tax_invoice_date, @invoice_number, @invoice_date, " +
                                "@awb_mail, @po_type_id, @po_number, @gr_number, @invoice_amount, " +
                                "@invoice_currency, @dpp_original, @dpp, @ppn, " +
                                "@pph, @total_price, " +
                                "@file_invoice, @file_po, @file_lpb_gr, @file_tax_invoice, @file_delivery_note, @file_bast, " +
                                "@verification_status_id, @position_data, @type_of_payment)";

                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@ticket_number", model.ticket_number);
                    command.Parameters.AddWithValue("@vendor_number", model.vendor_number);
                    command.Parameters.AddWithValue("@country_id", model.country_id);
                    command.Parameters.AddWithValue("@mailing_address", model.mailing_address);
                    command.Parameters.AddWithValue("@district", model.district);
                    command.Parameters.AddWithValue("@village", model.village);
                    command.Parameters.AddWithValue("@province", model.province);
                    command.Parameters.AddWithValue("@phone_number", model.phone_number);
                    command.Parameters.AddWithValue("@company_id", model.company_id);
                    command.Parameters.AddWithValue("@area_code", model.area_code);
                    command.Parameters.AddWithValue("@top_id", model.top_id);
                    command.Parameters.AddWithValue("@top_duration", model.top_duration);
                    command.Parameters.AddWithValue("@tax_status_id", model.tax_status_id);
                    command.Parameters.AddWithValue("@tax_invoice_number", (String.IsNullOrEmpty(model.tax_invoice_number) ? DBNull.Value : new Helpers.GlobalFunction().ClearFormatTaxInvoiceNumber(model.tax_invoice_number)));
                    command.Parameters.AddWithValue("@tax_invoice_date", (String.IsNullOrEmpty(model.tax_invoice_date.ToString()) ? DBNull.Value : model.tax_invoice_date));
                    command.Parameters.AddWithValue("@invoice_number", model.invoice_number);
                    command.Parameters.AddWithValue("@invoice_date", model.invoice_date);
                    command.Parameters.AddWithValue("@awb_mail", (String.IsNullOrEmpty(model.awb_mail) ? DBNull.Value : model.awb_mail));
                    command.Parameters.AddWithValue("@po_type_id", model.po_type_id);
                    command.Parameters.AddWithValue("@po_number", model.po_number);
                    command.Parameters.AddWithValue("@gr_number", (String.IsNullOrEmpty(model.gr_number) ? DBNull.Value : model.gr_number));
                    command.Parameters.AddWithValue("@invoice_amount", new Helpers.GlobalFunction().ClearFormatCurrency(model.invoice_amount));
                    command.Parameters.AddWithValue("@invoice_currency", model.invoice_currency);
                    command.Parameters.AddWithValue("@dpp_original", model.dpp_original);
                    command.Parameters.AddWithValue("@dpp", model.dpp);
                    command.Parameters.AddWithValue("@ppn", model.ppn);
                    command.Parameters.AddWithValue("@pph", model.pph);
                    command.Parameters.AddWithValue("@total_price", model.total_price);
                    command.Parameters.AddWithValue("@file_invoice", file_invoice);
                    command.Parameters.AddWithValue("@file_po", file_po);
                    command.Parameters.AddWithValue("@file_lpb_gr", (string.IsNullOrEmpty(file_lpb_gr) ? DBNull.Value : file_lpb_gr));
                    command.Parameters.AddWithValue("@file_tax_invoice", file_tax_invoice);
                    command.Parameters.AddWithValue("@file_delivery_note", (string.IsNullOrEmpty(file_delivery_note) ? DBNull.Value : file_delivery_note));
                    command.Parameters.AddWithValue("@file_bast", (string.IsNullOrEmpty(file_bast) ? DBNull.Value : file_bast));
                    command.Parameters.AddWithValue("@verification_status_id", verification_status_id);
                    command.Parameters.AddWithValue("@position_data", position_data);
                    command.Parameters.AddWithValue("@type_of_payment", model.type_of_payment);
                    cs.Open();
                    returnValue = command.ExecuteNonQuery() ==  1 ? model.ticket_number : null;
                    // returnValue = model.ticket_number;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error di ticket: " + ex.Message);
                new Helpers.GlobalFunction().LogError("InsertTicket", ex.Message);
            }
            return returnValue;
        }

        public bool InsertLog(LogTicketModel model)
        {
            var result = false;
            try
            {
                var sql = "INSERT INTO log_ticket " +
                          "(ticket_number, process, note, created_by) " +
                          "values (@ticket_number, @process, @note, @created_by)";

                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@ticket_number", model.ticket_number);
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
                new Helpers.GlobalFunction().LogError("InsertLog", ex.Message);
            }
            return result;
        }

        public bool DeleteTicket(string ticket_number)
        {
        var result = false;
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "DELETE FROM tr_ticket WHERE ticket_number = @ticket_number";
                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@ticket_number", ticket_number);
                    cs.Open();

                    result = command.ExecuteNonQuery() == 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("DeleteTicket", ex.Message);
            }
            return result;
        }

        public bool updateTicket(string ticket_number, CreateTicketModel model, String file_invoice_name = null, String file_po_name = null, String file_lpb_gr_name = null, String file_tax_invoice_name = null, String file_delivery_note_name = null, String file_bast_name = null)
        {
            var result = false;
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "UPDATE tr_ticket SET " +
                              "country_id = @country_id, " +
                              "mailing_address = @mailing_address, " +
                              "district = @district, " +
                              "village = @village, " +
                              "province = @province, " +
                              "phone_number = @phone_number, " +
                              "company_id = @company_id, " +
                              "area_code = @area_code, " +
                              "top_id = @top_id, " +
                              "top_duration = @top_duration, " +
                              "tax_status_id = @tax_status_id, " +
                              "tax_invoice_number = @tax_invoice_number, " +
                              "tax_invoice_date = @tax_invoice_date, " +
                              "invoice_number = @invoice_number, " +
                              "invoice_date = @invoice_date, " +
                              "awb_mail = @awb_mail, " +
                              "po_type_id = @po_type_id, " +
                              "po_number = @po_number, gr_number = @gr_number, " +
                              "invoice_amount = @invoice_amount, " +
                              "invoice_currency = @invoice_currency, " +
                              "dpp = @dpp, ppn = @ppn, pph = @pph, " +
                              "total_price = @total_price, " +
                              (string.IsNullOrEmpty(file_invoice_name) ? "" : "file_invoice = @file_invoice, ") +
                              (string.IsNullOrEmpty(file_po_name) ? "" : "file_po = @file_po, ") +
                              (string.IsNullOrEmpty(file_lpb_gr_name) ? "" : "file_lpb_gr = @file_lpb_gr, ") +
                              (string.IsNullOrEmpty(file_tax_invoice_name) ? "" : "file_tax_invoice = @file_tax_invoice, ") +
                              (string.IsNullOrEmpty(file_delivery_note_name) ? "" : "file_delivery_note = @file_delivery_note, ") +
                              (string.IsNullOrEmpty(file_bast_name) ? "" : "file_bast = @file_bast, ") +
                              "type_of_payment = @type_of_payment, " +
                              "verification_status_id = @verification_status_id, " +
                              "verification_note = @verification_note, " +
                              (string.IsNullOrEmpty(model.posting_date.ToString()) ? "" : "posting_date = @posting_date, ") +
                              "position_data = @position_data, " +
                              "request_simulate = @request_simulate, " +
                              "is_finish = @is_finish, " +
                              "tax_amt_fc = @tax_amt_fc " +
                              "WHERE ticket_number = @ticket_number";
                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@country_id", model.country_id);
                    command.Parameters.AddWithValue("@mailing_address", model.mailing_address);
                    command.Parameters.AddWithValue("@district", model.district);
                    command.Parameters.AddWithValue("@village", model.village);
                    command.Parameters.AddWithValue("@province", model.province);
                    command.Parameters.AddWithValue("@phone_number", model.phone_number);
                    command.Parameters.AddWithValue("@company_id", model.company_id);
                    command.Parameters.AddWithValue("@area_code", model.area_code);
                    command.Parameters.AddWithValue("@top_id", model.top_id);
                    command.Parameters.AddWithValue("@top_duration", model.top_duration);
                    command.Parameters.AddWithValue("@tax_status_id", model.tax_status_id);
                    command.Parameters.AddWithValue("@tax_invoice_number", (String.IsNullOrEmpty(model.tax_invoice_number) ? DBNull.Value : new Helpers.GlobalFunction().ClearFormatTaxInvoiceNumber(model.tax_invoice_number)));
                    command.Parameters.AddWithValue("@tax_invoice_date", (String.IsNullOrEmpty(model.tax_invoice_date.ToString()) ? DBNull.Value : model.tax_invoice_date));
                    command.Parameters.AddWithValue("@invoice_number", model.invoice_number);
                    command.Parameters.AddWithValue("@invoice_date", model.invoice_date);
                    command.Parameters.AddWithValue("@awb_mail", (String.IsNullOrEmpty(model.awb_mail) ? DBNull.Value : model.awb_mail));
                    command.Parameters.AddWithValue("@po_type_id", model.po_type_id);
                    command.Parameters.AddWithValue("@po_number", model.po_number);
                    command.Parameters.AddWithValue("@gr_number", (String.IsNullOrEmpty(model.gr_number) ? DBNull.Value : model.gr_number));
                    command.Parameters.AddWithValue("@invoice_amount", new Helpers.GlobalFunction().ClearFormatCurrency(model.invoice_amount));
                    command.Parameters.AddWithValue("@invoice_currency", model.invoice_currency);
                    command.Parameters.AddWithValue("@dpp", model.dpp);
                    command.Parameters.AddWithValue("@ppn", model.ppn);
                    command.Parameters.AddWithValue("@pph", model.pph);
                    command.Parameters.AddWithValue("@total_price", model.total_price);
                    if (!String.IsNullOrEmpty(file_invoice_name))
                        command.Parameters.AddWithValue("@file_invoice", (string.IsNullOrEmpty(file_invoice_name) ? DBNull.Value : file_invoice_name));
                    if (!String.IsNullOrEmpty(file_po_name))
                        command.Parameters.AddWithValue("@file_po", (string.IsNullOrEmpty(file_po_name) ? DBNull.Value : file_po_name));
                    if (!String.IsNullOrEmpty(file_lpb_gr_name))
                        command.Parameters.AddWithValue("@file_lpb_gr", (string.IsNullOrEmpty(file_lpb_gr_name) ? DBNull.Value : file_lpb_gr_name));
                    if (!String.IsNullOrEmpty(file_tax_invoice_name))
                        command.Parameters.AddWithValue("@file_tax_invoice", (string.IsNullOrEmpty(file_tax_invoice_name) ? DBNull.Value : file_tax_invoice_name));
                    if (!String.IsNullOrEmpty(file_delivery_note_name))
                        command.Parameters.AddWithValue("@file_delivery_note", (string.IsNullOrEmpty(file_delivery_note_name) ? DBNull.Value : file_delivery_note_name));
                    if (!String.IsNullOrEmpty(file_bast_name))
                        command.Parameters.AddWithValue("@file_bast", (string.IsNullOrEmpty(file_bast_name) ? DBNull.Value : file_bast_name));
                    command.Parameters.AddWithValue("@verification_status_id", model.verification_status_id);
                    command.Parameters.AddWithValue("@verification_note", (String.IsNullOrEmpty(model.verification_note) ? DBNull.Value : model.verification_note));
                    command.Parameters.AddWithValue("@posting_date", (String.IsNullOrEmpty(model.posting_date.ToString()) ? DBNull.Value : model.posting_date));
                    command.Parameters.AddWithValue("@position_data", model.position_data);
                    command.Parameters.AddWithValue("@type_of_payment", model.type_of_payment);
                    command.Parameters.AddWithValue("@request_simulate", model.request_simulate);
                    command.Parameters.AddWithValue("@is_finish", model.is_finish);
                    command.Parameters.AddWithValue("@tax_amt_fc", new Helpers.GlobalFunction().ClearFormatCurrency(model.tax_amt_fc.ToString()));
                    command.Parameters.AddWithValue("@ticket_number", model.ticket_number);
                    cs.Open();

                    result = command.ExecuteNonQuery() == 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("updateTicket", ex.Message);
            }
            return result;
        }
        public bool updateTicketVerificationStatus(string ticket_number, int verification_status_id, string verification_note = null)
        {
            var result = false;
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "UPDATE tr_ticket SET " +
                                "verification_status_id = @verification_status_id ";
                    if(verification_note != null)
                        sql += ", verification_note = @verification_note ";
                    sql += "WHERE ticket_number = @ticket_number";
                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@verification_status_id", verification_status_id);
                    command.Parameters.AddWithValue("@ticket_number", ticket_number);
                    if(verification_note != null)
                        command.Parameters.AddWithValue("@verification_note", verification_note);

                    cs.Open();
                    result = command.ExecuteNonQuery() == 1;
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("updateTicketVerificationStatus", ex.Message);
            }
            return result;
        }
        public bool updateTicketReceivedDate(string ticket_number, DateTime received_date)
        {
            var result = false;
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "UPDATE tr_ticket SET " +
                                "received_date = @received_date " +
                                "WHERE ticket_number = @ticket_number";
                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@received_date", received_date);
                    command.Parameters.AddWithValue("@ticket_number", ticket_number);
                    cs.Open();
                    result = command.ExecuteNonQuery() == 1;
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("updateTicketReceivedDate", ex.Message);
            }
            return result;
        }
        public bool updateTicketPositionData(string ticket_number, int position_data)
        {
            var result = false;
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "UPDATE tr_ticket SET " +
                                "position_data = @position_data " +
                                "WHERE ticket_number = @ticket_number";
                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@position_data", position_data);
                    command.Parameters.AddWithValue("@ticket_number", ticket_number);
                    cs.Open();
                    result = command.ExecuteNonQuery() == 1;
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("updateTicketPositionData", ex.Message);
            }
            return result;
        }
        public bool InsertPoTicket(CreateTicketModel po, string ticket_number)
        {
            var result = false;
            try
            {
                var sql = "INSERT INTO tr_po_ticket(ticket_number, po_number, gr_qty, qty_billed, uom, currency, gr_amount, amount_invoice, amount_total, po_name, po_item_number, is_disabled, vendor_origin, document_no, item_1, gl_description) VALUES ";
                var qty_billed_list = po.qty_billed;
                var gr_qty = po.gr_qty;
                var uom = po.uom;
                var currency = po.currency;
                var gr_amount = po.gr_amount;
                var amount_invoice = po.amount_invoice;
                var amount_total = po.amount_total;
                var po_name = po.po_name;
                var po_item_number = po.po_item_number;
                var is_disabled = po.is_disabled;
                var vendor_origin = po.vendor_origin;
                var document_no = po.document_no;
                var item_1 = po.item_1;
                var gl_description = po.gl_description;
                for(var i = 0; i < uom.Length; i++) {
                    var gr_qty_replace = new Helpers.GlobalFunction().ClearFormatCurrency(gr_qty[i]);
                    var qty_billed_list_replace = new Helpers.GlobalFunction().ClearFormatCurrency(qty_billed_list[i]);
                    var gr_amount_replace = new Helpers.GlobalFunction().ClearFormatCurrency(gr_amount[i]);
                    var amount_invoice_replace = new Helpers.GlobalFunction().ClearFormatCurrency(amount_invoice[i]);
                    var amount_total_replace = new Helpers.GlobalFunction().ClearFormatCurrency(amount_total[i]);

                    var value_document_no = document_no[i] == "null" ? "NULL" : "'" + document_no[i] + "'";
                    var value_item_1 = item_1[i] == "null" ? "NULL" : "'" + item_1[i] + "'";
                    var value_gl_desc = gl_description[i] == "null" ? "NULL" : "'" + gl_description[i] + "'";
                    var value_vendor_origin = vendor_origin[i] == "null" ? "NULL" : "'" + vendor_origin[i] + "'";

                    if (i != uom.Length - 1 ) sql += "('"+ticket_number+"', '"+po.po_number+"', '"+gr_qty_replace+"', '"+qty_billed_list_replace+"', '"+uom[i].ToString()+"', '"+currency[i].ToString()+"', '"+gr_amount_replace+"', '"+amount_invoice_replace+"', '"+amount_total_replace+"', '"+po_name[i].ToString() +"', '"+po_item_number[i].ToString()+"', "+is_disabled[i]+", "+value_vendor_origin+", "+value_document_no+", "+value_item_1+", "+value_gl_desc+"),";
                    else sql += "('"+ticket_number+"', '"+po.po_number+"', '"+gr_qty_replace+"', '"+qty_billed_list_replace+"', '"+uom[i].ToString()+"', '"+currency[i].ToString()+"', '"+gr_amount_replace+"', '"+amount_invoice_replace+"', '"+amount_total_replace+"', '"+po_name[i].ToString() +"', '"+po_item_number[i].ToString()+"', "+is_disabled[i]+", "+value_vendor_origin+", "+value_document_no+", "+value_item_1+", "+value_gl_desc+")";
                }
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    Console.WriteLine("sql insert po ticket: "+sql);
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
                Console.WriteLine("Error di po: " + ex);
                new Helpers.GlobalFunction().LogError("InsertPoTicket", ex.Message);
            }
            return result;
        }
        public bool InsertGlTicket(CreateTicketModel gl, string ticket_number)
        {
            var result = false;
            try
            {
                var sql = "INSERT INTO tr_gl_ticket(ticket_number, gl_number, gl_debit_or_credit, gl_amount, gl_cost_center) VALUES ";
                var gl_number = gl.gl_number;
                var gl_debit_or_credit = gl.gl_debit_or_credit;
                var gl_amount = gl.gl_amount;
                var gl_cost_center = gl.gl_cost_center;
                for(var i = 0; i < gl_number.Length; i ++) {
                    var gl_amount_replace = new Helpers.GlobalFunction().ClearFormatCurrency(gl_amount[i].ToString());

                    if (i != gl_number.Length - 1 ) sql += "('"+ticket_number+"', '"+gl_number[i].ToString()+"', '"+gl_debit_or_credit[i].ToString()+"', '"+gl_amount_replace+"', '"+gl_cost_center[i].ToString()+"'),";
                    else sql += "('"+ticket_number+"', '"+gl_number[i].ToString()+"', '"+gl_debit_or_credit[i].ToString()+"', '"+gl_amount_replace+"', '"+gl_cost_center[i].ToString()+"')";
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
                Console.WriteLine("Error: " + ex.Message);
                new Helpers.GlobalFunction().LogError("InsertGlTicket", ex.Message);
            }
            return result;
        }
        public bool InsertMaterialTicket(CreateTicketModel material, string ticket_number)
        {
            var result = false;
            try
            {
                var sql = "INSERT INTO tr_material_ticket(ticket_number, material_id, material_plnt, material_amt, material_qty, material_d_or_c, material_total) VALUES ";
                var material_id = material.material_id;
                var material_plnt = material.material_plnt;
                var material_amt = material.material_amt;
                var material_qty = material.material_qty;
                var material_d_or_c = material.material_d_or_c;
                var material_total = material.material_total;
                for(var i = 0; i < material_id.Length; i ++) {
                    var material_amt_replace = new Helpers.GlobalFunction().ClearFormatCurrency(material_amt[i].ToString());
                    var material_qty_replace = new Helpers.GlobalFunction().ClearFormatCurrency(material_qty[i].ToString());
                    var material_total_replace = new Helpers.GlobalFunction().ClearFormatCurrency(material_total[i].ToString());
                    if (i != material_id.Length - 1 ) sql += "('"+ticket_number+"', '"+material_id[i].ToString()+"', '"+material_plnt[i].ToString()+"', '"+material_amt_replace+"', "+material_qty_replace+", '"+material_d_or_c[i].ToString()+"', '"+material_total_replace+"'),";
                    else sql += "('"+ticket_number+"', '"+material_id[i].ToString()+"', '"+material_plnt[i].ToString()+"', '"+material_amt_replace+"', "+material_qty_replace+", '"+material_d_or_c[i].ToString()+"', '"+material_total_replace+"')";
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
                Console.WriteLine("Error: " + ex.Message);
                new Helpers.GlobalFunction().LogError("InsertMaterialTicket", ex.Message);
            }
            return result;
        }
        public bool UpdateIsUsedSvcTransaction(string po_number, string[] po_item_number, string vendor_number, bool is_used)
        {
            // update temp_po_svc
            var result = false;
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "";
                    var used = is_used == false ? 0 : 1;
                    for(var i = 0; i < po_item_number.Length; i++) {
                        sql += "UPDATE temp_po_svc SET is_used = "+used+" WHERE vendor_number = '"+vendor_number+"' AND po_number = '"+po_number+"' AND po_item_number = "+po_item_number[i]+"; ";
                    }
                    SqlCommand command = new SqlCommand(sql, cs);
                    cs.Open();
                        
                    result = command.ExecuteNonQuery()== 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error di update po: " + ex);
                new Helpers.GlobalFunction().LogError("UpdateIsUsedSvcTransaction", ex.Message);
            }
            return result;
        }
        void queryTransaction(SqlConnection cs, SqlTransaction trans, string query) {

        }
        public string updateIsUsedPoTransactionQuery(dynamic model, Dictionary<string, dynamic> condition = null) {
            var sql = "";
            var po_number = model.po_number;
            var gr_number = model.gr_number;
            var company_id = model.company_id;
            var qty_billed = model.vendor_origin is Array  ?  model.qty_billed :  model.qty_billed.ToArray();
            var vendor_origin = model.vendor_origin is Array ? model.vendor_origin : model.vendor_origin.ToArray();
            var document_no = model.document_no;
            var item_1 = model.item_1;
            var po_type_id = model.po_type_id;
            var po_item_number = model.po_item_number is Array ? model.po_item_number : model.po_item_number.ToArray();
            var gr_amount = model.gr_amount is Array ? model.gr_amount : model.gr_amount.ToArray();
            bool is_custom_used = false;
            bool has_create_model = false;
            int custom_used = 0;
            if (condition != null)
                foreach (KeyValuePair<string, dynamic> con in condition)
                {
                    if (!string.IsNullOrEmpty(con.Value.ToString()))
                    {
                            if (con.Key == "is_used")  {
                                is_custom_used = true;
                                custom_used = con.Value;
                            }
                    }
                }
            if (model is CreateTicketModel) 
                has_create_model = true;
            

            for(var i = 0; i < po_item_number.Length; i++) {
                var is_used = is_custom_used ? custom_used : 0;
                if (has_create_model) {
                    var qty_billed_string = new Helpers.GlobalFunction().ClearFormatCurrency(qty_billed[i]);
                    var qty_billed_decimal = decimal.Parse(Regex.Replace(qty_billed_string, @"[^\d.]", ""));
                    if(qty_billed_decimal > 0 ) {
                        is_used = is_custom_used ? custom_used : 1; //default set is_used = 1 (for create model)
                    } else continue;
                }
                // use temp_po_svc
                var gr_sql = "";
                if (po_type_id == 1) sql += "UPDATE temp_po_svc";
                else  {
                // use temp_po_lpb
                    sql += "UPDATE temp_po_lpb";
                    gr_sql += " AND lpb_number= '"+gr_number+ "' AND document_no= '"+document_no[i]+ "' AND item_1= '"+item_1[i]+ "' ";
                }
                sql +=" SET is_used = "+is_used+" WHERE company_id = '"+company_id+"' ";
                if (po_type_id != 2) sql +=" AND vendor_number = '"+vendor_origin[i]+ "' ";
                sql +=" AND po_number = '"+po_number+"' AND po_item_number = '"+po_item_number[i]+ "' "+gr_sql+"; ";
            }

            Console.WriteLine("sql update po is_used: "+sql);
            return sql;
        }

        public bool UpdateIsUsedPoTransaction(CreateTicketModel model, Dictionary<string, dynamic> condition = null)
        {
            // update temp_po_svc
            var result = false;
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql =this.updateIsUsedPoTransactionQuery(model, condition);

                    SqlCommand command = new SqlCommand(sql, cs);
                    cs.Open();
                        
                    result = command.ExecuteNonQuery()== 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error di update po is_used: " + ex);
                new Helpers.GlobalFunction().LogError("UpdateIsUsedPoTransaction", ex.Message);
            }
            return result;
        }
        public bool UpdateIsUsedLpbTransaction(string po_number, string gr_number, string[] po_item_number, string vendor_number, bool is_used)
        {
            // update temp_po_lpb
            var result = false;
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "";
                    var used = is_used == false ? 0 : 1;
                    for(var i = 0; i < po_item_number.Length; i++) {
                        sql += "UPDATE temp_po_lpb SET is_used = "+used+" WHERE vendor_number = '"+vendor_number+"' AND po_number = '"+po_number+"' AND lpb_number = '"+gr_number+"' AND po_item_number = "+po_item_number[i]+"; ";
                    }
                    Console.WriteLine(sql);
                    SqlCommand command = new SqlCommand(sql, cs);
                    cs.Open();
                        
                    result = command.ExecuteNonQuery()== 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error di update lpb: " + ex);
                new Helpers.GlobalFunction().LogError("UpdateIsUsedLpbTransaction", ex.Message);
            }
            return result;
        }
        public bool DeletePoTicket(string ticket_number)
        {
            var result = false;
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "DELETE FROM tr_po_ticket WHERE ticket_number = @ticket_number";
                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@ticket_number", ticket_number);
                    cs.Open();

                    result = command.ExecuteNonQuery() == 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("DeletePoTicket", ex.Message);
            }
            return result;
        }
        public bool DeleteGlTicket(string ticket_number)
        {
            var result = false;
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "DELETE FROM tr_gl_ticket WHERE ticket_number = @ticket_number";
                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@ticket_number", ticket_number);
                    cs.Open();

                    result = command.ExecuteNonQuery() == 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("DeleteGlTicket", ex.Message);
            }
            return result;
        }
        public bool DeleteMaterialTicket(string ticket_number)
        {
            var result = false;
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "DELETE FROM tr_material_ticket WHERE ticket_number = @ticket_number";
                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@ticket_number", ticket_number);
                    cs.Open();

                    result = command.ExecuteNonQuery() == 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("DeleteMaterialTicket", ex.Message);
            }
            return result;
        }

        public int CountTicketData(int position_data, PostDatatableNetModel param, Dictionary<string, dynamic> condition = null, bool except_role_id = false, int session_role = 0,string user_id = null) {
            int result = 0;
            string status_column = session_role == 2 ? "B.label_for_vendor" : "B.label_for_intern";
            List<string> date_columns = new List<string>() { "created_at", "received_date", "posting_date", "remmitance_date" };
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    string string_condition = "";      
                    if (position_data > 0 && !except_role_id)
                    {
                        string_condition = " AND A.position_data = @position_data";
                    }
                    else if (position_data > 0 && except_role_id)
                    {
                        string_condition = " AND A.position_data != @position_data";
                    }


                    int[] indesso_role = {3,4,5,6,7,8 };
                    foreach (int item in indesso_role){
                        if (item == session_role){
                            List <string> area_list = this.GetUserArea(user_id);
                            
                            string_condition += " AND A.area_code IN ("+ string.Join(",", area_list) + ")";
                        }
                    }
                    foreach (KeyValuePair<string, dynamic> con in condition)
                    {
                        if (!string.IsNullOrEmpty(con.Value.ToString()))
                        {
                            if (System.Text.RegularExpressions.Regex.IsMatch(con.Key, @"^.*(\<|\>|\!\=|\>\=|\<\=).*$"))
                            {
                                string val_scalar = System.Text.RegularExpressions.Regex.Replace(con.Key, @"\<|\>|\!\=|\>\=|\<\=", "").Trim();
                                string key_name = val_scalar == "verification_status_id" || val_scalar == "vendor_number" ? "A." + con.Key : con.Key;
                                string_condition += " AND " + key_name + " @" + val_scalar;
                            }
                            else
                            {
                                string key_name = con.Key == "verification_status_id" || con.Key == "vendor_number" ? "A." + con.Key : con.Key;
                                string_condition += " AND " + key_name + " = @" + con.Key;
                            }
                        }
                    }
                    foreach (columnDatatable col in param.columns)
                    {
                        bool searchable = col.searchable == "true" ? true : false;
                        bool orderable = col.orderable == "true" ? true : false;
                        if (!string.IsNullOrEmpty(col.search.value) && searchable)
                        {
                            if (col.data == "status")
                            {
                                string_condition += " AND " + status_column + " LIKE @" + col.data;
                            }
                            else if (col.data == "vendor_name")
                            {
                                string_condition += " AND C.name = @" + col.data;
                            }
                            else if (date_columns.Contains(col.data))
                            {
                                string_condition += " AND convert(varchar(10),A."+col.data+", 120) = @" + col.data;
                            }
                            else
                            {
                                string_condition += " AND A." + col.data + " = @" + col.data;
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
                                command.Parameters.AddWithValue("@" + val_scalar, con.Value);
                            }
                            else
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
                            if (col.data == "status")
                            {
                                command.Parameters.AddWithValue("@" + col.data, "%" + col.search.value + "%");
                            }
                            else
                            {
                                command.Parameters.AddWithValue("@" + col.data, col.search.value);
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
                new Helpers.GlobalFunction().LogError("CountTicketData", ex.Message);
                Console.WriteLine("hallooo: "+ex.Message);
            }
            return result;
        }
        public List<string> GetUserArea(string user_id)
        {
            List<String> result = new List<String>();
            try
            {
                using (var cs = new ConnectionService(_appSettings).GetConnection())
                {
                    var sql = "SELECT area_code from tb_user_area WHERE user_id=" +user_id;

                    SqlCommand command = new SqlCommand(sql, cs);
                    cs.Open();
                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        
                        while (reader.Read()) {
                            result.Add(reader["area_code"].ToString());
                        }
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("GetUserArea", ex.Message);
            }
            return result;
        }

        public Dictionary<string, dynamic> GetTicketDatatable(int position_data, PostDatatableNetModel param, List<string> selected_column, Dictionary<string, dynamic> condition = null, bool except_role_id = false, int session_role = 0, string vendor_number = null, string user_id = null)
        {
            List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();
            int total_data = 0;
            int total_page = 0;
            string sorttype = null;
            string sortfield = null;
            List<string> date_columns = new List<string>() { "created_at", "received_date", "posting_date", "remmitance_date" };
            string status_column = session_role == 2 ? "B.label_for_vendor" : "B.label_for_intern";
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    string string_condition = "";
                    if ( position_data > 0 && !except_role_id )
                    {
                        string_condition = " AND A.position_data = @position_data";
                    } else if (position_data > 0 && except_role_id)
                    {
                        string_condition = " AND A.position_data != @position_data";
                    }
                    if (vendor_number != null) {
                        condition.Add("vendor_number", vendor_number);
                    }
                    if (session_role == 8) {
                        string_condition += " AND A.is_paid = 1";
                    }

                    int[] indesso_role = {3,4,5,6,7,8 };
                    foreach (int item in indesso_role){
                        if (item == session_role){
                            List <string> area_list = this.GetUserArea(user_id);
                            
                            string_condition += " AND A.area_code IN ("+ string.Join(",", area_list) + ")";
                        }
                    }
                    foreach (KeyValuePair<string, dynamic> con in condition)
                    {
                        if (!string.IsNullOrEmpty(con.Value.ToString()))
                        {
                            if (System.Text.RegularExpressions.Regex.IsMatch(con.Key, @"^.*(\<|\>|\!\=|\>\=|\<\=).*$"))
                            {
                                string val_scalar = System.Text.RegularExpressions.Regex.Replace(con.Key, @"\<|\>|\!\=|\>\=|\<\=", "").Trim();
                                string key_name = val_scalar == "verification_status_id" || con.Key == "vendor_number" ? "A." + con.Key : con.Key;
                                string_condition += " AND " + key_name + " @" + val_scalar;
                            }
                            else
                            {
                                if (con.Key == "label_for_vendor") {
                                    string_condition += " AND A.verification_status_id IN (7,8,15,18) ";
                                } else {
                                    string key_name = con.Key == "verification_status_id" || con.Key == "vendor_number" ? "A." + con.Key : con.Key;
                                    string_condition += " AND " + key_name + " = @" + con.Key;
                                }
                            }
                        }
                    }
                    foreach ( columnDatatable col in param.columns )
                    {
                        bool searchable = col.searchable == "true" ? true : false;
                        bool orderable = col.orderable == "true" ? true : false;
                        if (!string.IsNullOrEmpty(col.search.value) && searchable)
                        {
                            if (col.data == "status")
                            {
                                string_condition += " AND " + status_column + " LIKE @" + col.data;
                            }
                            else if (col.data == "vendor_name")
                            {
                                string_condition += " AND C.name = @" + col.data;
                            }
                            else if (date_columns.Contains(col.data))
                            {
                                string_condition += " AND convert(varchar(10),A." + col.data + ", 120) = @" + col.data;
                            }
                            else
                            {
                                string_condition += " AND A." + col.data + " = @" + col.data;
                            }
                        }    
                    }
                    // Console.WriteLine("string: "+string_condition);

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

                            if (sortfield == "status")
                            {
                                temp_string_order += " " + status_column + " " + sorttype;
                            }
                            else if (sortfield == "vendor_name")
                            {
                                temp_string_order += " C.name " + sorttype;
                            }
                            else
                            {
                                temp_string_order += " A." + sortfield + " " + sorttype;
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(temp_string_order))
                        string_order = " ORDER BY" + temp_string_order;

                    // Console.WriteLine("string condition: "+string_condition);
                    var sql = "SELECT A.created_at, A.received_date, A.ticket_number, A.posting_date, " +
                                "A.miro_number, A.posting_date, A.remmitance_number, A.remmitance_date, " +
                                "A.verification_status_id, "+ status_column + " AS status, A.position_data, A.vendor_number," +
                                "C.name AS vendor_name, A.invoice_number, A.invoice_amount, D.role_name AS position_label, " +
                                "A.rpa_status_description, A.status_rpa " +
                                "FROM tr_ticket A " +
                                "JOIN m_verification_status B ON A.verification_status_id = B.verification_status_id " +
                                "JOIN tr_vendor C ON A.vendor_number = C.vendor_number " +
                                "LEFT JOIN tb_role D ON A.position_data = D.role_id " +
                                "WHERE 1=1" + string_condition + string_order + " " +
                                "OFFSET @start ROWS FETCH NEXT @length ROWS ONLY";
                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@start", Convert.ToInt32(param.start));
                    command.Parameters.AddWithValue("@length", Convert.ToInt32(param.length));
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
                            if (col.data == "status")
                            {
                                command.Parameters.AddWithValue("@" + col.data, "%"+col.search.value+ "%");
                            } else
                            {
                                command.Parameters.AddWithValue("@" + col.data, col.search.value);
                            }
                            
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

                total_data = this.CountTicketData(position_data, param, condition, except_role_id, session_role,user_id);
                total_page = (total_data + param.length - 1) / param.length;
            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("GetTicketDatatable", ex.Message);
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

        public IEnumerable<TaxStatusModel> GetTaxStatus()
        {
            List<TaxStatusModel> result = new List<TaxStatusModel>();
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "SELECT tax_status_id, name FROM m_tax_status";

                    SqlCommand command = new SqlCommand(sql, cs);
                    cs.Open();
                    var reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result.Add(new TaxStatusModel() 
                            {
                                tax_status_id = Convert.ToInt32(reader["tax_status_id"]),
                                name = reader["name"].ToString()
                            });
                        }
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: "+ ex.Message);
                new Helpers.GlobalFunction().LogError("GetTaxStatus", ex.Message);
            }
            return result;
        }
        public IEnumerable<PoTypeModel> GetPOType(int po_type_group_id)
        {
            List<PoTypeModel> result = new List<PoTypeModel>();
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "SELECT po_type_id, name FROM m_po_type WHERE po_type_group_id = @po_type_group_id";

                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@po_type_group_id", po_type_group_id);
                    cs.Open();
                    var reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result.Add(new PoTypeModel() 
                            {
                                po_type_id = Convert.ToInt32(reader["po_type_id"]),
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
                new Helpers.GlobalFunction().LogError("GetPOType", ex.Message);
            }
            return result;
        }
        
        public IEnumerable<SimulateXlsRequest> GetSimulationResult(string ticket_number){
            IEnumerable<SimulateXlsRequest> result = new List<SimulateXlsRequest>();
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var q = cs.FluentQueryBuilder()
                                .Select($@"*")
                                .From($@"tr_simulate")
                                .Where($@"ticket_number = {ticket_number}");

                    result = q.Query<SimulateXlsRequest>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("GetSimulationResult", ex.Message);
            }

                Console.WriteLine(JsonSerializer.Serialize(result));

            return result;
        }
        public IEnumerable<PoTransactionModel> GetPONumber(int po_type_group_id, int po_type, string vendor_number, string company_id = null)
        {
            List<PoTransactionModel> result = new List<PoTransactionModel>();
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "";
                    if (po_type == 1) {
                        sql += "SELECT DISTINCT(A.po_number) FROM temp_po_svc A " +
                              "WHERE 1=1";
                    } else {
                        sql += "SELECT DISTINCT(A.po_number) FROM temp_po_lpb A " +
                              "WHERE 1=1";
                    } 
                    if(!String.IsNullOrEmpty(company_id))
                        sql += " AND A.company_id = @company_id";

                    if(po_type_group_id == 1) {
                        sql += " AND A.vendor_number = @vendor_number ";
                        if (po_type == 2) {
                            sql += " AND LOWER(A.gl_description) NOT LIKE '%accrued%' ";
                        } 
                    } else if(po_type_group_id == 2) {
                        sql += " AND LOWER(A.gl_description) LIKE '%accrued%'";
                    } else if(po_type_group_id == 3) {
                        sql += " AND A.vendor_number = @vendor_number";
                    }
                    Console.WriteLine("sql po_number: "+sql);
                    SqlCommand command = new SqlCommand(sql, cs);
                    if(!String.IsNullOrEmpty(company_id))
                        command.Parameters.AddWithValue("@company_id", company_id);
                    if(po_type_group_id == 1 || po_type_group_id == 3)
                        command.Parameters.AddWithValue("@vendor_number", vendor_number);

                    cs.Open();
                    var reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result.Add(new PoTransactionModel() 
                            {
                                po_number = reader["po_number"].ToString()
                            });
                        }
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("GetPONumber", ex.Message);
            }
            return result;
        }
        public PoTransactionModel GetPONumberByNumber(string company_id, string po_number, string vendor_number)
        {
            PoTransactionModel result = new PoTransactionModel();
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "SELECT A.po_item_number FROM temp_po_lpb A "+
                              "WHERE A.po_number = @po_number AND A.vendor_number = @vendor_number AND A.company_id = @company_id";
                    
                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@po_number", po_number);
                    command.Parameters.AddWithValue("@vendor_number", vendor_number);
                    command.Parameters.AddWithValue("@company_id", company_id);
                    cs.Open();
                    var reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result.po_item_number = reader["po_item_number"].ToString();
                        }
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("GetPONumberByNumber", ex.Message);
            }
            return result;
        }
        public IEnumerable<PoTransactionModel> GetPOTable(int po_type_group_id, string company_id, string po_number, string vendor_number, string gr_number = null)
        {
            List<PoTransactionModel> result = new List<PoTransactionModel>();
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "";
                    if(!(String.IsNullOrEmpty(gr_number))) {
                        sql = "SELECT A.po_number, A.po_item_number, A.company_id, A.po_name, A.uom, A.currency, A.amount_in_dc as amount, A.quantity_lpb as qty,  A.is_active, A.is_used, A.vendor_number as vendor_origin, A.document_no, A.item_1, A.gl_description " +
                                "FROM temp_po_lpb A " +
                                "WHERE A.po_number = @po_number AND A.lpb_number = @gr_number " +
                                "AND A.company_id = @company_id";
                    } else {
                        sql = "SELECT A.po_number, A.po_item_number, A.company_id, A.po_name, A.uom, A.currency, A.net_price as amount, A.po_quantity as qty, A.is_active, A.is_used, A.vendor_number as vendor_origin, NULL as document_no, NULL as item_1, NULL as gl_description " +
                                "FROM temp_po_svc A " +
                                "WHERE A.po_number = @po_number AND A.company_id = @company_id";
                    }

                    if(po_type_group_id == 1) {
                        sql += " AND A.vendor_number = @vendor_number ";
                        if (!(String.IsNullOrEmpty(gr_number))) {
                            sql += " AND LOWER(A.gl_description) NOT LIKE '%accrued%' ";
                        } 
                    } else if(po_type_group_id == 2) {
                        sql += " AND LOWER(A.gl_description) LIKE '%accrued%'";
                    } else if(po_type_group_id == 3) {
                        sql += " AND A.vendor_number = @vendor_number";
                    }

                    Console.WriteLine("sql ticekt : "+sql);
                    Console.WriteLine("po number : "+po_number);
                    Console.WriteLine("vendor number : "+vendor_number);
                    Console.WriteLine("gr number : "+gr_number);
                    Console.WriteLine("company : "+company_id);

                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@po_number", po_number);
                    command.Parameters.AddWithValue("@company_id", company_id);
                    if(po_type_group_id == 1 || po_type_group_id == 3)
                        command.Parameters.AddWithValue("@vendor_number", vendor_number);
                    if(!(String.IsNullOrEmpty(gr_number)))
                        command.Parameters.AddWithValue("@gr_number", gr_number);
                    cs.Open();
                    var reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result.Add(new PoTransactionModel(){
                                po_number = reader["po_number"].ToString(),
                                po_name = reader["po_name"].ToString(),
                                po_item_number = reader["po_item_number"].ToString(),
                                qty =  Convert.ToDecimal(reader["qty"].ToString()),
                                uom = reader["uom"].ToString(),
                                currency = reader["currency"].ToString(),
                                amount = Convert.ToDouble(reader["amount"].ToString()),
                                vendor_origin = reader["vendor_origin"] == DBNull.Value ? null : reader["vendor_origin"].ToString(),
                                document_no =  reader["document_no"] == DBNull.Value ? null : reader["document_no"].ToString(),
                                item_1 = reader["item_1"]  == DBNull.Value ? null :reader["item_1"].ToString(),
                                gl_description = reader["gl_description"]  == DBNull.Value ? null :reader["gl_description"].ToString(),
                                is_used = reader["is_used"].ToString() == "False" ? false : true,
                                is_active = reader["is_active"].ToString() == "False" ? false : true
                            });
                        }
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("GetPOTable", ex.Message);
            }
            return result;
        }
        public IEnumerable<LpbTransactionModel> GetGrNumber(string po_number, int po_type_group_id, int po_type, string vendor_number, string company_id = null)
        {
            List<LpbTransactionModel> result = new List<LpbTransactionModel>();
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "";
                    if (po_type == 1) {
                        sql += "SELECT DISTINCT(A.lpb_number) FROM temp_po_svc A " +
                              "WHERE A.po_number = @po_number";
                    } else {
                        sql += "SELECT DISTINCT(A.lpb_number) FROM temp_po_lpb A " +
                              "WHERE A.po_number = @po_number";
                    } 
                    if(!String.IsNullOrEmpty(company_id))
                        sql += " AND A.company_id = @company_id";

                    if(po_type_group_id == 1) {
                        sql += " AND A.vendor_number = @vendor_number ";
                        if (po_type == 2) {
                            sql += " AND LOWER(A.gl_description) NOT LIKE '%accrued%' ";
                        } 
                    } else if(po_type_group_id == 2) {
                        sql += " AND LOWER(A.gl_description) LIKE '%accrued%'";
                    } else if(po_type_group_id == 3) {
                        sql += " AND A.vendor_number = @vendor_number";
                    }

                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@po_number", po_number);
                    if(!String.IsNullOrEmpty(company_id))
                        command.Parameters.AddWithValue("@company_id", company_id);
                    if(po_type_group_id == 1 || po_type_group_id == 3)
                        command.Parameters.AddWithValue("@vendor_number", vendor_number);

                    cs.Open();
                    var reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result.Add(new LpbTransactionModel() 
                            {
                                lpb_number = reader["lpb_number"].ToString()
                            });
                        }
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("GetGrNumber", ex.Message);
            }
            return result;
        }
        public IEnumerable<LocationModel> GetLocation()
        {
            List<LocationModel> result = new List<LocationModel>();
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "SELECT location_id, name FROM m_location";

                    SqlCommand command = new SqlCommand(sql, cs);
                    cs.Open();
                    var reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result.Add(new LocationModel() 
                            {
                                location_id = Convert.ToInt32(reader["location_id"]),
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
                new Helpers.GlobalFunction().LogError("GetLocation", ex.Message);
            }
            return result;
        }
        public LocationModel GetAreaById(string area_code)
        {
            LocationModel result = new LocationModel();
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "SELECT location_id, name FROM m_location WHERE location_id = @area_code";

                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@area_code", area_code);

                    cs.Open();
                    var reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result.location_id = Convert.ToInt32(reader["location_id"]);
                            result.name = reader["name"].ToString();
                        }
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("GetAreaById", ex.Message);
            }
            return result;
        }
        public IEnumerable<GlItemModel> GetGlItem()
        {
            List<GlItemModel> result = new List<GlItemModel>();
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "SELECT gl_id, description FROM m_gl_items";

                    SqlCommand command = new SqlCommand(sql, cs);
                    cs.Open();
                    var reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result.Add(new GlItemModel() 
                            {
                                gl_id = reader["gl_id"].ToString(),
                                description = reader["description"].ToString()
                            });
                        }
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("GetGlItem", ex.Message);
            }
            return result;
        }
        public IEnumerable<CcItemModel> GetCcItem()
        {
            List<CcItemModel> result = new List<CcItemModel>();
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "SELECT cc_id, description FROM m_cc_items WHERE is_delete = 0";

                    SqlCommand command = new SqlCommand(sql, cs);
                    cs.Open();
                    var reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result.Add(new CcItemModel() 
                            {
                                cc_id = reader["cc_id"].ToString(),
                                description = reader["description"].ToString()
                            });
                        }
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("GetCcItem", ex.Message);
            }
            return result;
        }

        public IEnumerable<MaterialItemModel> GetMaterialItem()
        {
            List<MaterialItemModel> result = new List<MaterialItemModel>();
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "SELECT material_id, material_plnt, description FROM m_material_items WHERE is_delete = 0";

                    SqlCommand command = new SqlCommand(sql, cs);
                    cs.Open();
                    var reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result.Add(new MaterialItemModel() 
                            {
                                material_id = reader["material_id"].ToString(),
                                material_plnt = reader["material_plnt"].ToString(),
                                description = reader["description"].ToString()
                            });
                        }
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("GetMaterialItem", ex.Message);
            }
            return result;
        }

        
        public IEnumerable<MaterialItemModel> GetMaterialList(int offset, int limitRow, string search = null){
            IEnumerable<MaterialItemModel> result = new List<MaterialItemModel>();

            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var q = cs.QueryBuilder($"SELECT * FROM m_material_items");
                    result = q.Query<MaterialItemModel>();
                    // q.Append($"ORDER BY description OFFSET {offset} ROWS FETCH NEXT {limitRow} ROWS ONLY");

                    if(!String.IsNullOrEmpty(search)){
                        result = result.Where(t => !string.IsNullOrEmpty(t.description) && t.description.ToLower().Contains(search.ToLower()));
                    }

                    result = result.Skip(offset).Take(limitRow);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("GetMaterialList", ex.Message);
            }
            return result;
        }

        public IEnumerable<dynamic> GetMaterialListCount(){
            IEnumerable<dynamic> result = new List<dynamic>();

            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var q = cs.FluentQueryBuilder()
                    .Select($"COUNT(*) AS c")
                    .From($"m_material_items");

                    result = q.Query<dynamic>();

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("GetMaterialListCount", ex.Message);
            }
            return result;
        }

        public int CountMaterialItem(string q) 
        {
            int result = 0;
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "SELECT COUNT(material_id) as total FROM m_material_items WHERE description LIKE '%"+q+"%'" + " or material_id LIKE '%" +q+"%'";

                    SqlCommand command = new SqlCommand(sql, cs);
                    cs.Open();
                    var reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result = Convert.ToInt32(reader["total"].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = 0;
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("CountMaterialItem", ex.Message);
            }

            return result;
        }

        public IEnumerable<SelectModel> GetMaterialItem(string q)
        {
            List<SelectModel> result = new List<SelectModel>();
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "SELECT TOP 20 material_id, description FROM m_material_items WHERE description LIKE '%"+q+"%'" + " or material_id LIKE '%" +q+"%'";

                    SqlCommand command = new SqlCommand(sql, cs);
                    cs.Open();
                    var reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result.Add(new SelectModel() 
                            {
                                id = reader["material_id"].ToString(),
                                text = reader["material_id"].ToString() + " - " + reader["description"].ToString(),
                                value = reader["material_id"].ToString()
                            });
                        }
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("erorrrr"+ex);
                new Helpers.GlobalFunction().LogError("GetMaterialItem", ex.Message);
            }
            return result;
        }

        public MaterialItemModel GetMaterialItemById(string material_id)
        {
            MaterialItemModel result = new MaterialItemModel();
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "SELECT * FROM m_material_items" +
                              " WHERE material_id = @material_id";

                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@material_id", material_id);
                    cs.Open();
                    var reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result.material_id = reader["material_id"].ToString();
                            result.material_plnt = reader["material_plnt"].ToString();
                            result.description = reader["description"].ToString();
                        }
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("GetMaterialItemById", ex.Message);
            }
            return result;
        }

        public string GetMaxTicketNumber(string year, string month, string area_code = null)
        {
            string result = "";
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "SELECT MAX(ticket_number) AS ticket_number FROM tr_ticket " +
                              "WHERE YEAR(created_at) = @year " +
                              "AND MONTH(created_at) = @month";
                    if(!String.IsNullOrEmpty(area_code)) 
                        sql += " AND area_code = @area_code";

                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@year", year);
                    command.Parameters.AddWithValue("@month", month);
                    if(!String.IsNullOrEmpty(area_code)) 
                        command.Parameters.AddWithValue("@area_code", area_code);
                    cs.Open();
                    var reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result = reader["ticket_number"].ToString();
                        }
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                result = "";
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("GetMaxTicketNumber", ex.Message);
            }
            return result;
        }

        public IEnumerable<CompanyModel> GetCompany(string vendor_number)
        {
            List<CompanyModel> result = new List<CompanyModel>();
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "SELECT DISTINCT(A.company_id), B.name " +
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
                new Helpers.GlobalFunction().LogError("Log Vendor", ex.Message);
            }
            return result;
        }

        public TicketModel GetVendorForTicket(String vendor_number) {
            TicketModel result = new TicketModel();

            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "SELECT " +
                              "A.vendor_number, A.currency_id, A.top_id, B.interval AS top_interval, A.name, A.contact_person, " +
                              "A.additional_street_address, A.country_id AS country_vendor, " +
                              "A.telephone, A.email, A.tax_type_id, G.name AS tax_type_name, G.sap_code, A.npwp, " +
                              "A.localidr_bank_id, C.name AS localidr_bank_name, C.swift_code AS localidr_swift_code, " +
                              "A.localidr_bank_account, A.localidr_account_holder, " +
                              "A.localforex_bank_id, D.name AS localforex_bank_name, D.swift_code AS localforex_swift_code, " +
                              "A.localforex_bank_account, A.localforex_account_holder, A.localforex_currency_id, " +
                              "A.foreign_bank_country_id, F.name AS foreign_bank_country_name, " +
                              "A.foreign_bank_id, E.name AS foreign_bank_name, E.swift_code AS foreign_swift_code, " +
                              "A.foreign_bank_account, A.foreign_account_holder, A.foreign_currency_id, A.is_locked, " +
                              "A.mailing_address, A.mailing_district, A.mailing_village, A.mailing_province " +
                              "FROM tr_vendor A " +
                              "LEFT JOIN m_top B ON A.top_id = B.top_id " +
                              "LEFT JOIN m_bank C ON A.localidr_bank_id = C.bank_id " +
                              "LEFT JOIN m_bank D ON A.localforex_bank_id = D.bank_id " +
                              "LEFT JOIN m_bank E ON A.foreign_bank_id = E.bank_id " +
                              "LEFT JOIN m_country F ON A.foreign_bank_country_id = F.country_id " +
                              "LEFT JOIN m_tax_type G ON A.tax_type_id = G.tax_type_id " +
                              "WHERE A.vendor_number = @vendor_number";
                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@vendor_number", vendor_number);
                    cs.Open();
                    var reader = command.ExecuteReader();
                    {
                        while (reader.Read())
                        {
                            // result.is_locked = reader["is_locked"].ToString() == "1" ? true : false;
                            result.is_locked = reader["is_locked"].ToString() == "False" ? false : true;
                            result.vendor_number = reader["vendor_number"].ToString();
                            result.currency_id = reader["currency_id"].ToString();
                            result.top_id = reader["top_id"].ToString();
                            result.top_interval = Convert.ToInt32(reader["top_interval"].ToString());
                            result.name = reader["name"].ToString();
                            result.contact_person = reader["contact_person"].ToString();
                            result.additional_street_address = reader["additional_street_address"] == DBNull.Value ? null : reader["additional_street_address"].ToString();
                            result.country_vendor = reader["country_vendor"].ToString();
                            result.email = reader["email"].ToString();
                            result.email_list = reader["email"] == DBNull.Value ? new List<String>() : new Helpers.EmailFunction().GetEmailList(reader["email"].ToString(), ";");
                            result.tax_type_id = Convert.ToInt32(reader["tax_type_id"].ToString());
                            result.tax_type_name = reader["tax_type_name"].ToString();
                            result.sap_code = reader["sap_code"].ToString();
                            result.npwp = reader["npwp"] == DBNull.Value ? null : reader["npwp"].ToString();
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
                            result.foreign_bank_id = reader["foreign_bank_id"] == DBNull.Value ? null : reader["foreign_bank_id"].ToString();
                            result.foreign_bank_name = reader["foreign_bank_name"] == DBNull.Value ? null : reader["foreign_bank_name"].ToString();
                            result.foreign_bank_account = reader["foreign_bank_account"] == DBNull.Value ? null : reader["foreign_bank_account"].ToString();
                            result.foreign_account_holder = reader["foreign_account_holder"] == DBNull.Value ? null : reader["foreign_account_holder"].ToString();
                            result.mailing_address = reader["mailing_address"] == DBNull.Value ? null : reader["mailing_address"].ToString();
                            result.district = reader["mailing_district"] == DBNull.Value ? null : reader["mailing_district"].ToString();
                            result.village = reader["mailing_village"] == DBNull.Value ? null : reader["mailing_village"].ToString();
                            result.province = reader["mailing_province"] == DBNull.Value ? null : reader["mailing_province"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("GetVendorForTicket", ex.Message);
                Console.WriteLine("halo disini error nya: "+ex.Message);
            }
            return result;
        }

        public IEnumerable<LogTicketModel> GetLogTicket(string ticket_number)
        {
            List<LogTicketModel> result = new List<LogTicketModel>();
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "SELECT A.process, A.created_at, B.username AS created_by_name " +
                              "FROM log_ticket A " +
                              "LEFT JOIN tb_user B ON A.created_by = B.user_id " +
                              "WHERE A.ticket_number = '" + ticket_number +"'";
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
                            result.Add(new LogTicketModel()
                            {
                                process = reader["process"].ToString(),
                                created_by_name = reader["created_by_name"].ToString(),  
                                created_at = Convert.ToDateTime(reader["created_at"].ToString())                          
                            });                            
                        }
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("GetLogTicket", ex.Message);
            }
            return result;
        }

        public TicketModel GetTicketDetail(String ticket_number) {
            TicketModel result = new TicketModel();

            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "SELECT " + 
                              "A.ticket_number, A.miro_number, A.vendor_number, B.name, B.npwp, B.contact_person, B.email, B.pic_id, " +
                              "B.localidr_bank_id, E.name AS localidr_bank_name, E.swift_code AS localidr_swift_code, B.localidr_account_holder, B.localidr_bank_account, " +
                              "B.localforex_bank_id, F.name AS localforex_bank_name, F.swift_code AS localforex_swift_code, B.localforex_account_holder, B.localforex_bank_account, " +
                              "B.foreign_bank_id, G.name AS foreign_bank_name, B.foreign_bank_account, B.foreign_account_holder, " +
                              "A.country_id, B.country_id AS country_vendor, A.mailing_address, A.district, A.village, A.province, A.phone_number, " +
                              "A.company_id, I.name AS company_name, A.area_code, J.name AS location_name, " +
                              "A.top_id, K.baseline_date, K.interval AS top_interval, A.top_duration, L.sap_code, " +
                              "B.tax_type_id, L.name AS tax_type_name, A.tax_status_id, C.name AS tax_status_name, A.tax_invoice_number, A.tax_invoice_date, " +
                              "A.invoice_number, A.invoice_date, A.awb_mail, " +
                              "M.po_type_group_id, A.po_type_id, D.name AS po_type_name, A.po_number, A.gr_number, " +
                              "A.invoice_amount, A.invoice_currency, " +
                              "A.dpp, A.ppn, A.pph, A.total_price, " +
                              "A.file_invoice, A.file_po, A.file_lpb_gr, " +
                              "A.file_tax_invoice, A.file_delivery_note, A.file_bast, A.type_of_payment, " +
                              "A.is_paid, A.verification_status_id, H.label_for_intern, H.label_for_vendor, A.verification_note, A.position_data, " +
                              "A.created_at, A.updated_at, A.received_date, A.submitted_date, A.posting_date, " +
                              "A.remmitance_number, A.remmitance_date, A.request_simulate, A.is_finish, A.tax_amt_fc, A.status_rpa, A.rpa_status_description " +
                              "FROM tr_ticket A " +
                              "JOIN tr_vendor B ON A.vendor_number = B.vendor_number " +
                              "LEFT JOIN m_tax_status C ON A.tax_status_id = C.tax_status_id " +
                              "LEFT JOIN m_po_type D ON A.po_type_id = D.po_type_id " +
                              "LEFT JOIN m_bank E ON B.localidr_bank_id = E.bank_id " +
                              "LEFT JOIN m_bank F ON B.localforex_bank_id = F.bank_id " +
                              "LEFT JOIN m_bank G ON B.foreign_bank_id = G.bank_id " +
                              "LEFT JOIN m_verification_status H ON A.verification_status_id = H.verification_status_id " +
                              "LEFT JOIN m_company I ON A.company_id = I.company_id " +
                              "LEFT JOIN m_company_area J ON A.area_code = J.code " +
                              "LEFT JOIN m_top K ON A.top_id = K.top_id " +
                              "LEFT JOIN m_tax_type L ON B.tax_type_id = L.tax_type_id " +
                              "LEFT JOIN m_po_type_group M ON D.po_type_group_id = M.po_type_group_id " +
                              "WHERE A.ticket_number = @ticket_number"
                    ;
                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@ticket_number", ticket_number);

                    cs.Open();
                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result.ticket_number = reader["ticket_number"].ToString();
                            result.miro_number = reader["miro_number"] == DBNull.Value ? null : reader["miro_number"].ToString();
                            result.vendor_number = reader["vendor_number"].ToString();
                            result.name = reader["name"].ToString();
                            result.npwp = reader["npwp"].ToString();
                            result.contact_person = reader["contact_person"].ToString();
                            result.email = reader["email"].ToString();
                            result.email_list = reader["email"] == DBNull.Value ? new List<string>() : new Helpers.EmailFunction().GetEmailList(reader["email"].ToString(), ";");
                            result.pic_id = reader["pic_id"].ToString();
                            result.localidr_bank_id = reader["localidr_bank_id"] == DBNull.Value ? null : reader["localidr_bank_id"].ToString();
                            result.localidr_bank_name = reader["localidr_bank_name"] == DBNull.Value ? null : reader["localidr_bank_name"].ToString();
                            result.localidr_swift_code = reader["localidr_swift_code"] == DBNull.Value ? null : reader["localidr_swift_code"].ToString();
                            result.localidr_account_holder = reader["localidr_account_holder"] == DBNull.Value ? null : reader["localidr_account_holder"].ToString();
                            result.localidr_bank_account = reader["localidr_bank_account"] == DBNull.Value ? null : Convert.ToDecimal(reader["localidr_bank_account"].ToString());
                            result.localforex_bank_id = reader["localforex_bank_id"] == DBNull.Value ? null : reader["localforex_bank_id"].ToString();
                            result.localforex_bank_name = reader["localforex_bank_name"] == DBNull.Value ? null : reader["localforex_bank_name"].ToString();
                            result.localforex_swift_code = reader["localforex_swift_code"] == DBNull.Value ? null : reader["localforex_swift_code"].ToString();
                            result.localforex_account_holder = reader["localforex_account_holder"] == DBNull.Value ? null : reader["localforex_account_holder"].ToString();
                            result.localforex_bank_account = reader["localforex_bank_account"] == DBNull.Value ? null : Convert.ToDecimal(reader["localforex_bank_account"].ToString());
                            result.foreign_bank_name = reader["foreign_bank_name"] == DBNull.Value ? null : reader["foreign_bank_name"].ToString();
                            result.foreign_account_holder = reader["foreign_account_holder"] == DBNull.Value ? null : reader["foreign_account_holder"].ToString();
                            result.foreign_bank_account = reader["foreign_bank_account"] == DBNull.Value ? null : reader["foreign_bank_account"].ToString();
                            result.country_id = reader["country_id"].ToString();
                            result.country_vendor = reader["country_vendor"].ToString();
                            result.mailing_address = reader["mailing_address"].ToString();
                            result.district = reader["district"].ToString();
                            result.village = reader["village"].ToString();
                            result.province = reader["province"].ToString();
                            result.phone_number = reader["phone_number"].ToString();
                            result.company_id = reader["company_id"].ToString();
                            result.company_name = reader["company_name"].ToString();
                            result.area_code = reader["area_code"].ToString();
                            result.location_name = reader["location_name"].ToString();
                            result.sap_code = reader["sap_code"].ToString();
                            result.top_id = reader["top_id"].ToString();
                            result.baseline_date = reader["baseline_date"].ToString();
                            result.top_interval = Convert.ToInt32(reader["top_interval"].ToString());
                            result.top_duration = Convert.ToInt32(reader["top_duration"].ToString());
                            result.tax_type_id = Convert.ToInt32(reader["tax_type_id"].ToString());
                            result.tax_type_name = reader["tax_type_name"].ToString();
                            result.tax_status_id = Convert.ToInt32(reader["tax_status_id"].ToString());
                            result.tax_status_name = reader["tax_status_name"].ToString();
                            result.tax_invoice_number = reader["tax_invoice_number"] == DBNull.Value ? null : new Helpers.GlobalFunction().FormatTaxInvoiceNumber(reader["tax_invoice_number"].ToString(), false);
                            result.tax_invoice_date = reader["tax_invoice_date"] == DBNull.Value ? null : Convert.ToDateTime(reader["tax_invoice_date"].ToString());
                            result.invoice_number = reader["invoice_number"].ToString();
                            result.invoice_date = Convert.ToDateTime(reader["invoice_date"].ToString());
                            result.awb_mail = reader["awb_mail"].ToString();
                            result.po_type_group_id = Convert.ToInt32(reader["po_type_group_id"].ToString());
                            result.po_type_id = Convert.ToInt32(reader["po_type_id"].ToString());
                            result.po_type_name = reader["po_type_name"].ToString();
                            result.po_number = reader["po_number"].ToString();
                            result.gr_number = reader["gr_number"] == DBNull.Value ? null : reader["gr_number"].ToString();
                            result.invoice_amount = Convert.ToDouble(reader["invoice_amount"].ToString());
                            result.invoice_currency = reader["invoice_currency"].ToString();
                            result.table_po = this.GetPoTicket(reader["ticket_number"].ToString());
                            result.table_gl = this.GetGlTicket(reader["ticket_number"].ToString());
                            result.table_material = this.GetMaterialTicket(reader["ticket_number"].ToString());
                            result.dpp = Convert.ToDouble(reader["dpp"].ToString());
                            result.ppn = Convert.ToDouble(reader["ppn"].ToString());
                            result.pph = Convert.ToDouble(reader["pph"].ToString());
                            result.total_price = Convert.ToDouble(reader["total_price"].ToString());
                            result.file_invoice = reader["file_invoice"].ToString();
                            result.file_po = reader["file_po"].ToString();
                            result.file_lpb_gr = reader["file_lpb_gr"] == DBNull.Value ? null : reader["file_lpb_gr"].ToString();
                            result.file_tax_invoice = reader["file_tax_invoice"].ToString();
                            result.file_delivery_note = reader["file_delivery_note"] == DBNull.Value ? null : reader["file_delivery_note"].ToString();
                            result.file_bast = reader["file_bast"] == DBNull.Value ? null : reader["file_bast"].ToString();
                            result.type_of_payment = Convert.ToInt32(reader["type_of_payment"].ToString());
                            result.is_paid = reader["is_paid"].ToString() == "False" ? false : true;
                            result.verification_status_id = Convert.ToInt32(reader["verification_status_id"].ToString());
                            result.status_for_intern = reader["label_for_intern"] == DBNull.Value ? null : reader["label_for_intern"].ToString();
                            result.status_for_vendor = reader["label_for_vendor"] == DBNull.Value ? null : reader["label_for_vendor"].ToString();
                            result.verification_note = reader["verification_note"] == DBNull.Value ? null : reader["verification_note"].ToString();
                            result.position_data = Convert.ToInt32(reader["position_data"].ToString());
                            result.created_at = Convert.ToDateTime(reader["created_at"].ToString());
                            result.updated_at = Convert.ToDateTime(reader["updated_at"].ToString());
                            result.received_date = reader["received_date"] == DBNull.Value ? null : Convert.ToDateTime(reader["received_date"].ToString());
                            result.submitted_date = reader["submitted_date"] == DBNull.Value ? null : Convert.ToDateTime(reader["submitted_date"].ToString());
                            result.posting_date = reader["posting_date"] == DBNull.Value ? null : Convert.ToDateTime(reader["posting_date"].ToString());
                            result.remmitance_number = reader["remmitance_number"] == DBNull.Value ? null : reader["remmitance_number"].ToString();
                            result.remmitance_date = reader["remmitance_date"] == DBNull.Value ? null : Convert.ToDateTime(reader["remmitance_date"].ToString());
                            result.request_simulate = reader["request_simulate"].ToString() == "False" ? false : true;
                            result.is_finish = reader["is_finish"].ToString() == "False" ? false : true;
                            result.tax_amt_fc = Convert.ToDouble(reader["tax_amt_fc"].ToString());
                            result.status_rpa = reader["status_rpa"] == DBNull.Value ? null : reader["status_rpa"].ToString();
                            result.rpa_status_description = reader["rpa_status_description"] == DBNull.Value ? null : reader["rpa_status_description"].ToString();
                            result.log_ticket = this.GetLogTicket(reader["ticket_number"].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("GetTicketDetail", ex.Message);
                Console.WriteLine(ex.Message);
            }
            return result;
        }
        public IEnumerable<TicketModel> GetTicketByPositionData(int position_data) {
            List<TicketModel> result = new List<TicketModel>();

            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "SELECT " + 
                              "A.ticket_number, A.vendor_number, A.miro_number, B.name, B.email, " +
                              "A.invoice_number, A.invoice_amount, A.invoice_currency, " +
                              "A.verification_status_id, C.label_for_intern AS verification_name, A.created_at, A.received_date " +
                              "FROM tr_ticket A " +
                              "JOIN tr_vendor B ON A.vendor_number = B.vendor_number " +
                              "LEFT JOIN m_verification_status C ON A.verification_status_id = C.verification_status_id " +
                              "WHERE A.position_data = @position_data"
                    ;
                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@position_data", position_data);

                    cs.Open();
                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result.Add(new TicketModel(){
                                ticket_number = reader["ticket_number"].ToString(),
                                vendor_number = reader["vendor_number"].ToString(),
                                miro_number = reader["miro_number"].ToString(),
                                name = reader["name"].ToString(),
                                email_list = reader["email"] == DBNull.Value ? new List<String>() : new Helpers.EmailFunction().GetEmailList(reader["email"].ToString(), ";"),
                                invoice_number = reader["invoice_number"].ToString(),
                                invoice_amount = Convert.ToDouble(reader["invoice_amount"].ToString()),
                                invoice_currency = reader["invoice_currency"].ToString(),
                                verification_status_id = Convert.ToInt32(reader["verification_status_id"].ToString()),
                                verification_name = reader["verification_name"].ToString(),
                                created_at = Convert.ToDateTime(reader["created_at"].ToString()),
                                received_date = reader["received_date"] == DBNull.Value ? null : Convert.ToDateTime(reader["received_date"].ToString())
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("GetTicketByPositionData", ex.Message);
                Console.WriteLine(ex.Message);
            }
            return result;
        }

        public IEnumerable<PoTicketModel> GetPoTicket(string ticket_number)
        {
            List<PoTicketModel> result = new List<PoTicketModel>();

            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "SELECT A.* "+
                              "FROM tr_po_ticket A " +
                            //   "LEFT JOIN m_po_transaction B ON A.po_number = B.po_number AND A.gr_qty = B.qty AND A.gr_amount = B.amount " +
                              "WHERE A.ticket_number = @ticket_number";

                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@ticket_number", ticket_number);

                    cs.Open();
                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result.Add(new PoTicketModel(){
                                ticket_number = reader["ticket_number"].ToString(),
                                po_number = reader["po_number"].ToString(),
                                po_name = reader["po_name"].ToString(),
                                po_item_number = reader["po_item_number"].ToString(),
                                gr_qty = Convert.ToDecimal(reader["gr_qty"].ToString()),
                                qty_billed = Convert.ToDecimal(reader["qty_billed"].ToString()),
                                uom = reader["uom"].ToString(),
                                currency = reader["currency"].ToString(),
                                gr_amount = Convert.ToDouble(reader["gr_amount"].ToString()),
                                amount_invoice = Convert.ToDouble(reader["amount_invoice"].ToString()),
                                is_disabled = reader["is_disabled"].ToString() == "False" ? false : true,
                                vendor_origin = reader["vendor_origin"] == DBNull.Value ? null : reader["vendor_origin"].ToString(),
                                document_no = reader["document_no"] == DBNull.Value ? null : reader["document_no"].ToString(),
                                item_1 = reader["item_1"] == DBNull.Value ? null :  reader["item_1"].ToString(),
                                gl_description = reader["gl_description"] == DBNull.Value ? null :  reader["gl_description"].ToString(),
                                amount_total = Convert.ToDouble(reader["amount_total"].ToString()),
                                created_at = Convert.ToDateTime(reader["created_at"].ToString()),
                                updated_at = Convert.ToDateTime(reader["updated_at"].ToString())
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("GetPoTicket", ex.Message);
                Console.WriteLine(ex.Message);
            }
            return result;
        }

        public IEnumerable<GlTicketModel> GetGlTicket(string ticket_number)
        {
            List<GlTicketModel> result = new List<GlTicketModel>();

            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "SELECT A.* "+
                              "FROM tr_gl_ticket A " +
                              "WHERE A.ticket_number = @ticket_number";

                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@ticket_number", ticket_number);

                    cs.Open();
                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result.Add(new GlTicketModel(){
                                ticket_number = reader["ticket_number"].ToString(),
                                gl_number = reader["gl_number"].ToString(),
                                gl_debit_or_credit = reader["gl_debit_or_credit"].ToString(),
                                gl_amount = Convert.ToDouble(reader["gl_amount"].ToString()),
                                gl_cost_center = Convert.ToDouble(reader["gl_cost_center"].ToString()),
                                gl_ticket_id = Convert.ToInt32(reader["gl_ticket_id"].ToString()),
                                created_at = Convert.ToDateTime(reader["created_at"].ToString()),
                                updated_at = Convert.ToDateTime(reader["updated_at"].ToString())
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("GetGlTicket", ex.Message);
                Console.WriteLine(ex.Message);
            }
            return result;
        }

        public IEnumerable<MaterialTicketModel> GetMaterialTicket(string ticket_number)
        {
            List<MaterialTicketModel> result = new List<MaterialTicketModel>();

            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "SELECT A.* "+
                              "FROM tr_material_ticket A " +
                              "WHERE A.ticket_number = @ticket_number";

                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@ticket_number", ticket_number);

                    cs.Open();
                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result.Add(new MaterialTicketModel(){
                                ticket_number = reader["ticket_number"].ToString(),
                                material_id = reader["material_id"].ToString(),
                                material_plnt = reader["material_plnt"].ToString(),
                                material_amt = Convert.ToDouble(reader["material_amt"].ToString()),
                                material_qty = Convert.ToInt32(reader["material_qty"].ToString()),
                                material_d_or_c = reader["material_d_or_c"].ToString(),
                                material_total = Convert.ToDouble(reader["material_total"].ToString()),
                                created_at = Convert.ToDateTime(reader["created_at"].ToString()),
                                updated_at = Convert.ToDateTime(reader["updated_at"].ToString())
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("GetMaterialTicket", ex.Message);
                Console.WriteLine(ex.Message);
            }
            return result;
        }

        public IEnumerable<dynamic> RpaGetTickets(int ticketCase){
          IEnumerable<dynamic> result = new dynamic[]{};

            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var q = cs.FluentQueryBuilder()
                    .Select($"t.*")
                    .Select($"cast(format(t.ppn, 'N0', 'de-de') as varchar(20)) AS ppn_case, cast(format(ABS(t.tax_amt_fc), 'N0', 'de-de') as varchar(20)) AS tax_amt_case, cast(format(t.invoice_amount, 'N0', 'de-de') as varchar(20))  AS invoice_amount_case, cast(format(ABS(t.pph), 'N0', 'de-de') as varchar(20)) AS pph_case")
                    .Select($"mt.material_id, mt.material_plnt, cast(format(mt.material_amt, 'N0', 'de-de') as varchar(20)) AS material_amt, mt.material_qty, mt.material_d_or_c")
                    .Select($"gm.gl_number, gm.gl_debit_or_credit, cast(format(gm.gl_amount, 'N0', 'de-de') as varchar(20)) AS gl_amount, gm.gl_cost_center")
                    .Select($"pt.item_id, pt.qty_billed, cast(format(pt.amount_invoice, 'N0', 'de-de') as varchar(20)) AS amount_invoice, cast(format(pt.amount_total, 'N0', 'de-de') as varchar(20)) AS amount_total, pt.gl_description, pt.po_number, pt.po_item_number")
                    .Select($"v.tax_type_id")
                    .Select($"ptg.po_type_group_id")
                    .Select($"tt.wht_code")
                    .Select($"ca.buss_area_code")
                    .Select($"sgc.name AS sgc_name")
                    .From($"tr_ticket t")
                    .From($"LEFT JOIN tr_material_ticket mt ON t.ticket_number = mt.ticket_number")
                     .From($"LEFT JOIN m_company_area ca ON t.area_code = ca.code")
                    .From($"LEFT JOIN tr_gl_ticket gm ON t.ticket_number = gm.ticket_number")
                    .From($"LEFT JOIN tr_vendor v ON t.vendor_number = v.vendor_number")
                    .From($"LEFT JOIN m_po_type ptg ON ptg.po_type_id = t.po_type_id")
                    .From($"LEFT JOIN m_tax_type tt ON v.tax_type_id = v.tax_type_id")
                    .From($"LEFT JOIN m_sg_categories sgc ON sgc.sg_category_id  = v.sg_category_id")
                    .From($"LEFT JOIN tr_po_ticket pt ON t.ticket_number = pt.ticket_number");
                  
                    switch (ticketCase) {
                        case 1:
                            q.Where($"position_data = 9 AND verification_status_id = 12");
                            break;
                        case 2:
                            q.Where($"request_simulate = 1");
                            break;   
                        case 3:
                            q.Where($"position_data = 9 AND verification_status_id = 15");
                            break;   
                    }

                  result = q.Query<dynamic>().GroupBy(e => e.ticket_number).Select(i => {
                      
                      DateTime dt = DateTime.Parse("2021-09-02T05:16:06.3");
                        var item_list = i.GroupBy(ml => new {ml.ticket_number, ml.item_id}).Select(t => new {
                        item_id = t.Select(m => m.item_id).FirstOrDefault(),
                        is_check = t.Select(m => m.qty_billed).FirstOrDefault() > 0 ? true : false,
                        qty_billed = t.Select(m => m.qty_billed).FirstOrDefault(),
                        amount_invoice = t.Select(m => m.amount_invoice).FirstOrDefault(),
                        amount_total = t.Select(m => m.amount_total).FirstOrDefault(),
                        gl_description = t.Select(m => m.gl_description).FirstOrDefault(),
                        po_number = t.Select(m => m.po_number).FirstOrDefault(),
                        po_item_number = t.Select(m => m.po_item_number).FirstOrDefault()

                        });
                        int idx = 0;
                        var item_list_new = new List<dynamic>();  
                        foreach (var it in item_list){
                            item_list_new.Add(new {
                                item_id =  idx,
                                is_check = it.is_check,
                                qty_billed = it.qty_billed,
                                amount_invoice = it.amount_invoice,
                                amount_total = it.amount_total,
                                gl_description = it.gl_description,
                                po_number = it.po_number,
                                po_item_number = it.po_item_number
                            });
                            idx ++;
                        }
                      return new {
                      ticket_number = i.Key,
                      company_id = i.Select(t => t.company_id).FirstOrDefault(),
                      vendor_number = i.Select(t => t.vendor_number).FirstOrDefault(),
                      tax_type_id = i.Select(t => t.tax_type_id).FirstOrDefault(),
                      tax_invoice_number = i.Select(t => t.tax_invoice_number == null ? null : new Helpers.GlobalFunction().FormatTaxInvoiceNumber(t.tax_invoice_number, true)).FirstOrDefault(),
                      service_status = i.Select(t => t.sgc_name).FirstOrDefault(),
                      invoice_wt_tax_amt_base_fc = i.Select(t => t.pph_case).FirstOrDefault(),
                      invoice_business_area = i.Select(t => t.buss_area_code).FirstOrDefault(),
                      invoice_number = i.Select(t => t.invoice_number).FirstOrDefault(),
                      invoice_po_type = i.Select(t => t.po_type_id).FirstOrDefault(),
                      invoice_po_type_group = i.Select(t => t.po_type_group_id).FirstOrDefault(),
                      invoice_po_number = i.Select(t => t.po_number).FirstOrDefault(),
                      invoice_amount = i.Select(t => t.invoice_amount_case).FirstOrDefault(),
                      invoice_currency = i.Select(t => t.invoice_currency).FirstOrDefault(),
                      invoice_wt_tax_amt_fc = i.Select(t => t.tax_amt_case).FirstOrDefault(),

                      invoice_date = String.Format("{0:dd.MM.yyyy}", i.Select(t => t.created_at).FirstOrDefault()),
                    //   material_id, t.material_plnt, t.material_amt, t.material_qty, t.material_d_or_c
                    // i.GroupBy(ml => ml.ticket_number).Select(t => new {ticket_number = t.Key, material_id = t.Select(m => m.material_id).FirstOrDefault()}).ToArray()
                      material_list = i.GroupBy(ml => new {ml.ticket_number, ml.material_id}).Select(t => {

                      return new {
                            material_id = t.Select(m => m.material_id).FirstOrDefault(),
                            material_plnt = t.Select(m => m.material_plnt).FirstOrDefault(),
                            material_amt = t.Select(m => m.material_amt).FirstOrDefault(),
                            material_qty = t.Select(m => m.material_qty).FirstOrDefault(),
                            material_d_or_c = t.Select(m => m.material_d_or_c).FirstOrDefault()
                      };

                      }).ToArray(),
                      item_list = item_list_new.ToArray(),
                      gl_list = i.GroupBy(ml => new {ml.ticket_number, ml.gl_number}).Select(t => new {
                            // ticket_number = t.Key,
                            gl_account = t.Select(m => m.gl_number).FirstOrDefault(),
                            gl_debit_or_credit = t.Select(m => m.gl_debit_or_credit).FirstOrDefault(),
                            gl_amount = t.Select(m => m.gl_amount).FirstOrDefault(),
                            gl_cost_center = t.Select(m => m.gl_cost_center).FirstOrDefault(),
                          }).ToArray(),
                    //   gl_list = i.Select(t => new {t.gl_number, t.gl_debit_or_credit, t.gl_amount, t.gl_cost_center}).ToArray()
                    //   gl_list = i.Select(t => new {t.gl_number}).GroupBy(e => e.gl_number).Select(i => new {
                    //       gl_number = i.Key
                    //   })
                      };
                  });

                  // var categories = new string[] { "Components", "Clothing", "Acessories" };

                  // var q = cn.FluentQueryBuilder()
                  //     .SelectDistinct($"c.Name as Category, sc.Name as Subcategory, p.Name, p.ProductNumber")
                  //     .From($"Product p")
                  //     .From($"INNER JOIN ProductSubcategory sc ON p.ProductSubcategoryID=sc.ProductSubcategoryID")
                  //     .From($"INNER JOIN ProductCategory c ON sc.ProductCategoryID=c.ProductCategoryID")
                  //     .Where($"c.Name IN {categories}");
                  
                  // var builder = cs.QueryBuilder($@"SELECT * FROM tr_vendor WHERE vendor_number = {vendor_number}");
                  // result = builder.Query<VendorModel>();
             
                  // var builder = new SqlBuilder();
                  // builder.Select("*");
                  // var builderTemplate = builder.AddTemplate("Select /**select**/ from tr_vendor");
                  // result = cs.Query(builderTemplate.RawSql);
                  // https://github.com/Drizin/DapperQueryBuilder

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("RpaGetTickets", ex.Message);
            }
            return result;
        }


        public IEnumerable<dynamic> RpaGetRejectedTickets(){
            IEnumerable<dynamic> result = new dynamic[]{};

            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var q = cs.FluentQueryBuilder()
                    .Select($"t.ticket_number, t.miro_number, t.submitted_date as posting_date, YEAR(t.submitted_date) AS fiscal_year, t.verification_note as reject_code")
                    .From($"tr_ticket t")
                    .Where($"position_data = 9 AND verification_status_id = 15");
                  


                    result = q.Query<dynamic>().GroupBy(e => e.ticket_number).Select(i => new {
                        ticket_number = i.Key,
                        miro_number = i.Select(t => t.miro_number).FirstOrDefault(),
                        posting_date = i.Select(t => t.posting_date).FirstOrDefault() == null ? null : String.Format("{0:dd.MM.yyyy}", i.Select(t => t.posting_date).FirstOrDefault()),
                        fiscal_year = i.Select(t => t.fiscal_year).FirstOrDefault(),
                        reject_code = i.Select(t => t.reject_code).FirstOrDefault(),

                  });

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("RpaGetRejectedTickets", ex.Message);
            }
            return result;
        }

    
        public async Task<int> RpaPaymentRemittance(List<RemittanceXlsRequest> remittanceXlsRequests){
            int result = 0;

            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                  cs.Open();
                  SqlTransaction trans = cs.BeginTransaction();
                  result = await cs.ExecuteAsync(@"UPDATE tr_ticket SET is_paid = 1, verification_status_id = 13, is_finish= 1, remmitance_date = @clearing_date, remmitance_number = @remmitance_number WHERE miro_number = @miro_number AND vendor_number = @vendor_number", remittanceXlsRequests, transaction: trans);

                  trans.Commit();

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("RpaPaymentRemittance", ex.Message);
            }
            return result;
            // throw new NotImplementedException();
        }
        
        public async Task<int> RpaTicketReversal(string miro_number, TicketReversalRequest requestBody){
 
            int result = 0;

            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                  cs.Open();
                  SqlTransaction trans = cs.BeginTransaction();
                  if (requestBody.status_rpa == "0") {
                    result = await cs.ExecuteAsync(@"UPDATE tr_ticket SET status_rpa = 0, rpa_status_description = @rpa_status_description WHERE miro_number = @miro_number", new{miro_number = miro_number,rpa_status_description = requestBody.rpa_status_description}, transaction: trans);
                  }else {
                    //   unlock is_used lpb_number
                    var ticketQuerySelect = cs.FluentQueryBuilder()
                                                .Select($"t.ticket_number, t.po_number, t.gr_number, t.po_type_id, t.company_id")
                                                .Select($"pt.item_id, pt.po_item_number, pt.vendor_origin, pt.document_no, pt.item_1")
                                                .From($"tr_ticket t")
                                                .From($"LEFT JOIN tr_po_ticket pt ON t.ticket_number = pt.ticket_number AND pt.qty_billed > 0")
                                                .Where($"miro_number = {miro_number}");
            //                                               var vendor_origin = model.vendor_origin;
            // var document_no = model.document_no;
            // var item_1 = model.item_1;
            // var po_item_number = model.po_item_number;
                    var ticketResult = ticketQuerySelect.Query<dynamic>(transaction: trans).GroupBy(e => e.ticket_number).Select( i => {
                        // document_no = i.GroupBy(d => new {d.ticket_number})
                        var document_no = new List<dynamic>();  
                        var item_1 = new List<dynamic>(); 
                        var po_item_number = new List<dynamic>();  
                        var vendor_origin = new List<dynamic>();
                        var po_list = i.GroupBy(ml => new {ml.ticket_number, ml.item_id})
                            .Select(t => new {
                            po_item_number = t.Select(m => m.po_item_number).FirstOrDefault(),
                            vendor_origin = t.Select(m => m.vendor_origin).FirstOrDefault(),
                            document_no = t.Select(m => m.document_no).FirstOrDefault(),
                            item_1 = t.Select(m => m.item_1).FirstOrDefault(),
                        }); 
                        foreach (var it in po_list){
                            po_item_number.Add(it.po_item_number);
                            vendor_origin.Add(it.vendor_origin);
                            document_no.Add(it.document_no);
                            item_1.Add(it.item_1);
                        }
                        return new {
                            ticket_number = i.Key,
                            po_number = i.Select(t => t.po_number).FirstOrDefault(),
                            gr_number = i.Select(t => t.gr_number).FirstOrDefault(),
                            po_type_id = i.Select(t => t.po_type_id).FirstOrDefault(),
                            company_id = i.Select(t => t.company_id).FirstOrDefault(),
                            document_no = document_no,
                            item_1 = item_1,
                            po_item_number = po_item_number,
                            vendor_origin = vendor_origin, 
                            qty_billed = new List<dynamic>(),
                            gr_amount = new List<dynamic>()

                        };
                    });
                    Dictionary<string, dynamic> condition = new Dictionary<string, dynamic>();
                    condition.Add("is_used", 0);
                    var updatePoMasterUsedSql = this.updateIsUsedPoTransactionQuery(ticketResult.First(), condition);
                    await cs.ExecuteAsync(@updatePoMasterUsedSql, transaction: trans);
                    result = await cs.ExecuteAsync(@"UPDATE tr_ticket SET verification_status_id = 18, position_data =2, is_finish =1, status_rpa = 1, rpa_status_description = null WHERE miro_number = @miro_number", new{miro_number = miro_number}, transaction: trans);
                  }

                  trans.Commit();

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("RpaTicketReversal", ex.Message);
            }
            return result;
        }
        
        public async Task<int> RpaUpdateMiroNumber(string ticket_number, UpdateMIRONumberPayloadBody payload){
 
            int result = 0;

            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                  cs.Open();
                  var position_data = 7;
                  string miro_number =  payload.miro_number;
                  string rpa_status_description = null;
                  string status_rpa = payload.status_rpa;
                  if (status_rpa == "0") {
                      position_data = 6;
                      rpa_status_description = payload.rpa_status_description;
                      miro_number = null;

                  }
                  SqlTransaction trans = cs.BeginTransaction();
                  result = await cs.ExecuteAsync($@"UPDATE tr_ticket SET 
                  status_rpa = @status_rpa, position_data = @position_data, miro_number = @miro_number, submitted_date = @date_miro_input, rpa_status_description = @rpa_status_description WHERE ticket_number = @ticket_number", new {
                      ticket_number = ticket_number, 
                      date_miro_input = payload.date_miro_input, 
                      miro_number = miro_number, 
                      position_data = position_data, 
                      status_rpa = status_rpa,
                      rpa_status_description= rpa_status_description  }, transaction: trans);

                  trans.Commit();

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("RpaUpdateMiroNumber", ex.Message);
            }
            return result;
        }


        public async Task<int> RpaSimulateResult(List<SimulateXlsRequest> SimulateXlsRequestList, string status_rpa){
            int resultUpdate = 0;
            int resultInsert = 0;
            int resultDelete = 0;

            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    cs.Open();
                    SqlTransaction trans = cs.BeginTransaction();
                    resultUpdate = await cs.ExecuteAsync(@"UPDATE tr_ticket SET request_simulate = @request_simulate, verification_status_id = @verification_status_id, rpa_status_description = @rpa_status_description, status_rpa =  @status_rpa WHERE ticket_number = @ticket_number", SimulateXlsRequestList, transaction: trans);
                    if (status_rpa != "0") {
                    resultDelete = await cs.ExecuteAsync($@"DELETE FROM tr_simulate where ticket_number = @ticket_number",  SimulateXlsRequestList, transaction: trans);
                    resultInsert = await cs.ExecuteAsync($@"INSERT INTO tr_simulate (
                            ticket_number,
                            position,
                            account_type,
                            g_l,
                            act_mat_ast_vndr,
                            amount,
                            currency,
                            po_number,
                            po_item_number,
                            tax_code,
                            jurisd_code,
                            tax_date,
                            bus_area,
                            cost_center,
                            co_area,
                            order_1,
                            asset,
                            sub_number,
                            asset_val,
                            material,
                            quantity,
                            base_unit,
                            plant,
                            part_bus_ar,
                            cost_object,
                            profit_segment,
                            profit_center,
                            partner_prctr,
                            wbs_element,
                            network,
                            sales_order,
                            sls_item,
                            vendor_number,
                            posting_key,
                            posting_line_id,
                            created_auto
                    ) VALUES (
                            @ticket_number,
                            @position,
                            @account_type,
                            @g_l,
                            @act_mat_ast_vndr,
                            @amount,
                            @currency,
                            @po_number,
                            @po_item_number,
                            @tax_code,
                            @jurisd_code,
                            @tax_date,
                            @bus_area,
                            @cost_center,
                            @co_area,
                            @order_1,
                            @asset,
                            @sub_number,
                            @asset_val,
                            @material,
                            @quantity,
                            @base_unit,
                            @plant,
                            @part_bus_ar,
                            @cost_object,
                            @profit_segment,
                            @profit_center,
                            @partner_prctr,
                            @wbs_element,
                            @network,
                            @sales_order,
                            @sls_item,
                            @vendor_number,
                            @posting_key,
                            @posting_line_id,
                            @created_auto
                    )", SimulateXlsRequestList, transaction: trans);
                    }

                  trans.Commit();

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("RpaSimulateResult", ex.Message);
            }
            return resultInsert;
            // throw new NotImplementedException();
        } 

          public string GetEmailCompanyArea(string code)
         {
            var result = "";
            try
            {
                using (var cs = new ConnectionService(_appSettings).GetConnection())
                {
                    var sql = "SELECT email from m_company_area WHERE code = @code";

                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@code", code);
                    cs.Open();
                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result = reader["email"].ToString();
                        }
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("GetEmailCompanyArea", ex.Message);
            }
            return result;
        }
    
    }
}