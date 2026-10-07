using System.Net.Http.Json;
using Filmothèque.Data;
using Filmothèque.Models;
using Microsoft.EntityFrameworkCore;

namespace Filmothèque.Services
{
    public class TmdbService : ITmdbService
    {
        private readonly HttpClient _httpClient;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        // Base URL conforme TMDB
        private const string ImgBaseUrl = "https://image.tmdb.org/t/p/w500";

        public TmdbService(HttpClient httpClient, ApplicationDbContext context, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _context = context;
            _configuration = configuration;
        }

        public async Task<int> ImportPopularMoviesAsync(int pageCount = 1)
        {
            var apiKey = _configuration["Tmdb:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
                throw new InvalidOperationException("La clé d'API TMDB n'est pas configurée.");

            int importedCount = 0;

            for (int page = 1; page <= pageCount; page++)
            {
                var url = $"https://api.themoviedb.org/3/movie/popular?api_key={apiKey}&language=fr-FR&page={page}";
                var response = await _httpClient.GetFromJsonAsync<TmdbResponse>(url);

                if (response?.Results == null) continue;

                foreach (var dto in response.Results)
                {
                    // Éviter les doublons
                    if (await _context.Films.AnyAsync(f => f.Title == dto.Title))
                        continue;

                    DateTime.TryParse(dto.ReleaseDate, out var releaseDate);

                    var film = new Film
                    {
                        Title = dto.Title,
                        Description = string.IsNullOrWhiteSpace(dto.Overview) ? "Aucune description." : dto.Overview,
                        ReleaseDate = releaseDate == default ? DateTime.Now : releaseDate,
                        Rating = Math.Round(dto.VoteAverage / 2, 1), // Conversion /5
                        Price = 9.99m,
                        StockQuantity = 10,
                        Genre = MapGenre(dto.GenreIds.FirstOrDefault()),
                        // Construction correcte de l'URL de l'image
                        ImageUrl = string.IsNullOrEmpty(dto.PosterPath)
                            ? "/images/default-movie.jpg"
                            : $"{ImgBaseUrl}{dto.PosterPath}"
                    };

                    _context.Films.Add(film);
                    importedCount++;
                }
            }

            await _context.SaveChangesAsync();
            return importedCount;
        }

        private static GenreFilm MapGenre(int tmdbGenreId)
        {
            return tmdbGenreId switch
            {
                28 => GenreFilm.Action,
                12 => GenreFilm.Aventure,
                16 => GenreFilm.Animation,
                35 => GenreFilm.Comédie,
                80 => GenreFilm.Thriller,    
                99 => GenreFilm.Documentaire,
                18 => GenreFilm.Drame,
                10751 => GenreFilm.Animation,
                14 => GenreFilm.Aventure,  
                36 => GenreFilm.Drame,    
                27 => GenreFilm.Horreur,
                10402 => GenreFilm.Comédie,   
                9648 => GenreFilm.Thriller,    
                10749 => GenreFilm.Romance,
                878 => GenreFilm.SF,
                10770 => GenreFilm.Drame,   
                53 => GenreFilm.Thriller,
                10752 => GenreFilm.Action,    
                37 => GenreFilm.Aventure,     
                _ => GenreFilm.Action
            };
        }
    }
}