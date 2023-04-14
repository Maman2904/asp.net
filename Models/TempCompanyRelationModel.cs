using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tufol.Models
{
  [Table("temp_company_relations")]
  public class TempCompanyRelationModel
  {
    public int company_relation_id { get;set; }
    public string? purchase_organization_id { get; set; }
    public int vendor_id { get; set; }
    public string company_id { get; set; }
    [DataType(DataType.Date)]
    public DateTime created_at { get; set; }
    [DataType(DataType.Date)]
    public DateTime updated_at { get; set; }
  }
}
