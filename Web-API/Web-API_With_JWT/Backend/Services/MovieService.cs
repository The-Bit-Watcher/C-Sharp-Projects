using Ass3WebApi.Models;
using System.Text.Json;
using Ass3WebApi.Services;

namespace Ass3WebApi.Services
{
    public class MovieService : IMovieService
    {
        private readonly HttpClient _httpClient;
        private readonly string _omdbApiKey;
        private readonly ILogger<MovieService> _logger;

        // OMDb API endpoints
        private const string OMD_BASE_URL = "http://www.omdbapi.com/";

        public MovieService(IConfiguration configuration, ILogger<MovieService> logger)
        {
            _httpClient = new HttpClient();
            _omdbApiKey = configuration["OMDb:ApiKey"] ?? "4ccc4995";
            _logger = logger;
        }

        public async Task<List<Movie>> SearchMoviesAsync(string title)
        {
            try
            {
                // Using API key: 4ccc4995
                var url = $"{OMD_BASE_URL}?apikey={_omdbApiKey}&s={Uri.EscapeDataString(title)}&type=movie";
                _logger.LogInformation("Calling OMDb API: {Url}", url);

                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("OMDb API returned {StatusCode}", response.StatusCode);
                    return new List<Movie>();
                }

                var json = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("OMDb Response: {Json}", json.Substring(0, Math.Min(500, json.Length)));

                var result = JsonSerializer.Deserialize<OmdbSearchResponse>(json);

                if (result?.Search == null || result.Response == "False")
                {
                    return new List<Movie>();
                }

                var movies = new List<Movie>();
                foreach (var item in result.Search)
                {
                    var movie = new Movie
                    {
                        ImdbId = item.ImdbID,
                        Title = item.Title,
                        Year = int.TryParse(item.Year, out var year) ? year : 0,
                        Poster = item.Poster,
                        Actors = item.Actors ?? "",
                        Genre = item.Genre ?? ""
                    };
                    movies.Add(movie);
                }

                return movies;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching movies for title: {Title}", title);
                return new List<Movie>();
            }
        }

        public async Task<Movie?> GetMovieDetailsAsync(string imdbId)
        {
            try
            {
                // Using API key: 4ccc4995
                var url = $"{OMD_BASE_URL}?apikey={_omdbApiKey}&i={imdbId}&plot=full";
                _logger.LogInformation("Calling OMDb API for details: {Url}", url);

                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<OmdbMovieDetail>(json);

                if (result?.Response == "False")
                {
                    _logger.LogWarning("Movie not found for IMDB ID: {ImdbId}", imdbId);
                    return null;
                }

                var movie = new Movie
                {
                    ImdbId = result.ImdbID ?? string.Empty,
                    Title = result.Title ?? string.Empty,
                    Year = int.TryParse(result.Year, out var year) ? year : 0,
                    Poster = result.Poster ?? string.Empty,
                    Actors = result.Actors ?? string.Empty,
                    Genre = result.Genre ?? string.Empty,
                    Plot = result.Plot ?? string.Empty,
                    Director = result.Director ?? string.Empty,
                    Writer = result.Writer ?? string.Empty,
                    Rated = result.Rated ?? string.Empty,
                    Runtime = result.Runtime ?? string.Empty,
                    ImdbRating = result.ImdbRating ?? string.Empty
                };

                return movie;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting movie details for IMDB ID: {ImdbId}", imdbId);
                return null;
            }
        }

        // OMDb API Response Classes
        private class OmdbSearchResponse
        {
            public List<OmdbSearchItem>? Search { get; set; }
            public string? Response { get; set; }
            public string? Error { get; set; }
        }

        private class OmdbSearchItem
        {
            public string? Title { get; set; }
            public string? Year { get; set; }
            public string? ImdbID { get; set; }
            public string? Type { get; set; }
            public string? Poster { get; set; }
            public string? Actors { get; set; }
            public string? Genre { get; set; }
        }

        private class OmdbMovieDetail
        {
            public string? Title { get; set; }
            public string? Year { get; set; }
            public string? Rated { get; set; }
            public string? Released { get; set; }
            public string? Runtime { get; set; }
            public string? Genre { get; set; }
            public string? Director { get; set; }
            public string? Writer { get; set; }
            public string? Actors { get; set; }
            public string? Plot { get; set; }
            public string? Language { get; set; }
            public string? Country { get; set; }
            public string? Awards { get; set; }
            public string? Poster { get; set; }
            public string? ImdbRating { get; set; }
            public string? ImdbID { get; set; }
            public string? Type { get; set; }
            public string? Response { get; set; }
        }
    }
}