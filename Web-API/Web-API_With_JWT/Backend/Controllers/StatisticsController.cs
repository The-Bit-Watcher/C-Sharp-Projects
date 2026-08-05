using Ass3WebApi.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Ass3WebApi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StatisticsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public StatisticsController(ApplicationDbContext context)
        {
            _context = context;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim ?? "0");
        }

        [HttpGet("genres")]
        public async Task<IActionResult> GetGenreStats()
        {
            var userId = GetUserId();

            var watchedMovies = await _context.WatchedItems
                .Include(w => w.Movie)
                .Where(w => w.UserId == userId && w.Movie != null)
                .ToListAsync();

            var genreCount = new Dictionary<string, int>();

            foreach (var item in watchedMovies)
            {
                if (!string.IsNullOrEmpty(item.Movie?.Genre))
                {
                    var genres = item.Movie.Genre.Split(',')
                        .Select(g => g.Trim())
                        .Where(g => !string.IsNullOrEmpty(g));

                    foreach (var genre in genres)
                    {
                        if (genreCount.ContainsKey(genre))
                            genreCount[genre] += item.TimesWatched;
                        else
                            genreCount[genre] = item.TimesWatched;
                    }
                }
            }

            var topGenres = genreCount
                .OrderByDescending(x => x.Value)
                .Take(6)
                .Select(x => new { Genre = x.Key, Count = x.Value })
                .ToList();

            return Ok(topGenres);
        }

        [HttpGet("yearly")]
        public async Task<IActionResult> GetYearlyStats()
        {
            var userId = GetUserId();

            var watchedMovies = await _context.WatchedItems
                .Include(w => w.Movie)
                .Where(w => w.UserId == userId && w.Movie != null && w.Movie.Year > 0)
                .ToListAsync();

            var yearlyCount = watchedMovies
                .GroupBy(w => w.Movie!.Year)
                .Select(g => new { Year = g.Key, Count = g.Sum(x => x.TimesWatched) })
                .OrderBy(x => x.Year)
                .ToList();

            return Ok(yearlyCount);
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummaryStats()
        {
            var userId = GetUserId();

            var watchedItems = await _context.WatchedItems
                .Include(w => w.Movie)
                .Where(w => w.UserId == userId)
                .ToListAsync();

            var totalWatched = watchedItems.Sum(w => w.TimesWatched);
            var uniqueMovies = watchedItems.Count;

            var mostWatched = watchedItems
                .OrderByDescending(w => w.TimesWatched)
                .FirstOrDefault();

            var averageRating = watchedItems
                .Where(w => w.Movie != null && !string.IsNullOrEmpty(w.Movie.ImdbRating))
                .Select(w => w.Movie!.ImdbRating)
                .ToList();

            double avgRating = 0;
            if (averageRating.Any())
            {
                var validRatings = averageRating
                    .Where(r => double.TryParse(r, out _))
                    .Select(r => double.Parse(r!))
                    .ToList();

                if (validRatings.Any())
                    avgRating = validRatings.Average();
            }

            return Ok(new
            {
                TotalWatched = totalWatched,
                UniqueMovies = uniqueMovies,
                MostWatchedMovie = mostWatched?.Movie?.Title,
                MostWatchedCount = mostWatched?.TimesWatched ?? 0,
                AverageImdbRating = Math.Round(avgRating, 2)
            });
        }
    }
}