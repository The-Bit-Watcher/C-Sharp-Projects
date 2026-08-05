using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;

namespace Ass3WebApi.Models
{
    public class WatchedItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int MovieId { get; set; }

        public int TimesWatched { get; set; } = 1;

        public DateTime FirstWatchedAt { get; set; } = DateTime.UtcNow;

        public DateTime LastWatchedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [ForeignKey("MovieId")]
        public virtual Movie? Movie { get; set; }

    }
}
