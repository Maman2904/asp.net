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
    public class AllowedExtensionsAttribute:ValidationAttribute
    {
        private readonly string[] _extensions;
        public AllowedExtensionsAttribute(string[] extensions)
        {
            _extensions = extensions;
        }

        protected override ValidationResult IsValid(
    object value, ValidationContext validationContext)
    {
        var file = value as IFormFile;
        if (file != null)
        {
            var extension = Path.GetExtension(file.FileName);
            if (!_extensions.Contains(extension.ToLower()))
            {
                return new ValidationResult(GetErrorMessage());
            }
        }
        
        return ValidationResult.Success;
    }

    public string GetErrorMessage()
    {
        return $"This file extension is not allowed!";
    }
}

     public class MaxFileSizeAttribute : ValidationAttribute
    {
    private readonly int _maxFileSize;
    public MaxFileSizeAttribute(int maxFileSize)
    {
        _maxFileSize = maxFileSize;
    }

    protected override ValidationResult IsValid(
    object value, ValidationContext validationContext)
    {
        var file = value as IFormFile;
        if (file != null)
        {
           if (file.Length > _maxFileSize)
            {
                return new ValidationResult(GetError());
            }
        }

        return ValidationResult.Success;
    }

    public string GetError()
    {
        return $"Maximum allowed file size is { _maxFileSize} bytes.";
    }
}


    [Table("temp_vendor")]
    public class RegistrationModel
    {

        public int vendor_id {get; set;}

        [Required(ErrorMessage = "Vendor type is required")]
        public int vendor_type_id { get; set; }
        public string? vendor_type_name { get; set; }
        public string account_group_id { get; set; }
        public string? vendor_number { get; set; }
        public string type {get; set;}
        public int? is_extension { get; set; }
        public int sg_category_id { get; set; }
        public IEnumerable<CompanyModel> company { get; set; }
        
        [Required(ErrorMessage = "Company is required")]
        public string[] company_id {get; set;}

        [Required(ErrorMessage = "Currency is required")]
        // [MaxLength(5), MinLength(2)] 
        public string currency_id {get; set;}
        public string[]? purchase_organization_id {get; set;}

        [StringLength(5, ErrorMessage = "Payment term length can't be more than 5")]
        public string? top_id {get; set;}

        [Required(ErrorMessage = "Title is required")]        
        // [MaxLength(10), MinLength(2)]
        public string title_id {get; set;}

        [Required(ErrorMessage = "Name is required")]
        [StringLength(256, ErrorMessage = "Name length can't be more than 256")]
        public string name {get; set;}

        [Required(ErrorMessage = "Contact person is required")]
        [StringLength(256, ErrorMessage = "Contact person length can't be more than 256")]
        public string contact_person {get; set;}

        [Required(ErrorMessage = "Search term is required")]
        [StringLength(20, ErrorMessage = "Search term length can't be more than 20")] 
        public string search_term {get; set;}

        [Required(ErrorMessage = "Street address is required")]
        [StringLength(60, ErrorMessage = "Street address length can't be more than 60")] 
        public string street_address {get; set;}
        
        [StringLength(40, ErrorMessage = "Additional street address length can't be more than 40")]
        public string? additional_street_address {get; set;}

        [Required(ErrorMessage = "City is required")]
        [StringLength(60, ErrorMessage = "City length can't be more than 60")]
        public string city {get; set;}

        [Required(ErrorMessage = "Postal code is required")]
        // [StringLength(5, MinimumLength = 5, ErrorMessage = "Postal code length can't be more or less than 5")]
        public string postal_code {get; set;}

        [Required(ErrorMessage = "Country is required")]
        // [MaxLength(3)]
        public string country_id {get; set;}

        [Required(ErrorMessage = "Telephone is required")]
        [StringLength(60, ErrorMessage = "Telephone length can't be more than 60")] 
        public string telephone{get; set;}

        [Required(ErrorMessage = "Fax is required")]
        [StringLength(20, ErrorMessage = "Fax length can't be more than 20")] 
        public string fax {get; set;}

        [Required(ErrorMessage = "Email is required")]
        [MaxLength(132)]
        public string email {get; set;}
        
        [Required(ErrorMessage = "Services category is required")]
        public int tax_type_id {get; set;}

        [Required(ErrorMessage = "Tax number type is required")]
        public int tax_number_type_id {get; set;}
        
        public string? npwp {get; set;}
        public string? id_card_number {get; set;}
        public string? sppkp_number {get; set;}
        public string? localidr_bank_id {get; set;}
        public Decimal? localidr_bank_account {get; set;}
        public string? localidr_account_holder {get; set;}

        // [MaxLength(7)]
        public string? localforex_bank_id {get; set;}

        public Decimal? localforex_bank_account {get; set;}

        // [MaxLength(256)]
        public string? localforex_account_holder {get; set;}

        // [MaxLength(5)]
        public string? localforex_currency_id {get; set;}
        public string? foreign_bank_country_id {get; set;}
        public string? foreign_bank_id {get; set;}
        public string? foreign_bank_name {get; set;}

        // [MaxLength(15)]
        public string? foreign_bank_swift_code {get; set;}
        
        [RegularExpression("^[0-9]{0,20} IBAN [0-9]{0,20}$", ErrorMessage = "Please follow this format: xxxx IBAN xxxx")]
        public string? foreign_bank_account {get; set;}
        public string? foreign_account_holder {get; set;}
        public string? foreign_currency_id {get; set;}
        // [MaxLength(256)]
        public string? reference_correspondent {get; set;}

        // [MaxLength(256)]
        public string? reference_country_origin {get; set;}
        public string? pic_id {get; set;}

        [MaxFileSize(2* 1024 * 1024)]
        [AllowedExtensions(new string[] { ".pdf" })]
        public IFormFile? file_id_card {get; set;}

        [MaxFileSize(2* 1024 * 1024)]
        [AllowedExtensions(new string[] { ".pdf" })]
        public IFormFile? file_npwp {get; set;}

        [MaxFileSize(2* 1024 * 1024)]
        [AllowedExtensions(new string[] { ".pdf" })]
        public IFormFile? file_sppkp {get; set;}

        // [Required(ErrorMessage = "Please select a file.")]
        [MaxFileSize(2* 1024 * 1024)]
        [AllowedExtensions(new string[] { ".pdf" })]
        public IFormFile? file_vendor_statement {get; set;}
        public int? verification_status_id {get; set;}
        public string? verification_note {get; set;}
        public int? position_data {get; set;}
        public string? token {get; set;}
        public string? partner_function {get; set;}
        public string? updated_data {get; set;}
        public string? change_request_note {get; set;}
        public bool? status_rpa {get; set;}
        public string? rpa_status_description {get; set;}
    }
}