using Ass3WebApi.Models;

namespace Ass3WebApi.Services
{
    public interface IMovieService
    {
        Task<List<Movie>> SearchMoviesAsync(string title);
        Task<Movie?> GetMovieDetailsAsync(string imdbId);
    }
}
