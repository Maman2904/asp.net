using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace tufol.Models
{
    [Table("temp_vendor")]
    public class TempVendorModel
    {
        public int vendor_id { get; set; }
        public string vendor_number { get; set; }
        public string? type {get; set;}
        public int vendor_type_id { get; set; }
        public string? account_group_id { get; set; }
        public string vendor_type_name { get; set; }
        public string vendor_type_code { get; set; }
        public string scheme_group { get; set; }
        public string? scheme_group_name { get; set; }
        public string gl { get; set; }
        public string gl_name { get; set; }
        public int sg_category_id { get; set; }
        public string sg_category_name { get; set; }
        public IEnumerable<CompanyModel> company { get; set; }
        public string currency_id { get; set; }
        public string? top_id { get; set; }
        public string top_name { get; set; }
        public string title_id { get; set; }
        public string name { get; set; }
        public string contact_person { get; set; }
        public string search_term { get; set; }
        public string street_address { get; set; }
        public string? additional_street_address { get; set; }
        public string city { get; set; }
        public string postal_code { get; set; }
        public string country_id { get; set; }
        public string country_name { get; set; }
        public string telephone { get; set; }
        public string? fax { get; set; }
        public string email { get; set; }
        public List<String> email_list { get; set; }
        public int tax_type_id { get; set; }
        public string tax_type_name { get; set; }
        public string wht_code { get; set; }
        public string tax_rates { get; set; }
        public int tax_number_type_id { get; set; }
        public string tax_number_type_code { get; set; }
        public string tax_number_type_name { get; set; }
        public string? npwp { get; set; }
        public string? id_card_number { get; set; }
        public string? sppkp_number { get; set; }
        public string? localidr_bank_id { get; set; }
        public string? localidr_bank_name { get; set; }
        public string? localidr_swift_code { get; set; }
        public decimal? localidr_bank_account { get; set; }
        public string? localidr_account_holder { get; set; }
        public string? localforex_bank_id { get; set; }
        public string? localforex_bank_name { get; set; }
        public string? localforex_swift_code { get; set; }
        public decimal? localforex_bank_account { get; set; }
        public string? localforex_account_holder { get; set; }
        public string? localforex_currency_id { get; set; }
        public string? foreign_bank_country_id { get; set; }
        public string? foreign_bank_country_name { get; set; }
        public string? foreign_bank_id { get; set; }
        public string? foreign_bank_name { get; set; }
        public string? foreign_bank_swift_code { get; set; }
        public string? foreign_bank_account { get; set; }
        public string? foreign_account_holder { get; set; }
        public string? foreign_currency_id { get; set; }
        public string? reference_correspondent { get; set; }
        public string? reference_country_origin { get; set; }
        public string? pic_id { get; set; }
        public string? person_number { get; set; }
        public string? email_procurement_group { get; set; }
        public string file_id_card { get; set; }
        public string? file_npwp { get; set; }
        public string? file_sppkp { get; set; }
        public string? file_vendor_statement { get; set; }
        public int? verification_status_id { get; set; }
        public string? status_for_vendor { get; set; }
        public string? status_for_intern { get; set; }
        public string? verification_note { get; set;}
        public int? position_data { get; set; }
        public string? token { get; set; }
        public string? verified_email { get; set; }
        public List<String> verified_email_list { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public IEnumerable<TempLogVendorStatusModel> temp_log_vendor_status { get; set; }
        public string? partner_function {get; set;}
        public string? partner_function_name {get; set;}
        public string? updated_data {get;set;}
        public int? is_extension {get;set;}
        public bool? status_rpa {get; set;}
        public string? rpa_status_description {get; set;}
    }
}