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
    [Table("tr_vendor")]
    public class VendorDatatableModel
    {
        public string vendor_number {get; set;}
        public string pic_id {get; set;}
        public string vendor_type_name { get; set; }
        public string name {get; set;}
        public string street_address {get; set;}
        public string additional_street_address {get; set;}
        public string city {get; set;}
        public string postal_code {get; set;}
        public string country_name {get; set;}
        public string telephone{get; set;}
    }
}