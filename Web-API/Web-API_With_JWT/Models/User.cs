using Ass3WebApi.Models;
using System.ComponentModel.DataAnnotations;

namespace Ass3WebApi.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ICollection<WatchlistItem> Watchlist { get; set; } = new List<WatchlistItem>();
        public virtual ICollection<WatchedItem> WatchedList { get; set; } = new List<WatchedItem>();
    }
}