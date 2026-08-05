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
    public class WatchedController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMovieService _movieService;

        public WatchedController(ApplicationDbContext context, IMovieService movieService)
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
        public async Task<ActionResult<List<WatchedItem>>> GetWatchedList()
        {
            var userId = GetUserId();

            var watchedList = await _context.WatchedItems
                .Include(w => w.Movie)
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.LastWatchedAt)
                .ToListAsync();

            return Ok(watchedList);
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsWatched([FromBody] Movie movie)
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

            // Remove from watchlist if present
            var watchlistItem = await _context.WatchlistItems
                .FirstOrDefaultAsync(w => w.UserId == userId && w.MovieId == existingMovie.Id);

            if (watchlistItem != null)
            {
                _context.WatchlistItems.Remove(watchlistItem);
            }

            // Check if already in watched list
            var watchedItem = await _context.WatchedItems
                .FirstOrDefaultAsync(w => w.UserId == userId && w.MovieId == existingMovie.Id);

            if (watchedItem != null)
            {
                watchedItem.TimesWatched += 1;
                watchedItem.LastWatchedAt = DateTime.UtcNow;
                _context.WatchedItems.Update(watchedItem);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Times watched incremented", watchedItem });
            }
            else
            {
                var newWatchedItem = new WatchedItem
                {
                    UserId = userId,
                    MovieId = existingMovie.Id,
                    TimesWatched = 1,
                    FirstWatchedAt = DateTime.UtcNow,
                    LastWatchedAt = DateTime.UtcNow
                };

                _context.WatchedItems.Add(newWatchedItem);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Movie marked as watched", watchedItem = newWatchedItem });
            }
        }

        [HttpPut("{id}/increment")]
        public async Task<IActionResult> IncrementTimesWatched(int id)
        {
            var userId = GetUserId();

            var watchedItem = await _context.WatchedItems
                .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);

            if (watchedItem == null)
            {
                return NotFound(new { message = "Watched item not found" });
            }

            watchedItem.TimesWatched += 1;
            watchedItem.LastWatchedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Times watched incremented", watchedItem });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveFromWatched(int id)
        {
            var userId = GetUserId();

            var watchedItem = await _context.WatchedItems
                .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);

            if (watchedItem == null)
            {
                return NotFound(new { message = "Watched item not found" });
            }

            _context.WatchedItems.Remove(watchedItem);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Movie removed from watched list" });
        }

        [HttpPost("{id}/reset")]
        public async Task<IActionResult> ResetTimesWatched(int id)
        {
            var userId = GetUserId();

            var watchedItem = await _context.WatchedItems
                .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);

            if (watchedItem == null)
            {
                return NotFound(new { message = "Watched item not found" });
            }

            watchedItem.TimesWatched = 1;
            watchedItem.FirstWatchedAt = DateTime.UtcNow;
            watchedItem.LastWatchedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Times watched reset", watchedItem });
        }
    }
}