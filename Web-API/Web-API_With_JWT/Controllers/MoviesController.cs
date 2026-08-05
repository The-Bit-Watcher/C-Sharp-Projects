using Ass3WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Ass3WebApi.Services;

namespace Ass3WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService _movieService;
        private readonly ILogger<MoviesController> _logger;

        public MoviesController(IMovieService movieService, ILogger<MoviesController> logger)
        {
            _movieService = movieService;
            _logger = logger;
        }

        [HttpGet("search")]
        public async Task<ActionResult<List<Movie>>> SearchMovies([FromQuery] string title)
        {
            _logger.LogInformation("Search request received for title: {Title}", title);

            if (string.IsNullOrWhiteSpace(title))
            {
                return BadRequest(new { message = "Title parameter is required" });
            }

            var results = await _movieService.SearchMoviesAsync(title);
            _logger.LogInformation("Search returned {Count} results", results.Count);

            return Ok(results);
        }

        [HttpGet("details/{imdbId}")]
        public async Task<ActionResult<Movie>> GetMovieDetails(string imdbId)
        {
            _logger.LogInformation("Details request received for IMDB ID: {ImdbId}", imdbId);

            if (string.IsNullOrWhiteSpace(imdbId))
            {
                return BadRequest(new { message = "IMDB ID is required" });
            }

            var result = await _movieService.GetMovieDetailsAsync(imdbId);

            if (result == null)
            {
                return NotFound(new { message = "Movie not found" });
            }

            return Ok(result);
        }
    }
}