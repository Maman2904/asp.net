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

  public class VendorType
  {
    public int? vendor_type_id { get; set; }
    public string? name { get; set; }
    public string? sap_code { get; set; }
    public string? scheme_group_id { get; set; }
    public string? gl_id { get; set; }

  }

  public class PartnerFunction
  {
    public string province_id { get; set; }
    public string name { get; set; }
  }


  //  “vendor_schema”: “ZL”,
  //   “account_group”: “ZK21”,
  //   “cash_management”: “ZK21”,
  //     “recon_account”:  21111000,

  public class SecondaryCompanyList {
    public int? company_relation_id { get;set; }
    public string? purchase_organization_id { get; set; }
    public string? company_id { get; set; }
  }


  [Table("temp_vendor")]
  public class RpaTempVendorModel
  {
    public int? vendor_id { get; set; }
    public string? vendor_number { get; set; }
    public string? type { get; set; }
    public int? is_extension { get; set; }
    public int? primary_company_relation_id { get; set; }
     public string? primary_company_id { get; set; }
    public string? primary_company_purchase_organization_id { get; set; }
    public List<SecondaryCompanyList>? secondary_company_list { get; set; }
    public VendorType? vendor_type { get; set; }
    public string? vendor_schema { get; set; }
    public string? account_group { get; set; }
    public string? cash_management { get; set; }
    public string? recon_account { get; set; }
    public PartnerFunction? partner_function { get; set; }
    public List<string>? department {get;set;}
    public string? services_goods_categories { get; set; }
    public string? currency { get; set; }
    public string term_payment { get; set; }
    public string top_name { get; set; }
    public string? top_baseline_date { get; set; }
    public string? top_interval { get; set; }
    public string? title { get; set; }
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
    public List<Dictionary<String,String>> rpa_telephone_list {get;set;}
    public List<String> telephone_list { get; set; }
    public string? fax { get; set; }
    public string email { get; set; }
    public List<String> verified_email_list { get; set; }
    public List<Dictionary<String,String>> rpa_email_list {get;set;}
    public string? services_categories { get; set; }
    public string? withholding_tax_type { get; set; }
    public string? withholding_tax_code { get; set; }
    public string? tax_number_type { get; set; }
    public string? tax_number_code { get; set; }
    public string? npwp { get; set; }
    public string? id_card_no { get; set; }
    public string? sppkp_number { get; set; }
    public string? localidr_bank_key { get; set; }
    public string? localidr_bank_name { get; set; }
    public string? localidr_bank_swift_code { get; set; }
    public decimal? localidr_bank_account { get; set; }
    public string? localidr_account_holder { get; set; }
    public string? localforex_bank_key { get; set; }
    public string? localforex_bank_name { get; set; }
    public string? localforex_bank_swift_code { get; set; }
    public decimal? localforex_bank_account { get; set; }
    public string? localforex_account_holder { get; set; }
    public string? localforex_currency { get; set; }
    public string? foreign_bank_country_id { get; set; }
    public string? foreign_bank_key { get; set; }
    public string? foreign_bank_name { get; set; }
    public string? foreign_bank_swift_code { get; set; }
    public string? foreign_bank_account { get; set; }
    public string? foreign_account_holder { get; set; }
    public string? foreign_currency { get; set; }
    public string? reference_correspondent { get; set; }
    public string? reference_country_origin { get; set; }
    public string? pic_initial { get; set; }
    public string? pic_area { get; set; }
    public string? pic_group_email { get; set; }
    public string? pic_pers_num { get; set; }
    public string? file_id_card { get; set; }
    public string? file_npwp { get; set; }
    public string? file_sppkp { get; set; }
    public string? file_vendor_statement { get; set; }
    public int? status_rpa { get; set; }
    public string? rpa_status_description { get; set; }

  }
}