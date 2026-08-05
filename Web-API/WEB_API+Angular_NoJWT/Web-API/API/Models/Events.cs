using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HAS01.API.Models
{
    public class Events
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string EventName { get; set; } = string.Empty;

        [Required]
        public string Location { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TicketPrice { get; set; }
    }
}
