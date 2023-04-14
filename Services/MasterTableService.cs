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
    public class MasterTableService : IMasterTable
    {
        private readonly Helpers.AppSettings _appSettings;
        public MasterTableService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
            //for date date.ToString("dd MMMM yyyy", new System.Globalization.CultureInfo("id-ID"));
        }


        public async Task<int> InsertMasterLPB(List<TempPoLpbModel> model){
          int result = 0;

            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                  cs.Open();
                  SqlTransaction trans = cs.BeginTransaction();
                  result = await cs.ExecuteAsync(@"INSERT INTO 
                  temp_po_lpb(po_number, vendor_number, po_item_number, po_name, currency, po_quantity, uom, lpb_number, lpb_item_number, quantity_lpb, company_id, document_no, item_1, posting_date, doc_header_text, clearing_date, clearing_doc, pk, acc_ty, sg, d_c, bus_a, gl, gl_description, amount_in_dc, amount_in_lc, material_code, order_1, sales_doc, item_2,asset, s_no, order_2, material_number, desc_vendor_or_credit, division, period, lpb_clearing, lpb_clear_date, voucher, cost_ctr, account_type, type, op_number, alt_bom, rcp_group, tr_ty, tr_s, count, doc_currency, mov_type, ref_doc_migo, is_active) 
                  VALUES(@po_number, @vendor_number, @po_item_number, @po_name, @currency, @po_quantity, @uom, @lpb_number, @lpb_item_number, @quantity_lpb, @company_id, @document_no, @item_1, @posting_date, @doc_header_text, @clearing_date, @clearing_doc, @pk, @acc_ty, @sg, @d_c, @bus_a, @gl, @gl_description, @amount_in_dc, @amount_in_lc, @material_code, @order_1, @sales_doc, @item_2, @asset, @s_no, @order_2, @material_number, @desc_vendor_or_credit, @division, @period, @lpb_clearing, @lpb_clear_date, @voucher, @cost_ctr, @account_type, @type, @op_number, @alt_bom, @rcp_group, @tr_ty, @tr_s, @count, @doc_currency, @mov_type, @ref_doc_migo, @is_active)", model, transaction: trans);

                  trans.Commit();

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("InsertMasterLPB", ex.Message);
            }
            return result;

        }
        public List<PoTransactionModel> GetTempPoTransaction(string company_id)
        {
            List<PoTransactionModel> result = new List<PoTransactionModel>();
            try
            {
                using (var cs = new ConnectionService(_appSettings).GetConnection())
                {
                    var sql = "SELECT po_number,po_item_number,company_id,vendor_number,po_name,currency,uom,SUM(case when amount_in_lc > 0 then amount_in_lc else null end) as amount,SUM(case when po_quantity > 0 then po_quantity else null end) as qty, bus_a "+
                                "FROM temp_po_lpb where company_id = "+company_id +" "+
                                "GROUP BY po_number, po_item_number,company_id, vendor_number,po_name,currency,uom, bus_a";

                    SqlCommand command = new SqlCommand(sql, cs);
                    cs.Open();
                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                             result.Add(new PoTransactionModel()
                             {
                               po_number = reader["po_number"] == DBNull.Value ? null : reader["po_number"].ToString(),
                               po_item_number = reader["po_item_number"] == DBNull.Value ? null : reader["po_item_number"].ToString(),
                               company_id = reader["company_id"] == DBNull.Value ? null : reader["company_id"].ToString(),
                               vendor_number = reader["vendor_number"] == DBNull.Value ? null : reader["vendor_number"].ToString(),
                               po_name =reader["po_name"] == DBNull.Value ? null : reader["po_name"].ToString(),
                               currency = reader["currency"] == DBNull.Value ? null : reader["currency"].ToString(),
                               plnt_area = reader["bus_a"] == DBNull.Value ? null : reader["bus_a"].ToString(),
                               uom = reader["uom"] == DBNull.Value ? null : reader["uom"].ToString(),
                               amount = reader["amount"] == DBNull.Value ? 0 : Convert.ToDouble(reader["amount"]),
                               qty = reader["qty"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["qty"]),
                             });
                        }
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("erorrr: "+ex);
                new Helpers.GlobalFunction().LogError("GetTempPoTransaction", ex.Message);
            }
            Console.WriteLine("json"+ JsonSerializer.Serialize(result));
            //Console.WriteLine("ress :"+ result);
            return result;
        }

        public async Task<int> InsertMasterPoTransaction(List<PoTransactionModel> model)
        {
            int result = 0;

            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                  cs.Open();
                  SqlTransaction trans = cs.BeginTransaction();
                  result = await cs.ExecuteAsync(@"INSERT INTO
                            m_po_transaction(po_number,po_item_number,company_id,vendor_number,po_name,currency,uom,amount,qty,is_service, plnt_area)
                            VALUES(@po_number, @po_item_number,@company_id, @vendor_number, @po_name, @currency, @uom, @amount, @qty, 0, @plnt_area)", model, transaction: trans);

                  trans.Commit();

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("InsertMasterPoTransaction", ex.Message);
            }
            return result;
        }

        public List<LpbTransactionModel> GetTempLpbTransaction(string company_id)
        {
            List<LpbTransactionModel> result = new List<LpbTransactionModel>();
            try
            {
                using (var cs = new ConnectionService(_appSettings).GetConnection())
                {
                    var sql = "SELECT lpb_number, lpb_item_number, po_number, po_item_number, vendor_number, uom, currency, company_id, SUM(case when amount_in_dc > 0 then amount_in_dc else null end) as amount, SUM(case when quantity_lpb > 0 then quantity_lpb else null end) as qty "+ 
                                "FROM temp_po_lpb where company_id = "+company_id +" GROUP BY po_number, po_item_number, lpb_number, lpb_item_number, vendor_number, uom, currency, company_id";

                    SqlCommand command = new SqlCommand(sql, cs);
                    cs.Open();
                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                             result.Add(new LpbTransactionModel()
                             {
                                lpb_number = reader["lpb_number"] == DBNull.Value ? null : reader["lpb_number"].ToString(),
                               lpb_item_number = reader["lpb_item_number"] == DBNull.Value ? null : reader["lpb_item_number"].ToString(),
                               po_number = reader["po_number"] == DBNull.Value ? null : reader["po_number"].ToString(),
                               po_item_number = reader["po_item_number"] == DBNull.Value ? null : reader["po_item_number"].ToString(),
                               vendor_number = reader["vendor_number"] == DBNull.Value ? null : reader["vendor_number"].ToString(),
                               currency = reader["currency"] == DBNull.Value ? null : reader["currency"].ToString(),
                               uom = reader["uom"] == DBNull.Value ? null : reader["uom"].ToString(),
                               amount = reader["amount"] == DBNull.Value ? 0 : Convert.ToDouble(reader["amount"]),
                               qty = reader["qty"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["qty"]),
                               company_id = reader["company_id"] == DBNull.Value ? null : reader["company_id"].ToString(),
                             });
                        }
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("erorrr: "+ex);
                new Helpers.GlobalFunction().LogError("GetTempLpbTransaction", ex.Message);
            }
            //Console.WriteLine("json"+ JsonSerializer.Serialize(result));
            return result;
        }

        public async Task<int> InsertMasterLpbTransaction(List<LpbTransactionModel> model)
        {
            int result = 0;

            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                  cs.Open();
                  SqlTransaction trans = cs.BeginTransaction();
                  result = await cs.ExecuteAsync(@"INSERT INTO
                            m_lpb_transaction(lpb_number,lpb_item_number,po_number,po_item_number,vendor_number,currency,uom,amount,qty,company_id, is_active)
                            VALUES(@lpb_number,@lpb_item_number,@po_number, @po_item_number, @vendor_number, @currency, @uom, @amount, @qty, @company_id, @is_active)", model, transaction: trans);

                  trans.Commit();

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("InsertMasterLpbTransaction", ex.Message);
            }
            return result;
        }

        public async Task<int> InsertMasterService(List<TempPoServiceModel> model)
        {
            int result = 0;

            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                  cs.Open();
                  SqlTransaction trans = cs.BeginTransaction();
                  result = await cs.ExecuteAsync(@"INSERT INTO 
                  temp_po_svc(po_item_number, type, cat, pgr, poh, doc_date, material, po_name, material_group, d, i, a, plnt, s_loc, po_quantity, uom, company_id, quantity_2, sku, net_price, currency, per, quantity_3, open_tgt_qty, quantity_to_be_del, amount_to_be_del, quantity_to_be_inv, amount_to_be_inv, number, po_number, vendor_number, p_org, l, tracking_number, agmt, item_2, targ_val, tot_open_val, open_value, rel_value,rel_qty,vp_start,vper_end,quot_dd_in,s,coll_number,ctl,info_rec,number_2,grp,strat,release,rel,ist_loc,vendor_name_2,opu,tx,tax_jur,net_value,i_2,notified,stk_seg,req_seg,s_2,conf_item_number,sort_number,ext_h_cat,ru,reqmt_price,smart_no,char1,char_desc_1,char2,char_desc_2,char3,char_desc_3) 
                  VALUES(@po_item_number, @type, @cat, @pgr, @poh, @doc_date, @material, @po_name, @material_group, @d, @i, @a, @plnt, @s_loc, @po_quantity, @uom, @company_id, @quantity_2, @sku, @net_price, @currency, @per, @quantity_3, @open_tgt_qty, @quantity_to_be_del, @amount_to_be_del, @quantity_to_be_inv, @amount_to_be_inv, @number, @po_number, @vendor_number, @p_org, @l, @tracking_number, @agmt, @item_2, @targ_val, @tot_open_val, @open_value, @rel_value, @rel_qty, @vp_start, @vper_end, @quot_dd_in, @s, @coll_number, @ctl, @info_rec, @number_2, @grp, @strat, @release, @rel, @ist_loc, @vendor_name_2, @opu, @tx, @tax_jur, @net_value, @i_2, @notified, @stk_seg, @req_seg, @s_2, @conf_item_number, @sort_number, @ext_h_cat, @ru, @reqmt_price, @smart_no, @char1, @char_desc_1, @char2, @char_desc_2, @char3, @char_desc_3)", model, transaction: trans);
                  trans.Commit();

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("InsertMasterService", ex.Message);
            }
            Console.WriteLine("res"+result);
            return result;

        }

        public async Task<int> InsertMasterPoOpen(List<PoOpenModel> model)
        {
            int result = 0;

            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                  cs.Open();
                  SqlTransaction trans = cs.BeginTransaction();
                  result = await cs.ExecuteAsync(@"INSERT INTO 
                  m_po_open(po_item_number, type, cat, pgr, poh, doc_date, material, po_name, material_group, d, i, a, plnt, s_loc, po_quantity, uom, quantity_2, sku, net_price, currency, per, quantity_3, open_tgt_qty, quantity_to_be_del, amount_to_be_del, quantity_to_be_inv, amount_to_be_inv, number, po_number, vendor_number, p_org, l, tracking_number, agmt, item_2, targ_val, tot_open_val, open_value, rel_value,rel_qty,vp_start,vper_end,quot_dd_in,s,coll_number,ctl,info_rec,number_2,grp,strat,release,rel,ist_loc,vendor_name_2,opu,tx,tax_jur,net_value,i_2,notified,stk_seg,req_seg,s_2,conf_item_number,sort_number,ext_h_cat,ru,reqmt_price,smart_no,char1,char_desc_1,char2,char_desc_2,char3,char_desc_3) 
                  VALUES(@po_item_number, @type, @cat, @pgr, @poh, @doc_date, @material, @po_name, @material_group, @d, @i, @a, @plnt, @s_loc, @po_quantity, @uom, @quantity_2, @sku, @net_price, @currency, @per, @quantity_3, @open_tgt_qty, @quantity_to_be_del, @amount_to_be_del, @quantity_to_be_inv, @amount_to_be_inv, @number, @po_number, @vendor_number, @p_org, @l, @tracking_number, @agmt, @item_2, @targ_val, @tot_open_val, @open_value, @rel_value, @rel_qty, @vp_start, @vper_end, @quot_dd_in, @s, @coll_number, @ctl, @info_rec, @number_2, @grp, @strat, @release, @rel, @ist_loc, @vendor_name_2, @opu, @tx, @tax_jur, @net_value, @i_2, @notified, @stk_seg, @req_seg, @s_2, @conf_item_number, @sort_number, @ext_h_cat, @ru, @reqmt_price, @smart_no, @char1, @char_desc_1, @char2, @char_desc_2, @char3, @char_desc_3)", model, transaction: trans);
                  trans.Commit();

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("InsertMasterPoOpen", ex.Message);
            }
            Console.WriteLine("open"+result);
            return result;
        }

        public bool DeleteTempPoLpb(string company_id)
        {
            var result = false;
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "DELETE FROM temp_po_lpb WHERE company_id = @company_id";
                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@company_id", company_id);
                    cs.Open();

                    result = command.ExecuteNonQuery() == 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("DeleteTempPoLpb", ex.Message);
            }
            return result;
        }

        public bool DeletePoTransaction(string company_id)
        {
            var result = false;
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "DELETE FROM m_po_transaction WHERE is_service = 0 AND company_id = @Company_id";
                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@company_id", company_id);
                    cs.Open();

                    result = command.ExecuteNonQuery() == 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("DeletePoTransaction", ex.Message);
            }
            return result;
        }

        public bool DeleteLpbTransaction(string company_id)
        {
            var result = false;
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "DELETE FROM m_lpb_transaction WHERE company_id = @Company_id";
                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@company_id", company_id);
                    cs.Open();

                    result = command.ExecuteNonQuery() == 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("DeleteLpbTransaction", ex.Message);
            }
            return result;
        }

        public bool DeleteTempPoService()
        {
            var result = false;
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "DELETE FROM temp_po_svc";
                    SqlCommand command = new SqlCommand(sql, cs);
                    cs.Open();

                    result = command.ExecuteNonQuery() == 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("DeleteTempPoService", ex.Message);
            }
            return result;
        }

        public bool DeletePoTransactionService(string is_service)
        {
            var result = false;
            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = "DELETE FROM m_po_transaction WHERE is_service = @is_service";
                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@is_service", is_service);
                    cs.Open();

                    result = command.ExecuteNonQuery() == 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("DeletePoTransactionService", ex.Message);
            }
            return result;
        }

        public async Task<int> InsertMasterPoTransactionService(List<PoTransactionModel> model)
        {
            int result = 0;

            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                  cs.Open();
                  SqlTransaction trans = cs.BeginTransaction();
                  result = await cs.ExecuteAsync(@"INSERT INTO
                            m_po_transaction(po_number,po_item_number,vendor_number,po_name,currency,uom, company_id, amount,qty,is_service, plnt_area, is_active)
                            VALUES(@po_number, @po_item_number, @vendor_number, @po_name, @currency, @uom, @company_id, @amount, @qty, @is_service, @plnt_area, @is_active)", model, transaction: trans);

                  trans.Commit();

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("InsertMasterPoTransactionService", ex.Message);
            }
            return result;
        }

        public async Task<List<PoOpenModel>> UpdateMasterPoOpen(List<PoOpenModel> model)
        {
            var result = new List<PoOpenModel>();
            IEnumerable<dynamic> resultSelect = new dynamic[]{};

            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                  cs.Open();
                  SqlTransaction trans = cs.BeginTransaction();

                    foreach(var item in model){
                        var querySelect = cs.FluentQueryBuilder()
                    .Select($"t.*")
                    .From($"m_po_open t")
                    .Where($"po_item_number = {item.po_item_number} AND po_number = {item.po_number}");
                    resultSelect = querySelect.Query<PoOpenModel>(transaction: trans);
                    if(resultSelect.Count() > 0){
                        await cs.ExecuteAsync(@"UPDATE m_po_open set 
                        type = @type, cat = @cat, pgr = @pgr, poh = @poh, doc_date = @doc_date, material = @material,
                        po_name = @po_name, material_group = @material_group, d = @d, i =@i, a = @a, plnt = @plnt, s_loc = @s_loc, po_quantity = @po_quantity,
                        uom = @uom, quantity_2 = @quantity_2, sku = @sku, net_price = @net_price, currency = @currency, per = @per, quantity_3 = @quantity_3,
                        open_tgt_qty = @open_tgt_qty, quantity_to_be_del = @quantity_to_be_del, amount_to_be_del = @amount_to_be_del, quantity_to_be_inv = @quantity_to_be_inv,
                        amount_to_be_inv = @amount_to_be_inv, number = @number, vendor_number = @vendor_number, p_org = @p_org, l = @l,
                        tracking_number = @tracking_number, agmt = @agmt, item_2 = @item_2, targ_val = @targ_val, tot_open_val = @tot_open_val, open_value = @open_value,
                        rel_value = @rel_value, rel_qty = @rel_qty, vp_start = @vp_start, vper_end = @vper_end, quot_dd_in = @quot_dd_in, s = @s, coll_number = @coll_number,
                        ctl = @ctl, info_rec = @info_rec, number_2 = @number_2, grp = @grp, strat = @strat, release = @release, rel = @rel, ist_loc = @ist_loc,
                        vendor_name_2 = @vendor_name_2, opu = @opu, tx = @tx, tax_jur = @tax_jur, net_value = @net_value, i_2 = @i_2, notified = @notified, stk_seg = @stk_seg,
                        req_seg = @req_seg, s_2 = @s_2, conf_item_number = @conf_item_number, sort_number = @sort_number, ext_h_cat = @ext_h_cat, ru = @ru, reqmt_price = @reqmt_price,
                        smart_no = @smart_no, char1 = @char1, char_desc_1 = @char_desc_1, char2 = @char2, char_desc_2 = @char_desc_2, char3 = @char3, char_desc_3 = @char_desc_3
                        WHERE po_item_number = @po_item_number AND po_number = @po_number", item, transaction: trans);

                        } else {
                            result.Add(item);
                        }
                    }
                    trans.Commit();


                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("UpdateMasterPoOpen", ex.Message);
            }
            return result;
        }

        public List<PoTransactionModel> GetTempPoTransactionService()
        {
             List<PoTransactionModel> result = new List<PoTransactionModel>();
            try
            {
                using (var cs = new ConnectionService(_appSettings).GetConnection())
                {
                    var sql = "SELECT po_number, po_item_number, vendor_number, po_name, currency, uom, company_id, net_price as amount, po_quantity as qty, plnt "+
                                "FROM temp_po_svc ";
                                //  +"GROUP BY po_number, po_item_number, vendor_number,po_name,currency,uom, company_id, plnt";

                    SqlCommand command = new SqlCommand(sql, cs);
                    cs.Open();
                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                             result.Add(new PoTransactionModel()
                             {
                               po_number = reader["po_number"] == DBNull.Value ? null : reader["po_number"].ToString(),
                               po_item_number = reader["po_item_number"] == DBNull.Value ? null : reader["po_item_number"].ToString(),
                               vendor_number = reader["vendor_number"] == DBNull.Value ? null : reader["vendor_number"].ToString(),
                               po_name =reader["po_name"] == DBNull.Value ? null : reader["po_name"].ToString(),
                               currency = reader["currency"] == DBNull.Value ? null : reader["currency"].ToString(),
                                plnt_area = reader["plnt"] == DBNull.Value ? null : reader["plnt"].ToString(),
                               uom = reader["uom"] == DBNull.Value ? null : reader["uom"].ToString(),
                               amount = reader["amount"] == DBNull.Value ? 0 : Convert.ToDouble(reader["amount"]),
                               qty = reader["qty"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["qty"]),
                               company_id = reader["company_id"] == DBNull.Value ? null : reader["company_id"].ToString(),
                               is_service = true,
                             });
                        }
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("erorrr: "+ex);
                new Helpers.GlobalFunction().LogError("GetTempPoTransactionService", ex.Message);
            }
            //Console.WriteLine("ress :"+ result);
            return result;
        }

        public async Task<List<VendorModelRpa>> UpdateMasterVendor(List<VendorModelRpa> model)
        {
            var result = new List<VendorModelRpa>();
            IEnumerable<dynamic> resultSelect = new dynamic[]{};

            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                  cs.Open();
                  SqlTransaction trans = cs.BeginTransaction();
                    foreach(var item in model){
                        var querySelect = cs.FluentQueryBuilder()
                        .Select($"t.*")
                        .From($"tr_vendor t")
                        .Where($"vendor_number = {item.vendor_number}");
                        resultSelect = querySelect.Query<VendorModelRpa>(transaction: trans);
                        if(resultSelect.Count() > 0){
                            await cs.ExecuteAsync(@"UPDATE tr_vendor set 
                            vendor_number = @vendor_number, contact_person = @contact_person, search_term = @search_term,
                            currency_id = @currency_id, top_id = @top_id, name = @name,
                            street_address = @street_address, additional_street_address = @additional_street_address,
                            city = @city, country_id= @country_id, telephone= @telephone, email = @email,
                            tax_type_id = @tax_type_id, tax_number_type_id = @tax_number_type_id, npwp = @npwp,
                            id_card_number = @id_card_number, postal_code = @postal_code, vendor_type_id = @vendor_type_id, title_id = @title_id,
                            is_locked = @is_locked, change_request_status = @change_request_status, localidr_bank_id = @localidr_bank_id, localidr_bank_account = @localidr_bank_account,
                            localidr_account_holder = @localidr_account_holder, foreign_bank_id = @foreign_bank_id,
                            foreign_bank_account = @foreign_bank_account, foreign_account_holder = @foreign_account_holder, change_request_note=@change_request_note              
                            WHERE vendor_number = @vendor_number", item, transaction: trans);

                        } else {
                            result.Add(item);
                        }
                    }
                    trans.Commit();


                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("erorr update master po: "+ex);
                new Helpers.GlobalFunction().LogError("UpdateMasterVendor", ex.Message);
            }
            return result;
        }

        
        public async Task<int> InsertMasterVendor(List<VendorModelRpa> model)
        {
            int result = 0;

            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {

                  cs.Open();
                  SqlTransaction trans = cs.BeginTransaction(); 
                  result = await cs.ExecuteAsync(@"INSERT INTO 
                  tr_vendor(vendor_number, contact_person, search_term, currency_id, top_id, name, street_address, additional_street_address, city, country_id, telephone, email, tax_type_id, tax_number_type_id, npwp, id_card_number, postal_code, vendor_type_id, title_id, is_locked, change_request_status, localidr_bank_id, localidr_bank_account, localidr_account_holder, foreign_bank_id, foreign_bank_account, foreign_account_holder, change_request_note ) 
                  VALUES(@vendor_number, @contact_person, @search_term, @currency_id, @top_id, @name, @street_address, @additional_street_address, @city, @country_id, @telephone, @email, @tax_type_id, @tax_number_type_id, @npwp, @id_card_number, @postal_code, @vendor_type_id, @title_id, @is_locked, @change_request_status, @localidr_bank_id, @localidr_bank_account, @localidr_account_holder, @foreign_bank_id, @foreign_bank_account, @foreign_account_holder, @change_request_note)", model, transaction: trans);
    
                  trans.Commit();

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("InsertMasterVendor", ex.Message);
            }
            return result;

        }

        public async Task<int> InsertCompanyRelations(List<CompanyRelationModel> model)
        {
            int result = 0;

            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                  cs.Open();
                  SqlTransaction trans = cs.BeginTransaction();
                  result = await cs.ExecuteAsync(@"INSERT INTO 
                  tr_company_relations(vendor_number, company_id, purchase_organization_id) 
                  VALUES(@vendor_number, @company_id, @purchase_organization_id)", model, transaction: trans);

                  trans.Commit();

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("InsertCompanyRelations", ex.Message);
            }
            return result;
        }

        public async Task<int> DeleteCompanyRelations(List<CompanyRelationModel> vendor_list)
        {
            int result = 0;

            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                  cs.Open();
                  SqlTransaction trans = cs.BeginTransaction();
                  result = await cs.ExecuteAsync(
                      @"DELETE FROM tr_company_relations WHERE vendor_number = @vendor_number",
                    vendor_list.Select(x => new { vendor_number = x.vendor_number }).ToArray(), transaction: trans);

                  trans.Commit();

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("DeleteCompanyRelations", ex.Message);
            }
            return result;
        }

        public int VendorTypeId(string sap_code)
        {
            var result = 0;
            try
            {
                using (var cs = new ConnectionService(_appSettings).GetConnection())
                {
                    var sql = "SELECT TOP 1 vendor_type_id FROM m_vendor_type WHERE sap_code = @sap_code";

                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@sap_code", sap_code);
                    cs.Open();
                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result= Convert.ToInt32(reader["vendor_type_id"]);
                        }
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("VendorTypeId", ex.Message);
            }
            return result;
        }

        public int TaxTypeId(string sap_code)
        {
             var result = 0;
            try
            {
                using (var cs = new ConnectionService(_appSettings).GetConnection())
                {
                    var sql = "SELECT tax_type_id FROM m_tax_type WHERE sap_code = @sap_code or wht_code=@sap_code";

                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@sap_code", sap_code);
                    cs.Open();
                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result= Convert.ToInt32(reader["tax_type_id"]);
                        }
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("TaxTypeId", ex.Message);
            }
            return result;
        }

        public int TaxNumberTypeId(string npwp)
        {
            int result = 0;
            if (npwp == "NON NPWP" || npwp == "NONNPWP")
            {
                result = 3;
            }
            if (npwp == "NPWP NON PKP" || npwp == "NPWPNONPKP")
            {
                result = 1;
            }
            if(npwp == "NPWP PKP" || npwp == "NPWPPKP")
            {
                result = 2;
            }
            if(string.IsNullOrEmpty(npwp))
            {
                result = 2;
            }
            return result;
        }

        public string CekVendor(string model)
        {
             var result = "";
            try
            {
                using (var cs = new ConnectionService(_appSettings).GetConnection())
                {
                    var sql = "SELECT vendor_number FROM tr_vendor WHERE vendor_number = @vendor_number";

                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@vendor_number", model);
                    cs.Open();
                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result = reader["vendor_number"].ToString();
                        }
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("CekVendor", ex.Message);
            }
            return result;
        }

        public string GetCompanyIdByPurchOrg(string purch_org, string vendor_number)
        {
             var result = "";
            try
            {
                using (var cs = new ConnectionService(_appSettings).GetConnection())
                {
                    var sql = "SELECT company_id FROM tr_company_relations WHERE purchase_organization_id = @purchase_organization_id AND vendor_number = @vendor_number";

                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@purchase_organization_id", purch_org);
                    command.Parameters.AddWithValue("@vendor_number", vendor_number);
                    cs.Open();
                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result = reader["company_id"].ToString();
                        }
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("GetCompanyIdByPurchOrg", ex.Message);
            }
            return result;
        }

        public async Task<int> InsertUserMaster(List<UserModel> model)
        {
             int result = 0;

            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                  cs.Open();
                  SqlTransaction trans = cs.BeginTransaction();
                  result = await cs.ExecuteAsync(@"INSERT INTO 
                  tb_user(vendor_number, name, email, username, password, role_id) 
                  VALUES(@vendor_number, @name, @email, @username, @password, @role_id)", model, transaction: trans);

                  trans.Commit();

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("InsertUserMaster", ex.Message);
            }
            return result;
        }

        public bool Top(string top_id)
        {
            bool result = false;
            try
            {
                using (var cs = new ConnectionService(_appSettings).GetConnection())
                {
                    var sql = "SELECT COUNT(*) AS TOTAL from m_top WHERE top_id=@top_id";

                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@top_id", top_id);
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
                new Helpers.GlobalFunction().LogError("Top", ex.Message);
            }
            return result;
        }

        public bool Currency(string currency_id)
        {
            bool result = false;
            try
            {
                using (var cs = new ConnectionService(_appSettings).GetConnection())
                {
                    var sql = "SELECT COUNT(*) AS TOTAL from m_currency WHERE currency_id=@currency_id";

                    SqlCommand command = new SqlCommand(sql, cs);
                    command.Parameters.AddWithValue("@currency_id", currency_id);
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
                new Helpers.GlobalFunction().LogError("Currency", ex.Message);
            }
            return result;
        }

        public async Task<List<TempPoServiceModel>> UpdateMasterPoSVC(List<TempPoServiceModel> model)
        {
            var result = new List<TempPoServiceModel>();
            IEnumerable<dynamic> resultSelect = new dynamic[]{};

            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                  cs.Open();
                  SqlTransaction trans = cs.BeginTransaction();
                    foreach(var item in model){
                        var querySelect = cs.FluentQueryBuilder()
                        .Select($"t.*")
                        .From($"temp_po_svc t")
                        .Where($"po_number = {item.po_number} AND po_item_number = {item.po_item_number}");
                        resultSelect = querySelect.Query<TempPoServiceModel>(transaction: trans);
                        if(resultSelect.Count() > 0){
                            await cs.ExecuteAsync(@"UPDATE temp_po_svc set 
                            type = @type, cat = @cat, pgr = @pgr,
                            poh = @poh, doc_date = @doc_date, material = @material,
                            po_name = @po_name, material_group = @material_group,
                            d = @d, i= @i, a= @a, plnt = @plnt, s_loc = @s_loc, po_quantity = @po_quantity, uom = @uom,
                            company_id = @company_id, quantity_2 = @quantity_2, sku = @sku, net_price = @net_price,
                            currency = @currency, per = @per, quantity_3 = @quantity_3, open_tgt_qty = @open_tgt_qty,
                            quantity_to_be_del = @quantity_to_be_del, amount_to_be_del = @amount_to_be_del,
                            quantity_to_be_inv = @quantity_to_be_inv, amount_to_be_inv = @amount_to_be_inv,
                            number = @number, vendor_number = @vendor_number, p_org = @p_org, l = @l, tracking_number = @tracking_number,
                            agmt = @agmt, item_2 = @item_2, targ_val = @targ_val, tot_open_val = @tot_open_val,
                            open_value = @open_value, rel_value= @rel_value,rel_qty= @rel_qty,vp_start = @vp_start,vper_end= @vper_end,
                            quot_dd_in = @quot_dd_in,s= @s,coll_number=@coll_number,ctl=@ctl,info_rec=@info_rec,number_2=@number_2,
                            grp=@grp,strat=@strat,release=@release,rel=@rel,ist_loc=@ist_loc,vendor_name_2=@vendor_name_2,opu=@opu,
                            tx = @tx,tax_jur=@tax_jur,net_value=@net_value,i_2=@i_2,notified=@notified,stk_seg=@stk_seg,
                            req_seg=req_seg,s_2=@s_2,conf_item_number=@conf_item_number,sort_number=@sort_number,
                            ext_h_cat=@ext_h_cat,ru=@ru,reqmt_price=@reqmt_price,smart_no=@smart_no,
                            char1=@char1,char_desc_1=@char_desc_1,char2=@char2,char_desc_2=@char_desc_2,char3=@char3,char_desc_3=@char_desc_3 
               
                            WHERE po_number = @po_number AND po_item_number = @po_item_number AND is_used = 0", item, transaction: trans);

                        } else {
                            result.Add(item);
                        }
                    }
                    trans.Commit();


                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("erorr: "+ex);
                new Helpers.GlobalFunction().LogError("UpdateMasterPoSVC", ex.Message);
            }
            return result;
        }

        
        public async Task<List<TempPoLpbModel>> UpdateMasterPoLPB(List<TempPoLpbModel> model)
        {
            var result = new List<TempPoLpbModel>();
            IEnumerable<dynamic> resultSelect = new dynamic[]{};

            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                  cs.Open();
                  SqlTransaction trans = cs.BeginTransaction();
                    foreach(var item in model){
                        var querySelect = cs.FluentQueryBuilder()
                        .Select($"t.*")
                        .From($"temp_po_lpb t")
                        .Where($"document_no = {item.document_no} AND item_1 = {item.item_1}");
                        resultSelect = querySelect.Query<TempPoLpbModel>(transaction: trans);
                        if(resultSelect.Count() > 0){
                            Console.WriteLine("item no: "+item.document_no);
                            await cs.ExecuteAsync(@"UPDATE temp_po_lpb set 

                            po_number = @po_number, vendor_number = @vendor_number, po_item_number = @po_item_number,
                            po_name = @po_name, currency = @currency, po_quantity = @po_quantity, uom = @uom,
                            lpb_number = @lpb_number, lpb_item_number = @lpb_item_number, quantity_lpb = @quantity_lpb,
                            company_id = @company_id, posting_date = @posting_date, doc_header_text = @doc_header_text,
                            clearing_date = @clearing_date, clearing_doc = @clearing_doc, pk = @pk, acc_ty = @acc_ty,
                            sg = @sg, d_c = @d_c, bus_a = @bus_a, gl = @gl, gl_description = @gl_description, amount_in_dc = @amount_in_dc,
                            amount_in_lc = @amount_in_lc, material_code = @material_code, order_1 = @order_1, sales_doc= @sales_doc,
                            item_2 = @item_2,asset = @asset, s_no = @s_no, order_2 = @order_2, material_number = @material_number,
                            desc_vendor_or_credit = @desc_vendor_or_credit, division = @division, period = @period,
                            lpb_clearing = @lpb_clearing, lpb_clear_date = @lpb_clear_date, voucher = @voucher, cost_ctr = @cost_ctr,
                            account_type = @account_type, type = @type, op_number = @op_number, alt_bom = @alt_bom, rcp_group = @rcp_group,
                            tr_ty = @tr_ty, tr_s = @tr_s, count = @count, doc_currency = @doc_currency, mov_type = @mov_type, ref_doc_migo = @ref_doc_migo, is_active = @is_active 
                            WHERE document_no = @document_no AND item_1 = @item_1 AND is_used = 0", item, transaction: trans);

                        } else {
                            result.Add(item);
                        }
                    }
                    trans.Commit();


                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("erorr: "+ex);
                new Helpers.GlobalFunction().LogError("UpdateMasterPoLPB", ex.Message);
            }
            return result;
        }
        public bool CekLpbTransaction(LpbTransactionModel model)
        {
            Console.WriteLine("model :"+model.po_number+" model :"+model.po_item_number+" model :"+model.lpb_number+" model :"+model.lpb_item_number);
           bool result = false;
            try
            {
                using (var cs = new ConnectionService(_appSettings).GetConnection()) 
                {
                    
                        var sql = "SELECT COUNT(*) AS TOTAL FROM m_lpb_transaction WHERE po_number="+model.po_number+" AND po_item_number="+model.po_item_number+" AND lpb_number="+model.lpb_number+" AND lpb_item_number="+model.lpb_item_number;

                    SqlCommand command = new SqlCommand(sql, cs);
                    // command.Parameters.AddWithValue("@lpb_number", model.lpb_number);
                    // command.Parameters.AddWithValue("@lpb_item_number", model.lpb_number);
                    // command.Parameters.AddWithValue("@po_number", model.po_number);
                    // command.Parameters.AddWithValue("@po_item_number", model.po_item_number);
                    cs.Open();
                    var reader = command.ExecuteReader();

                    if (reader.HasRows)
                    Console.WriteLine("sql :"+sql);
                    {
                        while (reader.Read())
                        {
                            result= Convert.ToInt32(reader["TOTAL"].ToString()) > 0 ? true: false;
                            //Console.WriteLine("sqlyy :"+resultok);
                        }
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("resulterrr "+ ex.Message);
                new Helpers.GlobalFunction().LogError("CekLpbTransaction", ex.Message);
            }
            Console.WriteLine("result "+result);
            return result;
        }

        public bool UpdateLpbTransaction(LpbTransactionModel model)
        {
            var result = false;
            try
            {
                var sql = "Update m_lpb_transaction set " +
                          "vendor_number ="+model.vendor_number+", qty ="+model.qty+", uom = '"+model.uom+"', amount = "+model.amount+", currency = '"+model.currency+"', company_id ="+model.company_id+" " +
                          "WHERE po_number = "+model.po_number+" AND po_item_number = "+model.po_item_number+" AND lpb_number = "+model.lpb_number+"  AND lpb_item_number = "+model.lpb_item_number+";";

                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(sql, cs);
                    // command.Parameters.AddWithValue("@vendor_number", model.vendor_number);
                    // command.Parameters.AddWithValue("@qty", model.qty);
                    // command.Parameters.AddWithValue("@uom", model.uom);
                    // command.Parameters.AddWithValue("@amount", model.amount);
                    // command.Parameters.AddWithValue("@currency", model.currency);
                    // command.Parameters.AddWithValue("@company_id", model.company_id);
                    // command.Parameters.AddWithValue("@po_number", model.po_number);
                    // command.Parameters.AddWithValue("@po_item_number", model.po_item_number);
                    // command.Parameters.AddWithValue("@lpb_number", model.lpb_number);
                    // command.Parameters.AddWithValue("@lpb_item_number", model.lpb_number);
                    cs.Open();
                    Console.WriteLine("sql "+sql);
                    result = command.ExecuteNonQuery() == 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("UpdateLpbTransaction", ex.Message);
            }
            Console.WriteLine("ressult "+ result);
            return result;
        }

        public bool CekIsUsedLpb(LpbTransactionModel model)
        {
             bool result = false;
            try
            {
                using (var cs = new ConnectionService(_appSettings).GetConnection()) 
                {
                    
                        var sql = "SELECT is_used "+ 
                                "FROM m_lpb_transaction WHERE po_number = "+model.po_number+" AND po_item_number = "+model.po_item_number+" AND lpb_number = "+model.lpb_number+"  AND lpb_item_number = "+model.lpb_item_number+";";

                    SqlCommand command = new SqlCommand(sql, cs);
                    // command.Parameters.AddWithValue("@po_number", model.po_number);
                    // command.Parameters.AddWithValue("@po_item_number", model.po_item_number);
                    // command.Parameters.AddWithValue("@lpb_number", model.lpb_number);
                    // command.Parameters.AddWithValue("@lpb_item_number", model.lpb_number);
                    cs.Open();
                    Console.WriteLine("sql cek "+sql);
                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result= Convert.ToBoolean(reader["is_used"].ToString());
                        }
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("CekIsUsedLpb", ex.Message);
            }
            Console.WriteLine("is used po "+result);
            return result;
        }

        public bool CekPoTransaction(PoTransactionModel model)
        {
             bool result = false;
            try
            {
                using (var cs = new ConnectionService(_appSettings).GetConnection()) 
                {
                    
                        var sql = "SELECT COUNT(*) AS TOTAL "+ 
                                "FROM m_po_transaction WHERE po_number ="+model.po_number+" AND po_item_number = "+model.po_item_number;

                    SqlCommand command = new SqlCommand(sql, cs);
                    // command.Parameters.AddWithValue("@po_number", model.po_number);
                    // command.Parameters.AddWithValue("@po_item_number", model.po_item_number);
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
                new Helpers.GlobalFunction().LogError("CekPoTransaction", ex.Message);
            }
            return result;
        }

        public bool CekIsUsedPo(PoTransactionModel model)
        {
            bool result = false;
            try
            {
                using (var cs = new ConnectionService(_appSettings).GetConnection()) 
                {
                    
                        var sql = "SELECT is_used "+ 
                                "FROM m_po_transaction WHERE po_number ="+model.po_number+" AND po_item_number = "+model.po_item_number;

                    SqlCommand command = new SqlCommand(sql, cs);
                    // command.Parameters.AddWithValue("@po_number", model.po_number);
                    // command.Parameters.AddWithValue("@po_item_number", model.po_item_number);
                    cs.Open();
                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result= Convert.ToBoolean(reader["is_used"].ToString());
                        }
                    }
                    cs.Close();
                }
            }
            catch (Exception ex)
            {
                new Helpers.GlobalFunction().LogError("CekIsUsedPo", ex.Message);
            }
            return result;
        }

        public bool UpdatePoTransaction(PoTransactionModel model)
        {
             var result = false;
            try
            {
                var sql = "Update m_po_transaction set " +
                         "vendor_number ="+model.vendor_number+", qty = "+model.qty.ToString().Replace(",","")+", uom = '"+model.uom+"', amount = "+model.amount+", currency = '"+model.currency+"', company_id = "+model.company_id+", po_name = '"+model.po_name+"', is_service = 1 " +
                          "WHERE po_number = "+model.po_number+" AND po_item_number = "+model.po_item_number;

                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(sql, cs);
                    // command.Parameters.AddWithValue("@vendor_number", model.vendor_number);
                    // command.Parameters.AddWithValue("@qty", model.qty);
                    // command.Parameters.AddWithValue("@uom", model.uom);
                    // command.Parameters.AddWithValue("@amount", model.amount);
                    // command.Parameters.AddWithValue("@po_name", model.po_name);
                    // command.Parameters.AddWithValue("@currency", model.currency);
                    // command.Parameters.AddWithValue("@company_id", model.company_id);
                    // command.Parameters.AddWithValue("@po_number", model.po_number);
                    // command.Parameters.AddWithValue("@po_item_number", model.po_item_number);
                    cs.Open();
                     Console.WriteLine("sql cek "+sql);
                    result = command.ExecuteNonQuery() == 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("UpdatePoTransaction", ex.Message);
            }
            Console.WriteLine("update po "+result);
            return result;
        }

        public async Task<int> InsertMasterLogVendor(List<LogVendorModel> model)
        {
            int result = 0;

            try
            {
                using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
                {

                  cs.Open();
                  SqlTransaction trans = cs.BeginTransaction(); 
                  result = await cs.ExecuteAsync(@"INSERT INTO 
                  log_vendor(vendor_number, process, note, created_by )
                  VALUES (@vendor_number, @process, @note, @created_by )", model, transaction: trans);
    
                  trans.Commit();

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                new Helpers.GlobalFunction().LogError("InsertMasterLogVendor", ex.Message);
            }
            return result;
        }
        // public async Task<int> InsertMasterBank(List<BankModel> model)
        // {
        //      int result = 0;

        //     try
        //     {
        //         using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
        //         {
        //           cs.Open();
        //           SqlTransaction trans = cs.BeginTransaction();
        //           result = await cs.ExecuteAsync(@"INSERT INTO 
        //           m_bank(bank_id, name, country_of_origin, swift_code) 
        //           VALUES(@bank_id, @name, @country_of_origin, @swift_code)", model, transaction: trans);

        //           trans.Commit();

        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine(ex);
        //     }
        //     return result;
        // }

        // public bool InsertVendor(VendorModelRpa model)
        // {
        //      var result = false;
        //     var returnValue = "";
        //     try
        //     {
        //           using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
        //           {
        //               var sql = "INSERT INTO "+
        //                 "tr_vendor(vendor_number, contact_person, search_term, currency_id, top_id, name, street_address, additional_street_address, city, country_id, telephone, email, tax_type_id, tax_number_type_id, npwp, id_card_number, postal_code, vendor_type_id, title_id, is_locked, change_request_status, localidr_bank_id, localidr_bank_account, localidr_account_holder, foreign_bank_id, foreign_bank_account, foreign_account_holder ) "+ 
        //                 "VALUES(@vendor_number, @contact_person, @search_term, @currency_id, @top_id, @name, @street_address, @additional_street_address, @city, @country_id, @telephone, @email, @tax_type_id, @tax_number_type_id, @npwp, @id_card_number, @postal_code, @vendor_type_id, @title_id, @is_locked, @change_request_status, @localidr_bank_id, @localidr_bank_account, @localidr_account_holder, @foreign_bank_id, @foreign_bank_account, @foreign_account_holder)";

        //             SqlCommand command = new SqlCommand(sql, cs);
        //             command.Parameters.AddWithValue("@vendor_number", model.vendor_number);
        //             command.Parameters.AddWithValue("@contact_person", model.contact_person);
        //             command.Parameters.AddWithValue("@search_term", model.search_term);
        //             command.Parameters.AddWithValue("@currency_id", model.currency_id);
        //             command.Parameters.AddWithValue("@top_id", (string.IsNullOrEmpty(model.top_id) ? DBNull.Value : model.top_id));
        //             command.Parameters.AddWithValue("@name", model.name);
        //             command.Parameters.AddWithValue("@street_address", model.street_address);
        //             command.Parameters.AddWithValue("@additional_street_address", (string.IsNullOrEmpty(model.additional_street_address) ? DBNull.Value : model.additional_street_address));
        //             command.Parameters.AddWithValue("@city", model.city);
        //             command.Parameters.AddWithValue("@country_id", model.country_id);
        //             command.Parameters.AddWithValue("@telephone", model.telephone);
        //             command.Parameters.AddWithValue("@email", model.email);
        //             command.Parameters.AddWithValue("@tax_type_id", model.tax_type_id);
        //             command.Parameters.AddWithValue("@npwp", (string.IsNullOrEmpty(model.npwp.ToString()) ? DBNull.Value : model.npwp));
        //             command.Parameters.AddWithValue("@id_card_number", (string.IsNullOrEmpty(model.id_card_number.ToString()) ? DBNull.Value : model.id_card_number));
        //             command.Parameters.AddWithValue("@postal_code", model.postal_code);
        //             command.Parameters.AddWithValue("@vendor_type_id", model.vendor_type_id);
        //             command.Parameters.AddWithValue("@title_id", model.title_id);
        //             command.Parameters.AddWithValue("@tax_number_type_id", model.tax_number_type_id);
        //             command.Parameters.AddWithValue("@is_locked", model.is_locked);
        //             command.Parameters.AddWithValue("@change_request_status", model.change_request_status);
        //             command.Parameters.AddWithValue("@localidr_bank_id", (string.IsNullOrEmpty(model.localidr_bank_id) ? DBNull.Value : model.localidr_bank_id));
        //             command.Parameters.AddWithValue("@localidr_bank_account", (string.IsNullOrEmpty(model.localidr_bank_account.ToString()) ? DBNull.Value : model.localidr_bank_account));
        //             command.Parameters.AddWithValue("@localidr_account_holder", (string.IsNullOrEmpty(model.localidr_account_holder.ToString()) ? DBNull.Value : model.localidr_account_holder));
        //             //command.Parameters.AddWithValue("@foreign_bank_country_id", (string.IsNullOrEmpty(model.foreign_bank_country_id) ? DBNull.Value : model.foreign_bank_country_id));
        //             command.Parameters.AddWithValue("@foreign_bank_id", (String.IsNullOrEmpty(model.foreign_bank_id) ? DBNull.Value : model.foreign_bank_id));
        //             // command.Parameters.AddWithValue("@foreign_bank_name", (string.IsNullOrEmpty(model.foreign_bank_name) ? DBNull.Value : model.foreign_bank_name));
        //             // command.Parameters.AddWithValue("@foreign_bank_swift_code", (string.IsNullOrEmpty(model.foreign_bank_swift_code) ? DBNull.Value : model.foreign_bank_swift_code));
        //             command.Parameters.AddWithValue("@foreign_bank_account", (string.IsNullOrEmpty(model.foreign_bank_account) ? DBNull.Value : model.foreign_bank_account));
        //             command.Parameters.AddWithValue("@foreign_account_holder", (String.IsNullOrEmpty(model.foreign_account_holder) ? DBNull.Value : model.foreign_account_holder));
        //             cs.Open();
        //             result = command.ExecuteNonQuery() == 1 ;
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine(ex.Message);
        //     }
        //     return result;
        // }

        // public async Task<int> InsertMasterTempVendor(List<TempVendorModel> model)
        // {
        //     int result = 0;

        //     try
        //     {
        //         using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
        //         {
        //           cs.Open();
        //           SqlTransaction trans = cs.BeginTransaction();
        //           result = await cs.ExecuteAsync(@"INSERT INTO 
        //           temp_vendor(vendor_number, contact_person, search_term, currency_id, top_id, name, street_address, additional_street_address, city, country_id, telephone, email, tax_type_id, tax_number_type_id, npwp, id_card_number, postal_code, vendor_type_id, title_id, position_data, verification_status_id, verification_note, type, sg_category_id, file_id_card ) 
        //           VALUES(@vendor_number, @contact_person, @search_term, @currency_id, @top_id, @name, @street_address, @additional_street_address, @city, @country_id, @telephone, @email, @tax_type_id, @tax_number_type_id, @npwp, @id_card_number, @postal_code, @vendor_type_id, @title_id, @position_data, @verification_status_id, @verification_note, @type, @sg_category_id, @file_id_card)", model, transaction: trans);

        //           trans.Commit();

        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine(ex);
        //     }
        //     return result;
        // }

        // public async Task<int> InsertTempCompanyRelations(List<TempCompanyRelationModel> model)
        // {
        //     int result = 0;

        //     try
        //     {
        //         using (SqlConnection cs = new SqlConnection(_appSettings.ConnectionString))
        //         {
        //           cs.Open();
        //           SqlTransaction trans = cs.BeginTransaction();
        //           result = await cs.ExecuteAsync(@"INSERT INTO 
        //           temp_company_relations(vendor_id, company_id, purchase_organization_id) 
        //           VALUES(@vendor_id, @company_id, @purchase_organization_id)", model, transaction: trans);

        //           trans.Commit();

        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine(ex);
        //     }
        //     return result;
        // }

        // public bool CekSapTaxType(string sap_code)
        // {
        //     bool result = false;
        //     try
        //     {
        //         using (var cs = new ConnectionService(_appSettings).GetConnection())
        //         {
        //             var sql = "SELECT COUNT(*) AS TOTAL from m_tax_type WHERE sap_code=@sap_code";

        //             SqlCommand command = new SqlCommand(sql, cs);
        //             command.Parameters.AddWithValue("@sap_code", sap_code);
        //             cs.Open();
        //             var reader = command.ExecuteReader();
        //             if (reader.HasRows)
        //             {
        //                 while (reader.Read())
        //                 {
        //                     result= Convert.ToInt32(reader["TOTAL"].ToString())> 0 ? true : false;
        //                 }
        //             }
        //             cs.Close();
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         new Helpers.GlobalFunction().LogError("cek sap_code", ex.Message);
        //     }
        //     return result;
        // }

        // public int CekQuantity()
        // {
        //     int result = 0;
        //     try
        //     {
        //         using (var cs = new ConnectionService(_appSettings).GetConnection())
        //         {
        //             var sql = "SELECT COUNT(*) AS TOTAL from (SELECT lpb_number, lpb_item_number, po_number, po_item_number, vendor_number, uom, currency, SUM(case when amount_in_dc > 0 then amount_in_dc else null end) as amount, SUM(case when quantity_lpb > 0 then quantity_lpb else null end) as qty "+ 
        //                         "FROM temp_po_lpb GROUP BY po_number, po_item_number, lpb_number, lpb_item_number, vendor_number, uom, currency)";

        //             SqlCommand command = new SqlCommand(sql, cs);
        //             cs.Open();
        //             var reader = command.ExecuteReader();
        //             if (reader.HasRows)
        //             {
        //                 while (reader.Read())
        //                 {
        //                     result= Convert.ToInt32(reader["TOTAL"].ToString());
        //                 }
        //             }
        //             cs.Close();
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         new Helpers.GlobalFunction().LogError("cek quantity", ex.Message);
        //     }
        //     return result;
        // }
    }
}