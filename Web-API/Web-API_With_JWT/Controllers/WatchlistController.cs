using Ass3WebApi.Data;
using Ass3WebApi.Models;
using Ass3WebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Ass3WebApi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WatchlistController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMovieService _movieService;

        public WatchlistController(ApplicationDbContext context, IMovieService movieService)
        {
            _context = context;
            _movieService = movieService;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim ?? "0");
        }

        [HttpGet]
        public async Task<ActionResult<List<WatchlistItem>>> GetWatchlist()
        {
            var userId = GetUserId();

            var watchlist = await _context.WatchlistItems
                .Include(w => w.Movie)
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.DateAdded)
                .ToListAsync();

            return Ok(watchlist);
        }

        [HttpPost]
        public async Task<IActionResult> AddToWatchlist([FromBody] Movie movie)
        {
            var userId = GetUserId();

            // Check if movie exists in database
            var existingMovie = await _context.Movies
                .FirstOrDefaultAsync(m => m.ImdbId == movie.ImdbId);

            if (existingMovie == null)
            {
                // Fetch full movie details from OMDb
                var movieDetails = await _movieService.GetMovieDetailsAsync(movie.ImdbId);

                if (movieDetails == null)
                {
                    return BadRequest(new { message = "Movie not found" });
                }

                existingMovie = movieDetails;
                _context.Movies.Add(existingMovie);
                await _context.SaveChangesAsync();
            }

            // Check if already in watchlist
            var existingItem = await _context.WatchlistItems
                .FirstOrDefaultAsync(w => w.UserId == userId && w.MovieId == existingMovie.Id);

            if (existingItem != null)
            {
                return BadRequest(new { message = "Movie already in watchlist" });
            }

            // Check if already in watched list
            var watchedItem = await _context.WatchedItems
                .FirstOrDefaultAsync(w => w.UserId == userId && w.MovieId == existingMovie.Id);

            if (watchedItem != null)
            {
                return BadRequest(new { message = "Movie already in watched list" });
            }

            var watchlistItem = new WatchlistItem
            {
                UserId = userId,
                MovieId = existingMovie.Id,
                DateAdded = DateTime.UtcNow
            };

            _context.WatchlistItems.Add(watchlistItem);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Movie added to watchlist", watchlistItem });
        }

        [HttpDelete("{imdbId}")]
        public async Task<IActionResult> RemoveFromWatchlist(string imdbId)
        {
            var userId = GetUserId();

            var movie = await _context.Movies.FirstOrDefaultAsync(m => m.ImdbId == imdbId);

            if (movie == null)
            {
                return NotFound(new { message = "Movie not found" });
            }

            var watchlistItem = await _context.WatchlistItems
                .FirstOrDefaultAsync(w => w.UserId == userId && w.MovieId == movie.Id);

            if (watchlistItem == null)
            {
                return NotFound(new { message = "Movie not in watchlist" });
            }

            _context.WatchlistItems.Remove(watchlistItem);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Movie removed from watchlist" });
        }
    }
}