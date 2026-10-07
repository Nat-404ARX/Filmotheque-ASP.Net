using Filmothèque.Models;

namespace Filmothèque.Services
{
    public interface IFilmService
    {
        Task<IEnumerable<Film>> GetAllFilmsAsync(string? searchString, GenreFilm? genre);
        Task<Film?> GetFilmByIdAsync(int id);
        Task CreateFilmAsync(Film film);
        Task UpdateFilmAsync(Film film);
        Task DeleteFilmAsync(int id);
        Task DeleteAllFilmsAsync();
        Task<bool> FilmExistsAsync(int id);
    }
}