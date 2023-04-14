using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace tufol.Models
{
  [Table("tb_user_access_api")]
  public class UserAccessApiModel
  {
    public string username { get; set; }
    public string access_token { get; set; }

    [DataType(DataType.Date)]
    public DateTime created_at { get; set; }

    [DataType(DataType.Date)]
    public DateTime updated_at { get; set; }
  }
}