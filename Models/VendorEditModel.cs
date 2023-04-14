using System.ComponentModel.DataAnnotations;

namespace tufol.Models
{
    public class VendorEditModel : RegistrationModel
    {
        [Required(ErrorMessage = "Payment term is required")]
        [StringLength(5, ErrorMessage = "Payment term length can't be more than 5")]
        new public string? top_id {get; set;}

        [Required(ErrorMessage = "Partner Function is required")]
        new public string? partner_function {get; set;}
    }
}