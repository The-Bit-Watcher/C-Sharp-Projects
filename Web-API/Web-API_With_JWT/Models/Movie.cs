using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Ass3WebApi.Models
{
    public class Movie
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ImdbId { get; set; } = string.Empty;

        [Required]
        public string Title { get; set; } = string.Empty;

        public int Year { get; set; } = 0;

        public string Poster { get; set; } = string.Empty;

        public string Actors { get; set; } = string.Empty;

        public string Genre { get; set; } = string.Empty;

        public string Plot { get; set; } = string.Empty;

        public string Director { get; set; } = string.Empty;

        public string Writer { get; set; } = string.Empty;

        public string Rated { get; set; } = string.Empty;

        public string Runtime { get; set; } = string.Empty;

        public string ImdbRating { get; set; } = string.Empty;
    }
}
