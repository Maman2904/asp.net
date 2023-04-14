using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace tufol.Models
{
    [Table("m_announcement")]
    public class AnnouncementModel
    {
        [MaxLength(50, ErrorMessage = "maximal 50 Chracter")]
        public string key_flag { get; set; }

        [MaxLength(100, ErrorMessage = "maximal 100 Chracter")]
        public string? title { get; set; }
        public string? content { get; set; }
        [MaxLength(100, ErrorMessage = "maximal 100 Chracter")]
        public string? title_en { get; set; }
        public string? content_en { get; set; }
    }
}