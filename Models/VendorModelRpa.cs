using System;

namespace tufol.Models
{
    public class VendorModelRpa
    {
        public string? vendor_number {get; set;}
        public string? currency_id {get; set;}
        public string? top_id {get; set;}
        public string? title_id {get; set;}
        public int? vendor_type_id {get; set;}
        public string? name {get; set;}
        //public string contact_person {get; set;}
        public string? search_term {get; set;}
        public string? street_address {get; set;}
        public string? additional_street_address {get; set;}
        public string? city {get; set;}
        public string? postal_code {get; set;}
        public string? country_id {get; set;}
        public string? telephone{get; set;}
        public string? email {get; set;}
        public int? tax_type_id {get; set;}
        public int? tax_number_type_id {get; set;}
        public string? npwp {get; set;}
        public string? id_card_number {get; set;}
        public string? contact_person {get; set;}
        // public string? sppkp_number {get; set;}
        public string? localidr_bank_id {get; set;}
        //public string? localidr_bank_name {get; set;}
        //public string? localidr_swift_code {get; set;}
        public decimal? localidr_bank_account {get; set;}
        public string? localidr_account_holder {get; set;}
        // public string? localforex_bank_id {get; set;}
        // public string? localforex_bank_name {get; set;}
        // public string? localforex_swift_code {get; set;}
        // public decimal? localforex_bank_account {get; set;}
        // public string? localforex_account_holder {get; set;}
        // public string? localforex_currency_id {get; set;}
        //public string? foreign_bank_country_id {get; set;}
        //public string? foreign_bank_country_name {get; set;}
        public string? foreign_bank_id {get; set;}
        //public string? foreign_bank_name {get; set;}
        //public string? foreign_bank_swift_code {get; set;}
        public string? foreign_bank_account {get; set;}
        public string? foreign_account_holder {get; set;}
        // public string? foreign_currency_id {get; set;}
        // public string for_curr_id {get; set;}
        // public string? reference_correspondent {get; set;}
        // public string? reference_country_origin {get; set;}
        // public string? pic_id {get; set;}
        // public string? person_number {get; set;}
        // public string? email_procurement_group {get; set;}
        // public string? file_id_card {get; set;}
        // public string? file_npwp {get; set;}
        // public string? file_sppkp {get; set;}
        // public string? file_vendor_statement {get; set;}
        public bool is_locked {get; set;}
        // public int? position_data {get; set;}
        // public string? token {get; set;}
        //public DateTime created_at { get; set; }
        //public DateTime updated_at { get; set; }
        // public IEnumerable<LogVendorModel> log_vendor { get; set; }
        public int change_request_status {get; set;}
        public string? change_request_note {get; set;}
        // public string? partner_function {get; set;}
        // public string? partner_function_name {get; set;}
        // public bool status_rpa {get; set;}
        // public string? rpa_status_description {get; set;}
    }
}