using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace tufol.Models
{
    public class SelectModel
    {
        public string id { get; set; }
        public string text { get; set; }
        public string value { get; set; }
    }
}